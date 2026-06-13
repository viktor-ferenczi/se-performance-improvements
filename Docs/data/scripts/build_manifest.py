#!/usr/bin/env python3
"""Build the documentation manifest (data/manifest.jsonl) for the
Performance Improvements plugin.

For every tracked C# source file it records: name, path, the documentation
description path (mirrored under Docs/files/), line count, byte size, SHA256
hash, the module it belongs to, and a complexity tier. The hash is the cache
key: on a re-run, files whose hash is unchanged keep their existing
description and are skipped.

Re-run with:  python3 Docs/data/scripts/build_manifest.py
"""
import hashlib
import json
import os
import subprocess
import sys

ROOT = subprocess.check_output(
    ["git", "rev-parse", "--show-toplevel"], text=True
).strip()

# --- Module assignment -----------------------------------------------------
# Each rule is (module_id, predicate over the repo-relative posix path).
# First match wins, so order matters (most specific first).
MODULE_RULES = [
    ("client-settings", lambda p: p.startswith("ClientPlugin/Settings/")),
    ("client-plugin",   lambda p: p.startswith("ClientPlugin/")),
    # Server-only patches physically relocated under ServerPlugin/ are still
    # documented with the subsystem patches they belong to.
    ("simulation-and-blocks", lambda p: p.startswith("ServerPlugin/Patches/")),
    ("server-plugin",   lambda p: p.startswith("ServerPlugin/")),
    ("logging",         lambda p: p.startswith("Shared/Logging/")),
    ("tools",           lambda p: p.startswith("Shared/Tools/")),
    ("shared-plugin-core", lambda p: p.startswith("Shared/Plugin/") or p.startswith("Shared/Config/")),
    ("patch-infrastructure", lambda p: p == "Shared/Patches/PatchHelpers.cs"),
    ("merge-and-paste", lambda p: p.startswith("Shared/Patches/MergeAndPaste/")),
    ("conveyor",        lambda p: p.startswith("Shared/Patches/Conveyor/")),
    ("physics",         lambda p: p.startswith("Shared/Patches/Physics/")),
    ("safe-zone",       lambda p: p.startswith("Shared/Patches/SafeZone/")),
    ("keen-overhead-removal", lambda p: p.startswith("Shared/Patches/Bullshit/")),
    ("memory-allocation", lambda p: p.startswith("Shared/Patches/Memory/") or p.startswith("Shared/Patches/Voxel/")),
    ("world-loading",   lambda p: p.startswith("Shared/Patches/ScriptCompiler/") or p.startswith("Shared/Patches/DefinitionManager/")),
    ("simulation-and-blocks", lambda p: p.startswith("Shared/Patches/")),
    ("tests",           lambda p: p.startswith("Tests/")),
]

# --- Tier assignment -------------------------------------------------------
# Tier 1: complex/important, must be understood in full (strong model).
# Tier 2: medium complexity, the common case.
# Tier 3: tiny / repetitive / boilerplate (cheap model or programmatic).
TIER1 = {
    "Shared/Patches/Conveyor/MyGridConveyorSystemPatch.cs",
    "Shared/Patches/Conveyor/MyCubeGridPatchForConveyor.cs",
    "Shared/Patches/TargetingSystem/MyLargeTurretTargetingSystemPatch.cs",
    "Shared/Patches/ScriptCompiler/MyScriptCompilerPatch.cs",
    "Shared/Patches/SafeZone/MySafeZonePatch.cs",
    "Shared/Patches/Physics/MyClusterTreePatch.cs",
    "Shared/Patches/Memory/MyDefinitionIdToStringPatch.cs",
    "Shared/Patches/PatchHelpers.cs",
    "Shared/Tools/PreloaderHelpers.cs",
    "Shared/Tools/TranspilerHelpers.cs",
    "Shared/Tools/Cache.cs",
    "Shared/Tools/TwoLayerCache.cs",
    "Shared/Tools/UintCache.cs",
    "Shared/Tools/EnsureCode.cs",
    "Shared/Tools/RwLock.cs",
    "Shared/Tools/Hashing.cs",
    "ClientPlugin/Config.cs",
    "ClientPlugin/Plugin.cs",
    "ServerPlugin/Plugin.cs",
    "Shared/Plugin/Common.cs",
}


def module_for(path):
    for module_id, pred in MODULE_RULES:
        if pred(path):
            return module_id
    return "unclassified"


def tier_for(path, lines):
    if path in TIER1:
        return 1
    if lines <= 35:
        return 3
    return 2


def main():
    os.chdir(ROOT)
    files = subprocess.check_output(
        ["git", "ls-files", "*.cs"], text=True
    ).split()
    records = []
    for path in sorted(files):
        with open(path, "rb") as f:
            data = f.read()
        sha = hashlib.sha256(data).hexdigest()
        lines = data.count(b"\n") + (0 if data.endswith(b"\n") or not data else 1)
        rec = {
            "name": os.path.basename(path),
            "path": path,
            "doc": f"Docs/files/{path}.md",
            "lines": lines,
            "bytes": len(data),
            "sha256": sha,
            "module": module_for(path),
            "tier": tier_for(path, lines),
        }
        records.append(rec)

    out = os.path.join(ROOT, "Docs", "data", "manifest.jsonl")
    with open(out, "w") as f:
        for rec in records:
            f.write(json.dumps(rec) + "\n")

    # Summary to stderr so it doesn't pollute the data file.
    by_module = {}
    by_tier = {1: 0, 2: 0, 3: 0}
    for rec in records:
        by_module.setdefault(rec["module"], 0)
        by_module[rec["module"]] += 1
        by_tier[rec["tier"]] += 1
    print(f"{len(records)} files -> {out}", file=sys.stderr)
    print("By module:", file=sys.stderr)
    for m, n in sorted(by_module.items()):
        print(f"  {m:24s} {n}", file=sys.stderr)
    print(f"By tier: T1={by_tier[1]} T2={by_tier[2]} T3={by_tier[3]}", file=sys.stderr)


if __name__ == "__main__":
    main()
