"""Builds a profiling world from Workshop blueprints whose programmable blocks carry a program.

    python3 build_world.py <base world zip> <blueprint folders...> --out <world dir> [--max N]

Each blueprint folder holds a bp.sbc (Workshop download). Every grid of a blueprint is placed
1.5 km from the others on a square lattice in space, with fresh entity ids (references between
its grids remapped), one owner for every block (the world's first identity) and full batteries,
so the scripts find their blocks accessible and powered. Writes blueprints.tsv into the world
folder: blueprint, grids, blocks, PBs with a program.
"""

import argparse
import random
import re
import shutil
import zipfile
from pathlib import Path

SPACING = 1500.0
ID = re.compile(r"(?<=>)(\d{15,20})(?=<)")


def blueprint_grids(bp: Path):
    """The CubeGrid elements of a bp.sbc as text, with their program count"""
    text = bp.read_text(encoding="utf-8-sig", errors="replace")
    grids = re.findall(r"<CubeGrid>(.*?)</CubeGrid>", text, re.S)
    programs = sum(1 for g in grids for p in re.findall(r"<Program>(.*?)</Program>", g, re.S) if p.strip())
    return grids, programs


def remap(grids: list[str], rng: random.Random) -> list[str]:
    """Gives every entity id of the blueprint a new value, the same one in every reference to it"""
    ids = {m for g in grids for m in re.findall(r"<EntityId>(\d+)</EntityId>", g)}
    new = {}
    for old in ids:
        # Keep the id's object type (top byte), randomize the rest
        top = int(old) >> 56
        new[old] = str((top << 56) | rng.getrandbits(56))
    return [ID.sub(lambda m: new.get(m.group(1), m.group(1)), g) for g in grids]


def place(grid: str, offset: tuple[float, float, float]) -> str:
    def shift(m):
        x, y, z = (float(m.group(i)) + offset[i - 1] for i in (1, 2, 3))
        return f'<Position x="{x}" y="{y}" z="{z}" />'

    head, sep, rest = grid.partition("</PositionAndOrientation>")
    head = re.sub(r'<Position x="([^"]+)" y="([^"]+)" z="([^"]+)" />', shift, head, count=1)
    return head + sep + rest


def prepare(grid: str, owner: str) -> str:
    grid = re.sub(r"<Owner>\d+</Owner>", f"<Owner>{owner}</Owner>", grid)
    grid = re.sub(r"<BuiltBy>\d+</BuiltBy>", f"<BuiltBy>{owner}</BuiltBy>", grid)
    # Blueprints are saved without physics, and a PB on a grid without physics never compiles
    grid = grid.replace("<CreatePhysics>false</CreatePhysics>", "<CreatePhysics>true</CreatePhysics>")
    # Full batteries: CurrentStoredPower takes the block's MaxStoredPower
    grid = re.sub(
        r"<CurrentStoredPower>[^<]*</CurrentStoredPower>(\s*)<MaxStoredPower>([^<]*)</MaxStoredPower>",
        r"<CurrentStoredPower>\2</CurrentStoredPower>\1<MaxStoredPower>\2</MaxStoredPower>",
        grid,
    )
    return grid


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("base")
    ap.add_argument("blueprints", nargs="+", type=Path)
    ap.add_argument("--out", required=True, type=Path)
    ap.add_argument("--max", type=int, default=1000)
    ap.add_argument("--seed", type=int, default=626)
    args = ap.parse_args()

    out = args.out
    if out.exists():
        shutil.rmtree(out)
    tmp = out.parent / (out.name + ".unzip")
    shutil.rmtree(tmp, ignore_errors=True)
    with zipfile.ZipFile(args.base) as archive:
        archive.extractall(tmp)
    (src,) = [p.parent for p in tmp.rglob("Sandbox.sbc")]
    shutil.move(src, out)
    shutil.rmtree(tmp)
    # The binary save would win over the edited XML
    for b5 in out.glob("*B5"):
        b5.unlink()

    sandbox = (out / "Sandbox.sbc").read_text(encoding="utf-8-sig")
    owner = re.search(r"<IdentityId>(\d+)</IdentityId>", sandbox).group(1)

    rng = random.Random(args.seed)
    placed = []
    report = ["blueprint\tgrids\tblocks\tprograms"]
    for folder in args.blueprints:
        bp = folder / "bp.sbc"
        if not bp.exists() or len(report) > args.max:
            continue
        grids, programs = blueprint_grids(bp)
        if programs == 0:
            continue
        i = len(report) - 1
        side = 12
        offset = (5000.0 + SPACING * (i % side), 0.0, 5000.0 + SPACING * (i // side))
        grids = [prepare(place(g, offset), owner) for g in remap(grids, rng)]
        placed.extend(grids)
        blocks = sum(g.count("<MyObjectBuilder_CubeBlock ") for g in grids)
        report.append(f"{folder.name}\t{len(grids)}\t{blocks}\t{programs}")
    print(f"{len(report) - 1} blueprints, {len(placed)} grids")

    sbs = out / "SANDBOX_0_0_0_.sbs"
    text = sbs.read_text(encoding="utf-8-sig")
    entities = "".join(
        f'<MyObjectBuilder_EntityBase xsi:type="MyObjectBuilder_CubeGrid">{g}</MyObjectBuilder_EntityBase>\n'
        for g in placed
    )
    text = text.replace("</SectorObjects>", entities + "</SectorObjects>", 1)
    sbs.write_text(text, encoding="utf-8")

    for name in ("Sandbox.sbc", "Sandbox_config.sbc"):
        path = out / name
        t = path.read_text(encoding="utf-8-sig")
        for key, value in {
            "SessionName": "PbProfile",
            "BlockLimitsEnabled": "NONE",
            "EnableIngameScripts": "true",
            "ExperimentalMode": "true",
            "AutoSaveInMinutes": "0",
            "TrashRemovalEnabled": "false",
        }.items():
            t = re.sub(rf"<{key}>[^<]*</{key}>", f"<{key}>{value}</{key}>", t)
        path.write_text(t, encoding="utf-8")
    (out / "blueprints.tsv").write_text("\n".join(report) + "\n")


if __name__ == "__main__":
    main()
