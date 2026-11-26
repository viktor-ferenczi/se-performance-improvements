"""Attach race rig (issue #161): an isolated Magnetar server (Linux) with the attach probe and,
unless --vanilla, the Performance plugin from this checkout. The probe makes rotor tops on worker
threads, whose Init marks an in-scene stator for reattach, and counts the changes of the stators'
BlocksWaitingOnAttachUpdate made off the main thread, the exceptions of the methods changing it
and, at the end, whether the set still matches the stators. Prints the probe lines and PASS or
FAIL.

    ATTACH_SLOT=18 python3 rig.py <world dir> [--vanilla] [--seconds 60] [--tops 16]

<world dir>  any world, for example the dedicated server's Content/CustomWorlds/Empty World
--vanilla    without Performance, the baseline; PERFORMANCE_REPO names another checkout to load
             it from instead of this one

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
REPO = Path(os.environ.get("PERFORMANCE_REPO", HERE.parents[1]))
WORKSPACE = Path(os.environ.get("SE_WORKSPACE", HOME / "ws/se"))
PARALLEL = WORKSPACE / "testing/parallel-test-work"
DIRECT_TRANSPORT = WORKSPACE / "se1/direct-transport"
MAGNETAR = Path(os.environ.get("MAGNETAR", HOME / ".config/Magnetar"))
DS64 = Path(
    os.environ.get(
        "DS64",
        HOME
        / ".steam/steam/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64",
    )
)
PERFORMANCE_SERVER_ID = "454B0F55-C727-4DAB-B1A9-5AE6050A0932"

SLOT = int(os.environ["ATTACH_SLOT"])
TASK = (HOME / f".se-test/slots/{SLOT}/owner").read_text().strip()
ROOT = HOME / f".se-test/{TASK}-ds0"
CONFIG = ROOT / "magnetar"
DATA = ROOT / "data"
LOG = ROOT / "server.log"
PROBE = re.compile(r"AttachProbe: (.*)")
DONE = re.compile(
    r"AttachProbe: done fix=(\S+) spawned=(\d+) created=(\d+) offMain=(\d+)"
    r" exceptions=(\d+) inconsistent=(-?\d+)"
)


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


def server_pids() -> list[int]:
    """Processes of this rig's server: every process whose command line names its config folder"""
    pids = []
    for proc in Path("/proc").iterdir():
        try:
            if (
                proc.name.isdigit()
                and str(CONFIG).encode() in (proc / "cmdline").read_bytes()
            ):
                pids.append(int(proc.name))
        except OSError:
            pass
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


def configure(world: Path, vanilla: bool) -> None:
    CONFIG.mkdir(parents=True, exist_ok=True)
    if not (CONFIG / "Sources").exists():
        shutil.copytree(MAGNETAR / "Magnetar/Sources", CONFIG / "Sources")
        shutil.copy(MAGNETAR / "Magnetar/config.xml", CONFIG / "config.xml")
    sources = CONFIG / "Sources/sources.xml"
    register_source(
        sources, "direct-transport", DIRECT_TRANSPORT, "DirectTransportServer.xml"
    )
    register_source(sources, "attach-probe", HERE / "probe-plugin", "AttachProbe.xml")
    plugins = ["direct-transport", "attach-probe"]
    if not vanilla:
        register_source(sources, PERFORMANCE_SERVER_ID, REPO, "PerformanceServer.xml")
        plugins.append(PERFORMANCE_SERVER_ID)
    write_profile(plugins)

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
        "ServerName": "Attach race rig",
        "WorldName": world.name,
        "PauseGameWhenEmpty": "false",
        "AutoRestartEnabled": "false",
        "IgnoreLastSession": "true",
        "RemoteApiEnabled": "false",
        "LoadWorld": target,
        # The DS applies its own session settings over the world's
        "ExperimentalMode": "true",
        "NetworkType": "steam",
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

    (DATA / "Performance.cfg").unlink(missing_ok=True)


def log_text() -> str:
    # The lines go to the DS log and to the console, whichever has them
    texts = [p.read_text(errors="replace") for p in [LOG, *DATA.glob("*.log")]]
    return max(texts, key=lambda t: len(PROBE.findall(t)))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("world", type=Path)
    ap.add_argument("--vanilla", action="store_true")
    ap.add_argument("--seconds", type=int, default=60)
    ap.add_argument("--tops", type=int, default=16)
    ap.add_argument("--ram", default="4")
    args = ap.parse_args()

    stop(None)
    configure(args.world.resolve(), args.vanilla)
    # Older workspaces have one lock for both games, and their scripts take no game argument
    game = ["se1"] if "se1|se2" in (PARALLEL / "run-test.sh").read_text() else []
    process = subprocess.Popen(
        [
            str(PARALLEL / "run-test.sh"), *game, args.ram, "--", str(MAGNETAR / "MagnetarInterim.bin"),
            "-multiInstance", "-stableLogs", "-noimplicitmod", "-consent", "deny",
            "-config", str(CONFIG), "-ds64", str(DS64), "-path", str(DATA),
        ],  # fmt: skip
        cwd=MAGNETAR,
        env={
            **os.environ,
            "SE_DIRECT_TRANSPORT": "1",
            "ATTACH_PROBE_SECONDS": str(args.seconds),
            "ATTACH_PROBE_TOPS": str(args.tops),
        },
        stdout=open(LOG, "w"),
        stderr=subprocess.STDOUT,
        stdin=subprocess.DEVNULL,
        start_new_session=True,
    )
    started = time.monotonic()
    crashed = False
    try:
        deadline = started + 900 + args.seconds
        while not DONE.search(log_text()) and time.monotonic() < deadline:
            if not server_pids() and process.poll() is not None:
                crashed = True
                break
            time.sleep(5)
    finally:
        stop(process.pid)

    text = log_text()
    for line in PROBE.findall(text):
        if not line.startswith("exception"):
            print(line)
    for line in PROBE.findall(text):
        if line.startswith("exception"):
            print(line[:300])
    done = DONE.search(text)
    print(f"== after {time.monotonic() - started:.0f} s")
    if not done:
        print(f"FAIL: no result line{' (server exited)' if crashed else ''}; see {LOG}")
        return 1
    fix, spawned, created, off_main, exceptions, inconsistent = done.groups()
    ok = int(off_main) == 0 and int(exceptions) == 0 and int(inconsistent) == 0
    print(
        f"{'PASS' if ok else 'FAIL'}: fix={fix} tops={spawned} offMain={off_main}"
        f" exceptions={exceptions} inconsistent={inconsistent}"
    )
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
