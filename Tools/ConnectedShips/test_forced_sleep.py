"""Exercise forced sleep, managed activation, transfer release and pilot wake on Magnetar."""

import os

import rig


def main():
    slot = rig.slot()
    try:
        rig.prepare_world()
        rig.prepare_server(performance=True)
        (rig.DATA / "Performance.cfg").write_text(
            "<PerformanceConfig><SleepSupportedGrids>true</SleepSupportedGrids>"
            "<SleepHoldSeconds>0.25</SleepHoldSeconds></PerformanceConfig>"
        )
        rig.start(180)
        rig.probe("station Support 1 5")
        rig.probe("spawn Free 0 5 0 0 0 500 0 0")
        rig.probe("spawn Ship 0 8 1 0 0 200 0 0")
        result = rig.probe("force-sleep-check Ship Support", 30)
        print(result, flush=True)
        for expected in (
            "unsupportedFree=True",
            "supportedHeld=True asleep=True",
            "activationHeld=True",
            *(
                ["clusterReleased=True stillAsleep=True", "transferExcluded=True"]
                if os.getenv("SLEEP_TEST_CLUSTER_DLL")
                else ["transferReleased=True stillAsleep=True"]
            ),
            "sleptAgain=True",
            "controlReleased=True awake=True",
            "heldBeforeDisable=True",
            "disabledReleased=True registered=False",
        ):
            assert expected in result, expected
    finally:
        try:
            rig.kill()
        finally:
            rig._run(rig.PARALLEL / "slot.sh", "release", slot["SLOT"])


if __name__ == "__main__":
    main()
