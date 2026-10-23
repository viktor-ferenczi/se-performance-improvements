"""Summarizes the PB probe tables of one rig run as Markdown.

    python3 analyze.py <probe dir> [--corpus <scan corpus dir> --titles <details.json>]

With a corpus, each program is named after the corpus script it shares most 8-word shingles
with (at least 30 %). A speedscope trace in the probe dir (rig.py --trace) adds the split of
PB time into script code, game API and runner overhead.
"""

import argparse
import csv
import heapq
import json
import re
import statistics
from collections import Counter, defaultdict
from pathlib import Path

UPDATE_TYPES = {
    1: "Terminal",
    2: "Trigger",
    8: "Mod",
    16: "Script",
    32: "Update1",
    64: "Update10",
    128: "Update100",
    256: "Once",
    512: "IGC",
}


def read(path: Path) -> list[dict]:
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t"))


def pct(values: list[float], q: float) -> float:
    if not values:
        return 0.0
    values = sorted(values)
    return values[min(len(values) - 1, int(q * len(values)))]


def makespan(durations: list[float], workers: int) -> float:
    """Longest-processing-time-first schedule: an upper bound close to the best split"""
    loads = [0.0] * workers
    for d in sorted(durations, reverse=True):
        heapq.heapreplace(loads, loads[0] + d)
    return max(loads)


def shingles(text: str) -> set[int]:
    words = re.findall(r"\w+", text)
    return {hash(" ".join(words[i : i + 8])) for i in range(max(0, len(words) - 7))}


def identify(programs: Path, corpus: Path | None, titles: dict[str, str]) -> dict[str, str]:
    names = {}
    refs = {}
    if corpus:
        for d in corpus.iterdir():
            text = "\n".join(p.read_text(errors="replace") for p in d.glob("*.cs"))
            refs[d.name] = shingles(text)
    for p in programs.glob("*.cs"):
        text = p.read_text(errors="replace")
        own = shingles(text)
        best, score = None, 0.0
        for rid, ref in refs.items():
            if own and ref:
                s = len(own & ref) / min(len(own), len(ref))
                if s > score:
                    best, score = rid, s
        if best and score >= 0.3:
            names[p.stem] = f"{titles.get(best, best)[:40]} ({score:.0%})"
        else:
            comment = next((ln.strip(" /*\t") for ln in text.splitlines() if re.search(r"[A-Za-z]{4}", ln)), "")
            names[p.stem] = "? " + comment[:40]
    return names


def trace_split(probe: Path) -> list[str]:
    files = list(probe.glob("*.speedscope.json"))
    if not files:
        return []
    data = json.load(open(files[0]))
    frames = [f["name"] for f in data["shared"]["frames"]]
    totals = Counter()
    api = Counter()
    for profile in data["profiles"]:
        if profile.get("type") != "evented":
            continue
        stack: list[int] = []
        last = None
        for event in profile["events"]:
            if last is not None and stack:
                kind, member = classify([frames[i] for i in stack])
                if kind:
                    totals[kind] += event["at"] - last
                    if member:
                        api[member] += event["at"] - last
            last = event["at"]
            if event["type"] == "O":
                stack.append(event["frame"])
            elif stack:
                stack.pop()
    whole = sum(totals.values())
    if not whole:
        return ["No PB run seen in the trace."]
    out = ["| part | share of PB run time |", "|---|---|"]
    out += [f"| {k} | {v / whole:.1%} |" for k, v in totals.most_common()]
    pre = [(k, v) for k, v in api.most_common() if k.startswith("pre-run: ")]
    calls = [(k, v) for k, v in api.most_common() if not k.startswith("pre-run: ")]
    out += ["", "Before the script, by callee (top 10):", "", "| callee | share |", "|---|---|"]
    out += [f"| `{k[9:]}` | {v / whole:.1%} |" for k, v in pre[:10]]
    out += ["", "Game API members by sampled time (top 25):", "", "| member | share |", "|---|---|"]
    out += [f"| `{k}` | {v / whole:.1%} |" for k, v in calls[:25]]
    return out


