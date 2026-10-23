"""SE-626 PB profiling rig: loads a world on an isolated Magnetar server (Linux) with the PB probe
and prints where the probe tables went.

    PB_SLOT=18 python3 rig.py <world dir> [--trace SECONDS] [--performance] [--bench]
                              [--warmup FRAMES] [--frames FRAMES]

--trace     records a dotnet-trace sample profile for that many seconds once measuring starts
--performance  adds the Performance plugin from this checkout
--option NAME=VALUE  sets a Performance config option (repeatable), the others keep their defaults
--bench     runs under run-bench.sh (alone on the machine) instead of run-test.sh

Slot rules: testing/parallel-test-work/README.md in the workspace. The Magnetar install and
the Dedicated Server are the user's (MAGNETAR, DS64 override them; DS_CFG names the
SpaceEngineers-Dedicated.cfg to start from); nothing in them is written.
"""

import argparse
import os
import re
import shutil
import signal
import subprocess
import sys
import time
import xml.etree.ElementTree as ET
from pathlib import Path

HOME = Path.home()
HERE = Path(__file__).resolve().parent
REPO = HERE.parents[1]
WORKSPACE = Path(os.environ.get("SE_WORKSPACE", HOME / "ws/se"))
PARALLEL = WORKSPACE / "testing/parallel-test-work"
DIRECT_TRANSPORT = WORKSPACE / "se1/direct-transport"
MAGNETAR = Path(os.environ.get("MAGNETAR", HOME / ".config/Magnetar"))
DS64 = Path(
    os.environ.get(
        "DS64", HOME / ".steam/debian-installation/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64"
    )
)
# Non-interactive ssh sessions lack ~/.dotnet/tools on PATH
DOTNET_TRACE = shutil.which("dotnet-trace") or str(HOME / ".dotnet/tools/dotnet-trace")
PERFORMANCE_SERVER_ID = "454B0F55-C727-4DAB-B1A9-5AE6050A0932"

SLOT = int(os.environ["PB_SLOT"])
TASK = (HOME / f".se-test/slots/{SLOT}/owner").read_text().strip()
ROOT = HOME / f".se-test/{TASK}-ds0"
CONFIG = ROOT / "magnetar"
DATA = ROOT / "data"
PROBE_DIR = ROOT / "probe"
TIMEOUT = 3600


def register_source(sources: Path, name: str, folder: Path, file: str) -> None:
    tree = ET.parse(sources)
    local = tree.getroot().find("LocalPluginSources")
    if local is None:
        local = ET.SubElement(tree.getroot(), "LocalPluginSources")
    # Another entry with the same manifest would load that folder's copy instead
    for plugin in local.findall("LocalPlugin"):
        if plugin.findtext("Name") == name or plugin.findtext("File") == file:
            local.remove(plugin)
    plugin = ET.SubElement(local, "LocalPlugin")
    for tag, value in (("Name", name), ("Folder", str(folder)), ("File", file), ("Enabled", "true")):
        ET.SubElement(plugin, tag).text = value
    tree.write(sources, encoding="utf-8", xml_declaration=True)


def write_profile(ids: list[str]) -> None:
    folders = "".join(f"<LocalFolderConfig><Id>{i}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>" for i in ids)
    (CONFIG / "Profiles").mkdir(exist_ok=True)
    (CONFIG / "Profiles/Current.xml").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        '<Profile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema">'
        f"<Name>Current</Name><GitHub /><DevFolder>{folders}</DevFolder>"
        "<Local /><Mods /></Profile>\n",
        encoding="utf-8",
    )


def alive(pid: int) -> bool:
    try:
        return str(CONFIG).encode() in Path(f"/proc/{pid}/cmdline").read_bytes()
    except OSError:
        return False


def server_pids() -> list[int]:
    """Processes of this rig's server: every process whose command line names its config folder"""
    pids = []
    for proc in Path("/proc").iterdir():
        if proc.name.isdigit() and alive(int(proc.name)):
            pids.append(int(proc.name))
    return pids


def stop(group: int | None) -> None:
    if group is not None:
        try:
            os.killpg(group, signal.SIGTERM)
        except ProcessLookupError:
            pass
    deadline = time.monotonic() + 60
    while server_pids() and time.monotonic() < deadline:
        time.sleep(1)
    for pid in server_pids():
        os.kill(pid, signal.SIGKILL)


