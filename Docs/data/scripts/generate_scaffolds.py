#!/usr/bin/env python3
"""Generate documentation scaffolds with correct relative links.

Reads Docs/data/manifest.jsonl and Docs/data/modules.json and writes:

  * Docs/files/<source-path>.md     one scaffold per source file
  * Docs/modules/<module>.md        one scaffold per module
  * Docs/data/linkmap.json          relative links, for verification/reuse

Every cross-link is computed with os.path.relpath so it resolves correctly on
GitHub and in local Markdown viewers, regardless of nesting depth. Each
scaffold contains {{PLACEHOLDER}} markers that the documentation sub-agents
replace with authored prose; the metadata table and navigation footer must be
preserved verbatim so links stay correct.

Incremental: a scaffold is only (re)written if it does not already exist, so
re-running never clobbers authored content. Delete a scaffold to regenerate it.

Re-run with:  python3 Docs/data/scripts/generate_scaffolds.py
"""
import json
import os
import subprocess

ROOT = subprocess.check_output(
    ["git", "rev-parse", "--show-toplevel"], text=True
).strip()
DOCS = os.path.join(ROOT, "Docs")


def rel(from_file, to_file):
    """Relative link from one doc file to another path (both repo-relative)."""
    return os.path.relpath(to_file, os.path.dirname(from_file)).replace(os.sep, "/")


def load_manifest():
    recs = []
    with open(os.path.join(DOCS, "data", "manifest.jsonl")) as f:
        for line in f:
            line = line.strip()
            if line:
                recs.append(json.loads(line))
    return recs


def main():
    recs = load_manifest()
    with open(os.path.join(DOCS, "data", "modules.json")) as f:
        modmeta = json.load(f)
    modules = modmeta["modules"]

    by_module = {}
    for r in recs:
        by_module.setdefault(r["module"], []).append(r)
    for m in by_module:
        by_module[m].sort(key=lambda r: r["path"])

    linkmap = {"files": {}, "modules": {}}
    created = 0

    # ---- per-file scaffolds ------------------------------------------------
    for r in recs:
        doc = r["doc"]                       # Docs/files/<path>.md
        module_doc = f"Docs/modules/{r['module']}.md"
        links = {
            "module": rel(doc, module_doc),
            "source": rel(doc, r["path"]),
            "toc": rel(doc, "Docs/TOC.md"),
            "index": rel(doc, "Docs/Index.md"),
        }
        linkmap["files"][r["path"]] = links
        mod_title = modules[r["module"]]["title"]
        is_patch = r["name"].endswith("Patch.cs")
        patch_marker = "{{PATCH_SECTION}}" if is_patch else "{{PATCH_SECTION_OPTIONAL}}"

        body = f"""# `{r['path']}`

{{{{SUMMARY}}}}

|  |  |
| --- | --- |
| **Module** | [{mod_title}]({links['module']}) |
| **Source** | [`{r['name']}`]({links['source']}) ({r['lines']} lines) |
| **Kind** | {{{{KIND}}}} |
| **Role** | {{{{ROLE}}}} |

## Purpose

{{{{PURPOSE}}}}

## Key members

{{{{MEMBERS}}}}

{patch_marker}

## References

{{{{REFERENCES}}}}

---

*[Handbook]({links['toc']}) · [Module: {mod_title}]({links['module']}) · [Index]({links['index']})*
"""
        abspath = os.path.join(ROOT, doc)
        os.makedirs(os.path.dirname(abspath), exist_ok=True)
        if not os.path.exists(abspath):
            with open(abspath, "w") as f:
                f.write(body)
            created += 1

    # ---- per-module scaffolds ---------------------------------------------
    for module_id, members in sorted(by_module.items()):
        meta = modules.get(module_id, {"title": module_id, "summary": ""})
        doc = f"Docs/modules/{module_id}.md"
        toc = rel(doc, "Docs/TOC.md")
        index = rel(doc, "Docs/Index.md")
        member_links = []
        for r in members:
            member_links.append({
                "path": r["path"],
                "name": r["name"],
                "doc_link": rel(doc, r["doc"]),
                "source_link": rel(doc, r["path"]),
                "tier": r["tier"],
                "lines": r["lines"],
            })
        linkmap["modules"][module_id] = {
            "doc": doc, "toc": toc, "index": index, "members": member_links,
        }

        rows = "\n".join(
            f"| [`{m['name']}`]({m['doc_link']}) | {{{{ONE_LINER}}}} |"
            for m in member_links
        )
        body = f"""# {meta['title']}

{meta['summary']}

{{{{MODULE_OVERVIEW}}}}

## Files

| File | Summary |
| --- | --- |
{rows}

## How it fits together

{{{{MODULE_DETAIL}}}}

---

*[Handbook TOC]({toc}) · [Index]({index})*
"""
        abspath = os.path.join(ROOT, doc)
        if not os.path.exists(abspath):
            with open(abspath, "w") as f:
                f.write(body)
            created += 1

    with open(os.path.join(DOCS, "data", "linkmap.json"), "w") as f:
        json.dump(linkmap, f, indent=2)

    print(f"Scaffolds created: {created}")
    print(f"Modules: {len(by_module)}  Files: {len(recs)}")
    print(f"linkmap -> Docs/data/linkmap.json")


if __name__ == "__main__":
    main()
