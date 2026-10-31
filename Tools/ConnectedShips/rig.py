"""Rig of the connected ships tests: an isolated Magnetar server with no client, driven
through the dock probe (probe/Probe.cs).

Every port and folder comes from a test slot (testing/parallel-test-work in the se
workspace). CS_TASK names the slot. Paths are overridable:

    CS_PARALLEL   folder of slot.sh, run-test.sh and ram-guard.sh
    CS_REPO       Performance working copy (default: this repo)
    CS_MAGNETAR   Magnetar install (~/.config/Magnetar)
    CS_DS64       DedicatedServer64 of the Steam install

For mp_check.py also:

    CS_REMOTE     Remote plugin working copy
    CS_TRANSPORT  DirectTransport working copy
    CS_PULSAR     Pulsar install the client folder is copied from (~/.config/Pulsar)
    CS_PULSAR_CONFIG  Pulsar config folder to start from, if the install has none
    CS_NEW_PULSAR new-pulsar-instance.sh of the se workspace
    CS_BIN64      Bin64 of the game client
    CS_BUGFIXES   Bugfixes working copy for the client. A headless client hits a game crash without
                  it (ControlledEntityEnergyEstimatedTimeRemaining.Update)
"""

from __future__ import annotations

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
REPO = Path(os.environ.get("CS_REPO", HERE.parents[1]))
PROBE = HERE / "probe"
PARALLEL = Path(
    os.environ.get("CS_PARALLEL", REPO.parents[2] / "testing" / "parallel-test-work")
)
MAGNETAR = Path(os.environ.get("CS_MAGNETAR", HOME / ".config/Magnetar"))
STEAM = HOME / ".steam/steam/steamapps/common"
if not STEAM.exists():
    STEAM = HOME / ".steam/debian-installation/steamapps/common"
DS64 = Path(
    os.environ.get("CS_DS64", STEAM / "SpaceEngineersDedicatedServer/DedicatedServer64")
)
EMPTY_WORLD = DS64.parent / "Content" / "CustomWorlds" / "Empty World"
TASK = os.environ.get("CS_TASK", "connected-ships")
ROOT = HOME / ".se-test" / f"{TASK}-ds0"
CONFIG = ROOT / "magnetar"
DATA = ROOT / "data"
WORLD = DATA / "Saves" / "ConnectedShips"
PROBE_DIR = ROOT / "probe"
LOG = ROOT / "server.log"
PID = ROOT / "server.pid"

REMOTE = Path(os.environ.get("CS_REMOTE", REPO.parent / "remote"))
TRANSPORT = Path(os.environ.get("CS_TRANSPORT", REPO.parents[1] / "direct-transport"))
PULSAR = Path(os.environ.get("CS_PULSAR", HOME / ".config/Pulsar"))
NEW_PULSAR = Path(
    os.environ.get(
        "CS_NEW_PULSAR",
        REPO.parents[2] / "testing" / "pulsar-dev-instances" / "new-pulsar-instance.sh",
    )
)
BIN64 = Path(os.environ.get("CS_BIN64", STEAM / "SpaceEngineers/Bin64"))
CLIENT_ROOT = HOME / ".se-test" / f"{TASK}-c0"
CLIENT_DATA = HOME / ".se-test" / f"{TASK}-c0-data"
CLIENT_PID = CLIENT_DATA / "game.pid"

PERFORMANCE_SERVER_ID = "454B0F55-C727-4DAB-B1A9-5AE6050A0932"
PERFORMANCE_CLIENT_ID = "viktor-ferenczi/se-performance-improvements"
BUGFIXES = Path(os.environ.get("CS_BUGFIXES", REPO.parent / "bugfixes"))
PROBE_ID = "6E1D2C40-7B7A-4B0B-9A6E-2D5F0C9E7981"

