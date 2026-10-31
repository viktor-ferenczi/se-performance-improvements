using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Shared.Tools;

// Pulsar and Magnetar compile plugins non-deterministically into an assembly with a random name
// (DotNetCompat_qej1e43n.ipq), so a rebuild from unchanged source differs from the previous build
// in its name, its MVID, its timestamp and the layout of its string heap, which also moves every
// string offset in the metadata tables. The content ID ignores all of those and covers the rest,
// so it changes when the code of a rewriter does, not when the rewriter is merely rebuilt.
//
// A rewriter build with the same content ID behaves the same, but mods compiled against the old
// build reference its random name (linux-compat adds shim calls to them), so RedirectOldBuilds
// is what makes reusing those compiled mods safe.
internal static class RewriterIdentity
{
    // Path.GetRandomFileName, as appended by the loaders to the plugin's name
    private static readonly Regex RandomSuffix = new Regex(@"_[a-z0-9]{8}\.[a-z0-9]{3}$");

    // Falls back to the MVID when the file cannot be read, costing recompilations only
    public static Guid ContentId(Assembly assembly)
    {
        try
        {
            return ContentId(File.ReadAllBytes(assembly.Location));
        }
        catch (Exception)
        {
            return assembly.ManifestModule.ModuleVersionId;
        }
    }

    public static Guid ContentId(byte[] image)
    {
        using var peReader = new PEReader(new MemoryStream(image));
        var reader = peReader.GetMetadataReader();
        using var content = new MemoryStream();
        using var writer = new BinaryWriter(content);

        WriteCodeAndData(writer, image, peReader.PEHeaders);
        WriteHeap(writer, image, peReader.PEHeaders, reader, HeapIndex.Blob);
        WriteHeap(writer, image, peReader.PEHeaders, reader, HeapIndex.UserString);
        WriteMetadata(writer, reader);
        writer.Flush();

        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(content.GetBuffer(), 0, (int)content.Length);
        return new Guid(hash.Take(16).ToArray());
    }

    // Maps references to the random name of an older build to the loaded build of the same plugin.
    // Assemblies with a name of their own (prebuilt plugins) keep it across builds, they need none.
    public static void RedirectOldBuilds(IEnumerable<Assembly> rewriters)
    {
        var builds = new Dictionary<string, Assembly>();
        foreach (var assembly in rewriters)
        {
            var plugin = PluginNameOf(assembly.GetName().Name);
            if (plugin != null)
                builds[plugin] = assembly;
        }

        if (builds.Count == 0)
            return;

        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
        {
            var plugin = PluginNameOf(new AssemblyName(args.Name).Name);
            return plugin != null && builds.TryGetValue(plugin, out var build) ? build : null;
        };
    }

    // The name without the random suffix, null if it has none
    public static string PluginNameOf(string assemblyName)
    {
        var match = RandomSuffix.Match(assemblyName ?? "");
        return match.Success ? assemblyName.Substring(0, match.Index) : null;
    }

    // Method bodies, field data and resources: the section holding the metadata, without the metadata.
    // Other sections have nothing the code depends on, .rsrc even repeats the assembly name.
    private static void WriteCodeAndData(BinaryWriter writer, byte[] image, PEHeaders headers)
    {
        var metadataStart = headers.MetadataStartOffset;
        var metadataEnd = metadataStart + headers.MetadataSize;
        var section = headers.SectionHeaders.First(s =>
            s.PointerToRawData <= metadataStart
            && metadataEnd <= s.PointerToRawData + s.SizeOfRawData
        );
        writer.Write(image, section.PointerToRawData, metadataStart - section.PointerToRawData);
        writer.Write(
            image,
            metadataEnd,
            section.PointerToRawData + section.SizeOfRawData - metadataEnd
        );
    }

    private static void WriteHeap(
        BinaryWriter writer,
        byte[] image,
        PEHeaders headers,
        MetadataReader reader,
        HeapIndex heap
    )
    {
        var start = headers.MetadataStartOffset + reader.GetHeapMetadataOffset(heap);
        writer.Write(image, start, reader.GetHeapSize(heap));
    }

    // The tables, with their strings in place of the offsets. Leaves out what holds the assembly
    // name: the assembly and module rows, except for the version. Blob heap offsets and tokens
    // are stable, since neither heap nor table order depends on the name.
    private static void WriteMetadata(BinaryWriter writer, MetadataReader reader)
    {
        writer.Write(reader.GetAssemblyDefinition().Version.ToString());

        foreach (TableIndex table in Enum.GetValues(typeof(TableIndex)))
            writer.Write(reader.GetTableRowCount(table));

        foreach (var handle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(handle);
            writer.Write(reader.GetString(type.Namespace));
            writer.Write(reader.GetString(type.Name));
            writer.Write((int)type.Attributes);
            writer.Write(type.BaseType.IsNil ? 0 : MetadataTokens.GetToken(type.BaseType));

            foreach (var implHandle in type.GetInterfaceImplementations())
                writer.Write(
                    MetadataTokens.GetToken(reader.GetInterfaceImplementation(implHandle).Interface)
                );

            foreach (var methodHandle in type.GetMethods())
            {
                var method = reader.GetMethodDefinition(methodHandle);
                writer.Write(reader.GetString(method.Name));
                writer.Write((int)method.Attributes);
                writer.Write((int)method.ImplAttributes);
                writer.Write(MetadataTokens.GetHeapOffset(method.Signature));
                foreach (var parameterHandle in method.GetParameters())
                {
                    var parameter = reader.GetParameter(parameterHandle);
                    writer.Write(reader.GetString(parameter.Name));
                    writer.Write((int)parameter.Attributes);
                }
            }

            foreach (var fieldHandle in type.GetFields())
            {
                var field = reader.GetFieldDefinition(fieldHandle);
                writer.Write(reader.GetString(field.Name));
                writer.Write((int)field.Attributes);
                writer.Write(MetadataTokens.GetHeapOffset(field.Signature));
            }

            foreach (var propertyHandle in type.GetProperties())
                writer.Write(reader.GetString(reader.GetPropertyDefinition(propertyHandle).Name));

            foreach (var eventHandle in type.GetEvents())
                writer.Write(reader.GetString(reader.GetEventDefinition(eventHandle).Name));
        }

        foreach (var handle in reader.TypeReferences)
        {
            var type = reader.GetTypeReference(handle);
            writer.Write(reader.GetString(type.Namespace));
            writer.Write(reader.GetString(type.Name));
            writer.Write(
                type.ResolutionScope.IsNil ? 0 : MetadataTokens.GetToken(type.ResolutionScope)
            );
        }

        foreach (var handle in reader.MemberReferences)
        {
            var member = reader.GetMemberReference(handle);
            writer.Write(reader.GetString(member.Name));
            writer.Write(MetadataTokens.GetToken(member.Parent));
            writer.Write(MetadataTokens.GetHeapOffset(member.Signature));
        }

        foreach (var handle in reader.AssemblyReferences)
        {
            var reference = reader.GetAssemblyReference(handle);
            writer.Write(reader.GetString(reference.Name));
            writer.Write(reference.Version.ToString());
        }

        foreach (var handle in reader.CustomAttributes)
        {
            var attribute = reader.GetCustomAttribute(handle);
            writer.Write(MetadataTokens.GetToken(attribute.Parent));
            writer.Write(MetadataTokens.GetToken(attribute.Constructor));
            writer.Write(MetadataTokens.GetHeapOffset(attribute.Value));
        }
    }
}
