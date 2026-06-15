#!/usr/bin/env python3
"""Resolve [[wiki-links]] in the documentation tree into relative Markdown links.

Sub-agents write cross-references as wiki-links so they never have to compute
relative paths by hand:

    [[Cache]]            -> link to the doc for the source file  Cache.cs
    [[Cache|the cache]]  -> same target, custom link text "the cache"
    [[module:conveyor]]  -> link to the module page  Docs/modules/conveyor.md

This script rewrites every such token into a correct relative Markdown link
computed with os.path.relpath, so links resolve on GitHub and locally. It is
idempotent: already-resolved Markdown links are left untouched.

Ambiguous basenames (e.g. Plugin.cs exists in two projects) must be qualified
as [[ClientPlugin/Plugin]] or [[ServerPlugin/Plugin]]; unresolved tokens are
reported and left in place so they are easy to find.

Re-run with:  python3 Docs/data/scripts/resolve_wikilinks.py
"""
import json
import os
import re
import subprocess

ROOT = subprocess.check_output(
    ["git", "rev-parse", "--show-toplevel"], text=True
).strip()
DOCS = os.path.join(ROOT, "Docs")

WIKILINK = re.compile(r"\[\[([^\]]+)\]\]")


def load_manifest():
    recs = []
    with open(os.path.join(DOCS, "data", "manifest.jsonl")) as f:
        for line in f:
            line = line.strip()
            if line:
                recs.append(json.loads(line))
    return recs


def build_targets(recs):
    """Map link keys -> repo-relative doc path."""
    by_base = {}        # basename without .cs -> [doc paths]
    by_pathkey = {}     # 'Dir/Base' suffix match -> doc path
    for r in recs:
        base = r["name"][:-3] if r["name"].endswith(".cs") else r["name"]
        by_base.setdefault(base, []).append(r["doc"])
        # allow qualifying by any path suffix, e.g. ClientPlugin/Plugin
        src_noext = r["path"][:-3] if r["path"].endswith(".cs") else r["path"]
        by_pathkey[src_noext] = r["doc"]
    return by_base, by_pathkey


def resolve_token(token, by_base, by_pathkey):
    """Return (target_doc_repo_relative, default_text) or (None, reason)."""
    if token.startswith("module:"):
        mid = token[len("module:"):].strip()
        target = f"Docs/modules/{mid}.md"
        if os.path.exists(os.path.join(ROOT, target)):
            return target, mid
        return None, f"unknown module '{mid}'"
    if token in ("TOC", "Handbook", "Index"):
        name = "TOC" if token != "Index" else "Index"
        return f"Docs/{name}.md", token
    # path-qualified, e.g. ClientPlugin/Plugin
    if "/" in token:
        key = token
        if key in by_pathkey:
            return by_pathkey[key], os.path.basename(token)
        # suffix match
        matches = [v for k, v in by_pathkey.items() if k.endswith("/" + token) or k.endswith(token)]
        if len(matches) == 1:
            return matches[0], os.path.basename(token)
        return None, f"ambiguous/unknown path '{token}'"
    # bare basename
    docs = by_base.get(token)
    if not docs:
        return None, f"unknown name '{token}'"
    if len(docs) > 1:
        return None, f"ambiguous name '{token}' ({len(docs)} matches; qualify with a path)"
    return docs[0], token


def main():
    recs = load_manifest()
    by_base, by_pathkey = build_targets(recs)

    md_files = []
    for dirpath, _, names in os.walk(DOCS):
        if os.sep + "data" in dirpath + os.sep:
            continue  # skip working data, scripts
        for n in names:
            if n.endswith(".md"):
                md_files.append(os.path.join(dirpath, n))

    total_resolved = 0
    unresolved = []
    for md in md_files:
        with open(md) as f:
            text = f.read()
        repo_rel = os.path.relpath(md, ROOT)

        def repl(match):
            nonlocal total_resolved
            raw = match.group(1)
            if "|" in raw:
                key, text_override = raw.split("|", 1)
                key = key.strip()
                text_override = text_override.strip()
            else:
                key, text_override = raw.strip(), None
            target, info = resolve_token(key, by_base, by_pathkey)
            if target is None:
                unresolved.append((repo_rel, raw, info))
                return match.group(0)
            link = os.path.relpath(target, os.path.dirname(repo_rel)).replace(os.sep, "/")
            label = text_override if text_override else f"`{os.path.basename(target)[:-3]}`" if target.startswith("Docs/files/") else info
            total_resolved += 1
            return f"[{label}]({link})"

        new_text = WIKILINK.sub(repl, text)
        if new_text != text:
            with open(md, "w") as f:
                f.write(new_text)

    print(f"Resolved {total_resolved} wiki-links across {len(md_files)} files.")
    if unresolved:
        print(f"\n{len(unresolved)} UNRESOLVED:")
        for f, raw, reason in unresolved:
            print(f"  {f}: [[{raw}]] -> {reason}")


if __name__ == "__main__":
    main()
