#!/usr/bin/env python3
"""SE-798: how much ships joined by a locked connector fight each other while they stop.

Each round spawns a group far from anything, with every ship's dampeners on, kicks it to a speed
and a spin, and follows it until it should have stopped. applied/net above 1 means the ships push
against each other: applied adds up each ship's force or torque, net is what is left of their sum.

usage: fight.py <option 0|1> [--no-bench-lock]
"""

import sys

import rig

# name: ships as (length, thrusters per face, gyros), docked in a chain
GROUPS = {
    "equal": [(12, 6, 3), (12, 6, 3)],
    "big-small": [(25, 15, 9), (8, 3, 1)],
    "chain": [(25, 15, 9), (12, 6, 3), (12, 6, 3)],
    "no-gyro-strongest": [(25, 15, 0), (8, 3, 3)],
}


def summary(result: dict) -> str:
    return (
        f"settle={result['settle_s']}s"
        f" thrust_impulse={float(result['thrust_applied']):.3g}"
        f" thrust_ratio={ratio(result['thrust_applied'], result['thrust_net']):.2f}"
        f" torque_impulse={float(result['torque_applied']):.3g}"
        f" torque_ratio={ratio(result['torque_applied'], result['torque_net']):.2f}"
        f" jitter={result['jitter']} max_offset={result['max_offset']}"
        f" end_v={result['v']} end_w={result['w']}"
    )


def run_group(name: str, ships: list, at: float) -> None:
    names = [f"{name}{i}" for i in range(len(ships))]
    for i, (length, thrust, gyros) in enumerate(ships):
        rig.probe(
            f"spawn {names[i]} 0 {length} {thrust} {gyros} 0 {at + i * 200} 2000 2000"
        )
    # Back connector of a ship (1) to the front connector of the next one (0)
    for a, b in zip(names, names[1:]):
        print(rig.probe(f"dock {a} 1 {b} 0"), flush=True)
    rig.probe("stats 120")
    rig.probe(f"kick {names[0]} 20 0 0 0.2 0.1 0")
    stop = rig.fields(rig.probe(f"trace {names[0]} 3600"))
    print(f"{name} stop: {summary(stop)}", flush=True)

    # The first ship is the strongest in thrust. It pushes and turns as under a pilot, for 5 s,
    # while the others have no input: how fast the group gets and how much of it is lost.
    rig.probe(f"kick {names[0]} 0 0 0 0 0 0")
    rig.probe("stats 60")
    rig.probe(f"input {names[0]} 0 0 -1 0 0 0")
    push = rig.fields(rig.probe(f"trace {names[0]} 300"))
    print(f"{name} push: {summary(push)}", flush=True)
    rig.probe(f"input {names[0]} 0 0 0 0 0 0")
    rig.probe(f"kick {names[0]} 0 0 0 0 0 0")
    rig.probe("stats 60")
    rig.probe(f"input {names[0]} 0 0 0 1 0 0")
    turn = rig.fields(rig.probe(f"trace {names[0]} 300"))
    print(f"{name} turn: {summary(turn)}", flush=True)
    rig.probe(f"input {names[0]} 0 0 0 0 0 0")


def ratio(applied: str, net: str) -> float:
    return float(applied) / float(net) if float(net) > 0 else 0.0


def main(calm: bool, bench: bool) -> None:
    rig.prepare_world()
    rig.prepare_server(performance=True)
    rig.set_option(calm)
    rig.start(bench=bench)
    try:
        for i, (name, ships) in enumerate(GROUPS.items()):
            run_group(name, ships, (i + 1) * 20000.0)
    finally:
        rig.stop()


if __name__ == "__main__":
    main(sys.argv[1] == "1", "--no-bench-lock" not in sys.argv)
