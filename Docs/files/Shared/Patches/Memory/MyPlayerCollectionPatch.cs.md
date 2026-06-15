# `Shared/Patches/Memory/MyPlayerCollectionPatch.cs`

*Rate-limits `MyPlayerCollection.SendDirtyBlockLimits` to once every 180 ticks (~3 seconds) to reduce network and CPU overhead from too-frequent block-limit syncs.*

|  |  |
| --- | --- |
| **Module** | [Memory Allocation Patches](../../../../modules/memory-allocation.md) |
| **Source** | [`MyPlayerCollectionPatch.cs`](../../../../../Shared/Patches/Memory/MyPlayerCollectionPatch.cs) (26 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyPlayerCollection.SendDirtyBlockLimits` is called too frequently, generating unnecessary network traffic and CPU work to sync block-count limits to clients on every tick. This Prefix simply skips the call unless the current plugin tick is divisible by 180, reducing the sync rate to approximately once every 3 seconds. The fix is gated on `Config.FixBlockLimit`. No state or cache is required — the suppression is entirely tick-modulo based.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SendDirtyBlockLimitsPrefix()` | Harmony Prefix | Returns `false` (skip original) for 179 out of every 180 ticks when the fix is enabled. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyPlayerCollection.SendDirtyBlockLimits` | Prefix | Suppresses the call for all but 1 in 180 ticks, reducing block-limit sync frequency. |

## References

- [memory-allocation](../../../../modules/memory-allocation.md)

---

*[Handbook](../../../../TOC.md) · [Module: Memory Allocation Patches](../../../../modules/memory-allocation.md) · [Index](../../../../Index.md)*