SETTINGS = {
    "GameMode": "Creative",
    "OnlineMode": "PUBLIC",
    "EnableRespawnShips": "false",
    "TrashRemovalEnabled": "false",
    "AutoSaveInMinutes": "0",
    "ExperimentalMode": "true",
    "EnableSunRotation": "false",
    "ThrusterDamage": "false",
    "EnableEncounters": "false",
    "GlobalEncounterCap": "0",
    "CargoShipsEnabled": "false",
    "EnableContainerDrops": "false",
    "EnableDrones": "false",
    "EnablePlanetaryEncounters": "false",
}


def _run(*command) -> str:
    return subprocess.run(
        [str(c) for c in command], check=True, capture_output=True, text=True
    ).stdout.strip()


def slot() -> dict[str, int]:
    number = _run(PARALLEL / "slot.sh", "claim", TASK)
    text = _run(PARALLEL / "slot.sh", "env", number)
    values = {k: int(v) for k, v in re.findall(r"(\w+_BASE)=(\d+)", text)}
    values["SLOT"] = int(number)
    return values


def _set(text: str, name: str, value: str) -> str:
    new, count = re.subn(
        rf"<{name}>[^<]*</{name}>|<{name}\s*/>", f"<{name}>{value}</{name}>", text
    )
    if count:
        return new
    return text.replace("</Settings>", f"<{name}>{value}</{name}></Settings>", 1)


def prepare_world() -> None:
    if WORLD.exists():
        shutil.rmtree(WORLD)
    WORLD.parent.mkdir(parents=True, exist_ok=True)
    shutil.copytree(EMPTY_WORLD, WORLD)
    (WORLD / "SANDBOX_0_0_0_.sbsB5").unlink(missing_ok=True)
    (WORLD / "Sandbox.sbcB5").unlink(missing_ok=True)
    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        path = WORLD / name
        text = path.read_text(encoding="utf-8")
        for key, value in SETTINGS.items():
            text = _set(text, key, value)
        path.write_text(text, encoding="utf-8")


def _sources(plugins: dict[str, tuple[Path, str]]) -> None:
    path = CONFIG / "Sources" / "sources.xml"
    tree = ET.parse(path)
    root = tree.getroot()
    local = root.find("LocalPluginSources")
    if local is not None:
        root.remove(local)
    local = ET.SubElement(root, "LocalPluginSources")
    for name, (folder, file) in plugins.items():
        plugin = ET.SubElement(local, "LocalPlugin")
        for tag, value in (
            ("Name", name),
            ("Folder", str(folder)),
            ("File", file),
            ("Enabled", "true"),
        ):
            ET.SubElement(plugin, tag).text = value
    tree.write(path, encoding="utf-8", xml_declaration=True)


def set_option(calm: bool) -> None:
    """The server plugin reads <UserDataPath>/Performance.cfg, other options keep their defaults"""
    DATA.mkdir(parents=True, exist_ok=True)
    (DATA / "Performance.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        f"<PerformanceConfig><CalmConnectedShips>{str(calm).lower()}</CalmConnectedShips></PerformanceConfig>\n"
    )


def prepare_server(performance: bool, transport: bool = False) -> None:
    """Magnetar's config, with the probe and optionally Performance and DirectTransport as dev
    folders"""
    if not (CONFIG / "Sources").exists():
        CONFIG.mkdir(parents=True, exist_ok=True)
        template = MAGNETAR / "Magnetar"
        shutil.copytree(template / "Sources", CONFIG / "Sources")
        shutil.copy(template / "config.xml", CONFIG / "config.xml")
        if (template / "GitHub").exists():
            shutil.copytree(template / "GitHub", CONFIG / "GitHub")
    plugins = {"dock-probe": (PROBE, "DockProbe.xml")}
    ids = [PROBE_ID]
    if performance:
        plugins[PERFORMANCE_SERVER_ID] = (REPO, "PerformanceServer.xml")
        ids.append(PERFORMANCE_SERVER_ID)
    if transport:
        plugins["direct-transport"] = (TRANSPORT, "DirectTransportServer.xml")
        ids.append("direct-transport")
    _sources(plugins)
    folders = "".join(
        f"<LocalFolderConfig><Id>{i}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        for i in ids
    )
    (CONFIG / "Profiles").mkdir(exist_ok=True)
    (CONFIG / "Profiles" / "Current.xml").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        '<Profile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema">'
        f"<Name>Current</Name><GitHub /><DevFolder>{folders}</DevFolder>"
        "<Local /><Mods /></Profile>\n",
        encoding="utf-8",
    )
    ports = slot()
    DATA.mkdir(parents=True, exist_ok=True)
    (DATA / "SpaceEngineers-Dedicated.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        '<MyConfigDedicated xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema">'
        f"<IP>127.0.0.1</IP><SteamPort>{ports['DS_STEAM_PORT_BASE']}</SteamPort>"
        f"<ServerPort>{ports['DS_SERVER_PORT_BASE']}</ServerPort>"
        f"<ServerName>Connected Ships {TASK}</ServerName>"
        "<WorldName>ConnectedShips</WorldName>"
        "<PauseGameWhenEmpty>false</PauseGameWhenEmpty>"
        "<AutoRestartEnabled>false</AutoRestartEnabled>"
        "<IgnoreLastSession>true</IgnoreLastSession>"
        "<RemoteApiEnabled>false</RemoteApiEnabled>"
        f"<LoadWorld>{WORLD}</LoadWorld>"
        "</MyConfigDedicated>\n",
        encoding="utf-8",
    )