def classify(stack: list[str]) -> tuple[str | None, str | None]:
    """root..leaf frame names to (part, API member) for a sample inside a PB run"""
    # dotnet-trace ends stacks with pseudo frames like CPU_TIME, which have no module, and a
    # sample stops the thread at a GC poll, which adds Thread.PollGC frames below the real leaf
    while stack and "!" not in stack[-1]:
        stack = stack[:-1]
    stack = stack[: next((i for i, f in enumerate(stack) if "PollGC" in f), len(stack))]
    core = next((i for i, f in enumerate(stack) if "RunSandboxedProgramActionCore" in f), None)
    if core is None:
        # Inside ExecuteCode before the script: the terminal bind and ownership writes
        run = next((i for i, f in enumerate(stack) if "RunSandboxedProgramAction" in f), None)
        if run is None:
            return None, None
        below = stack[run + 1 : run + 2]
        return "pre-run (before the script)", "pre-run: " + (frame_name(below[0]) if below else "itself")
    script = [i for i in range(core, len(stack)) if is_script(stack[i])]
    if not script:
        return "runner (outside the script)", None
    below = stack[script[-1] + 1 :]
    if not below:
        return "script code", None
    callee = below[0]
    if "IlInjector" in callee:
        return "instruction counter", None
    module = callee.split("!", 1)[0]
    if module.startswith(("Sandbox", "SpaceEngineers", "VRage.Game", "VRage!", "VRage.Library")) or module == "VRage":
        return "game API", frame_name(callee)
    if module.startswith("VRage.Math"):
        return "script code (VRageMath)", None
    return "script code (BCL)", None


def frame_name(frame: str) -> str:
    return frame.split("!", 1)[-1].split("(", 1)[0]


