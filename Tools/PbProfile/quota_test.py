"""SE-1370 test: the Performance plugin's per-player PB time quota (ScriptPlayerQuota) on an isolated
Magnetar server.

    PB_SLOT=<slot> python3 quota_test.py [--cases gate,report,hard,soft,defaults] <PbSchedulerTest world zip>

The world is the PB scheduler test world (testing/windows-test-kits/tm-1460-pb-counter/worlds in
the workspace): 36 PBs of one player, each sorting a list every tick, about 1.5 ms a run on
Linux, so the player's PBs run about 54 ms a frame. Each case loads it for 600 frames:

    gate      Enforce with 0.1 ms limits but FixScriptCounter off: the quota stays off, no PB is turned off
    report    Report with 0.1 ms limits: no PB is turned off, the "would turn off" line is logged
    hard      Enforce, hard limit 0.1 ms: every PB is turned off, one per frame
    soft      Enforce, soft limit 0.1 ms, hard 1000 ms: every PB is turned off, one per 3 frames
    defaults  Enforce at the default quota limits (10 ms soft, 100 ms hard): PBs are turned off until
              the rest fit about 10 ms, the turned off ones say why in their detailed info

Prints PASS and FAIL lines and exits 1 on any FAIL. Needs rig.py next to it.
"""

import csv
import os
import re
import shutil
import subprocess
import sys
import zipfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
HOME = Path.home()

PB_COUNT = 36
FRAMES = 600
COUNTER = ["--option", "FixScriptCounter=true"]
TINY = ["--option", "ScriptPlayerSoftLimitMs=0.1", "--option", "ScriptPlayerHardLimitMs=0.1"]

CASES = {
    "gate": ["--option", "ScriptPlayerQuota=Enforce", *TINY],
    "report": [*COUNTER, "--option", "ScriptPlayerQuota=Report", *TINY],
    "hard": [
        *COUNTER,
        *("--option", "ScriptPlayerQuota=Enforce"),
        *("--option", "ScriptPlayerSoftLimitMs=1000", "--option", "ScriptPlayerHardLimitMs=0.1"),
    ],
    "soft": [
        *COUNTER,
        *("--option", "ScriptPlayerQuota=Enforce"),
        *("--option", "ScriptPlayerSoftLimitMs=0.1", "--option", "ScriptPlayerHardLimitMs=1000"),
    ],
    # The per-PB soft limit (5 ms in 3 runs) would stop scripts after a stall, so it is raised here
    "defaults": [*COUNTER, "--option", "ScriptPlayerQuota=Enforce", "--option", "ScriptSoftLimitMs=100"],
}

TURNED_OFF = re.compile(r"PB player quota turned off PB .*?\((\d+)\) on .*\(tick (\d+)\)")


def read_tsv(path: Path) -> list[dict]:
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t"))


def check(results: list[bool], ok: bool, text: str) -> None:
    results.append(ok)
    print(f"{'PASS' if ok else 'FAIL'} {text}", flush=True)


def run_case(task: str, world: Path, case: str) -> Path:
    subprocess.run(
        [sys.executable, str(HERE / "rig.py"), str(world), "--performance", "--ram", "4"]
        + ["--warmup", "0", "--frames", str(FRAMES), *CASES[case]],
        check=True,
        stdout=subprocess.DEVNULL,
    )
    root = HOME / f".se-test/{task}-ds0"
    logs = HOME / f".se-test/{task}-quota-test/{case}"
    shutil.rmtree(logs, ignore_errors=True)
    logs.mkdir(parents=True)
    for log in [root / "server.log", root / "magnetar/info.log", *root.glob("data/*.log"), *root.glob("probe/*")]:
        if log.is_file():
            shutil.copy(log, logs)
    return logs


def main() -> int:
    args = sys.argv[1:]
    cases = list(CASES)
    if args[:1] == ["--cases"]:
        cases, args = args[1].split(","), args[2:]
    if len(args) != 1:
        sys.exit(__doc__)
    slot = int(os.environ["PB_SLOT"])
    task = (HOME / f".se-test/slots/{slot}/owner").read_text().strip()
    base = HOME / f".se-test/{task}-quota-test"
    shutil.rmtree(base / "PbSchedulerTest", ignore_errors=True)
    base.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(args[0]) as z:
        z.extractall(base)
    world = base / "PbSchedulerTest"

    results: list[bool] = []
    for case in cases:
        logs = run_case(task, world, case)
        pbs = [r for r in read_tsv(logs / "pbs.tsv") if r["name"].startswith("PBTEST load")]
        off = [r for r in pbs if r["enabled"] != "True"]
        text = "\n".join(p.read_text(errors="replace") for p in logs.glob("*.log"))
        lines = text.splitlines()
        turned = [(int(m.group(1)), int(m.group(2))) for m in map(TURNED_OFF.search, lines) if m]
        ticks = sorted(t for _, t in turned)
        gaps = [b - a for a, b in zip(ticks, ticks[1:])]
        would = [ln for ln in lines if "PB player quota would turn off" in ln]
        errors = [ln for ln in lines if "Performance" in ln and ("Critical:" in ln or "Error:" in ln)]
        print(
            f"== {case}: {len(pbs)} test PBs, {len(off)} turned off, {len(turned)} turn-off lines, "
            f"tick gaps {min(gaps, default=0)}..{max(gaps, default=0)}, {len(would)} report lines",
            flush=True,
        )
        check(results, len(pbs) == PB_COUNT, f"{case}: all {PB_COUNT} test PBs found ({len(pbs)})")
        check(results, not errors, f"{case}: no Performance errors {errors[:1]}")
        stopped = [r for r in pbs if r["instance"] != "True"]
        check(results, not stopped, f"{case}: no script stopped, turning off keeps the script ({len(stopped)})")

        if case in ("gate", "report"):
            check(results, not off and not turned, f"{case}: no PB turned off ({len(off)})")
            if case == "report":
                check(results, bool(would), f"{case}: report line logged: {would[:1]}")
            else:
                check(results, not would, f"{case}: no report line")
            continue

        why = [r for r in off if "Turned off by the server" in r["info"]]
        check(results, len(why) == len(off), f"{case}: every turned off PB says why ({len(why)} of {len(off)})")
        check(results, len(turned) == len(off), f"{case}: one log line per turned off PB ({len(turned)})")
        if case == "hard":
            check(results, len(off) == PB_COUNT, f"{case}: every PB turned off ({len(off)})")
            check(results, min(gaps, default=1) >= 1, f"{case}: at most one PB per frame (min gap {min(gaps, default=None)})")
        elif case == "soft":
            check(results, len(off) == PB_COUNT, f"{case}: every PB turned off ({len(off)})")
            check(results, min(gaps, default=3) >= 3, f"{case}: one PB per 3 frames at most (min gap {min(gaps, default=None)})")
        else:
            # About 1.5 ms a PB: the 10 ms soft limit leaves a handful running
            on = PB_COUNT - len(off)
            check(results, 2 <= on <= 12, f"{case}: {on} PBs left running, about 10 ms worth")
            check(results, min(gaps, default=3) >= 3, f"{case}: only the soft limit acted (min gap {min(gaps, default=None)})")

    failed = not all(results)
    print("== RESULT", "FAIL" if failed else "PASS")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
