# `Shared/Patches/Connector/MyShipConnectorPatch.cs`

*Fixes a connector state-synchronisation bug by replacing `UpdateConnectionState` with a patched version that avoids redundant re-initialization and stale state on the server (file is currently compiled out via `#if UNTESTED`).*

|  |  |
| --- | --- |
| **Module** | [Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) |
| **Source** | [`MyShipConnectorPatch.cs`](../../../../../Shared/Patches/Connector/MyShipConnectorPatch.cs) (66 lines) |
| **Kind** | Nested Harmony patch class (currently disabled — file wrapped in `#if UNTESTED`) |
| **Role** | Correctness/performance patch (contributed by zznty) |

## Purpose

The vanilla `MyShipConnector.UpdateConnectionState` runs every update tick for all connectors, re-evaluating their connection state even when nothing has changed. In worlds with many connected ships this produces unnecessary CPU work.

The fix uses a Prefix to gate the expensive update path: on the first call (`m_isInitOnceBeforeFrameUpdate == true`) it invokes the original method via a `[HarmonyReversePatch]` stub and subscribes a deferred callback to `m_connectionState.ValueChanged` so that future state changes trigger a one-shot `UpdateConnectionState` scheduled for the next post-simulation frame. On subsequent calls without an active connection (`Other == null`) it clears the stale sync state on the server rather than running the full re-evaluation. The method is allowed to proceed to the original implementation only when master-to-slave re-sync is genuinely required.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `ConnectorFix.UpdatePrefix` | Prefix | Intercepts `UpdateConnectionState`; handles init, clears stale state on server, and gates the original call to master-sync cases only. |
| `ConnectorFix.UpdateConnectionState` | Reverse-patch stub | Provides a direct call into the original `UpdateConnectionState` body for use during first-frame initialization. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `MyShipConnector.UpdateConnectionState` | Prefix | Skips redundant per-tick re-evaluation; uses event-driven re-sync and a reverse-patch stub to call the original only when needed. |

## References

- [simulation-and-blocks](../../../../modules/simulation-and-blocks.md)

---

*[Handbook](../../../../TOC.md) · [Module: Simulation & Block Patches](../../../../modules/simulation-and-blocks.md) · [Index](../../../../Index.md)*
