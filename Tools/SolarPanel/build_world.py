"""Builds the solar panel test world: the game's Empty World with one static grid that is a
flat sheet of large solar panels, all facing the same way, turned so the sun is at a known
angle when the world starts and moving fast across the panels.

    python3 build_world.py <Empty World dir> --out <world dir> [--columns 30] [--rows 40]
        [--angle 60] [--sun-minutes 120]

The sun of the Empty World starts at its base direction when the elapsed game time is zero and
turns around a fixed axis. The panels' normal is put --angle degrees from the base direction,
towards where the sun goes, so a panel starts at cos(angle) of its maximum output and the
output changes at sin(angle) of the fastest rate (at 60 degrees: 50% output, 87% of the
fastest rate). If the sun turns the other way the angle grows instead, with a similar rate.
"""

import argparse
import math
import re
import shutil
from pathlib import Path

# MySunProperties.Default.BaseSunDirectionNormalized, also the Empty World's base direction
SUN = (0.33946735, 0.70979536, -0.61721337)
UP = (0.0, 1.0, 0.0)


def cross(a, b):
    return (
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0],
    )


def norm(a):
    length = math.sqrt(sum(x * x for x in a))
    return tuple(x / length for x in a)


def vec(tag, v):
    return f'<{tag} x="{v[0]:.7f}" y="{v[1]:.7f}" z="{v[2]:.7f}" />'


def panel_normal(angle_deg):
    # MySunProperties.SunRotationAxis; the sun turns from SUN towards cross(axis, SUN)
    axis = norm(cross(cross(SUN, UP), SUN))
    towards = norm(cross(axis, SUN))
    a = math.radians(angle_deg)
    normal = tuple(math.cos(a) * s + math.sin(a) * t for s, t in zip(SUN, towards))
    return normal, axis


def panels_grid(columns, rows, forward, up):
    blocks = "".join(
        '<MyObjectBuilder_CubeBlock xsi:type="MyObjectBuilder_SolarPanel">'
        "<SubtypeName>LargeBlockSolarPanel</SubtypeName>"
        f"<EntityId>{110000000000000000 + j * columns + i}</EntityId>"
        + vec("Min", (4 * i, 2 * j, 0)).replace(".0000000", "")
        + '<BlockOrientation Forward="Forward" Up="Up" />'
        "</MyObjectBuilder_CubeBlock>"
        for j in range(rows)
        for i in range(columns)
    )
    return (
        '<MyObjectBuilder_EntityBase xsi:type="MyObjectBuilder_CubeGrid">'
        "<SubtypeName /><EntityId>100000000000000001</EntityId>"
        "<PersistentFlags>CastShadows InScene</PersistentFlags>"
        "<PositionAndOrientation>"
        + vec("Position", (0, 0, 0))
        + vec("Forward", forward)
        + vec("Up", up)
        + "</PositionAndOrientation>"
        f"<GridSizeEnum>Large</GridSizeEnum><CubeBlocks>{blocks}</CubeBlocks>"
        "<IsStatic>true</IsStatic><IsUnsupportedStation>true</IsUnsupportedStation>"
        "<DisplayName>Solar Field</DisplayName>"
        "<DestructibleBlocks>true</DestructibleBlocks>"
        "</MyObjectBuilder_EntityBase>"
    )


def set_value(text, name, value):
    new, count = re.subn(
        rf"<{name}>[^<]*</{name}>|<{name}\s*/>", f"<{name}>{value}</{name}>", text
    )
    assert count, name
    return new


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("template", type=Path)
    ap.add_argument("--out", type=Path, required=True)
    ap.add_argument("--columns", type=int, default=30)
    ap.add_argument("--rows", type=int, default=40)
    ap.add_argument("--angle", type=float, default=60)
    ap.add_argument("--sun-minutes", type=float, default=120)
    args = ap.parse_args()

    world = args.out
    shutil.rmtree(world, ignore_errors=True)
    shutil.copytree(args.template, world)
    for backup in world.glob("*B5"):
        backup.unlink()

    # The grid's forward is the panels' normal: LargeBlockSolarPanel faces its block forward
    normal, axis = panel_normal(args.angle)
    sector = world / "SANDBOX_0_0_0_.sbs"
    text = sector.read_text(encoding="utf-8")
    grid = panels_grid(args.columns, args.rows, normal, axis)
    text = re.sub(r"<SectorObjects\s*/>", "<SectorObjects></SectorObjects>", text)
    text = text.replace("<SectorObjects>", f"<SectorObjects>{grid}", 1)
    sector.write_text(text, encoding="utf-8")

    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        path = world / name
        text = path.read_text(encoding="utf-8")
        for key, value in {
            "EnableSunRotation": "true",
            "SunRotationIntervalMinutes": f"{args.sun_minutes}",
            "AutoSaveInMinutes": "0",
            "ExperimentalMode": "true",
        }.items():
            text = set_value(text, key, value)
        if name == "Sandbox.sbc":
            # The sun starts at its base direction
            text = set_value(text, "ElapsedGameTime", "0")
        path.write_text(text, encoding="utf-8")

    print(
        f"{args.columns * args.rows} panels, normal {tuple(round(x, 3) for x in normal)}, "
        f"{args.angle:g} degrees from the sun at start"
    )


if __name__ == "__main__":
    main()
