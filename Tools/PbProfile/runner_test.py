"""SE-626 stage 2 test: the Performance plugin's ScriptRunner on an isolated Magnetar server.

    PB_SLOT=<slot> python3 runner_test.py [--client | --bench] <base world zip> <blueprint folders (6, one PB each)...>

Builds a world from six one-grid blueprints, gives their PBs the test programs below and runs
the probe's RunnerTest (probe-plugin/Source/RunnerTest.cs) three times:

    off      FixScriptCounter off: running on a worker thread is refused
    vanilla  FixScriptCounter on, the 3000 ms limit: everything runs, the runaway script stops
    limit    FixScriptCounter on, ScriptRunLimitMs=100: the slow script stops with the new text

With --bench it measures what the per-thread counter costs instead: all six PBs run a tight
loop of 40000 counted blocks every tick, three rounds with FixScriptCounter off and on, under
run-bench.sh (alone on the machine).

With --client it runs the vanilla case once on an isolated headless Pulsar client instead (the
client is the server in an offline world), which checks the client Preloader and its config path.
Needs the Remote plugin checkout for loading the world.

Prints the PASS and FAIL lines of each case and exits 1 on any FAIL or missing result.
"""

import csv
import html
import os
import re
import shutil
import signal
import subprocess
import sys
import time
from pathlib import Path

HERE = Path(__file__).resolve().parent
HOME = Path.home()
sys.path.insert(0, str(HERE))
import rig  # noqa: E402

WORKSPACE = rig.WORKSPACE
REMOTE = Path(os.environ.get("REMOTE_REPO", WORKSPACE / "se1/plugins/remote"))

PROGRAMS = {
    "normal": "public void Main(string argument) { int sum = 0; for (int i = 0; i < 10000; i++) { sum += i; } "
    'Echo("sum=" + sum + " instructions=" + Runtime.CurrentInstructionCount); }',
    # Close to the 50000 limit on its own: a counter shared with the runaway script would overflow
    "long": "public void Main(string argument) { int sum = 0; for (int i = 0; i < 45000; i++) { sum += i; } "
    'Echo("sum=" + sum + " instructions=" + Runtime.CurrentInstructionCount); }',
    # Worst case for the counter: a tight loop of counted blocks on every tick
    "bench": "public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update1; } "
    "public void Main(string argument) { int sum = 0; for (int i = 0; i < 40000; i++) { sum += i; } }",
    "runaway": "public void Main(string argument) { int i = 0; while (true) { i++; } }",
    "slow": "public void Main(string argument) { var list = new List<int>(Enumerable.Range(0, 200000)); "
    'for (int i = 0; i < 100; i++) { list.Reverse(); list.Sort(); } Echo("sorted " + list.Count); }',
}
NAMES = ["normal1", "normal2", "long", "runaway", "slow1", "slow2"]

CASES = {
    "off": [],
    "vanilla": ["--option", "FixScriptCounter=true"],
    "limit": ["--option", "FixScriptCounter=true", "--option", "ScriptRunLimitMs=100"],
}


def build(base: str, blueprints: list[str], world: Path, names: list[str] = NAMES) -> None:
    subprocess.run(
        [sys.executable, str(HERE / "build_world.py"), base, *blueprints, "--out", str(world)],
        check=True,
        stdout=subprocess.DEVNULL,
    )
    sbs = world / "SANDBOX_0_0_0_.sbs"
    text = sbs.read_text(encoding="utf-8")
    order = iter(reversed(names))

    def program(m):
        name = next(order)
        body = m.group(0)
        body = re.sub(
            r"<CustomName>.*?</CustomName>", f"<CustomName>PBTEST {name}</CustomName>", body, count=1, flags=re.S
        )
        source = html.escape(PROGRAMS[name.rstrip("0123456789")], quote=False)
        body = re.sub(r"<Program>.*?</Program>", lambda _: f"<Program>{source}</Program>", body, count=1, flags=re.S)
        return re.sub(r"<Storage>.*?</Storage>", "<Storage />", body, flags=re.S)

    # The blueprint grids come last in the save, after the base world's own PBs. Only PBs with a
    # name and a program are used, so the edits above have something to replace.
    pattern = re.compile(r'xsi:type="MyObjectBuilder_MyProgrammableBlock">.*?</MyObjectBuilder_CubeBlock>', re.S)
    matches = [m for m in pattern.finditer(text) if "<CustomName>" in m.group(0) and "<Program>" in m.group(0)]
    if len(matches) < len(names):
        sys.exit(f"expected {len(names)} blueprint PBs in the test world, found {len(matches)}")
    for m in reversed(matches[-len(names) :]):
        text = text[: m.start()] + program(m) + text[m.end() :]
    sbs.write_text(text, encoding="utf-8")


