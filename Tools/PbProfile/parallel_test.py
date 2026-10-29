"""SE-626 stage 4 test: PB scripts with proxies in the parallel phase, on an isolated Magnetar server.

    PB_SLOT=<slot> python3 parallel_test.py [--cases serial,parallel] <base world zip> <blueprint folders (one PB each)...>

Builds a world with COPIES copies of each blueprint, one grid group each. Every PB runs the
"state" script below on every tick: a sort of about a millisecond, a CustomData write read back
in the same run, a text surface write, a GetBlocksOfType with a callback, and the PB's position
and name in its Echo. With FixScriptCounter, FixScriptScheduler and FixScriptProxies on:

    serial    one PB thread: everything runs on the main thread, the proxies record and replay
              nothing as no write log is active there
    parallel  4 PB threads: the scripts run in the parallel phase (soft limit 50 ms, see CASES)

Both cases check that no PB was stopped, that each PB's DetailedInfo and CustomData are from the
same, last run and that the script read its own CustomData write back. The parallel case also
checks that the runs were made on the workers and that about 4 times as many ran per frame,
and prints the mean and 95th percentile of the session update time of both.

Prints PASS and FAIL lines and exits 1 on any FAIL. Needs rig.py and runner_test.py next to it.
"""

import csv
import os
import re
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
THREADS = 4
WARMUP = int(os.environ.get("PB_TEST_WARMUP", 600))
FRAMES = 3600

runner_test.PROGRAMS["state"] = (
    "List<int> list = new List<int>(); List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); "
    "public Program() { for (int i = 0; i < 40000; i++) { list.Add(i); } Runtime.UpdateFrequency = UpdateFrequency.Update1; } "
    "int runs = 0; "
    "public void Main(string argument) { "
    "for (int i = 0; i < 4; i++) { list.Reverse(); list.Sort(); } "
    'runs++; Me.CustomData = "run " + runs; bool back = Me.CustomData == "run " + runs; '
    'Me.GetSurface(0).WriteText("run " + runs); '
    "GridTerminalSystem.GetBlocksOfType(blocks, b => b.IsSameConstructAs(Me)); "
    'Echo("run " + runs + " back=" + back + " blocks=" + blocks.Count + " pos=" + Me.GetPosition().Length().ToString("0") + " name=" + Me.CustomName); }'
)

OPTIONS = [
    "--option",
    "FixScriptCounter=true",
    "--option",
    "FixScriptScheduler=true",
    "--option",
    "FixScriptProxies=true",
]
# About 4 s after the world is ready, every run in this world takes about 7 times as long for
# 40 frames, on the main thread and the workers alike. With 4 threads every PB runs every few
# frames, so the default 5 ms soft limit sees 3 slow runs in a row and stops every script; the
# parallel case raises it. PB_TEST_WARMUP=0 shows the stall in runs.tsv.
CASES = {
    "serial": [*OPTIONS, "--option", "ScriptThreadCountMode=Manual", "--option", "ScriptThreadCount=1"],
    "parallel": [
        *OPTIONS,
        "--option",
        "ScriptThreadCountMode=Manual",
        "--option",
        f"ScriptThreadCount={THREADS}",
        "--option",
        "ScriptSoftLimitMs=50",
    ],
}


def read_tsv(path: Path) -> list[dict]:
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t"))


def check(results: list[bool], ok: bool, text: str) -> None:
    results.append(ok)
    print(f"{'PASS' if ok else 'FAIL'} {text}", flush=True)


