#!/usr/bin/env python3
"""SE-798: which ships keep their dampeners and gyros, on an isolated Magnetar server with no
client. Prints PASS or FAIL per check and a summary, and exits non-zero on a failure. See client
test mp_check.py for a real pilot.

usage: test_connected.py [--no-gravity | --gravity-only]
"""

import re
import sys
import time

import rig

failures = []


def check(name: str, ok: bool, detail: object = "") -> None:
    print(
        ("PASS " if ok else "FAIL ")
        + name
        + (f"  {detail}" if detail and not ok else ""),
        flush=True,
    )
    if not ok:
        failures.append(name)


def quiet(names: list[str], kind: str = "thrust") -> list[str]:
    report = rig.state()
    return [n for n in names if report[n][f"quiet_{kind}"] == "True"]


def settle() -> None:
    # The rule is worked out once a second
    rig.probe("stats 90")


def group(prefix: str, ships: list, at: float) -> list[str]:
    names = [f"{prefix}{i}" for i in range(len(ships))]
    for i, (length, thrust, gyros) in enumerate(ships):
        rig.probe(
            f"spawn {names[i]} 0 {length} {thrust} {gyros} 0 {at + i * 200} 2000 2000"
        )
    for a, b in zip(names, names[1:]):
        reply = rig.probe(f"dock {a} 1 {b} 0")
        check(f"{prefix}: {a} locks to {b}", "connected=True" in reply, reply)
    settle()
    return names


def push(name: str) -> None:
    rig.probe(f"input {name} 0 0 -1 0 0 0")
    settle()


def release(name: str) -> None:
    rig.probe(f"input {name} 0 0 0 0 0 0")
    rig.probe(f"kick {name} 0 0 0 0 0 0")
    rig.probe("stats 300")


def test_rule(at: float) -> None:
    names = group("rule", [(25, 15, 9), (12, 6, 3), (8, 3, 1)], at)
    check("at rest every ship stays on", quiet(names) == [], quiet(names))

    push(names[0])
    check(
        "pushed: the strongest keeps, the others go quiet",
        quiet(names) == names[1:],
        quiet(names),
    )
    check("same for gyros", quiet(names, "gyro") == names[1:], quiet(names, "gyro"))
    report = rig.state()
    check(
        "no ship's own setting changed",
        all(
            report[n]["dampeners"] == "True" and report[n]["comp_dampeners"] == "True"
            for n in names
        ),
        report,
    )
    release(names[0])
    check("at rest again every ship is back on", quiet(names) == [], quiet(names))

    # A ship which is flown stays as it is, the strongest of the rest too
    rig.probe(f"fly {names[2]} 1")
    push(names[2])
    check(
        "a flown weak ship stays on, the strongest too",
        quiet(names) == names[1:2],
        quiet(names),
    )
    release(names[2])
    rig.probe(f"fly {names[2]} 0")
    rig.probe(f"fly {names[0]} 1")
    push(names[0])
    check(
        "with the strongest flown, the others are quiet",
        quiet(names) == names[1:],
        quiet(names),
    )
    release(names[0])
    rig.probe(f"fly {names[0]} 0")

    # What a player sets by hand is the player's: the ship without dampeners doesn't count
    rig.probe(f"dampeners {names[0]} 0")
    push(names[1])
    report = rig.state()
    check(
        "dampeners off on the strongest stay off",
        report[names[0]]["dampeners"] == "False",
    )
    check(
        "the next strongest takes over",
        names[1] not in quiet(names) and names[2] in quiet(names),
        quiet(names),
    )
    release(names[1])
    rig.probe(f"dampeners {names[0]} 1")
    push(names[0])
    rig.probe(f"dampeners {names[2]} 0")
    settle()
    report = rig.state()
    check("a quiet ship can be set by hand", report[names[2]]["dampeners"] == "False")
    rig.probe(f"dampeners {names[2]} 1")
    settle()
    report = rig.state()
    check("and set back", report[names[2]]["dampeners"] == "True")

    # Disconnecting restores a ship at once
    rig.probe(f"undock {names[2]}")
    check(
        "the undocked ship is on right away", names[2] not in quiet(names), quiet(names)
    )
    check(
        "the rest of the group keeps the rule", quiet(names) == names[1:2], quiet(names)
    )
    rig.probe(f"undock {names[1]}")
    check("all ships on after undocking everything", quiet(names) == [], quiet(names))
    release(names[0])


def test_separate_strongest(at: float) -> None:
    names = group("split", [(25, 15, 0), (8, 3, 3)], at)
    push(names[0])
    check(
        "thrust stays with the strongest thrusters",
        quiet(names) == names[1:],
        quiet(names),
    )
    check(
        "gyros stay with the strongest gyros",
        quiet(names, "gyro") == names[:1],
        quiet(names, "gyro"),
    )


def test_stops(at: float) -> None:
    names = group("stop", [(25, 15, 9), (8, 3, 1)], at)
    rig.probe(f"kick {names[0]} 20 0 0 0.2 0.1 0")
    rig.probe("stats 90")
    check("moving: the weak ship is quiet", quiet(names) == names[1:], quiet(names))
    result = rig.fields(rig.probe(f"trace {names[0]} 2400"))
    check("the group comes to a stop", result["settle_s"] != "-1", result)
    check("stopped: every ship is back on", quiet(names) == [], quiet(names))
    print("stop:", result, flush=True)


