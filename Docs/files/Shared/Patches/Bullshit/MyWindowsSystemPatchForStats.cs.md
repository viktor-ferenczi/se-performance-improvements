# `Shared/Patches/Bullshit/MyWindowsSystemPatchForStats.cs`

*Serves `MyWindowsSystem.ProcessPrivateMemory` from a cache refreshed once per second, so the per-frame statistics update stops reading procfs on Linux.*

|  |  |
| --- | --- |
| **Module** | [Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) |
| **Source** | [`MyWindowsSystemPatchForStats.cs`](../../../../../Shared/Patches/Bullshit/MyWindowsSystemPatchForStats.cs) (65 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

`MyGeneralStats.Update` runs once per frame and reads the process' private memory size through `MyVRage.Platform.System.ProcessPrivateMemory`, which only feeds the statistics log lines and the replication statistics. On Windows that is one `GetProcessMemoryInfo` call; on Linux the compatibility layer answers it with `Process.PrivateMemorySize64`, which creates a `Process` object and parses `/proc/<pid>/stat` and `status` on every call, about half a millisecond per frame on the main thread. See the *Process memory statistics read every frame* section of `Docs/PerformanceFixes.md`.

A Prefix on the getter returns the cached value while it is younger than a second of simulation ticks; otherwise the original runs and the Postfix stores its result. The `EnsureCode` hash lists both the shipped body and the body linux-compat's preloader substitutes on Linux. Gated by `FixMemoryStats`, which is on by default on both the client and the server.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `cached` / `refreshAt` | Static fields | The last value and the tick from which the next read is allowed. |
| `ProcessPrivateMemoryGetterPrefix` | Prefix | Serves the cached value inside the refresh window. |
| `ProcessPrivateMemoryGetterPostfix` | Postfix | Stores a fresh value and schedules the next refresh. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyWindowsSystem.ProcessPrivateMemory` (getter) | Prefix | Returns the cached size for up to a second. |
| `MyWindowsSystem.ProcessPrivateMemory` (getter) | Postfix | Caches a freshly read size. |

## References

- [`MyWindowsSystemPatch.cs`](../Physics/MyWindowsSystemPatch.cs.md) — the other patch on the platform system class (Havok thread count)
- [keen-overhead-removal](../../../../modules/keen-overhead-removal.md) — module overview

---

*[Handbook](../../../../TOC.md) · [Module: Keen Overhead Removal](../../../../modules/keen-overhead-removal.md) · [Index](../../../../Index.md)*