def run_case(task: str, world: Path, case: str, options: list[str]) -> Path:
    subprocess.run(
        [sys.executable, str(HERE / "rig.py"), str(world), "--performance", "--ram", "4"]
        + ["--warmup", str(WARMUP), "--frames", str(FRAMES), *options],
        check=True,
        stdout=subprocess.DEVNULL,
    )
    root = HOME / f".se-test/{task}-ds0"
    logs = HOME / f".se-test/{task}-parallel-test/{case}"
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
    world = HOME / f".se-test/{task}-parallel-test/PbParallelTest"
    runner_test.build(args[0], blueprints, world, [f"state{i}" for i in range(1, pb_count + 1)])

    results: list[bool] = []
    runs_per_frame = {}
    for case in cases:
        logs = run_case(task, world, case, CASES[case])
        pbs = [r for r in read_tsv(logs / "pbs.tsv") if r["name"].startswith("PBTEST state")]
        test_pbs = {r["pb"] for r in pbs}
        runs = [r for r in read_tsv(logs / "runs.tsv") if r["pb"] in test_pbs and r["depth"] == "0"]
        frames = read_tsv(logs / "frames.tsv")
        session = sorted(float(f["session_us"]) / 1000 for f in frames)
        per_pb = defaultdict(int)
        for r in runs:
            per_pb[r["pb"]] += 1
        off_main = sum(1 for r in runs if r["main"] == "0")
        runs_per_frame[case] = len(runs) / max(1, len(frames))
        print(
            f"== {case}: {pb_count} PBs ({len(test_pbs)} found), {len(frames)} frames, "
            f"{runs_per_frame[case]:.1f} runs/frame ({off_main} of {len(runs)} off the main thread), "
            f"session {statistics.mean(session):.2f} ms mean {session[int(len(session) * 0.95)]:.2f} ms p95, "
            f"runs per PB {min(per_pb.values(), default=0)}..{max(per_pb.values(), default=0)}",
            flush=True,
        )
        for where, rows in (
            ("main", [r for r in runs if r["main"] == "1"]),
            ("workers", [r for r in runs if r["main"] == "0"]),
        ):
            script = sorted(float(r["script_us"]) / 1000 for r in rows)
            total = sorted(float(r["total_us"]) / 1000 for r in rows)
            if script:
                print(
                    f"   {where}: {len(rows)} runs, script {statistics.mean(script):.2f} ms mean {script[int(len(script) * 0.95)]:.2f} p95 {script[-1]:.2f} max, "
                    f"total {statistics.mean(total):.2f} ms mean {total[-1]:.2f} max",
                    flush=True,
                )
        check(results, len(test_pbs) == pb_count, f"{case}: all {pb_count} test PBs found")
        text = "\n".join(p.read_text(errors="replace") for p in logs.glob("*.log"))
        errors = [ln for ln in text.splitlines() if "Performance" in ln and ("Critical:" in ln or "Error:" in ln)]
        check(results, not errors, f"{case}: no Performance errors {errors[:1]}")
        failed = [ln for ln in text.splitlines() if "writes of scripts run off the main thread failed" in ln]
        check(results, not failed, f"{case}: no failed writes {failed[:1]}")

        stopped = [r for r in pbs if r["instance"] != "True"]
        check(
            results,
            not stopped,
            f"{case}: no test PB stopped ({len(stopped)} stopped) {[r['info'] for r in stopped][:1]}",
        )
        bad = []
        for r in pbs:
            m = re.match(r"run (\d+) back=True blocks=\d+ pos=\d+ name=PBTEST state", r["info"])
            if not m or r["custom_data"] != f"run {m.group(1)}":
                bad.append(f"{r['info'][:80]!r} / {r['custom_data']!r}")
        check(
            results,
            not bad,
            f"{case}: DetailedInfo and CustomData from the same last run, read back ({len(bad)} bad) {bad[:2]}",
        )

        if case == "serial":
            check(results, off_main == 0, f"{case}: every run on the main thread")
        else:
            check(results, off_main > len(runs) // 4, f"{case}: runs on the workers ({off_main} of {len(runs)})")
            share = [ln for ln in text.splitlines() if "of the runs in parallel" in ln]
            print(f"   {share[-1].strip() if share else 'no scheduler line'}")

    if "serial" in runs_per_frame and "parallel" in runs_per_frame:
        ratio = runs_per_frame["parallel"] / max(0.1, runs_per_frame["serial"])
        check(results, ratio > THREADS * 0.6, f"parallel ran {ratio:.1f} times the runs per frame of serial")

    failed = not all(results)
    print("== RESULT", "FAIL" if failed else "PASS")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
