"""SE-626 test: the Performance plugin's PB scheduler (FixScriptScheduler) on an isolated Magnetar server.

    PB_SLOT=<slot> python3 scheduler_test.py [--cases off,on,soft] <base world zip> <blueprint folders (one PB each)...>

Builds a world with COPIES copies of each blueprint, all PBs running a sort of about a millisecond
on every tick, and loads it three times with FixScriptCounter on:

    off  FixScriptScheduler off: every PB runs every frame
    on   FixScriptScheduler on at its defaults (5 ms budget): the PB time per frame stays within
         the budget plus one run, no PB waits much longer than the others, and the once a
         minute overload line is in the log
    soft ScriptSoftLimitMs=1: every PB is stopped on its third run (its Echo), with the soft limit message

Prints PASS and FAIL lines and exits 1 on any FAIL. Needs rig.py and runner_test.py next to it.
"""

import csv
import os
import shutil
import statistics
import subprocess
import sys
from collections import defaultdict
from pathlib import Path

HERE = Path(__file__).resolve().parent
HOME = Path.home()
sys.path.insert(0, str(HERE))
import runner_test  # noqa: E402

COPIES = 6
BUDGET_MS = 5  # the default ScriptFrameBudgetMs
WARMUP = 600
FRAMES = 3600

runner_test.PROGRAMS["load"] = (
    "List<int> list = new List<int>(); "
    "public Program() { for (int i = 0; i < 40000; i++) { list.Add(i); } Runtime.UpdateFrequency = UpdateFrequency.Update1; } "
    "int runs = 0; "
    'public void Main(string argument) { for (int i = 0; i < 4; i++) { list.Reverse(); list.Sort(); } Echo("run " + ++runs); }'
)

# name: options, warmup frames, measured frames
CASES = {
    "off": (["--option", "FixScriptCounter=true"], WARMUP, FRAMES),
    "on": (["--option", "FixScriptCounter=true", "--option", "FixScriptScheduler=true"], WARMUP, FRAMES),
    # Every run is over a 1 ms soft limit: each PB stops on its third run, counted from the first frame
    "soft": (["--option", "FixScriptCounter=true", "--option", "ScriptSoftLimitMs=1"], 0, 300),
}


def read_tsv(path: Path) -> list[dict]:
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t"))


def check(results: list[bool], ok: bool, text: str) -> None:
    results.append(ok)
    print(f"{'PASS' if ok else 'FAIL'} {text}", flush=True)


def run_case(task: str, world: Path, case: str, options: list[str], warmup: int, frames: int) -> Path:
    subprocess.run(
        [sys.executable, str(HERE / "rig.py"), str(world), "--performance", "--ram", "4"]
        + ["--warmup", str(warmup), "--frames", str(frames), *options],
        check=True,
        stdout=subprocess.DEVNULL,
    )
    root = HOME / f".se-test/{task}-ds0"
    logs = HOME / f".se-test/{task}-scheduler-test/{case}"
    shutil.rmtree(logs, ignore_errors=True)
    logs.mkdir(parents=True)
    for log in [root / "server.log", root / "magnetar/info.log", *root.glob("data/*.log"), *root.glob("probe/*")]:
        if log.is_file():
            shutil.copy(log, logs)
    return logs


