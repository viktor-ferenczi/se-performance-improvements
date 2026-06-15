# `Shared/Tools/IgnoresAccessChecksToAttribute.cs`

*Provides the `IgnoresAccessChecksToAttribute` class required at runtime when the plugin is loaded by Pulsar/Magnetar rather than built directly in an IDE.*

|  |  |
| --- | --- |
| **Module** | [Shared Tools & Data Structures](../../../modules/tools.md) |
| **Source** | [`IgnoresAccessChecksToAttribute.cs`](../../../../Shared/Tools/IgnoresAccessChecksToAttribute.cs) (20 lines) |
| **Kind** | Sealed attribute class `IgnoresAccessChecksToAttribute : Attribute` (conditionally compiled) |
| **Role** | Publicizer support |

## Purpose

When the plugin is compiled in a developer IDE, Krafs.Publicizer injects `IgnoresAccessChecksToAttribute` into the output assembly automatically. However, when Pulsar or Magnetar builds the plugin from source the Krafs tooling is not available, so the attribute would be missing at runtime and the CLR would reject the assembly.

This file supplies the attribute definition under `#if !DEV_BUILD`, ensuring it is included in production builds only. The attribute tells the .NET runtime to skip access checks for the named assembly, enabling the direct field/method accesses that replace runtime reflection.

## Key members

| Member | Kind | Description |
| --- | --- | --- |
| `IgnoresAccessChecksToAttribute(assemblyName)` | Constructor | Records the target assembly name. |
| `AssemblyName` | Property | The assembly whose access checks are bypassed. |

## References

- [`GameAssembliesToPublicize.cs`](GameAssembliesToPublicize.cs.md) — lists the assemblies decorated with this attribute at the assembly level.
- [`MySessionExtensions.cs`](MySessionExtensions.cs.md) — example of code that relies on publicized access enabled by this attribute.

---

*[Handbook](../../../TOC.md) · [Module: Shared Tools & Data Structures](../../../modules/tools.md) · [Index](../../../Index.md)*
