# Authoring guide for documentation sub-agents

You are documenting part of the **Performance Improvements for Space Engineers**
plugin — a paired client/server plugin that applies Harmony patches to the game
to make it run faster. Each fix is individually toggleable via the plugin config.

Read these for context before writing:
- `README.md` — purpose, features, installation.
- `Docs/PerformanceFixes.md` — the **rationale** for every performance fix (the
  symptom, the fix, the Keen support ticket). Use it as the "why" for patch files.

## What you produce

For every source file assigned to you there is already a **scaffold** at
`Docs/files/<source-path>.md` containing a metadata table and `{{PLACEHOLDER}}`
markers. **Fill the placeholders with `Edit` calls.** Do NOT rewrite the
`**Module**` / `**Source**` table rows or the navigation footer — they contain
pre-computed links that must stay byte-for-byte identical.

Replace each placeholder:

| Placeholder | Replace with |
| --- | --- |
| `{{SUMMARY}}` | One italic sentence: what this file is. e.g. `*Caches conveyor reachability per logical grid group.*` |
| `{{KIND}}` | C# shape, e.g. `Static Harmony patch class`, `Static utility class`, `Interface`, `Struct`, `Sealed class : MyGameLogicComponent`. |
| `{{ROLE}}` | Short role tag, e.g. `Performance patch`, `Cache primitive`, `Plugin entry point`, `Configuration`, `UI element`, `Unit test`. |
| `{{PURPOSE}}` | 1–3 short paragraphs: what it does and **why it helps performance**. For patches, name the symptom and the mechanism. Reference the matching section of `Docs/PerformanceFixes.md` when one exists. |
| `{{MEMBERS}}` | A Markdown table `\| Member \| Kind \| Description \|` of the notable methods/fields/properties. Skip trivial boilerplate. For very small files a 1–2 row table or a single sentence is fine. |
| `{{PATCH_SECTION}}` | (patch files) A section: a `## Patch targets` heading then a table `\| Target \| Patch \| Effect \|` where Patch is `Prefix` / `Postfix` / `Transpiler` and Target is the patched `MyType.Method`. |
| `{{PATCH_SECTION_OPTIONAL}}` | (non-patch files) Replace with an **empty string** (delete the placeholder line entirely). |
| `{{REFERENCES}}` | A short bullet list of cross-references using wiki-links (see below). If there is nothing meaningful to link, write `- None.` |

## Cross-references — use wiki-links, never hand-written paths

Do **not** compute relative paths. Write cross-references as wiki-links; a script
resolves them to correct links later:

- `[[Cache]]` → the doc for the source file `Cache.cs`.
- `[[Cache|the generic cache]]` → same target, custom link text.
- `[[module:conveyor]]` → the module page for the `conveyor` module.
- For an ambiguous basename (e.g. `Plugin.cs` exists twice) qualify it:
  `[[ClientPlugin/Plugin]]` or `[[ServerPlugin/Plugin]]`.

Use wiki-links wherever you mention another type/file in the project (in Purpose,
Members, References). Keep references meaningful — link the things a reader would
actually want to jump to (shared tools used, the patch infrastructure, sibling
patches in the same subsystem).

## Module pages

For every module assigned to you, also fill its scaffold at
`Docs/modules/<module>.md`:
- `{{MODULE_OVERVIEW}}` — 1–2 paragraphs introducing the subsystem and how its
  files relate. For patch modules, summarize the performance problem(s) it solves.
- The `{{ONE_LINER}}` cells in the **Files** table — one concise clause per file.
- `{{MODULE_DETAIL}}` — "How it fits together": the data/control flow between the
  files, shared state, ordering constraints, and links (wiki-links) to other
  modules it interacts with.

## Style

- Be precise and grounded in the actual code — never invent behavior.
- Concise and technical; assume the reader is a Space Engineers plugin developer.
- Match the terminology already used in `Docs/PerformanceFixes.md` and the source.
- Keep each file doc focused; deep cross-cutting narrative belongs in the module page.
