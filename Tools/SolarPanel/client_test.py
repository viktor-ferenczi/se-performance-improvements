"""Solar panel rig, client side: loads the world of build_world.py offline on a headless, isolated
Pulsar client with the client plugin of this checkout and the solar probe, then prints the
probe's 10 second lines and their averages. In an offline game the client is the server, so
this checks the client plugin's patch the way rig.py checks the server plugin's.

    SOLAR_SLOT=18 python3 client_test.py <world dir> [--option NAME=VALUE] [--seconds 120]

Needs the Remote plugin checkout (REMOTE_REPO, default se1/plugins/remote) for loading the world,
and httpx for its client (uv run --no-project --with httpx python3 client_test.py ...). BIN64
names the game's Bin64 folder on hosts without a Steam client.
Slot rules: testing/parallel-test-work/README.md in the workspace.
"""

import argparse
import os
import re
import shutil
import signal
import subprocess
import sys
import time
from pathlib import Path

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))
import rig  # noqa: E402

HOME = rig.HOME
REMOTE = Path(os.environ.get("REMOTE_REPO", rig.WORKSPACE / "se1/plugins/remote"))
PULSAR = HOME / f".se-test/{rig.TASK}-c0"
APPDATA = HOME / f".se-test/{rig.TASK}-c0-data"


def prepare(world: Path, options: list[str]) -> tuple[Path, int]:
    if not (PULSAR / "Interim.bin").exists():
        subprocess.run(
            [
                str(
                    rig.WORKSPACE
                    / "testing/pulsar-dev-instances/new-pulsar-instance.sh"
                ),
                str(PULSAR),
                str(HOME / ".config/Pulsar"),
            ],
            check=True,
            stdout=subprocess.DEVNULL,
        )
    legacy = PULSAR / "Legacy"
    sources = legacy / "Sources/sources.xml"
    # A Pulsar install that never ran the Legacy edition has no sources yet. The core plugins
    # (dotnet-compat, linux-compat) come from PluginHub.
    if not sources.exists():
        sources.parent.mkdir(parents=True, exist_ok=True)
        sources.write_text(
            '<?xml version="1.0" encoding="utf-8"?>\n<SourcesConfig><ShowWarning>false</ShowWarning>'
            "<LocalHubSources /><RemoteHubSources><RemoteHub><Name>PluginHub</Name>"
            "<Repo>StarCpt/PluginHub</Repo><Branch>main</Branch><Enabled>true</Enabled>"
            "<Trusted>true</Trusted></RemoteHub></RemoteHubSources><RemotePluginSources />"
            "<LocalPluginSources /><ModSources /></SourcesConfig>\n"
        )
    rig.register_source(sources, "remote", REMOTE, "Remote.xml")
    rig.register_source(sources, "performance", rig.REPO, "PerformanceClient.xml")
    rig.register_source(sources, "solar-probe", HERE / "probe-plugin", "SolarProbe.xml")
    ids = ["remote", "viktor-ferenczi/se-performance-improvements", "solar-probe"]
    folders = "".join(
        f"<LocalFolderConfig><Id>{i}</Id><DebugBuild>false</DebugBuild></LocalFolderConfig>"
        for i in ids
    )
    (legacy / "Profiles").mkdir(exist_ok=True)
    (legacy / "Profiles/Current.xml").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n<Profile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        f'xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Name>Current</Name><GitHub /><DevFolder>{folders}</DevFolder>'
        "<Local /><Mods /></Profile>\n"
    )

    port = 25000 + 10 * rig.SLOT
    (APPDATA / "Storage").mkdir(parents=True, exist_ok=True)
    (APPDATA / "Remote.cfg").write_text(
        '<?xml version="1.0" encoding="utf-8"?>\n<PluginConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" '
        'xmlns:xsd="http://www.w3.org/2001/XMLSchema">\n  <Enabled>true</Enabled>\n  <ListenIP>127.0.0.1</ListenIP>\n'
        f"  <ListenPort>{port}</ListenPort>\n  <AdminPassword>SpaceEngineers</AdminPassword>\n</PluginConfig>\n"
    )
    # Options it lacks keep their defaults
    settings = "".join(f"<{k}>{v}</{k}>" for k, v in (o.split("=", 1) for o in options))
    (APPDATA / "Storage/Performance.cfg").write_text(
        f'<?xml version="1.0" encoding="utf-8"?>\n<Config>{settings}</Config>\n'
    )
    game_cfg = APPDATA / "SpaceEngineers.cfg"
    if not game_cfg.exists():
        shutil.copy(HOME / ".config/SpaceEngineers/SpaceEngineers.cfg", game_cfg)
    text = re.sub(
        r"(<Key>ExperimentalMode</Key>\s*<Value[^>]*>)[^<]*(</Value>)",
        lambda m: m.group(1) + "true" + m.group(2),
        game_cfg.read_text(),
    )
    game_cfg.write_text(text)

    target = APPDATA / "Saves/tests/SolarField"
    shutil.rmtree(target, ignore_errors=True)
    shutil.copytree(world, target)
    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        path = target / name
        path.write_text(
            re.sub(
                r"<OnlineMode>[^<]*</OnlineMode>",
                "<OnlineMode>OFFLINE</OnlineMode>",
                path.read_text(),
            )
        )
    for old in APPDATA.glob("*.log"):
        old.unlink()
    return target, port


