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

- Synced with the `auto-research` branch: new `MyToolbarItemTerminalGroupPatch` page (simulation-and-blocks), `MyAssemblerPatch` rewritten as the active master assembler cache, `Generation` and the `FlagForRecomputation` hook on the conveyor cache core, `FixToolbar` on the config pages. Later in the same branch: the image loading
pages (`ImageLoader`, `ImageSharpRuntime`, `MyImagePatch`), the asteroid voxel
preload pages (`MySandboxGamePatchForVoxelPreload`,
`MyStorageBasePatchForVoxelPreload`), `MyGridTargetingPatch`,
`MyWindowsSystemPatchForStats`, and `MyPhysicsPatch` replaced by
`MyWindowsSystemPatch` (configurable Havok thread count). Now 101 files,
16 modules.

- Initial generation: 87 files, 16 modules (commit `ca1c1ee`).
- Synced with code commit `ffc18d6` ("Fixes"): removed `WineDetector` doc;
  relocated `MyWheelPatch` / `MyLcdSurfaceComponentPatch` docs to
  `Docs/files/ServerPlugin/Patches/`; refreshed `Config.cs` (C# `field` keyword,
  `FixWheelTrail`/`FixTextPanel` client stubs), `PluginLogger.cs` and
  `PreloaderHelpers.cs`; dropped stale "Torch" framing now that the branch is
  Magnetar-only. Now 86 files, 16 modules.
- Synced with code commits `a588665` ("Fixed patch timing") and `773795e`
  ("Updated hashes"): documented the two-phase dedicated-server patching split.
  Added new `Preloader.cs` doc (early Harmony bootstrap installed before world
  load); rewrote `PatchHelpers.cs` (three entry points —
  `HarmonyPatchAll` / `HarmonyPatchUncategorized` / `HarmonyPatchCategory` — on a
  shared `VerifyAndApply` scaffold, with before/after `GetPatchedMethods()`
  applied-patch logging), `ServerPlugin/Plugin.cs` (early `EarlyStartup`
  bootstrap + `Common.AttachPlugin` from `Init` + deferred `"Late"` category),
  `Common.cs` (`SetPlugin` vs `AttachPlugin` seam), `EnsureCode.cs` (phase-scoped
  `VerifyUncategorized` / `VerifyCategory`), and `MyP2PQoSAdapterPatch.cs`
  (`[HarmonyPatchCategory("Late")]` deferral). Refreshed the
  `patch-infrastructure`, `server-plugin`, `shared-plugin-core` and
  `keen-overhead-removal` module pages. Regenerated `manifest.jsonl` (LF-based),
  `linkmap.json` and `Index.md`. Back to 87 files, 16 modules.

- Backfilled the five pages the manifest listed without a doc, which made
  `build_index.py` fail with `FileNotFoundError` and forced hand edits of
  `Index.md`: `Shared/Stats/Statistics.cs`, `Shared/Stats/StatisticsSnapshot.cs`
  and `ServerPlugin/Stats/PerformanceStats.cs` (runtime statistics pipeline,
  added by the Telemetry change `ad4dd79`) and `Shared/Tools/ModRewriterVersions.cs`
  / `Shared/Tools/LegacyModRewriters.cs` (compilation cache keys, `748f70c` and
  `ab8cdab`). Added a `Shared/Stats/` rule to `build_manifest.py` assigning the
  statistics driver to `shared-plugin-core` (it was `unclassified`); refreshed the
  `shared-plugin-core`, `server-plugin` and `tools` module pages, `modules.json`
  and the TOC. Now 98 files, 16 modules, `Index.md` generated again.

- Consistency check after `d221973`: regenerated `manifest.jsonl` (it predated the
  settings wrapping change `1f3e985` and the thread slider changes), refreshed the
  line counts of ten pages and `Index.md` (the `XmlSerializationReaderPatch` row was
  hand-inserted), and corrected the pages still calling `Game` the server default of
  the Havok thread count and the memory statistics and target group fixes off on the
  server (reverted in `9bbbc24`). Now 102 files, 16 modules.

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
- `Docs/files/<source-path>.md` — 102 per-file pages (mirror the source tree)
- `Docs/data/` — manifest, module map, link map, authoring guide, generator scripts
