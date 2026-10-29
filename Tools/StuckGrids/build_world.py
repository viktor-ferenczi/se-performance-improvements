"""Builds the SE-753 repro world: one station in its no-damage safe zone, and a ship from a
blueprint placed above the station, nose down, to be launched into it by the probe.

    python3 build_world.py <server save> <blueprint bp.sbc> --out <world dir>
        [--station "Alpha Station"] [--target 0,14,0] [--gap 30]

The save is an Octahedron server save. Only the station grid and the safe zone of the same name
are kept, with no mods, planets or asteroids, so nothing else costs simulation time. --target is
the station cell the ship is aimed at (0,14,0 is near the top of the Octahedron station's upper
glass pyramid, the lobby side). The ship's longest axis is turned along the station's down
vector, its center placed --gap meters plus half its length above the target.

Writes stuck.env into the world folder: the ship's main grid id and the station id, read by
rig.py.
"""

import argparse
import math
import re
import shutil
from pathlib import Path

ENTITY = re.compile(
    r'<MyObjectBuilder_EntityBase xsi:type="(\w+)">.*?</MyObjectBuilder_EntityBase>',
    re.S,
)
VEC = r'<{0} x="([^"]+)" y="([^"]+)" z="([^"]+)" />'
GRID_SIZE = {"Large": 2.5, "Small": 0.5}


def vec(text, tag):
    return [float(v) for v in re.search(VEC.format(tag), text).groups()]


def add(a, b):
    return [x + y for x, y in zip(a, b)]


def sub(a, b):
    return [x - y for x, y in zip(a, b)]


def mul(a, s):
    return [x * s for x in a]


def dot(a, b):
    return sum(x * y for x, y in zip(a, b))


def cross(a, b):
    return [
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0],
    ]


def norm(a):
    return mul(a, 1 / math.sqrt(dot(a, a)))


def basis(text):
    """Columns right, up, backward of a PositionAndOrientation, as the game builds the matrix"""
    forward, up = norm(vec(text, "Forward")), norm(vec(text, "Up"))
    return [cross(forward, up), up, mul(forward, -1)]


def apply(cols, v):
    return [sum(cols[c][r] * v[c] for c in range(3)) for r in range(3)]


def transpose_apply(cols, v):
    return [dot(cols[c], v) for c in range(3)]


def quaternion(cols):
    """Quaternion of the rotation whose matrix columns are cols (Shepperd's method)"""
    m = [[cols[c][r] for c in range(3)] for r in range(3)]
    trace = m[0][0] + m[1][1] + m[2][2]
    if trace > 0:
        s = math.sqrt(trace + 1) * 2
        return [
            (m[2][1] - m[1][2]) / s,
            (m[0][2] - m[2][0]) / s,
            (m[1][0] - m[0][1]) / s,
            s / 4,
        ]
    i = max(range(3), key=lambda k: m[k][k])
    j, k = (i + 1) % 3, (i + 2) % 3
    s = math.sqrt(1 + m[i][i] - m[j][j] - m[k][k]) * 2
    q = [0.0] * 4
    q[i] = s / 4
    q[j] = (m[j][i] + m[i][j]) / s
    q[k] = (m[k][i] + m[i][k]) / s
    q[3] = (m[k][j] - m[j][k]) / s
    return q


def set_pose(grid, position, cols):
    forward, up = mul(cols[2], -1), cols[1]
    q = quaternion(cols)
    pose = (
        "<PositionAndOrientation>"
        f'<Position x="{position[0]!r}" y="{position[1]!r}" z="{position[2]!r}" />'
        f'<Forward x="{forward[0]!r}" y="{forward[1]!r}" z="{forward[2]!r}" />'
        f'<Up x="{up[0]!r}" y="{up[1]!r}" z="{up[2]!r}" />'
        f"<Orientation><X>{q[0]!r}</X><Y>{q[1]!r}</Y><Z>{q[2]!r}</Z><W>{q[3]!r}</W></Orientation>"
        "</PositionAndOrientation>"
    )
    return re.sub(
        r"<PositionAndOrientation>.*?</PositionAndOrientation>",
        pose,
        grid,
        count=1,
        flags=re.S,
    )


