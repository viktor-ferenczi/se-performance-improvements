# `Shared/Tools/Workarounds.cs`

*Shim extension methods that paper over missing or broken game/framework APIs, currently providing a null-safe `GetValueOrDefault` for `Dictionary<MyDefinitionId, MyDefinitionBase>`.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`Workarounds.cs`](../../../../Shared/Tools/Workarounds.cs) (16 lines) |
| **Kind** | Static utility class `Workarounds` |
| **Role** | Misc utility |

## Purpose

`Workarounds` collects small extension methods that exist to compensate for gaps in the game API or the target .NET Framework version that cannot be handled more elegantly elsewhere. The sole current member provides `GetValueOrDefault` for `Dictionary<MyDefinitionId, MyDefinitionBase>`, which is absent in the .NET Framework 4.8 `Dictionary<TKey, TValue>` API (the generic overload was added in .NET Core / .NET 5+). Issue #81 is the tracker for this workaround.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `GetValueOrDefault(this Dictionary<MyDefinitionId, MyDefinitionBase>, MyDefinitionId)` | Extension method | Returns the value for the given key, or `null` if the key is not present. |

## References

- None.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