def configure(world: Path, performance: bool) -> None:
    CONFIG.mkdir(parents=True, exist_ok=True)
    if not (CONFIG / "Sources").exists():
        shutil.copytree(MAGNETAR / "Magnetar/Sources", CONFIG / "Sources")
        shutil.copy(MAGNETAR / "Magnetar/config.xml", CONFIG / "config.xml")
    sources = CONFIG / "Sources/sources.xml"
    register_source(sources, "direct-transport", DIRECT_TRANSPORT, "DirectTransportServer.xml")
    register_source(sources, "pb-probe", HERE / "probe-plugin", "PbProbe.xml")
    ids = ["direct-transport", "pb-probe"]
    if performance:
        register_source(sources, PERFORMANCE_SERVER_ID, REPO, "PerformanceServer.xml")
        ids.append(PERFORMANCE_SERVER_ID)
    write_profile(ids)

    target = DATA / "Saves" / world.name
    shutil.rmtree(target, ignore_errors=True)
    shutil.copytree(world, target)
    template = Path(os.environ.get("DS_CFG", HOME / ".config/SpaceEngineersDedicated/SpaceEngineers-Dedicated.cfg"))
    text = template.read_text(encoding="utf-8")
    for tag, value in {
        "IP": "127.0.0.1",
        "ServerPort": 28000 + 10 * SLOT,
        "SteamPort": 29000 + 10 * SLOT,
        "ServerName": "SE-626 PB profiling",
        "WorldName": world.name,
        "PauseGameWhenEmpty": "false",
        "AutoRestartEnabled": "false",
        "IgnoreLastSession": "true",
        "RemoteApiEnabled": "false",
        "LoadWorld": target,
        # The DS applies its own session settings over the world's
        "EnableIngameScripts": "true",
        "ExperimentalMode": "true",
        "BlockLimitsEnabled": "NONE",
    }.items():
        text, count = re.subn(rf"<{tag}>.*?</{tag}>|<{tag} />", lambda _: f"<{tag}>{value}</{tag}>", text, flags=re.S)
        assert count == 1, tag
    (DATA / "SpaceEngineers-Dedicated.cfg").write_text(text, encoding="utf-8")
    for old in DATA.glob("*.log"):
        old.unlink()


def wait_for(text: str, deadline: float) -> bool:
    log = PROBE_DIR / "probe.log"
    while time.monotonic() < deadline:
        if log.exists() and text in log.read_text(errors="replace"):
            return True
        if not server_pids():
            return False
        time.sleep(2)
    return False


def trace(seconds: int) -> None:
    """Samples the server process (the one with a diagnostics socket) for the given time"""
    sockets = {p.name.split("-")[2] for p in Path("/tmp").glob("dotnet-diagnostic-*-socket")}
    pids = [p for p in server_pids() if str(p) in sockets]
    if not pids:
        print("trace: no .NET process of the server found")
        return
    out = PROBE_DIR / "trace.nettrace"
    subprocess.run(
        [DOTNET_TRACE, "collect", "-p", str(pids[-1]), "--duration", f"00:00:{seconds // 60:02}:{seconds % 60:02}", "-o", str(out)],
        stdout=subprocess.DEVNULL,
        check=False,
    )  # fmt: skip
    subprocess.run(
        [DOTNET_TRACE, "convert", "--format", "speedscope", str(out)], stdout=subprocess.DEVNULL, check=False
    )


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("world", type=Path)
    ap.add_argument("--trace", type=int, default=0)
    ap.add_argument("--performance", action="store_true")
    ap.add_argument("--option", action="append", default=[])
    ap.add_argument("--bench", action="store_true")
    ap.add_argument("--warmup", type=int, default=1200)
    ap.add_argument("--frames", type=int, default=3600)
    ap.add_argument("--ram", default="6")
    args = ap.parse_args()

    stop(None)
    configure(args.world.resolve(), args.performance)
    # The server plugin reads <UserDataPath>/Performance.cfg; options it lacks keep their defaults
    options = "".join(f"  <{k}>{v}</{k}>\n" for k, v in (o.split("=", 1) for o in args.option))
    (DATA / "Performance.cfg").write_text(
        f'<?xml version="1.0" encoding="utf-8"?>\n<PerformanceConfig>\n{options}</PerformanceConfig>\n'
    )
    shutil.rmtree(PROBE_DIR, ignore_errors=True)
    PROBE_DIR.mkdir(parents=True)
    runner = "run-bench.sh" if args.bench else "run-test.sh"
    env = {
        **os.environ,
        "SE_DIRECT_TRANSPORT": "1",
        "PB_PROBE_DIR": str(PROBE_DIR),
        "PB_PROBE_WARMUP": str(args.warmup),
        "PB_PROBE_FRAMES": str(args.frames),
    }
    process = subprocess.Popen(
        [
            str(PARALLEL / runner), "se1", args.ram, "--", str(MAGNETAR / "MagnetarInterim.bin"),
            "-multiInstance", "-stableLogs", "-noimplicitmod", "-consent", "deny",
            "-config", str(CONFIG), "-ds64", str(DS64), "-path", str(DATA),
        ],  # fmt: skip
        cwd=MAGNETAR,
        env=env,
        stdout=open(ROOT / "server.log", "w"),
        stderr=subprocess.STDOUT,
        stdin=subprocess.DEVNULL,
        start_new_session=True,
    )
    started = time.monotonic()
    try:
        deadline = started + TIMEOUT
        # A probe that failed to build never writes this, and the server would run on
        if not wait_for("world ready", started + 900):
            print(f"== no 'world ready' from the probe; see {CONFIG}/info.log")
            return 1
        if args.trace:
            if not wait_for("window open", deadline):
                print("== server stopped or timed out before measuring")
                return 1
            trace(args.trace)
        result = "finished" if wait_for("DONE", deadline) else "SERVER STOPPED OR TIMEOUT"
    finally:
        stop(process.pid)
    print(f"== {result} after {time.monotonic() - started:.0f} s; tables in {PROBE_DIR}")
    print(
        (PROBE_DIR / "probe.log").read_text(errors="replace")
        if (PROBE_DIR / "probe.log").exists()
        else "(no probe log)"
    )
    return 0 if result == "finished" else 1


if __name__ == "__main__":
    sys.exit(main())