def main() -> int:
    args = sys.argv[1:]
    if len(args) < 2:
        sys.exit(__doc__)
    cases = list(CASES)
    if args[0] == "--cases":
        cases, args = args[1].split(","), args[2:]
    slot = int(os.environ["PB_SLOT"])
    task = (HOME / f".se-test/slots/{slot}/owner").read_text().strip()
    blueprints = args[1:] * COPIES
    pb_count = len(blueprints)
    world = HOME / f".se-test/{task}-scheduler-test/PbSchedulerTest"
    runner_test.build(args[0], blueprints, world, [f"load{i}" for i in range(1, pb_count + 1)])

    results: list[bool] = []
    for case in cases:
        options, warmup, measured = CASES[case]
        logs = run_case(task, world, case, options, warmup, measured)
        pbs = [r for r in read_tsv(logs / "pbs.tsv") if r["name"].startswith("PBTEST load")]
        test_pbs = {r["pb"] for r in pbs}
        stopped = [r for r in pbs if r["instance"] != "True"]
        runs = [r for r in read_tsv(logs / "runs.tsv") if r["pb"] in test_pbs and r["depth"] == "0"]
        frames = read_tsv(logs / "frames.tsv")
        per_frame = [float(f["pb_us"]) / 1000 for f in frames]
        run_ms = [float(r["total_us"]) / 1000 for r in runs]
        per_pb = defaultdict(list)
        for r in runs:
            per_pb[r["pb"]].append(int(r["frame"]))
        counts = [len(per_pb[pb]) for pb in test_pbs]
        waits = [b - a - 1 for frames_of in per_pb.values() for a, b in zip(frames_of, frames_of[1:])]
        print(
            f"== {case}: {pb_count} PBs ({len(test_pbs)} found), {len(frames)} frames, "
            f"{len(runs) / max(1, len(frames)):.1f} runs/frame, run {statistics.mean(run_ms):.2f} ms mean "
            f"{max(run_ms):.2f} ms max, PB {statistics.mean(per_frame):.2f} ms/frame mean {max(per_frame):.2f} max, "
            f"runs per PB {min(counts)}..{max(counts)}, longest wait {max(waits, default=0)} frames",
            flush=True,
        )
        check(results, len(test_pbs) == pb_count, f"{case}: all {pb_count} test PBs found")
        text = "\n".join(p.read_text(errors="replace") for p in logs.glob("*.log"))
        overload = [ln for ln in text.splitlines() if "PB scheduler over budget" in ln]
        errors = [ln for ln in text.splitlines() if "Performance" in ln and ("Critical:" in ln or "Error:" in ln)]
        check(results, not errors, f"{case}: no Performance errors {errors[:1]}")
        if case == "soft":
            message = "limit in 3 runs in a row"
            check(
                results,
                len(stopped) == pb_count and all(message in r["info"] for r in stopped),
                f"{case}: all {pb_count} PBs stopped by the soft limit ({len(stopped)} stopped)",
            )
            # The probe's window opens after the first runs, so the third run is read from its Echo
            third = sum(1 for r in stopped if r["info"].startswith("run 3 "))
            check(results, third == pb_count, f"{case}: each PB stopped on its third run ({third} of {pb_count})")
            continue

        check(results, not stopped, f"{case}: no test PB stopped ({len(stopped)} stopped)")
        if case == "off":
            check(results, min(counts) >= len(frames) - 1, f"{case}: every PB ran every frame")
            check(results, not overload, f"{case}: no overload line")
            continue

        # The first run of a frame always starts, the others only if their reservation fits
        limit = BUDGET_MS + max(run_ms) + 0.5
        over = sum(1 for ms in per_frame if ms > limit)
        check(
            results,
            over <= len(frames) // 100,
            f"{case}: PB time within {limit:.2f} ms in {len(frames) - over} of {len(frames)} frames",
        )
        check(
            results,
            len(runs) < pb_count * len(frames) / 2,
            f"{case}: runs delayed ({len(runs)} runs in {len(frames)} frames)",
        )
        # Oldest first: no PB waits much longer than the PBs due over the runs per frame
        bound = 1.5 * pb_count * len(frames) / len(runs)
        check(
            results,
            max(waits) <= bound,
            f"{case}: longest wait {max(waits)} frames within {bound:.0f} (runs per PB {min(counts)}..{max(counts)})",
        )
        check(results, bool(overload), f"{case}: overload line logged: {overload[:1]}")

    failed = not all(results)
    print("== RESULT", "FAIL" if failed else "PASS")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
