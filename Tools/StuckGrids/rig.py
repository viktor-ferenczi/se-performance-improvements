"""SE-753 stuck grids rig: loads the world of build_world.py on an isolated Magnetar server (Linux)
with the stuck grids probe, which launches the ship into the station, and prints the probe's
per-second table.

    STUCK_SLOT=18 python3 rig.py <world dir> [--performance] [--option NAME=VALUE] [--bench]
                                 [--warmup SECONDS] [--seconds SECONDS] [--shots SHOTS]

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
        "DS64",
        HOME
        / ".steam/debian-installation/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64",
    )
)
PERFORMANCE_SERVER_ID = "454B0F55-C727-4DAB-B1A9-5AE6050A0932"

SLOT = int(os.environ["STUCK_SLOT"])
TASK = (HOME / f".se-test/slots/{SLOT}/owner").read_text().strip()
ROOT = HOME / f".se-test/{TASK}-ds0"
CONFIG = ROOT / "magnetar"
DATA = ROOT / "data"
PROBE_DIR = ROOT / "probe"
TIMEOUT = 3600
EOS = os.environ.get("RIG_NETWORK") == "eos"
NATIVE = os.environ.get("RIG_NETWORK") in ("eos", "steam")


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
    for tag, value in (
        ("Name", name),
        ("Folder", str(folder)),
        ("File", file),
        ("Enabled", "true"),
    ):
        ET.SubElement(plugin, tag).text = value
    tree.write(sources, encoding="utf-8", xml_declaration=True)


def write_profile(ids: list[str]) -> None:
    folders = "".join(
        f"<LocalFolderConfig><Id>{i}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        for i in ids
    )
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
    if not NATIVE:
        register_source(
            sources, "direct-transport", DIRECT_TRANSPORT, "DirectTransportServer.xml"
        )
    register_source(sources, "stuck-probe", HERE / "probe-plugin", "StuckProbe.xml")
    ids = ["stuck-probe"] if NATIVE else ["direct-transport", "stuck-probe"]
    if performance:
        register_source(sources, PERFORMANCE_SERVER_ID, REPO, "PerformanceServer.xml")
        ids.append(PERFORMANCE_SERVER_ID)
    write_profile(ids)

    target = DATA / "Saves" / world.name
    shutil.rmtree(target, ignore_errors=True)
    shutil.copytree(world, target)
    template = Path(
        os.environ.get(
            "DS_CFG",
            HOME / ".config/SpaceEngineersDedicated/SpaceEngineers-Dedicated.cfg",
        )
    )
    text = template.read_text(encoding="utf-8")
    for tag, value in {
        "IP": "127.0.0.1",
        "ServerPort": 28000 + 10 * SLOT,
        "SteamPort": 29000 + 10 * SLOT,
        "ServerName": "SE-753 stuck grids",
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
        "NetworkType": "eos" if EOS else "steam",
    }.items():
        text, count = re.subn(
            rf"<{tag}>.*?</{tag}>|<{tag} />",
            lambda _: f"<{tag}>{value}</{tag}>",
            text,
            flags=re.S,
        )
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


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("world", type=Path)
    ap.add_argument("--performance", action="store_true")
    ap.add_argument("--option", action="append", default=[])
    ap.add_argument("--bench", action="store_true")
    ap.add_argument("--warmup", type=int, default=20)
    ap.add_argument("--seconds", type=int, default=180)
    ap.add_argument(
        "--shots", default="0,14,0,0,-1,0,200", help="see probe-plugin/Source/Plugin.cs"
    )
    ap.add_argument("--ram", default="6")
    args = ap.parse_args()

    stop(None)
    configure(args.world.resolve(), args.performance)
    # The server plugin reads <UserDataPath>/Performance.cfg; options it lacks keep their defaults
    options = "".join(
        f"  <{k}>{v}</{k}>\n" for k, v in (o.split("=", 1) for o in args.option)
    )
    (DATA / "Performance.cfg").write_text(
        f'<?xml version="1.0" encoding="utf-8"?>\n<PerformanceConfig>\n{options}</PerformanceConfig>\n'
    )
    shutil.rmtree(PROBE_DIR, ignore_errors=True)
    PROBE_DIR.mkdir(parents=True)
    runner = "run-bench.sh" if args.bench else "run-test.sh"
    # Older workspaces have one lock for both games, and their scripts take no game argument
    game = ["se1"] if "se1|se2" in (PARALLEL / runner).read_text() else []
    env = {
        **os.environ,
        **({} if NATIVE else {"SE_DIRECT_TRANSPORT": "1"}),
        **dict(
            line.split("=", 1)
            for line in (args.world / "stuck.env").read_text().split()
        ),
        "STUCK_PROBE_DIR": str(PROBE_DIR),
        "STUCK_WARMUP": str(args.warmup),
        "STUCK_SECONDS": str(args.seconds),
        "STUCK_SHOTS": args.shots,
    }
    process = subprocess.Popen(
        [
            str(PARALLEL / runner), *game, args.ram, "--", str(MAGNETAR / "MagnetarInterim.bin"),
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
        result = (
            "finished" if wait_for("DONE", deadline) else "SERVER STOPPED OR TIMEOUT"
        )
    finally:
        stop(process.pid)
    print(
        f"== {result} after {time.monotonic() - started:.0f} s; tables in {PROBE_DIR}"
    )
    for name in ("probe.log", "seconds.tsv"):
        path = PROBE_DIR / name
        print(path.read_text(errors="replace") if path.exists() else f"(no {name})")
    return 0 if result == "finished" else 1


if __name__ == "__main__":
    sys.exit(main())