def is_script(frame: str) -> bool:
    module, _, name = frame.partition("!")
    return name.startswith("Program.") or name.startswith("Program+")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("probe", type=Path)
    ap.add_argument("--corpus", type=Path)
    ap.add_argument("--titles", type=Path)
    args = ap.parse_args()
    titles = {}
    if args.titles:
        titles = {x["publishedfileid"]: x.get("title", "") for x in json.load(open(args.titles))}

    runs = read(args.probe / "runs.tsv")
    frames = read(args.probe / "frames.tsv")
    pbs = read(args.probe / "pbs.tsv")
    compiles = read(args.probe / "compile.tsv")
    names = identify(args.probe / "programs", args.corpus, titles)

    top = [r for r in runs if r["depth"] == "0"]
    total = [float(r["total_us"]) for r in top]
    script = [float(r["script_us"]) for r in top]
    session = [float(f["session_us"]) for f in frames]
    pb_frame = [float(f["pb_us"]) for f in frames]
    runs_frame = [int(f["pb_runs"]) for f in frames]
    n = len(frames)

    out = [f"# PB probe summary: {args.probe}", ""]
    with_program = [p for p in pbs if p["program_chars"] not in ("", "0")]
    out += [
        "## Census",
        "",
        f"- PBs: {len(pbs)}, with a program: {len(with_program)}, "
        f"with a live instance: {sum(p['instance'] == 'True' for p in with_program)}, "
        f"ran in the window: {sum(p['ran'] == 'True' for p in pbs)}",
        f"- termination reasons: {dict(Counter(p['termination'] for p in with_program))}",
        f"- distinct programs: {len({p['program_sha1'] for p in with_program})}",
        f"- compiles: {len(compiles)}, total {sum(float(c['compile_us']) for c in compiles) / 1e3:.0f} ms, "
        f"max {max((float(c['compile_us']) for c in compiles), default=0) / 1e3:.0f} ms",
        "",
        "## Per frame",
        "",
        f"{n} frames measured.",
        "",
        "| | mean | p50 | p95 | p99 | max |",
        "|---|---|---|---|---|---|",
    ]
    for label, values, unit in (
        ("session update (ms)", session, 1e3),
        ("PB time (ms)", pb_frame, 1e3),
        ("PB runs", runs_frame, 1),
    ):
        out.append(
            f"| {label} | {statistics.mean(values) / unit:.3f} | {pct(values, .5) / unit:.3f} | "
            f"{pct(values, .95) / unit:.3f} | {pct(values, .99) / unit:.3f} | {max(values) / unit:.3f} |"
        )
    out += [
        "",
        f"PB share of the session update: {sum(pb_frame) / max(1.0, sum(session)):.1%}",
        "",
        "## Single runs (top level, TryRun inside)",
        "",
        f"{len(top)} runs, {len(runs) - len(top)} nested.",
        "",
        "| | p50 | p90 | p99 | p99.9 | max | mean |",
        "|---|---|---|---|---|---|---|",
    ]
    for label, values in (("total (µs)", total), ("script (µs)", script)):
        out.append(
            f"| {label} | {pct(values, .5):.1f} | {pct(values, .9):.1f} | {pct(values, .99):.1f} | "
            f"{pct(values, .999):.1f} | {max(values, default=0):.1f} | {statistics.mean(values or [0]):.1f} |"
        )
    whole = sum(total) or 1.0
    out += [
        "",
        f"Outside the script (ownership writes, terminal bind, runner): {1 - sum(script) / whole:.1%} of PB time.",
        f"Runs over 1 ms: {sum(t > 1000 for t in total)} with {sum(t for t in total if t > 1000) / whole:.1%} of PB time; "
        f"over 5 ms: {sum(t > 5000 for t in total)} with {sum(t for t in total if t > 5000) / whole:.1%}.",
        "",
        "Update types: "
        + ", ".join(
            f"{name} {sum(1 for r in top if int(r['update_type']) & bit)}" for bit, name in UPDATE_TYPES.items()
        ),
        "",
        "## Parallel potential",
        "",
        "Main-thread PB time per frame if each frame's script parts were spread over N workers "
        '(LPT schedule, no scheduling overhead). "pre-run kept" leaves the part outside the script '
        '(ownership writes, terminal bind) serial on the main thread as it is today; "pre-run gone" '
        "assumes it is cached away.",
        "",
        "| workers | pre-run kept (ms/frame) | of serial | pre-run gone (ms/frame) | of serial | worst frame, kept (ms) |",
        "|---|---|---|---|---|---|",
    ]
    by_frame = defaultdict(list)
    pre_frame = defaultdict(float)
    for r in top:
        by_frame[r["frame"]].append(float(r["script_us"]))
        pre_frame[r["frame"]] += float(r["total_us"]) - float(r["script_us"])
    serial = sum(total) or 1.0
    for w in (1, 2, 4, 8, 1000):
        spans = {f: makespan(v, w) for f, v in by_frame.items()}
        kept = [spans[f] + pre_frame[f] for f in spans]
        label = "∞ (longest run)" if w == 1000 else str(w)
        out.append(
            f"| {label} | {sum(kept) / max(1, n) / 1e3:.3f} | {sum(kept) / serial:.0%} | "
            f"{sum(spans.values()) / max(1, n) / 1e3:.3f} | {sum(spans.values()) / serial:.0%} | "
            f"{max(kept, default=0) / 1e3:.2f} |"
        )

    per_pb = defaultdict(list)
    for r in top:
        per_pb[r["pb"]].append(float(r["total_us"]))
    info = {p["pb"]: p for p in pbs}
    ranked = sorted(per_pb.items(), key=lambda kv: -sum(kv[1]))
    shares = [sum(v) / whole for _, v in ranked]
    out += [
        "",
        "## Concentration",
        "",
        f"{len(ranked)} PBs ran. Top 1 has {sum(shares[:1]):.0%} of PB time, top 5 {sum(shares[:5]):.0%}, "
        f"top 10 {sum(shares[:10]):.0%}.",
        "",
        "| PB | program | runs | mean µs | p99 µs | max µs | share |",
        "|---|---|---|---|---|---|---|",
    ]
    for pb, v in ranked[:25]:
        p = info.get(pb, {})
        out.append(
            f"| {p.get('name', pb)[:30]} | {names.get(p.get('program_sha1', ''), '')} | {len(v)} | "
            f"{statistics.mean(v):.0f} | {pct(v, .99):.0f} | {max(v):.0f} | {sum(v) / whole:.1%} |"
        )

    split = trace_split(args.probe)
    if split:
        out += ["", "## Script code vs API (sampled)", ""] + split
    print("\n".join(out))


if __name__ == "__main__":
    main()