def _alive() -> int | None:
    try:
        pid = int(PID.read_text().strip())
        Path(f"/proc/{pid}").stat()
    except (OSError, ValueError):
        return None
    return pid


def start(timeout: float = 1800.0, bench: bool = False) -> None:
    if _alive():
        raise RuntimeError(f"The test server is already running, pid {_alive()}")
    for old in DATA.glob("SpaceEngineersDedicated*.log"):
        old.unlink()
    PROBE_DIR.mkdir(parents=True, exist_ok=True)
    for name in ("cmd", "out"):
        (PROBE_DIR / name).unlink(missing_ok=True)
    guard = (
        [PARALLEL / "run-bench.sh", "se1", "5", "--"]
        if bench
        else [PARALLEL / "run-test.sh", "5", "--"]
    )
    with open(LOG, "w") as out:
        process = subprocess.Popen(
            [
                str(c)
                for c in [
                    *guard,
                    MAGNETAR / "MagnetarInterim.bin",
                    "-multiInstance",
                    "-stableLogs",
                    "-noimplicitmod",
                    "-consent",
                    "deny",
                    "-config",
                    CONFIG,
                    "-ds64",
                    DS64,
                    "-path",
                    DATA,
                ]
            ],
            cwd=MAGNETAR,
            env={
                **os.environ,
                "DOCK_PROBE_DIR": str(PROBE_DIR),
                "SE_DIRECT_TRANSPORT": "1",
            },
            stdout=out,
            stderr=subprocess.STDOUT,
            stdin=subprocess.DEVNULL,
            start_new_session=True,
        )
    PID.write_text(str(process.pid))
    deadline = time.monotonic() + timeout
    while "Game ready" not in LOG.read_text(errors="replace"):
        if process.poll() is not None:
            raise RuntimeError(f"The server exited, see {LOG}")
        if time.monotonic() > deadline:
            raise TimeoutError(f"The server did not get ready, see {LOG}")
        time.sleep(2)


def stop(timeout: float = 180.0) -> None:
    """SIGTERM makes Magnetar save the world and quit"""
    pid = _alive()
    if pid is None:
        return
    os.killpg(pid, signal.SIGTERM)
    deadline = time.monotonic() + timeout
    while True:
        try:
            os.waitpid(pid, os.WNOHANG)
        except ChildProcessError:
            pass
        try:
            os.killpg(pid, 0)
        except ProcessLookupError:
            break
        if time.monotonic() > deadline:
            os.killpg(pid, signal.SIGKILL)
            break
        time.sleep(1)
    PID.unlink(missing_ok=True)


def kill() -> None:
    """Stops the server without a save, like a crash"""
    pid = _alive()
    if pid is not None:
        os.killpg(pid, signal.SIGKILL)
        time.sleep(2)
    PID.unlink(missing_ok=True)


