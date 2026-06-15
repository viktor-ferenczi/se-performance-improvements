# `Shared/Patches/Conveyor/MyAssemblerPatch.cs`

*Experimental (disabled) patch that replaces `MyAssembler.GetMasterAssembler` with a route-cache-based lookup, avoiding `Reachable` calls entirely.*

|  |  |
| --- | --- |
| **Module** | [Conveyor System Patches](../../../../modules/conveyor.md) |
| **Source** | [`MyAssemblerPatch.cs`](../../../../../Shared/Patches/Conveyor/MyAssemblerPatch.cs) (78 lines) |
| **Kind** | Static Harmony patch class (conditionally compiled `#if UNTESTED`) |
| **Role** | Experimental performance patch (not active) |

## Purpose

This file contains an experimental optimisation contributed by zznty. The original `MyAssembler.GetMasterAssembler` calls `MyGridConveyorSystem.Reachable` for each candidate assembler. The replacement Prefix fetches the pre-computed `ConveyorEndpointMapping` for the assembler (which is built by the route cache), shuffles the pull-element list, and iterates it to find the first non-slave, non-empty, friendly assembler — without invoking `Reachable` at all.

The patch is currently excluded from compilation with `#if UNTESTED` because it requires `conveyorSystem.GetConveyorEndpointMapping` and `MyGridConveyorSystem.ConveyorEndpointMapping` to be publicised via Krafs Publicizer, which is not yet wired up for all build targets (DS and Torch), and because it has not been tested sufficiently.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `EndpointBlocksCache` | `CacheList<IMyConveyorEndpointBlock>` | Thread-local reusable list used during master-assembler search. |
| `Prefix` | Harmony Prefix on `MyAssembler.GetMasterAssembler` | Replaces the original method using the endpoint mapping instead of `Reachable` calls. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyAssembler.GetMasterAssembler` | Prefix | Fully replaces the method using the conveyor endpoint mapping; skips `Reachable` lookups. (Compiled only under `#if UNTESTED`.) |

## References

- [`MyGridConveyorSystemPatch.cs`](MyGridConveyorSystemPatch.cs.md) — `Reachable` cache this patch aims to bypass
- [conveyor](../../../../modules/conveyor.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Conveyor System Patches](../../../../modules/conveyor.md) · [Index](../../../../Index.md)*
