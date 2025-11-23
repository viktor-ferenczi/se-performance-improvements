#!/usr/bin/env python3
"""
Parse C# code and split any if-else chain with 10+ typeof checks into a LUT-based approach.
Each chain gets its own dedicated LUT.
"""

import re
from tree_sitter import Language, Parser
import tree_sitter_c_sharp as ts_c_sharp

def read_file(path):
    with open(path, 'r', encoding='utf-8') as f:
        return f.read()

def write_file(path, content):
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

def find_typeof_chain(text, start_pos):
    """Find a complete if-else chain starting at start_pos, extract handlers and boundaries."""
    handlers = []
    chain_start = start_pos
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
            handler_body = text[brace_pos + 1:scan_pos - 1].strip()
            handlers.append({
                'type': type_name,
                'body': handler_body
            })
            pos = scan_pos
        else:
            break
    
    if handlers:
        return {
            'handlers': handlers,
            'start': chain_start,
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

def extract_surrounding_context(source_code, chain):
    """Extract method name and context around the chain."""
    # Find method containing this chain
    before_chain = source_code[:chain['start']]
    
    # Find the most recent method definition
    method_pattern = r'(private|public|protected|internal)\s+\w+\s+(\w+)\s*\([^)]*\)\s*{'
    matches = list(re.finditer(method_pattern, before_chain))
    if matches:
        last_match = matches[-1]
        method_name = last_match.group(2)
        
        # Find the preamble before the chain (code after method start but before chain)
        method_open = last_match.end() - 1
        preamble = source_code[method_open + 1:chain['start']].strip()
        
        # Find the postamble after the chain (remaining code in the method)
        # We need to find the method's closing brace
        brace_count = 1
        pos = method_open + 1
        while pos < len(source_code) and brace_count > 0:
            if source_code[pos] == '{':
                brace_count += 1
            elif source_code[pos] == '}':
                brace_count -= 1
            pos += 1
        
        postamble = source_code[chain['end']:pos - 1].strip()
        
        return {
            'method_name': method_name,
            'method_signature': source_code[last_match.start():method_open + 1],
            'method_start': last_match.start(),
            'method_end': pos,
            'preamble': preamble,
            'postamble': postamble
        }
    
    return None

def generate_lut_code(chain_info, chain_idx):
    """Generate LUT and replacement code for a chain."""
    method_name = chain_info['method_name']
    handlers = chain_info['handlers']
    
    # Build dictionary entries
    entries = []
    for h in handlers:
        body = h['body']
        body = body.replace('isNullable', 'true')
        body = body.replace('needType: true', 'needType: nt')
        body = body.replace('needType: false', 'needType: nt')
        
        # Compact simple one-liners
        if body.count('\n') <= 2 and len(body) < 200:
            body = ' '.join(body.split())
        
        entries.append(f"            {{ typeof({h['type']}), (w, n, ns, o, nt) => {{ {body} }} }}")
    
    lut_name = f"{method_name}_TypeHandlers_{chain_idx}"
    lut_init = f"private static readonly Dictionary<Type, Action<XmlSerializationWriter1, string, string, object, bool>> {lut_name} =\n"
    lut_init += "        new Dictionary<Type, Action<XmlSerializationWriter1, string, string, object, bool>>\n"
    lut_init += "        {\n"
    lut_init += ",\n".join(entries)
    lut_init += "\n        };"
    
    # Generate replacement code for the chain
    replacement = f"""if ({lut_name}.TryGetValue(type, out var handler))
{{
handler(this, n, ns, o, needType);
return;
}}"""
    
    return lut_init, replacement

def main():
    print("Reading LongSerializer.cs...")
    source_code = read_file('LongSerializer.cs')
    
    print("Finding if-else chains with typeof checks (10+ clauses)...")
    chains = find_all_typeof_chains(source_code, min_count=10)
    
    if not chains:
        print("No chains found with sufficient typeof checks")
        return
    
    print(f"Found {len(chains)} chain(s) to process:")
    
    # Extract context for each chain and process
    chain_infos = []
    for i, chain in enumerate(chains):
        context = extract_surrounding_context(source_code, chain)
        if context:
            chain_info = {**chain, **context}
            chain_infos.append(chain_info)
            print(f"  Chain {i+1}: {context['method_name']} ({len(chain['handlers'])} typeof checks)")
    
    # Sort chains by their position in reverse order so replacements don't affect positions
    chain_infos.sort(key=lambda x: x['start'], reverse=True)
    
    result_code = source_code
    luts = []
    
    for i, chain_info in enumerate(chain_infos):
        chain_idx = len(chain_infos) - i
        print(f"\nProcessing chain {chain_idx} in {chain_info['method_name']}...")
        
        # Generate LUT and replacement
        lut_init, replacement = generate_lut_code(chain_info, chain_idx)
        luts.append(lut_init)
        
        # Replace the chain in the code
        before_chain = result_code[:chain_info['start']]
        after_chain = result_code[chain_info['end']:]
        
        result_code = before_chain + replacement + '\n\n' + after_chain
    
    # Insert all LUTs at the beginning of the class
    class_start = result_code.find('public class XmlSerializationWriter1')
    class_opening_brace = result_code.find('{', class_start)
    
    before_class = result_code[:class_opening_brace + 1]
    after_class = result_code[class_opening_brace + 1:]
    
    all_luts = "\n\n".join(reversed(luts))
    result_code = before_class + "\n" + all_luts + "\n\n" + after_class
    
    # Write output
    output_path = 'LongSerializer_split2.cs'
    print(f"\nWriting to {output_path}...")
    write_file(output_path, result_code)
    print("Done!")

if __name__ == '__main__':
    main()
