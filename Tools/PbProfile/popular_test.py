"""SE-626 test: popular PB scripts with the Performance plugin at its defaults, against vanilla.

    PB_SLOT=<slot> python3 popular_test.py [--cases vanilla,default,pb] <world dir>

Loads the world on an isolated Magnetar server once per case and compares the state of every PB
at the end with the vanilla run:

    vanilla  no Performance plugin
    default  the Performance plugin with its default config
    counter  the plugin with the per-thread PB counter on (the hard and soft run limits)
    scheduler    the counter and the PB frame budget on
    pb       every PB option on: counter, frame budget, proxies, Auto threads
    standins pb with the soft limit at 100 ms and the thread decisions logged; the default
             mechanisms (StandIn) let scripts on a worker thread
    all      standins with every mechanism on, Sensing and Transfers too

The world comes from build_world.py: the SE-633 one from the most-subscribed ship and station
blueprints, or one from Workshop blueprints whose PBs carry popular Workshop scripts.

A PB fails a case if it ran in vanilla but not in the case, if its script instance is gone only
in the case, or if its DetailedInfo shows an exception or a termination only in the case. The
default case also needs no Performance errors in the logs. The other cases only report, as the PB
options are off by default and the frame budget and soft limit delay or stop scripts on purpose.
DetailedInfo texts that differ with the digits taken out are listed for review.

Prints PASS and FAIL lines and exits 1 on any FAIL of the default case.
"""

import csv
import os
import re
import shutil
import subprocess
import sys
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent
HOME = Path.home()

WARMUP = int(os.environ.get("PB_TEST_WARMUP", 1200))
FRAMES = int(os.environ.get("PB_TEST_FRAMES", 3600))
RAM = os.environ.get("PB_TEST_RAM", "10")

CASES = {
    "vanilla": None,
    "default": [],
    "counter": ["--option", "FixScriptCounter=true"],
    "scheduler": [*("--option", "FixScriptCounter=true"), *("--option", "FixScriptScheduler=true")],
    "pb": [
        *("--option", "FixScriptCounter=true"),
        *("--option", "FixScriptScheduler=true"),
        *("--option", "FixScriptProxies=true"),
    ],
}
# On a worker thread with the default mechanisms, and with all of them; the soft limit is raised,
# so it does not stop scripts the comparison is about
CASES["standins"] = [
    *CASES["pb"],
    *("--option", "ScriptSoftLimitMs=100"),
    *("--option", "ScriptLogThreadDecisions=true"),
]
CASES["all"] = [
    *CASES["standins"],
    *("--option", "ScriptOffMainSensing=true"),
    *("--option", "ScriptOffMainTransfers=true"),
]
DECISION = re.compile(r'PB thread decision: "(.*)" \((\d+)\) on "(.*)": (.*)$')

PROBLEM = re.compile(r"exception|terminated|too complex|took too long|stopped|error", re.I)


def read_tsv(path: Path) -> list[dict]:
    csv.field_size_limit(1 << 24)
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t", quoting=csv.QUOTE_NONE))


def check(results: list[bool], ok: bool, text: str) -> None:
    results.append(ok)
    print(f"{'PASS' if ok else 'FAIL'} {text}", flush=True)


def run_case(task: str, world: Path, label: str, options: list[str] | None) -> Path:
    args = [sys.executable, str(HERE / "rig.py"), str(world), "--ram", RAM]
    args += ["--warmup", str(WARMUP), "--frames", str(FRAMES)]
    if options is not None:
        args += ["--performance", *options]
    subprocess.run(args, check=True, stdout=subprocess.DEVNULL)
    root = HOME / f".se-test/{task}-ds0"
    logs = HOME / f".se-test/{task}-popular-test/{label}"
    shutil.rmtree(logs, ignore_errors=True)
    logs.mkdir(parents=True)
    for log in [root / "server.log", root / "magnetar/info.log", *root.glob("data/*.log"), *root.glob("probe/*")]:
        if log.is_file():
            shutil.copy(log, logs)
    return logs


def state(logs: Path) -> tuple[dict, Counter, str]:
    pbs = {r["pb"]: r for r in read_tsv(logs / "pbs.tsv") if r["program_chars"] not in ("", "0")}
    runs = Counter(r["pb"] for r in read_tsv(logs / "runs.tsv"))
    text = "\n".join(p.read_text(errors="replace") for p in logs.glob("*.log"))
    return pbs, runs, text


def label(r: dict) -> str:
    return f"{r['grid'][:30]} / {r['name'][:30]}"