def cells(grid):
    return [[int(v) for v in m] for m in re.findall(VEC.format("Min"), grid)]


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("save", type=Path)
    ap.add_argument("blueprint", type=Path)
    ap.add_argument("--out", type=Path, required=True)
    ap.add_argument("--station", default="Alpha Station")
    ap.add_argument("--target", default="0,14,0")
    ap.add_argument("--gap", type=float, default=30.0)
    args = ap.parse_args()

    shutil.rmtree(args.out, ignore_errors=True)
    args.out.mkdir(parents=True)
    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        text = (args.save / name).read_text(encoding="utf-8-sig")
        text = re.sub(r"<Mods>.*?</Mods>", "<Mods />", text, flags=re.S)
        text = re.sub(
            r"<SessionName>.*?</SessionName>",
            "<SessionName>StuckGrids</SessionName>",
            text,
        )
        (args.out / name).write_text(text, encoding="utf-8")

    sector = (args.save / "SANDBOX_0_0_0_.sbs").read_text(encoding="utf-8-sig")
    display = f"<DisplayName>{args.station}</DisplayName>"
    kept = [
        m.group(0)
        for m in ENTITY.finditer(sector)
        if m.group(1) in ("MyObjectBuilder_CubeGrid", "MyObjectBuilder_SafeZone")
        and display in m.group(0)
    ]
    station = next(e for e in kept if "MyObjectBuilder_CubeGrid" in e[:80])
    assert len(kept) == 2, f"expected the station and its safe zone, found {len(kept)}"
    station_id = re.search(r"<EntityId>(\d+)</EntityId>", station).group(1)
    station_cols = basis(station)
    target_cell = [float(v) for v in args.target.split(",")]
    target = add(
        vec(station, "Position"),
        apply(station_cols, mul(target_cell, GRID_SIZE["Large"])),
    )
    down = mul(station_cols[1], -1)

    bp = args.blueprint.read_text(encoding="utf-8-sig")
    grids = re.findall(r"<CubeGrid>.*?</CubeGrid>", bp, re.S)
    main_grid = max(grids, key=lambda g: g.count("<MyObjectBuilder_CubeBlock"))
    main_id = re.search(r"<EntityId>(\d+)</EntityId>", main_grid).group(1)
    size = GRID_SIZE[
        re.search(r"<GridSizeEnum>(\w+)</GridSizeEnum>", main_grid).group(1)
    ]
    main_cells = cells(main_grid)
    low = [min(c[i] for c in main_cells) for i in range(3)]
    high = [max(c[i] for c in main_cells) for i in range(3)]
    extent = [(high[i] - low[i] + 1) * size for i in range(3)]
    center_local = [(low[i] + high[i]) / 2 * size for i in range(3)]
    main_cols = basis(main_grid)
    old_center = add(vec(main_grid, "Position"), apply(main_cols, center_local))

    # New main grid basis: its longest local axis along the launch direction, the others filled in
    # with the station's forward so the ship comes down square to the station
    axis = max(range(3), key=lambda i: extent[i])
    new_cols = [None, None, None]
    new_cols[axis] = down
    other = (axis + 1) % 3
    new_cols[other] = norm(mul(station_cols[2], -1))
    third = 3 - axis - other
    new_cols[third] = cross(new_cols[(third + 1) % 3], new_cols[(third + 2) % 3])
    new_center = add(target, mul(down, -(extent[axis] / 2 + args.gap)))

    placed = []
    for grid in grids:
        cols = basis(grid)
        # rotation = new_cols * main_cols^T, applied to the grid's axes and its offset from the center
        rotated = [apply(new_cols, transpose_apply(main_cols, c)) for c in cols]
        offset = apply(
            new_cols, transpose_apply(main_cols, sub(vec(grid, "Position"), old_center))
        )
        grid = set_pose(grid, add(new_center, offset), rotated)
        # Blueprints are saved without physics
        grid = grid.replace(
            "<CreatePhysics>false</CreatePhysics>",
            "<CreatePhysics>true</CreatePhysics>",
        )
        # A ship, whatever it was saved as, and dampeners off, so nothing slows it before the hit
        grid = re.sub(r"<IsStatic>\w+</IsStatic>", "<IsStatic>false</IsStatic>", grid)
        grid = re.sub(r"<IsUnsupportedStation>\w+</IsUnsupportedStation>", "", grid)
        grid = re.sub(r"<DampenersEnabled>\w+</DampenersEnabled>", "", grid)
        grid = grid.replace(
            "<IsStatic>false</IsStatic>",
            "<IsStatic>false</IsStatic><DampenersEnabled>false</DampenersEnabled>",
            1,
        )
        grid = re.sub(
            r"<LinearVelocity [^>]*/>", '<LinearVelocity x="0" y="0" z="0" />', grid
        )
        grid = re.sub(
            r"<AngularVelocity [^>]*/>", '<AngularVelocity x="0" y="0" z="0" />', grid
        )
        placed.append(grid.replace("<CubeGrid>", '<MyObjectBuilder_EntityBase xsi:type="MyObjectBuilder_CubeGrid">', 1)
                      .rsplit("</CubeGrid>", 1)[0] + "</MyObjectBuilder_EntityBase>")  # fmt: skip

    sector = re.sub(
        r"<SectorObjects>.*?</SectorObjects>",
        lambda _: "<SectorObjects>" + "".join(kept + placed) + "</SectorObjects>",
        sector,
        count=1,
        flags=re.S,
    )
    (args.out / "SANDBOX_0_0_0_.sbs").write_text(sector, encoding="utf-8")
    (args.out / "stuck.env").write_text(
        f"STUCK_SHIP={main_id}\nSTUCK_STATION={station_id}\n"
    )
    print(
        f"station {station_id}, ship {main_id}: {len(grids)} grids, {extent[axis]:.0f} m long"
    )
    print(f"ship center {new_center}, target {target}")


if __name__ == "__main__":
    main()
