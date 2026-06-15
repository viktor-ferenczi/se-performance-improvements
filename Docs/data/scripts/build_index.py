#!/usr/bin/env python3
"""Generate Docs/Index.md: a flat, alphabetical-by-module index of every
documented source file, with the one-line summary each file doc carries.

Reads Docs/data/manifest.jsonl + Docs/data/modules.json, extracts the italic
summary line from each per-file doc, and emits a table per module. All links
are computed relative to Docs/Index.md.

Re-run with:  python3 Docs/data/scripts/build_index.py
"""
import json
import os
import re
import subprocess

ROOT = subprocess.check_output(
    ["git", "rev-parse", "--show-toplevel"], text=True
).strip()
DOCS = os.path.join(ROOT, "Docs")
SUMMARY_RE = re.compile(r"^\*(.+)\*\s*$")
# Inline links in a summary are relative to the source doc, not to Index.md,
# so unwrap [text](url) -> text when copying a summary into the index.
LINK_RE = re.compile(r"\[([^\]]+)\]\([^)]+\)")


def rel(target):
    return os.path.relpath(
        os.path.join(ROOT, target), os.path.join(DOCS)
    ).replace(os.sep, "/")


def summary_of(doc_repo_rel):
    path = os.path.join(ROOT, doc_repo_rel)
    with open(path) as f:
        for line in f.readlines()[:6]:
            m = SUMMARY_RE.match(line.strip())
            if m:
                return LINK_RE.sub(r"\1", m.group(1).strip())
    return ""


def main():
    recs = [json.loads(l) for l in open(os.path.join(DOCS, "data", "manifest.jsonl")) if l.strip()]
    modmeta = json.load(open(os.path.join(DOCS, "data", "modules.json")))
    modules = modmeta["modules"]
    order = [m for g in modmeta["groups"] for m in g["modules"]]

    by_module = {}
    for r in recs:
        by_module.setdefault(r["module"], []).append(r)

    out = ["# Index", "",
           "Every documented source file, grouped by module. "
           "See the [Handbook (TOC)](TOC.md) for the guided, top-down view.",
           "",
           f"**{len(recs)} files across {len(by_module)} modules.**", ""]

    for module_id in order:
        members = sorted(by_module.get(module_id, []), key=lambda r: r["path"])
        if not members:
            continue
        meta = modules.get(module_id, {"title": module_id})
        out.append(f"## [{meta['title']}]({rel('Docs/modules/' + module_id + '.md')})")
        out.append("")
        out.append("| File | Path | Summary |")
        out.append("| --- | --- | --- |")
        for r in members:
            link = rel(r["doc"])
            summary = summary_of(r["doc"])
            out.append(f"| [`{r['name']}`]({link}) | `{r['path']}` | {summary} |")
        out.append("")

    with open(os.path.join(DOCS, "Index.md"), "w") as f:
        f.write("\n".join(out) + "\n")
    print(f"Index.md written: {len(recs)} files, {len(by_module)} modules.")


if __name__ == "__main__":
    main()