def probe(command: str, timeout: float = 600.0) -> str:
    """Runs one probe command and returns its reply"""
    out = PROBE_DIR / "out"
    out.unlink(missing_ok=True)
    tmp = PROBE_DIR / "cmd.tmp"
    tmp.write_text(command)
    tmp.rename(PROBE_DIR / "cmd")
    deadline = time.monotonic() + timeout
    while not out.exists():
        if _alive() is None:
            raise RuntimeError(f"The server exited during {command!r}, see {LOG}")
        if time.monotonic() > deadline:
            raise TimeoutError(command)
        time.sleep(0.2)
    reply = out.read_text()
    if "error=" in reply:
        raise RuntimeError(f"{command!r}: {reply}")
    return reply.strip()


def fields(line: str) -> dict[str, str]:
    return dict(pair.split("=", 1) for pair in line.split())


def state() -> dict[str, dict[str, str]]:
    return {
        f["grid"]: f for f in (fields(line) for line in probe("state").splitlines())
    }


# ---------------------------------------------------------------------------
# A headless client for mp_check.py
# ---------------------------------------------------------------------------


def start_client() -> "RemoteAPI":
    """Joins the test server over DirectTransport, returns its Remote API once it plays"""
    sys.path.insert(0, str(REMOTE / "skills" / "se-remote"))
    from se_remote import RemoteAPI

    ports = slot()
    launcher = CLIENT_ROOT / "Interim.bin"
    if not launcher.exists():
        subprocess.run(
            [str(NEW_PULSAR), str(CLIENT_ROOT), str(PULSAR)],
            check=True,
            stdout=subprocess.DEVNULL,
        )
    legacy = CLIENT_ROOT / "Legacy"
    if not (legacy / "Sources").exists():
        template = Path(os.environ["CS_PULSAR_CONFIG"])
        shutil.copytree(template / "Sources", legacy / "Sources")
        shutil.copy(template / "config.xml", legacy / "config.xml")
    (legacy / "Profiles").mkdir(parents=True, exist_ok=True)
    (legacy / "Profiles" / "Current.xml").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        '<Profile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Name>Current</Name><GitHub /><DevFolder>'
        "<LocalFolderConfig><Id>remote</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        "<LocalFolderConfig><Id>direct-transport</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        "<LocalFolderConfig><Id>bugfixes</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        f"<LocalFolderConfig><Id>{PERFORMANCE_CLIENT_ID}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        "</DevFolder><Local /><Mods /></Profile>\n",
        encoding="utf-8",
    )
    tree = ET.parse(legacy / "Sources" / "sources.xml")
    root = tree.getroot()
    local = root.find("LocalPluginSources")
    if local is not None:
        root.remove(local)
    local = ET.SubElement(root, "LocalPluginSources")
    for name, folder, file in (
        ("remote", REMOTE, "Remote.xml"),
        ("direct-transport", TRANSPORT, "DirectTransportClient.xml"),
        ("bugfixes", BUGFIXES, "BugfixesClient.xml"),
        (PERFORMANCE_CLIENT_ID, REPO, "PerformanceClient.xml"),
    ):
        plugin = ET.SubElement(local, "LocalPlugin")
        for tag, value in (
            ("Name", name),
            ("Folder", str(folder)),
            ("File", file),
            ("Enabled", "true"),
        ):
            ET.SubElement(plugin, tag).text = value
    tree.write(
        legacy / "Sources" / "sources.xml", encoding="utf-8", xml_declaration=True
    )

    port = ports["REMOTE_PORT_BASE"]
    CLIENT_DATA.mkdir(parents=True, exist_ok=True)
    (CLIENT_DATA / "Remote.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n'
        "<PluginConfig><Enabled>true</Enabled><ListenIP>127.0.0.1</ListenIP>"
        f"<ListenPort>{port}</ListenPort><AdminPassword>SpaceEngineers</AdminPassword>"
        "</PluginConfig>\n",
        encoding="utf-8",
    )
    if not (CLIENT_DATA / "SpaceEngineers.cfg").exists():
        shutil.copy(
            HOME / ".config/SpaceEngineers/SpaceEngineers.cfg",
            CLIENT_DATA / "SpaceEngineers.cfg",
        )
    with open(CLIENT_DATA / "launch.log", "w") as out:
        process = subprocess.Popen(
            [
                str(c)
                for c in [
                    PARALLEL / "run-test.sh",
                    "4",
                    "--",
                    launcher,
                    "-multiInstance",
                    "-bin64",
                    BIN64,
                    "-lazySteam",
                    "-noprompt",
                    "-noupdate",
                    "-nosplash",
                    "-stablelogs",
                    "-sources",
                    "--no-steam",
                    "--client-id",
                    ports["CLIENT_ID_BASE"],
                    "--client-name",
                    "Pilot",
                    "-appdata",
                    CLIENT_DATA,
                    "--quality",
                    "minimal",
                    "--resolution",
                    "640x480",
                    "--no-audio",
                    "--headless",
                    "--no-render",
                    "--connect",
                    f"127.0.0.1:{ports['DS_SERVER_PORT_BASE']}",
                ]
            ],
            cwd=CLIENT_ROOT,
            env={**os.environ, "PULSAR_NO_RENDER": "true"},
            stdout=out,
            stderr=subprocess.STDOUT,
            stdin=subprocess.DEVNULL,
            start_new_session=True,
        )
    CLIENT_PID.write_text(str(process.pid))

    api = RemoteAPI(
        f"http://127.0.0.1:{port}",
        username="admin",
        password="SpaceEngineers",
        timeout=30.0,
    )
    deadline = time.monotonic() + 1500
    while time.monotonic() < deadline:
        if process.poll() is not None:
            raise RuntimeError(f"The client exited, see {CLIENT_DATA / 'launch.log'}")
        try:
            boxes = [
                s
                for s in api.list_screens()
                if s.get("type") == "MyGuiScreenMessageBox"
            ]
            for box in boxes:
                api.control_click(text="OK", screen=box["index"])
            if not boxes and api.get_state().get("ready"):
                return api
        except Exception:  # noqa: BLE001 -- not listening yet, or busy loading
            pass
        time.sleep(3)
    raise TimeoutError("The client did not get into the world")