def probe_lines() -> list[tuple]:
    found = [
        rig.LINE.findall(p.read_text(errors="replace")) for p in APPDATA.glob("*.log")
    ]
    return max(found, key=len, default=[])


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("world", type=Path)
    ap.add_argument("--option", action="append", default=[])
    ap.add_argument("--seconds", type=int, default=120)
    args = ap.parse_args()

    sys.path.insert(0, str(REMOTE / "skills/se-remote"))
    from se_remote import RemoteAPI

    target, port = prepare(args.world.resolve(), args.option)
    launch = [
        str(PULSAR / "Interim.bin"),
        "-multiInstance", "-lazySteam", "-noprompt", "-noupdate", "-nosplash", "-stablelogs", "-sources",
        "--no-steam", "-appdata", str(APPDATA), "--quality", "minimal", "--resolution", "640x480",
        "--no-audio", "--headless", "--no-render", "--client-id", str(76561199500010000 + 10 * rig.SLOT),
    ]  # fmt: skip
    # Hosts without a Steam client have no libraryfolders.vdf for Pulsar to find the game in
    if "BIN64" in os.environ:
        launch += ["-bin64", os.environ["BIN64"]]
    runner = rig.PARALLEL / "run-test.sh"
    # Older workspaces have one lock for both games, and their scripts take no game argument
    game = ["se1"] if "se1|se2" in runner.read_text() else []
    started = time.monotonic()
    process = subprocess.Popen(
        [str(runner), *game, "4", "--", *launch],
        cwd=PULSAR,
        env={**os.environ, "PULSAR_NO_RENDER": "true"},
        stdout=open(APPDATA / "launch.out", "w"),
        stderr=subprocess.STDOUT,
        stdin=subprocess.DEVNULL,
        start_new_session=True,
    )
    wanted = args.seconds // 10
    try:
        api = RemoteAPI(
            f"http://127.0.0.1:{port}", username="admin", password="SpaceEngineers"
        )
        deadline = started + 1800
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
        deadline = time.monotonic() + 900 + args.seconds
        while (
            len(probe_lines()) < wanted + 1
            and time.monotonic() < deadline
            and process.poll() is None
        ):
            # An XML-only save stops on an INFO box ("file format of this world is outdated")
            try:
                if (
                    not api.get_state().get("ready")
                    and api.get_screen(0).get("type") == "MyGuiScreenMessageBox"
                ):
                    api.control_click("OK")
            except Exception:  # noqa: BLE001 -- no screen or no API yet
                pass
            time.sleep(5)
    finally:
        # The game may outlive the launcher in its process group
        try:
            os.killpg(process.pid, signal.SIGTERM)
            process.wait(60)
        except ProcessLookupError:
            pass
        except subprocess.TimeoutExpired:
            os.killpg(process.pid, signal.SIGKILL)

    # The first line covers the world's first moments, partly before the panels computed
    lines = probe_lines()[1 : wanted + 1]
    print(
        f"== {len(lines)} of {wanted} probe lines after {time.monotonic() - started:.0f} s"
    )
    print("fix\tcomputed\tchanged\trecomputes\trecomputeMs")
    for line in lines:
        print("\t".join(line))
    if not lines:
        print(f"no probe lines; see the logs in {APPDATA}")
        return 1
    n = len(lines)
    print(
        "mean per 10 s\t"
        + "\t".join(f"{sum(float(l[i]) for l in lines) / n:.1f}" for i in range(1, 5))
    )
    return 0 if n == wanted else 1


if __name__ == "__main__":
    sys.exit(main())
