# `Shared/Patches/Serialization/XmlSerializationReaderPatch.cs`

*Reuses the name IDs of the game's generated XML readers per name table instead of rebuilding them for every polymorphic element.*

|  |  |
| --- | --- |
| **Module** | [World Loading Patches](../../../../modules/world-loading.md) |
| **Source** | [`XmlSerializationReaderPatch.cs`](../../../../../Shared/Patches/Serialization/XmlSerializationReaderPatch.cs) (259 lines) |
| **Kind** | Static Harmony patch class |
| **Role** | Performance patch |

## Purpose

Every polymorphic element of a world, blueprint or definition file (each cube block, inventory, entity component, definition) is read by its own `XmlSerializer.Deserialize` call through the game's `MyXmlSerializerBase`. Each call creates a new generated `XmlSerializationReader1`, and its `InitIDs` adds every name the assembly knows to the reader's name table: 4941 names for VRage.Game, 2065 for SpaceEngineers.ObjectBuilders, 938 for Sandbox.Game. That is most of the time spent parsing a world from XML. See the "XML deserialization" section of [PerformanceFixes.md](../../../../PerformanceFixes.md).

The patch reroutes the `InitIDs` call in the runtime's `XmlSerializationReader.Init`. The first reader on a name table runs the original `InitIDs` and its string fields are captured into an array. Later readers of the same class on the same name table get their fields copied from that array. The values are the ones `InitIDs` would have produced, because `NameTable.Add` returns the same atomized string for the same name on the same table. The field copies are emitted as `DynamicMethod`s, since each class has thousands of fields.

The target is the small runtime method, not the generated `InitIDs` bodies (over 100 KB of IL each), which Harmony needs about two seconds to rewrite on every game start.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `TargetMethod` | Harmony target | `XmlSerializationReader.Init`, which has one more parameter on .NET Framework than on .NET. |
| `Transpiler` | Harmony Transpiler | Replaces the single virtual `InitIDs` call with a call to `InitIDs(XmlSerializationReader)`; logs an error and leaves the method alone if the call is not found exactly once. |
| `InitIDs` | Static method | Copies the cached IDs when the name table has been seen, otherwise runs the original `InitIDs` and captures the result. Other reader types, and all readers while the option is off, run the original. |
| `IdTable` | Nested class | Per generated reader class: the emitted field readers/writers and a `ConditionalWeakTable` from name table to captured IDs. |

## Patch targets

| Target | Patch | Effect |
| --- | --- | --- |
| `XmlSerializationReader.Init` (System.Xml) | Transpiler | Routes the `InitIDs` call through the per-name-table cache. |

## References

- [world-loading](../../../../modules/world-loading.md)
- [`PatchHelpers.cs`](../PatchHelpers.cs.md)

---

*[Handbook](../../../../TOC.md) · [Module: World Loading Patches](../../../../modules/world-loading.md) · [Index](../../../../Index.md)*
