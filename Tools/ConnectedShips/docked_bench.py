#!/usr/bin/env python3
"""SE-797: what a ship docked to a station costs, with the game's own conversion of rooted
grids to fixed bodies and with that conversion undone.

N ships are docked to one static grid. The script first prints each grid's physics state, then
alternates the two modes on the running server, so the comparison survives other load drifting.

usage: docked_bench.py <ships> <length> <rotor 0|1> [rounds] [seconds] [--no-bench-lock]
"""

import sys

import rig


def main(
    ships: int, length: int, rotor: bool, rounds: int, seconds: int, bench: bool
) -> None:
    rig.prepare_world()
    rig.prepare_server(performance=False)
    rig.start(bench=bench)
    try:
        rig.probe(f"station Station {ships} {length + 8}")
        for i in range(ships):
            name = f"Ship{i}"
            rig.probe(
                f"spawn {name} 0 {length} 5 3 {int(rotor)} {500 + i * 60} 500 500"
            )
            print(rig.probe(f"dock Station {i} {name} 0"), flush=True)
            if rotor:
                rig.probe(f"rotor {name}")
        rig.probe("stats 300")

        report = rig.state()
        print(
            "vanilla state of Ship0:",
            {
                k: report["Ship0"][k]
                for k in (
                    "blocks",
                    "static",
                    "physics_static",
                    "motion",
                    "rooted",
                    "early",
                    "mass",
                )
            },
            flush=True,
        )
        fixed = sum(
            1
            for name, grid in report.items()
            if name.startswith("Ship") and grid["physics_static"] == "True"
        )
        print(f"ships with a fixed body: {fixed}/{ships}", flush=True)

        print(rig.probe("stats 1200"), "(fixed, baseline)", flush=True)
        results = {"fixed": [], "dynamic": []}
        ticks = seconds * 60
        for _ in range(rounds):
            for mode in ("dynamic", "fixed"):
                rig.probe(f"motion {mode}")
                rig.probe("stats 300")
                line = rig.probe(f"stats {ticks}")
                tops = [g for g in rig.state().values() if int(g["blocks"]) == 1]
                spinning = sum(1 for g in tops if float(g["w"]) > 0.1)
                print(
                    mode,
                    line,
                    f"rotor_tops={len(tops)} spinning={spinning}",
                    flush=True,
                )
                results[mode].append(rig.fields(line))

        print("\nsummary (mean of rounds)")
        for mode, rows in results.items():
            keys = (
                "step_mean",
                "step_p99",
                "tick_mean",
                "active_bodies",
                "active_islands",
            )
            means = {k: sum(float(r[k]) for r in rows) / len(rows) for k in keys}
            print(mode, " ".join(f"{k}={v:.3f}" for k, v in means.items()))
        rig.probe("motion fixed")
        rig.probe("undock Ship0")
        report = rig.state()
        print(
            "after undock of Ship0:",
            {
                k: report["Ship0"][k]
                for k in ("physics_static", "motion", "rooted", "mass")
            },
        )
    finally:
        rig.stop()


if __name__ == "__main__":
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    main(
        int(args[0]),
        int(args[1]),
        args[2] == "1",
        int(args[3]) if len(args) > 3 else 3,
        int(args[4]) if len(args) > 4 else 30,
        "--no-bench-lock" not in sys.argv,
    )
