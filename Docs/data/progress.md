# Documentation pipeline progress

Regenerate working data: `python3 Docs/data/scripts/build_manifest.py && python3 Docs/data/scripts/generate_scaffolds.py`
Resolve cross-references: `python3 Docs/data/scripts/resolve_wikilinks.py`
Rebuild index: `python3 Docs/data/scripts/build_index.py`
Verify links: `python3 Docs/data/scripts/check_links.py`

## Status — COMPLETE

- [x] Manifest built (`data/manifest.jsonl`, 87 files, 16 modules)
- [x] Module metadata (`data/modules.json`)
- [x] Scaffolds generated (`Docs/files/**`, `Docs/modules/**`)
- [x] Authoring guide (`data/AUTHORING.md`)
- [x] Sub-agent authoring of per-file + module docs (all 16 modules, 87 files)
- [x] Resolve wiki-links (634 resolved)
- [x] Build `Index.md` and `TOC.md`
- [x] Verify links (1330 relative links, all OK)
- [x] Link Docs from `README.md`, `PerformanceFixes.md`, `AGENTS.md`

## Update history

- Initial generation: 87 files, 16 modules (commit `ca1c1ee`).
- Synced with code commit `ffc18d6` ("Fixes"): removed `WineDetector` doc;
  relocated `MyWheelPatch` / `MyLcdSurfaceComponentPatch` docs to
  `Docs/files/ServerPlugin/Patches/`; refreshed `Config.cs` (C# `field` keyword,
  `FixWheelTrail`/`FixTextPanel` client stubs), `PluginLogger.cs` and
  `PreloaderHelpers.cs`; dropped stale "Torch" framing now that the branch is
  Magnetar-only. Now 86 files, 16 modules.

## Incremental re-run

On a code change, re-run `build_manifest.py` then `generate_scaffolds.py`: only files
whose SHA256 changed need re-documentation. `generate_scaffolds.py` never overwrites an
existing doc, so delete the `.md` of a changed file under `Docs/files/` to regenerate its
scaffold, re-document it, then run `resolve_wikilinks.py`, `build_index.py`, `check_links.py`.

## Layout

- `Docs/TOC.md` — handbook landing page (architecture + module groups)
- `Docs/Index.md` — flat file index (generated)
- `Docs/PerformanceFixes.md` — per-fix rationale (pre-existing, integrated)
- `Docs/modules/<module>.md` — 16 module pages
- `Docs/files/<source-path>.md` — 87 per-file pages (mirror the source tree)
- `Docs/data/` — manifest, module map, link map, authoring guide, generator scripts