def test_station(at: float) -> None:
    names = group("base", [(12, 6, 3), (8, 3, 1)], at)
    rig.probe(f"spawn base_st 1 6 0 0 0 {at} 3000 2000")
    # A static grid: the fixed body rule of the game takes over, nothing stays quiet
    rig.probe(f"dock base_st 1 {names[0]} 0")
    push(names[0])
    report = rig.state()
    check(
        "a group docked at a static grid isn't touched",
        quiet(names) == [],
        quiet(names),
    )
    check(
        "and its ships have fixed bodies",
        all(report[n]["physics_static"] == "True" for n in names),
        report,
    )


def test_restart() -> None:
    """Runs last: both a clean restart and a crash keep the settings and the rule"""
    names = group("keep", [(25, 15, 9), (8, 3, 1)], 90000.0)
    rig.probe(f"dampeners {names[1]} 0")
    push(names[0])
    check(
        "a ship with its dampeners off isn't the one to keep them",
        names[0] not in quiet(names),
        quiet(names),
    )
    rig.probe(f"dampeners {names[1]} 1")
    settle()
    check("before the restart", quiet(names) == names[1:], quiet(names))
    rig.probe("save")
    time.sleep(10)

    text = (rig.WORLD / "SANDBOX_0_0_0_.sbs").read_text(encoding="utf-8")
    # The game leaves the value out of the save when it is on, which is the default
    saved = [
        "<DampenersEnabled>false" not in grid
        for grid in text.split("<MyObjectBuilder_EntityBase ")
        if re.search(r"<DisplayName>keep\d</DisplayName>", grid)
    ]
    check(
        "the quiet ship is saved with its dampeners on",
        saved == [True, True],
        saved,
    )

    rig.kill()
    rig.start()
    settle()
    report = rig.state()
    check(
        "after a crash the ships are on",
        all(report[n]["dampeners"] == "True" for n in names),
        report,
    )
    push(names[0])
    check(
        "and they are still connected and follow the rule",
        quiet(names) == names[1:],
        quiet(names),
    )
    rig.stop()
    rig.start()
    settle()
    push(names[0])
    check("after a clean restart too", quiet(names) == names[1:], quiet(names))
    release(names[0])


def test_gravity() -> None:
    # No atmosphere, so thrusters give all they have
    planet = rig.fields(rig.probe("planet Moon 500000 0 0"))
    # Just above the highest hills, and well inside the moon's gravity
    at = float(planet["x"]) - float(planet["radius"]) - 1000
    y, z = float(planet["y"]), float(planet["z"])
    names = ["weak0", "weak1"]
    for name, x in zip(names, (0, 200)):
        rig.probe(f"spawn {name} 0 20 8 1 0 {at} {y + x} {z}")
    rig.probe("stats 60")
    check("dock", "connected=True" in rig.probe("dock weak0 1 weak1 0"))
    push(names[0])
    report = rig.state()
    check("the planet pulls", float(report["weak0"]["gravity"]) > 1, report["weak0"])
    check(
        "nobody goes quiet when one ship can't hold both",
        quiet(names) == [],
        quiet(names),
    )
    rig.probe(f"input {names[0]} 0 0 0 0 0 0")
    rig.probe("stats 600")
    report = rig.state()
    check("each holds itself up", float(report["weak0"]["v"]) < 1.0, report["weak0"])
    print(
        "weak:",
        {
            n: (
                report[n]["v"],
                report[n]["thrust"],
                report[n]["mass"],
                report[n]["gravity"],
            )
            for n in names
        },
    )

    names = ["lift0", "lift1"]
    for name, x, thrust in (("lift0", 1000, 40), ("lift1", 1200, 8)):
        rig.probe(f"spawn {name} 0 20 {thrust} 1 0 {at} {y + x} {z}")
    rig.probe("stats 60")
    check("dock", "connected=True" in rig.probe("dock lift0 1 lift1 0"))
    push("lift0")
    check(
        "the weak ship goes quiet when the strong one holds both",
        quiet(names) == ["lift1"],
        quiet(names),
    )
    rig.probe("input lift0 0 0 0 0 0 0")
    rig.probe("stats 1200")
    report = rig.state()
    check(
        "the group comes to rest without falling",
        float(report["lift0"]["v"]) < 1.0,
        report["lift0"],
    )


def test_off() -> None:
    names = group("off", [(25, 15, 9), (8, 3, 1)], 110000.0)
    push(names[0])
    check("on by default", quiet(names) == names[1:], quiet(names))
    release(names[0])
    rig.stop()
    rig.set_option(False)
    rig.start()
    settle()
    push(names[0])
    check("the setting turns it off", quiet(names) == [], quiet(names))
    release(names[0])
    (rig.DATA / "Performance.cfg").unlink(missing_ok=True)


def main(gravity: bool) -> None:
    rig.prepare_world()
    rig.prepare_server(performance=True)
    (rig.DATA / "Performance.cfg").unlink(missing_ok=True)
    rig.start()
    try:
        if "--gravity-only" not in sys.argv:
            test_rule(20000.0)
            test_separate_strongest(30000.0)
            test_stops(40000.0)
            test_station(50000.0)
        if gravity:
            test_gravity()
        if "--gravity-only" not in sys.argv:
            test_restart()
            test_off()
    finally:
        rig.stop()

    print(f"\n{'FAILED: ' + ', '.join(failures) if failures else 'ALL PASSED'}")
    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main("--no-gravity" not in sys.argv)
