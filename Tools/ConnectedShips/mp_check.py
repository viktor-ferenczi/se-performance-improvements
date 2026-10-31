#!/usr/bin/env python3
"""SE-798 with a real pilot: a headless client over DirectTransport flies the connected ships.
Client and server have to agree on what the ship does and on its dampener settings, and the
client's input must take effect. Prints PASS or FAIL per check.
"""

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


NAMES = ["mp0", "mp1", "mp2"]


def quiet() -> list[str]:
    report = rig.state()
    return [n for n in NAMES if report[n]["quiet_thrust"] == "True"]


def grid_of(api, name: str) -> dict:
    return next(
        g
        for g in api.list_grids()
        if g.get("name") == name or g.get("displayName") == name
    )


def main() -> None:
    rig.prepare_world()
    rig.prepare_server(performance=True, transport=True)
    (rig.DATA / "Performance.cfg").unlink(missing_ok=True)
    rig.start()
    api = None
    try:
        for i, (length, thrust, gyros) in enumerate(
            ((25, 15, 9), (12, 6, 3), (8, 3, 1))
        ):
            rig.probe(
                f"spawn {NAMES[i]} 0 {length} {thrust} {gyros} 0 {20000 + i * 200} 2000 2000"
            )
        for a, b in zip(NAMES, NAMES[1:]):
            check(
                f"{a} locks to {b}", "connected=True" in rig.probe(f"dock {a} 1 {b} 0")
            )
        rig.probe("stats 90")

        api = rig.start_client()
        rig.ensure_character(api)
        rig.focus_gameplay(api)

        # The weak ship is flown: it stays on, the strongest too, the middle one goes quiet
        rig.probe("pilot mp2 1")
        rig.probe("stats 120")
        report = rig.state()
        check(
            "the server has the pilot in the weak ship",
            report["mp2"]["controlled"] == "True",
            report["mp2"],
        )
        rig.probe("stats 120")
        check("seated and at rest nobody is quiet", quiet() == [], quiet())
        before = rig.state()
        api.set_input_state(keys=["W"], mode="override")
        rig.probe("stats 120")
        check(
            "flown and pushing: only the middle ship is quiet",
            quiet() == ["mp1"],
            quiet(),
        )
        api.clear_input_state()
        rig.probe("stats 300")
        rig.probe("pilot mp2 0")
        rig.probe("stats 120")

        # The strongest ship is flown: nobody fights it
        rig.probe("pilot mp0 1")
        rig.probe("stats 120")
        api.set_input_state(keys=["W"], mode="override")
        rig.probe("stats 300")
        check(
            "flown strongest: both others are quiet", quiet() == ["mp1", "mp2"], quiet()
        )
        report = rig.state()
        speed = float(report["mp0"]["v"])
        check(
            "the group speeds up under the client's input", speed > 1.0, report["mp0"]
        )
        print("speed after 5 s:", speed, flush=True)

        # The client follows the server
        client_speed = float(grid_of(api, "mp0")["speed"])
        print("client sees:", client_speed, flush=True)
        check("the client sees the group moving", client_speed > 1.0, client_speed)

        api.clear_input_state()
        rig.probe("stats 60")
        report = rig.state()
        check(
            "dampener settings stay on, on the server",
            all(report[n]["dampeners"] == "True" for n in NAMES),
            report,
        )

        # The pilot's own dampeners bring the group to a stop. The others are back on at rest.
        rig.probe("trace mp0 1800")
        report = rig.state()
        check(
            "the group stops under the flown ship's dampeners",
            float(report["mp0"]["v"]) < 0.05,
            report["mp0"],
        )
        check("stopped: nobody is quiet", quiet() == [], quiet())

        # Undock under way
        api.set_input_state(keys=["W"], mode="override")
        rig.probe("stats 180")
        rig.probe("undock mp2")
        report = rig.state()
        check("the undocked ship is on again right away", "mp2" not in quiet(), quiet())
        api.clear_input_state()
        rig.probe("pilot mp0 0")
        rig.probe("stats 60")
        print("client still in the world:", api.get_state().get("ready"), flush=True)
        check("the client stays in the world", bool(api.get_state().get("ready")))
    finally:
        rig.stop_client()
        rig.stop()

    print(f"\n{'FAILED: ' + ', '.join(failures) if failures else 'ALL PASSED'}")
    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