def main() -> int:
    args = sys.argv[1:]
    cases = ["vanilla", "default", "pb"]
    if args and args[0] == "--cases":
        cases, args = args[1].split(","), args[2:]
    slot = int(os.environ["PB_SLOT"])
    task = (HOME / f".se-test/slots/{slot}/owner").read_text().strip()
    world = Path(args[0])
    name = world.name

    results: list[bool] = []
    states = {}
    for case in cases:
        logs = run_case(task, world, f"{name}-{case}", CASES[case])
        states[case] = state(logs)
        pbs, runs, text = states[case]
        running = sum(1 for r in pbs.values() if r["instance"] == "True")
        print(
            f"== {name} {case}: {len(pbs)} PBs with a program, {running} with a script instance, "
            f"{sum(1 for p in pbs if runs[p])} ran, {sum(runs.values())} runs",
            flush=True,
        )
        if case == "vanilla":
            continue

        gate = results if case == "default" else []
        if CASES[case] is not None:
            errors = [ln for ln in text.splitlines() if "Performance" in ln and ("Critical:" in ln or "Error:" in ln)]
            check(gate, not errors, f"{name} {case}: no Performance errors {errors[:2]}")
        if "vanilla" not in states:
            continue
        base_pbs, base_runs, _ = states["vanilla"]
        # Projections get new entity ids on every load, so only the PBs that ran or had a script count
        missing = [p for p in base_pbs if p not in pbs and (base_runs[p] or base_pbs[p]["instance"] == "True")]
        not_ran = [label(base_pbs[p]) for p in base_pbs if base_runs[p] and p in pbs and not runs[p]]
        lost = [
            f"{label(pbs[p])}: {pbs[p]['info'][:120]!r}"
            for p in base_pbs
            if p in pbs and base_pbs[p]["instance"] == "True" and pbs[p]["instance"] != "True"
        ]
        broke = [
            f"{label(pbs[p])}: {pbs[p]['info'][:160]!r}"
            for p in base_pbs
            if p in pbs and PROBLEM.search(pbs[p]["info"]) and not PROBLEM.search(base_pbs[p]["info"])
        ]
        fixed = [
            label(pbs[p])
            for p in base_pbs
            if p in pbs and PROBLEM.search(base_pbs[p]["info"]) and not PROBLEM.search(pbs[p]["info"])
        ]
        check(gate, not missing, f"{name} {case}: every vanilla PB present ({len(missing)} missing)")
        check(gate, not not_ran, f"{name} {case}: every PB that ran in vanilla ran ({len(not_ran)} not) {not_ran[:5]}")
        check(gate, not lost, f"{name} {case}: no script instance lost ({len(lost)})")
        for x in lost[:20]:
            print(f"   lost: {x}")
        check(gate, not broke, f"{name} {case}: no new exception or termination ({len(broke)})")
        for x in broke[:20]:
            print(f"   new problem: {x}")
        if fixed:
            print(f"   problem in vanilla only ({len(fixed)}): {fixed[:10]}")

        digits = re.compile(r"[\d.,:]+")
        differ = [
            p for p in base_pbs if p in pbs and digits.sub("#", base_pbs[p]["info"]) != digits.sub("#", pbs[p]["info"])
        ]
        print(f"   DetailedInfo differs without digits: {len(differ)} of {len(base_pbs)}")
        for p in differ[:15]:
            print(
                f"     {label(pbs[p])}\n       vanilla: {base_pbs[p]['info'][:150]!r}\n       {case}: {pbs[p]['info'][:150]!r}"
            )
        if case in ("pb", "standins", "all"):
            share = [ln.strip() for ln in text.splitlines() if "of the runs in parallel" in ln]
            print(f"   {share[-1] if share else 'no parallel scheduler line'}")
        decisions = {}
        for ln in text.splitlines():
            m = DECISION.search(ln)
            if m:
                decisions[m.group(2)] = m.group(4)
        if decisions:
            on_worker = sum(1 for d in decisions.values() if d.startswith("on a worker"))
            print(f"   thread decisions: {on_worker} of {len(decisions)} PBs on a worker thread")
            reasons = Counter(re.sub(r" \(.*", "", d) for d in decisions.values() if not d.startswith("on a worker"))
            for reason, count in reasons.most_common(12):
                print(f"     {count:3} {reason[:150]}")

    failed = not all(results)
    print(f"== RESULT {name}", "FAIL" if failed else "PASS", flush=True)
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