def client(slot: int, task: str, world: Path) -> list[str]:
    """The vanilla case on a headless Pulsar client loading the test world offline"""
    sys.path.insert(0, str(REMOTE / "skills/se-remote"))
    from se_remote import RemoteAPI

    pulsar = HOME / f".se-test/{task}-c0"
    appdata = HOME / f".se-test/{task}-c0-data"
    launcher = pulsar / "Interim.bin"
    if not launcher.exists():
        subprocess.run(
            [
                str(WORKSPACE / "testing/pulsar-dev-instances/new-pulsar-instance.sh"),
                str(pulsar),
                str(HOME / ".config/Pulsar"),
            ],
            check=True,
            stdout=subprocess.DEVNULL,
        )
    legacy = pulsar / "Legacy"
    sources = legacy / "Sources/sources.xml"
    rig.register_source(sources, "remote", REMOTE, "Remote.xml")
    rig.register_source(sources, "performance", rig.REPO, "PerformanceClient.xml")
    rig.register_source(sources, "pb-probe", HERE / "probe-plugin", "PbProbe.xml")
    ids = ["remote", "viktor-ferenczi/se-performance-improvements", "pb-probe"]
    folders = "".join(f"<LocalFolderConfig><Id>{i}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>" for i in ids)
    (legacy / "Profiles").mkdir(exist_ok=True)
    (legacy / "Profiles/Current.xml").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n<Profile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        f'xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Name>Current</Name><GitHub /><DevFolder>{folders}</DevFolder>'
        "<Local /><Mods /></Profile>\n"
    )

    port = 25000 + 10 * slot
    (appdata / "Storage").mkdir(parents=True, exist_ok=True)
    (appdata / "Remote.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n<PluginConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema">\n  <Enabled>true</Enabled>\n  <ListenIP>127.0.0.1</ListenIP>\n'
        f"  <ListenPort>{port}</ListenPort>\n  <AdminPassword>SpaceEngineers</AdminPassword>\n</PluginConfig>\n"
    )
    # The client plugin's own config file, which its Preloader reads for FixScriptCounter
    (appdata / "Storage/Performance.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n<Config><FixScriptCounter>true</FixScriptCounter></Config>\n'
    )
    game_cfg = appdata / "SpaceEngineers.cfg"
    if not game_cfg.exists():
        shutil.copy(HOME / ".config/SpaceEngineers/SpaceEngineers.cfg", game_cfg)
    # The test world is experimental (no block limits); without this the load stops on a message box
    text = game_cfg.read_text()
    text = re.sub(
        r"(<Key>ExperimentalMode</Key>\s*<Value[^>]*>)[^<]*(</Value>)", lambda m: m.group(1) + "true" + m.group(2), text
    )
    game_cfg.write_text(text)
    target = appdata / "Saves/tests/PbRunnerTest"
    shutil.rmtree(target, ignore_errors=True)
    shutil.copytree(world, target)
    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        path = target / name
        path.write_text(re.sub(r"<OnlineMode>[^<]*</OnlineMode>", "<OnlineMode>OFFLINE</OnlineMode>", path.read_text()))

    probe = appdata / "probe"
    shutil.rmtree(probe, ignore_errors=True)
    args = [
        "-multiInstance", "-lazySteam", "-noprompt", "-noupdate", "-nosplash", "-stablelogs", "-sources",
        "--no-steam", "-appdata", str(appdata), "--quality", "minimal", "--resolution", "640x480",
        "--no-audio", "--headless", "--no-render", "--client-id", str(76561199500010000 + 10 * slot),
    ]  # fmt: skip
    env = {**os.environ, "PULSAR_NO_RENDER": "true", "PB_PROBE_TEST": "vanilla", "PB_PROBE_DIR": str(probe)}
    process = subprocess.Popen(
        [str(rig.PARALLEL / "run-test.sh"), "se1", "3.7", "--", str(launcher), *args],
        cwd=pulsar,
        env=env,
        stdout=open(appdata / "launch.log", "w"),
        stderr=subprocess.STDOUT,
        stdin=subprocess.DEVNULL,
        start_new_session=True,
    )
    try:
        api = RemoteAPI(f"http://127.0.0.1:{port}", username="admin", password="SpaceEngineers")
        deadline = time.monotonic() + 900
        while time.monotonic() < deadline and process.poll() is None:
            try:
                api.ping()
                break
            except Exception:  # noqa: BLE001
                time.sleep(2)
        try:
            api.load(str(target))
        except Exception:  # noqa: BLE001 -- the load outlives the HTTP timeout
            pass
        log = probe / "probe.log"
        while time.monotonic() < deadline and process.poll() is None:
            if log.exists() and "DONE" in log.read_text(errors="replace"):
                break
            # An XML-only save stops on an INFO box ("file format of this world is outdated")
            try:
                if not api.get_state().get("ready") and api.get_screen(0).get("type") == "MyGuiScreenMessageBox":
                    api.control_click("OK")
            except Exception:  # noqa: BLE001 -- no screen or no API yet
                pass
            time.sleep(2)
        return log.read_text(errors="replace").splitlines() if log.exists() else []
    finally:
        os.killpg(process.pid, signal.SIGTERM)
        try:
            process.wait(60)
        except subprocess.TimeoutExpired:
            os.killpg(process.pid, signal.SIGKILL)