def ensure_character(api, timeout: float = 300.0) -> None:
    """Respawns at the first spawn option until there is a live character"""
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        try:
            if api.get_character().get("state") not in (None, "dead"):
                return
        except Exception:  # noqa: BLE001 -- 503 without a character
            pass
        try:
            screens = api.list_screens()
            medical = next(
                (i for i, s in enumerate(screens) if "Medical" in s.get("type", "")),
                None,
            )
            if medical is not None:
                for button in ("Join", "Respawn"):
                    try:
                        api.control_click(text=button, screen=medical)
                        break
                    except Exception:  # noqa: BLE001 -- the other page of the screen
                        continue
        except Exception:  # noqa: BLE001 -- slow while the world streams in
            pass
        time.sleep(3)
    raise TimeoutError("No live character")


def focus_gameplay(api, timeout: float = 30.0) -> None:
    """Closes the welcome screen and anything else above the gameplay screen"""
    keep = ("MyGuiScreenGamePlay", "MyGuiScreenHudSpace")
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        screens = [s.get("type") for s in api.list_screens()]
        extra = [i for i, kind in enumerate(screens) if kind not in keep]
        if not extra:
            return
        if "MyGuiScreenLoading" not in screens:
            api.close_screen(extra[-1])
        time.sleep(0.5)
    raise TimeoutError("Could not get back to the gameplay screen")


def stop_client() -> None:
    try:
        pid = int(CLIENT_PID.read_text().strip())
        os.killpg(pid, signal.SIGTERM)
    except (OSError, ValueError):
        return
    deadline = time.monotonic() + 60
    while time.monotonic() < deadline:
        try:
            os.waitpid(pid, os.WNOHANG)
        except ChildProcessError:
            pass
        try:
            os.killpg(pid, 0)
        except ProcessLookupError:
            break
        time.sleep(1)
    else:
        os.killpg(pid, signal.SIGKILL)
    CLIENT_PID.unlink(missing_ok=True)
