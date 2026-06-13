#!/usr/bin/env python3
"""Verify every relative Markdown link in the documentation tree resolves.

Walks all *.md files under Docs/ (and the repo-root README.md), extracts every
inline Markdown link with a relative target, and checks the target exists on
disk. External links (http/https/mailto), in-page anchors (#...) and any
remaining [[wiki-links]] are reported separately.

Exit code is non-zero if any relative link is broken or any wiki-link remains,
so it can gate a build.

Re-run with:  python3 Docs/data/scripts/check_links.py
"""
import os
import re
import subprocess
import sys

ROOT = subprocess.check_output(
    ["git", "rev-parse", "--show-toplevel"], text=True
).strip()
DOCS = os.path.join(ROOT, "Docs")

LINK = re.compile(r"(?<!\!)\[[^\]]+\]\(([^)]+)\)")
WIKI = re.compile(r"\[\[[^\]]+\]\]")


def md_files():
    out = [os.path.join(ROOT, "README.md")]
    for dirpath, _, names in os.walk(DOCS):
        # data/ holds working files and the authoring guide, which intentionally
        # keeps unresolved [[wiki-link]] examples — not part of the handbook.
        if os.sep + "data" in dirpath + os.sep:
            continue
        for n in names:
            if n.endswith(".md"):
                out.append(os.path.join(dirpath, n))
    return out


def main():
    broken = []
    wiki_left = []
    checked = 0
    for md in md_files():
        with open(md) as f:
            text = f.read()
        base = os.path.dirname(md)
        for m in LINK.finditer(text):
            target = m.group(1).strip()
            if target.startswith(("http://", "https://", "mailto:", "#")):
                continue
            path_part = target.split("#", 1)[0]
            if not path_part:
                continue
            checked += 1
            resolved = os.path.normpath(os.path.join(base, path_part))
            if not os.path.exists(resolved):
                broken.append((os.path.relpath(md, ROOT), target))
        for m in WIKI.finditer(text):
            wiki_left.append((os.path.relpath(md, ROOT), m.group(0)))

    print(f"Checked {checked} relative links.")
    if broken:
        print(f"\n{len(broken)} BROKEN links:")
        for f, t in broken:
            print(f"  {f}: {t}")
    if wiki_left:
        print(f"\n{len(wiki_left)} UNRESOLVED wiki-links:")
        for f, t in wiki_left:
            print(f"  {f}: {t}")
    if not broken and not wiki_left:
        print("All links OK.")
        return 0
    return 1


if __name__ == "__main__":
    sys.exit(main())
