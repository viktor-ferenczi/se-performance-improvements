"""Writes the members of the game's PB whitelist that the script eligibility scan checks.

    python3 whitelist.py <whitelist dump> <out file>

The dump comes from the PB probe: run rig.py with PB_PROBE_WHITELIST=<file> on any world. It
lists every member of the game assemblies the game allows in PB scripts. This keeps the ones
ScriptEligibility looks up in the policy table and drops the ones it allows without a lookup
(value types, enums, delegates, fields, VRageMath, the ingame utilities, MemorySafeTypes,
MyGridProgram, the instruction counter), Equals/GetHashCode/ToString/GetType overrides on
classes (scripts reach game objects through interfaces, so those calls bind to System.Object),
compiler-generated types and other plugins' types.

The output, one member per line, is Tests/Shared/Patches/ScriptWhitelist.txt; the test
ScriptPolicyTest.EveryWhitelistedMemberHasARow fails for a line the policy table lacks.
Refresh it after a game update.
"""

import csv
import sys

PLAIN_NAMESPACES = ("VRageMath", "VRage.Game.ModAPI.Ingame.Utilities", "VRage.Scripting.MemorySafeTypes")
GAME_NAMESPACES = ("Sandbox.", "SpaceEngineers.", "VRage.", "VRageRender.", "System.")
OBJECT_METHODS = ("Equals", "GetHashCode", "ToString", "GetType")


def checked(r: dict) -> bool:
    t, kind = r["type"], r["type_kind"]
    if kind in ("Enum", "Delegate") or r["member_kind"] == "Field":
        return False
    if kind == "Struct" and t != "VRage.Game.GUI.TextPanel.MySpriteDrawFrame":
        return False
    if any(t == ns or t.startswith(ns + ".") for ns in PLAIN_NAMESPACES):
        return False
    if t == "Sandbox.ModAPI.Ingame.MyGridProgram" or t.startswith("VRage.Library.Compiler.IlInjector"):
        return False
    if kind == "Class" and r["member"] in OBJECT_METHODS:
        return False
    return t.startswith(GAME_NAMESPACES) and "<" not in t


def main() -> None:
    with open(sys.argv[1], newline="") as f:
        rows = list(csv.DictReader(f, delimiter="\t"))
    members = sorted({f"{r['type']}.{r['member']}" for r in rows if checked(r)})
    with open(sys.argv[2], "w") as f:
        f.write("\n".join(members) + "\n")
    print(f"{len(members)} of {len(rows)} whitelisted members written to {sys.argv[2]}")


if __name__ == "__main__":
    main()
