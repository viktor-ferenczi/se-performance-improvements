# `Shared/Patches/Physics/PhysicsFixes.cs`

*Shared utility class exposing a helper to reconfigure `MyClusterTree` cluster-size parameters at runtime.*

|  |  |
| --- | --- |
| **Module** | [Physics Patches](../../../../modules/physics.md) |
| **Source** | [`PhysicsFixes.cs`](../../../../../Shared/Patches/Physics/PhysicsFixes.cs) (17 lines) |
| **Kind** | Static utility class |
| **Role** | Physics helper |

## Purpose

Provides `SetClusterSize`, a single method that updates all four interdependent `MyClusterTree` static fields (`IdealClusterSize`, `IdealClusterSizeHalfSqr`, `MinimumDistanceFromBorder`, `MaximumForSplit`, `MaximumClusterSize`) in one consistent call. Keeping this logic in one place ensures the derived values always stay in sync whenever the cluster size is changed — for example from plugin configuration or external tooling.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `SetClusterSize(float clusterSize)` | Static method | Writes all five cluster-size statics on `MyClusterTree` from a single scalar input. |

## References

- [physics](../../../../modules/physics.md)
- [`MyClusterTreePatch.cs`](MyClusterTreePatch.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: Physics Patches](../../../../modules/physics.md) · [Index](../../../../Index.md)*
