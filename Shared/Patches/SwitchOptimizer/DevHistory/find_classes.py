#!/usr/bin/env python3
"""
Find all .cs files in a directory tree that contain methods with 10+ typeof checks.
Prints: NameSpace.ClassName,MethodName,relative/path/to/file.cs
"""

import os
import re
import sys


def find_typeof_chain(text, start_pos):
    """Find a complete if-else chain starting at start_pos."""
    handlers = []
    pos = start_pos
    
    while True:
        # Find next typeof check
        match = re.search(r'if\s*\(\s*type\s*==\s*typeof\(([^)]+)\)\s*\)', text[pos:])
        if not match:
            break
        
        # Check if this is part of a continuing chain (else if) or a new chain
        text_between = text[pos:pos + match.start()].strip()
        if handlers and not text_between.startswith('else'):
            # New chain starting, stop here
            break
        
        type_name = match.group(1).strip()
        
        # Find the opening brace after the condition
        block_start = pos + match.end()
        brace_pos = text.find('{', block_start)
        if brace_pos == -1:
            break
        
        # Count braces to find matching closing brace
        brace_count = 1
        scan_pos = brace_pos + 1
        while scan_pos < len(text) and brace_count > 0:
            if text[scan_pos] == '{':
                brace_count += 1
            elif text[scan_pos] == '}':
                brace_count -= 1
            scan_pos += 1
        
        if brace_count == 0:
            handlers.append({
                'type': type_name,
                'start': pos + match.start(),
                'end': scan_pos
            })
            pos = scan_pos
        else:
            break
    
    if handlers:
        return {
            'handlers': handlers,
            'start': handlers[0]['start'],
            'end': pos
        }
    return None


def find_all_typeof_chains(source_code, min_count=10):
    """Find all if-else chains with at least min_count typeof checks."""
    chains = []
    pos = 0
    
    while pos < len(source_code):
        # Look for the start of a typeof check
        match = re.search(r'if\s*\(\s*type\s*==\s*typeof\(', source_code[pos:])
        if not match:
            break
        
        chain_start = pos + match.start()
        chain = find_typeof_chain(source_code, chain_start)
        
        if chain and len(chain['handlers']) >= min_count:
            chains.append(chain)
            pos = chain['end']
        else:
            pos = chain_start + 1
    
    return chains


def extract_namespace(source_code):
    """Extract the namespace from C# source code."""
    # Look for namespace declaration
    namespace_match = re.search(r'namespace\s+([\w\.]+)', source_code)
    if namespace_match:
        return namespace_match.group(1)
    return None


def extract_class_name(source_code, position):
    """Extract the class name that contains the given position."""
    # Find all class declarations before this position
    before_pos = source_code[:position]
    
    # Match class declarations: public/private/etc class ClassName
    class_pattern = r'(?:public|private|protected|internal|static|\s)+class\s+(\w+)'
    matches = list(re.finditer(class_pattern, before_pos))
    
    if matches:
        # Get the last class declaration before the position
        last_match = matches[-1]
        class_start = last_match.end()
        
        # Verify that position is within this class by checking braces
        brace_pos = source_code.find('{', last_match.end())
        if brace_pos == -1 or brace_pos > position:
            return None
        
        # Count braces to see if position is within this class
        brace_count = 1
        scan_pos = brace_pos + 1
        while scan_pos < len(source_code) and brace_count > 0:
            if source_code[scan_pos] == '{':
                brace_count += 1
            elif source_code[scan_pos] == '}':
                brace_count -= 1
            
            if scan_pos >= position and brace_count > 0:
                # Position is within this class
                return last_match.group(1)
            scan_pos += 1
    
    return None


def extract_method_name(source_code, chain_start):
    """Extract the method name that contains the given chain."""
    before_chain = source_code[:chain_start]
    
    # Find the most recent method definition
    method_pattern = r'(?:private|public|protected|internal|static|\s)+\w+\s+(\w+)\s*\([^)]*\)\s*{'
    matches = list(re.finditer(method_pattern, before_chain))
    
    if matches:
        last_match = matches[-1]
        method_name = last_match.group(1)
        
        # Verify the chain is within this method
        method_open = last_match.end() - 1
        brace_count = 1
        pos = method_open + 1
        while pos < len(source_code) and brace_count > 0:
            if source_code[pos] == '{':
                brace_count += 1
            elif source_code[pos] == '}':
                brace_count -= 1
            
            if pos >= chain_start and brace_count > 0:
                return method_name
            pos += 1
    
    return None


def analyze_cs_file(filepath, root_dir):
    """Analyze a single C# file for typeof chains."""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            source_code = f.read()
    except Exception as e:
        return []
    
    chains = find_all_typeof_chains(source_code)
    if not chains:
        return []
    
    results = []
    namespace = extract_namespace(source_code)
    relative_path = os.path.relpath(filepath, root_dir)
    
    # Track already reported method/class combinations to avoid duplicates
    reported = set()
    
    for chain in chains:
        class_name = extract_class_name(source_code, chain['start'])
        method_name = extract_method_name(source_code, chain['start'])
        
        if class_name and method_name:
            # Build fully qualified name
            if namespace:
                fqn = f"{namespace}.{class_name}"
            else:
                fqn = class_name
            
            key = (fqn, method_name)
            if key not in reported:
                results.append({
                    'fqn': fqn,
                    'method': method_name,
                    'path': relative_path,
                    'chain_count': len(chain['handlers'])
                })
                reported.add(key)
    
    return results


def find_all_cs_files(directory):
    """Recursively find all .cs files in the directory."""
    cs_files = []
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                cs_files.append(os.path.join(root, file))
    return cs_files


def main():
    # Get the directory to search (default to current directory)
    search_dir = sys.argv[1] if len(sys.argv) > 1 else '.'
    
    if not os.path.isdir(search_dir):
        print(f"Error: {search_dir} is not a valid directory", file=sys.stderr)
        sys.exit(1)
    
    # Find all C# files
    cs_files = find_all_cs_files(search_dir)
    
    if not cs_files:
        print("No .cs files found", file=sys.stderr)
        return
    
    # Analyze each file
    all_results = []
    for cs_file in cs_files:
        results = analyze_cs_file(cs_file, search_dir)
        all_results.extend(results)
    
    # Print results
    if all_results:
        for result in all_results:
            print(f"{result['fqn']},{result['method']},{result['path']}")
    else:
        print("No methods with 10+ typeof checks found", file=sys.stderr)


if __name__ == '__main__':
    main()
