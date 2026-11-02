"""Solar panel rig: loads the world of build_world.py on an isolated Magnetar server (Linux) with
the Performance plugin from this checkout and the solar probe, collects the probe's 10 second
lines and prints them with their averages.

    SOLAR_SLOT=18 python3 rig.py <world dir> [--option NAME=VALUE] [--seconds 600] [--bench]

--option NAME=VALUE  sets a Performance config option (repeatable), for example
                     FixSolarPanel=false for the baseline; the others keep their defaults
--bench     runs under run-bench.sh (alone on the machine) instead of run-test.sh, use it when
            the recompute times matter, not only the counts

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
        / ".steam/steam/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64",
    )
)
PERFORMANCE_SERVER_ID = "454B0F55-C727-4DAB-B1A9-5AE6050A0932"

SLOT = int(os.environ["SOLAR_SLOT"])
TASK = (HOME / f".se-test/slots/{SLOT}/owner").read_text().strip()
ROOT = HOME / f".se-test/{TASK}-ds0"
CONFIG = ROOT / "magnetar"
DATA = ROOT / "data"
LOG = ROOT / "server.log"
LINE = re.compile(
    r"SolarProbe: fix=(\S+) computed=(\d+) changed=(\d+) recomputes=(\d+) recomputeMs=([\d.]+)"
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


def configure(world: Path, options: list[str]) -> None:
    CONFIG.mkdir(parents=True, exist_ok=True)
    if not (CONFIG / "Sources").exists():
        shutil.copytree(MAGNETAR / "Magnetar/Sources", CONFIG / "Sources")
        shutil.copy(MAGNETAR / "Magnetar/config.xml", CONFIG / "config.xml")
    sources = CONFIG / "Sources/sources.xml"
    register_source(
        sources, "direct-transport", DIRECT_TRANSPORT, "DirectTransportServer.xml"
    )
    register_source(sources, "solar-probe", HERE / "probe-plugin", "SolarProbe.xml")
    register_source(sources, PERFORMANCE_SERVER_ID, REPO, "PerformanceServer.xml")
    write_profile(["direct-transport", "solar-probe", PERFORMANCE_SERVER_ID])

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
        "ServerName": "Solar panel rig",
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

    # The server plugin reads <UserDataPath>/Performance.cfg; options it lacks keep their defaults
    values = "".join(
        f"  <{k}>{v}</{k}>\n" for k, v in (o.split("=", 1) for o in options)
    )
    (DATA / "Performance.cfg").write_text(
        f'<?xml version="1.0" encoding="utf-8"?>\n<PerformanceConfig>\n{values}</PerformanceConfig>\n'
    )


def probe_lines() -> list[tuple]:
    # The lines go to the DS log and to the console, whichever has them
    found = [
        LINE.findall(p.read_text(errors="replace")) for p in [LOG, *DATA.glob("*.log")]
    ]
    return max(found, key=len)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("world", type=Path)
    ap.add_argument("--option", action="append", default=[])
    ap.add_argument("--seconds", type=int, default=600)
    ap.add_argument("--bench", action="store_true")
    ap.add_argument("--ram", default="5")
    args = ap.parse_args()

    stop(None)
    configure(args.world.resolve(), args.option)
    runner = "run-bench.sh" if args.bench else "run-test.sh"
    # Older workspaces have one lock for both games, and their scripts take no game argument
    game = ["se1"] if "se1|se2" in (PARALLEL / runner).read_text() else []
    process = subprocess.Popen(
        [
            str(PARALLEL / runner), *game, args.ram, "--", str(MAGNETAR / "MagnetarInterim.bin"),
            "-multiInstance", "-stableLogs", "-noimplicitmod", "-consent", "deny",
            "-config", str(CONFIG), "-ds64", str(DS64), "-path", str(DATA),
        ],  # fmt: skip
        cwd=MAGNETAR,
        env={**os.environ, "SE_DIRECT_TRANSPORT": "1"},
        stdout=open(LOG, "w"),
        stderr=subprocess.STDOUT,
        stdin=subprocess.DEVNULL,
        start_new_session=True,
    )
    started = time.monotonic()
    wanted = args.seconds // 10
    try:
        # run-bench.sh may wait up to two hours for the machine to become free
        deadline = started + (7200 if args.bench else 0) + 900 + args.seconds
        while len(probe_lines()) < wanted + 1 and time.monotonic() < deadline:
            if not server_pids() and process.poll() is not None:
                break
            time.sleep(5)
    finally:
        stop(process.pid)

    # The first line covers the world's first moments, partly before the panels computed
    lines = probe_lines()[1 : wanted + 1]
    print(
        f"== {len(lines)} of {wanted} probe lines after {time.monotonic() - started:.0f} s"
    )
    print("fix\tcomputed\tchanged\trecomputes\trecomputeMs")
    for line in lines:
        print("\t".join(line))
    if not lines:
        print(f"no probe lines; see {LOG} and {CONFIG}/info.log")
        return 1
    n = len(lines)
    print(
        "mean per 10 s\t"
        + "\t".join(f"{sum(float(l[i]) for l in lines) / n:.1f}" for i in range(1, 5))
    )
    return 0 if n == wanted else 1


if __name__ == "__main__":
    sys.exit(main())
