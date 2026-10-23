"""Turns PbScan's per-script JSON into a member table: how many scripts use each game API member.

Usage: python3 aggregate.py scan.json members.tsv [titles.json]

Scripts that don't compile against the current game (removed API) are left out of the counts
and listed on stderr. titles.json is the Steam GetPublishedFileDetails list, for their names.
"""

import json
import sys
from collections import defaultdict


def main():
    scan = json.load(open(sys.argv[1]))
    titles = {}
    if len(sys.argv) > 3:
        titles = {x["publishedfileid"]: x.get("title", "") for x in json.load(open(sys.argv[3]))}

    # A Roslyn newer than the game's rejects `ref` on a using variable; the game compiles it
    tolerated = ("CS1657",)
    ok = [s for s in scan if all(any(t in e for t in tolerated) for e in s["Errors"])]
    for s in scan:
        if s not in ok:
            first = s["Errors"][0].split("error ", 1)[-1][:100]
            print(f"skipped {s['Id']} {titles.get(s['Id'], '')}: {len(s['Errors'])} errors, {first}", file=sys.stderr)

    rows = defaultdict(lambda: {"kind": "", "scripts": 0, "reads": 0, "writes": 0})
    for s in ok:
        for key, u in s["Members"].items():
            r = rows[key]
            r["kind"] = u["Kind"]
            r["scripts"] += 1
            r["reads"] += u["Reads"]
            r["writes"] += u["Writes"]

    with open(sys.argv[2], "w") as f:
        f.write("member\tkind\tscripts\treads\twrites\n")
        for key, r in sorted(rows.items(), key=lambda kv: (-kv[1]["scripts"], kv[0])):
            f.write(f"{key}\t{r['kind']}\t{r['scripts']}\t{r['reads']}\t{r['writes']}\n")

    kinds = defaultdict(int)
    for r in rows.values():
        kinds[r["kind"]] += 1
    print(f"{len(ok)} scripts, {len(rows)} distinct members: {dict(sorted(kinds.items()))}")


if __name__ == "__main__":
    main()