def bench(task: str, base: str, blueprints: list[str], rounds: int = 3) -> int:
    """Script time per run of the "bench" program with FixScriptCounter off and on, interleaved"""
    world = HOME / f".se-test/{task}-runner-test/PbCounterBench"
    build(base, blueprints, world, [f"bench{i}" for i in range(1, len(NAMES) + 1)])
    variants = {"off": [], "on": ["--option", "FixScriptCounter=true"]}
    results = {v: [] for v in variants}
    for _ in range(rounds):
        for variant, options in variants.items():
            subprocess.run(
                [sys.executable, str(HERE / "rig.py"), str(world), "--performance", "--bench", "--ram", "4"]
                + ["--warmup", "600", "--frames", "1800", *options],
                check=True,
                stdout=subprocess.DEVNULL,
            )
            probe = HOME / f".se-test/{task}-ds0/probe"
            bench_pbs = {r["pb"] for r in read_tsv(probe / "pbs.tsv") if r["name"].startswith("PBTEST bench")}
            runs = [float(r["script_us"]) for r in read_tsv(probe / "runs.tsv") if r["pb"] in bench_pbs]
            results[variant].append(sum(runs) / max(1, len(runs)))
            print(f"{variant}: {len(runs)} runs, script {results[variant][-1]:.1f} µs per run", flush=True)
    off, on = (sum(results[v]) / rounds for v in variants)
    print(f"== mean script time per run: off {off:.1f} µs, on {on:.1f} µs ({on / off - 1:+.1%})")
    return 0


def read_tsv(path: Path) -> list[dict]:
    with open(path, newline="") as f:
        return list(csv.DictReader(f, delimiter="\t"))


def main():
    args = sys.argv[1:]
    on_client = args[:1] == ["--client"]
    on_bench = args[:1] == ["--bench"]
    args = args[1:] if on_client or on_bench else args
    if len(args) != 1 + len(NAMES):
        sys.exit(__doc__)
    slot = int(os.environ["PB_SLOT"])
    task = (HOME / f".se-test/slots/{slot}/owner").read_text().strip()
    if on_bench:
        return bench(task, args[0], args[1:])
    world = HOME / f".se-test/{task}-runner-test/PbRunnerTest"
    build(args[0], args[1:], world)

    if on_client:
        lines = [ln for ln in client(slot, task, world) if re.search(r" (PASS|FAIL) ", ln)]
        print(f"== client vanilla: {len(lines)} checks")
        print("\n".join(lines))
        failed = len(lines) != 6 or any(" FAIL " in ln for ln in lines)
        print("== RESULT", "FAIL" if failed else "PASS")
        return 1 if failed else 0

    failed = False
    for case, options in CASES.items():
        result = subprocess.run(
            [sys.executable, str(HERE / "rig.py"), str(world), "--performance", "--ram", "4", *options],
            env={**os.environ, "PB_PROBE_TEST": case},
            capture_output=True,
            text=True,
        )
        lines = [ln for ln in result.stdout.splitlines() if re.search(r" (PASS|FAIL) ", ln)]
        # Keep each case's logs: the next case starts the server over the same folders
        logs = HOME / f".se-test/{task}-runner-test/logs-{case}"
        shutil.rmtree(logs, ignore_errors=True)
        logs.mkdir(parents=True)
        root = HOME / f".se-test/{task}-ds0"
        for log in [
            root / "server.log",
            root / "magnetar/info.log",
            *root.glob("data/*.log"),
            root / "probe/probe.log",
        ]:
            if log.exists():
                shutil.copy(log, logs)
        print(f"== {case}: {len(lines)} checks")
        print("\n".join(lines) or result.stdout[-2000:])
        expected = 1 if case == "off" else 6
        failed |= len(lines) != expected or any(" FAIL " in ln for ln in lines)
    print("== RESULT", "FAIL" if failed else "PASS")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
