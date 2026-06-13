# `Shared/Tools/GameAssembliesToPublicize.cs`

*Assembly-level `[IgnoresAccessChecksTo]` declarations listing every game assembly publicized by Krafs.Publicizer.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`GameAssembliesToPublicize.cs`](../../../../Shared/Tools/GameAssembliesToPublicize.cs) (8 lines) |
| **Kind** | Assembly-attribute file (no types, only `[assembly: …]` attributes) |
| **Role** | Publicizer support |

## Purpose

This file is the single source of truth for which game assemblies the plugin accesses via the Krafs.Publicizer workaround. It declares `[assembly: IgnoresAccessChecksTo("…")]` for each assembly so the .NET runtime accepts direct access to their internal and private members at runtime, matching the `<Publicize>` items in `ServerPlugin.csproj` and `ClientPlugin.csproj`.

Currently publicized: `Sandbox.Game`, `SpaceEngineers.Game`, `VRage.Math`, and `VRage.Scripting`. These let the plugin access internal fields such as `MySession.m_updateAllowed` and `MyScriptCompiler.m_conditionalCompilationSymbols` without runtime reflection, which is faster and more refactoring-friendly.

**Maintenance rule:** whenever a new `<Publicize>` entry is added to either `.csproj`, the matching `[assembly: IgnoresAccessChecksTo]` line must be added here as well, and vice-versa. Both must stay in sync — the matching attributes live in [`IgnoresAccessChecksToAttribute.cs`](IgnoresAccessChecksToAttribute.cs.md).

## Key members

- No types; the file contains only four `[assembly: IgnoresAccessChecksTo("…")]` attributes.

## References

- [`IgnoresAccessChecksToAttribute.cs`](IgnoresAccessChecksToAttribute.cs.md) — defines the `IgnoresAccessChecksToAttribute` type injected at runtime when not built in DEV mode.
- [`MySessionExtensions.cs`](MySessionExtensions.cs.md) — example consumer: accesses `MySession.m_updateAllowed` made available by `Sandbox.Game` publicization.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
