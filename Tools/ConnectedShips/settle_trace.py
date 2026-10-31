#!/usr/bin/env python3
"""Speed of a stopping group every 5 seconds, to tell a slow stop from a wobble that never ends.

usage: settle_trace.py <option 0|1> <seconds> [--no-bench-lock]
"""

import sys

import rig


def main(calm: bool, seconds: int, bench: bool) -> None:
    rig.prepare_world()
    rig.prepare_server(performance=True)
    rig.set_option(calm)
    rig.start(bench=bench)
    try:
        for name, x in (("a", 20000), ("b", 20200)):
            rig.probe(f"spawn {name} 0 12 6 3 0 {x} 2000 2000")
        print(rig.probe("dock a 1 b 0"), flush=True)
        rig.probe("stats 120")
        rig.probe("kick a 20 0 0 0.2 0.1 0")
        for t in range(5, seconds + 1, 5):
            r = rig.fields(rig.probe("trace a 300"))
            print(
                f"t={t}s v={r['v']} w={r['w']} jitter={r['jitter']} offset={r['max_offset']}"
                f" thrust={float(r['thrust_applied']):.3g} torque={float(r['torque_applied']):.3g}",
                flush=True,
            )
            if t == 30:
                print(rig.probe("series a 40"), flush=True)
    finally:
        rig.stop()


if __name__ == "__main__":
    main(sys.argv[1] == "1", int(sys.argv[2]), "--no-bench-lock" not in sys.argv)
