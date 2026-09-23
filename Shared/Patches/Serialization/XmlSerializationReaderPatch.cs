using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using HarmonyLib;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;

namespace Shared.Patches
{
    // Makes XML deserialization of worlds, blueprints and definitions several times faster by
    // not rebuilding the generated readers' name ID tables for every polymorphic element.
    //
    // The game's XML serializers are the pregenerated XmlSerializationReader1 classes in the
    // *.XmlSerializers assemblies. Polymorphic elements (every cube block, inventory, entity
    // component, definition) go through MyXmlSerializerBase.Deserialize, which calls
    // XmlSerializer.Deserialize for that one element. Each such call creates a new reader, and
    // the reader's InitIDs puts every element and attribute name the whole assembly knows
    // into the XmlReader's name table: 4941 NameTable.Add calls for VRage.Game, 2065 for
    // SpaceEngineers.ObjectBuilders. Loading a world from XML spends most of its parse time
    // there (measured 8.3 s of 11.3 s and 10.6 s of 11.5 s on two large worlds).
    //
    // InitIDs only assigns the reader's string fields from NameTable.Add, and Add returns the
    // same atomized string for the same name as long as it is the same name table. The nested
    // readers of one document share the document's name table, so after the first InitIDs
    // on a name table the result is known. It is captured into an array and later readers on
    // the same name table get their fields copied from it, with the same values InitIDs would
    // have produced.
    //
    // The patch reroutes the InitIDs call in the runtime's XmlSerializationReader.Init instead
    // of patching the generated InitIDs methods: those are over 100 KB of IL each and Harmony
    // needs about two seconds of every game start to rewrite them.
    [HarmonyPatchCategory(PatchHelpers.EarlyCategory)]
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class XmlSerializationReaderPatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;
        private static bool subscribed;

        // The generated readers with large ID tables. The other ones have under 150 IDs each.
        private static readonly HashSet<string> SerializerAssemblies = new HashSet<string>
        {
            "VRage.Game.XmlSerializers",
            "SpaceEngineers.ObjectBuilders.XmlSerializers",
            "Sandbox.Game.XmlSerializers",
        };

        private const string ReaderTypeName =
            "Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationReader1";

        // Null for the reader types left alone
        private static readonly ConcurrentDictionary<Type, IdTable> Tables =
            new ConcurrentDictionary<Type, IdTable>();

        private static readonly MethodInfo InitIDsMethod = AccessTools.DeclaredMethod(
            typeof(XmlSerializationReader),
            "InitIDs"
        );

        private static readonly Action<XmlSerializationReader> CallInitIDs =
            AccessTools.MethodDelegate<Action<XmlSerializationReader>>(InitIDsMethod);

        // XmlSerializationReader.Reader is protected
        private static readonly Func<XmlSerializationReader, XmlReader> GetReader =
            AccessTools.MethodDelegate<Func<XmlSerializationReader, XmlReader>>(
                AccessTools.PropertyGetter(typeof(XmlSerializationReader), "Reader")
            );

        public static void Configure()
        {
            var config = Config;
            if (config == null)
                return;

            if (!subscribed)
            {
                config.PropertyChanged += OnConfigChanged;
                subscribed = true;
            }

            enabled = config.Enabled && config.FixXmlDeserialization;
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            Configure();
        }

        // Internal, with one parameter more on .NET Framework than on .NET
        private static MethodBase TargetMethod()
        {
            return AccessTools
                .GetDeclaredMethods(typeof(XmlSerializationReader))
                .Single(m => m.Name == "Init");
        }

        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var il = instructions.ToList();
            var calls = il.FindAll(ci => ci.Calls(InitIDsMethod));
            if (calls.Count != 1)
            {
                Log.Error(
                    $"XmlSerializationReader.Init: expected one InitIDs call, found {calls.Count}, leaving it unpatched"
                );
                return il;
            }

            calls[0].opcode = OpCodes.Call;
            calls[0].operand = AccessTools.DeclaredMethod(
                typeof(XmlSerializationReaderPatch),
                nameof(InitIDs)
            );
            return il;
        }

        private static void InitIDs(XmlSerializationReader reader)
        {
            var table = enabled ? Tables.GetOrAdd(reader.GetType(), CreateTable) : null;
            if (table == null)
            {
                CallInitIDs(reader);
                return;
            }

            var nameTable = GetReader(reader).NameTable;
            if (table.TryApply(reader, nameTable))
                return;

            CallInitIDs(reader);
            table.Capture(reader, nameTable);
        }

        private static IdTable CreateTable(Type readerType)
        {
            if (
                readerType.FullName != ReaderTypeName
                || !SerializerAssemblies.Contains(readerType.Assembly.GetName().Name)
            )
                return null;

            return new IdTable(readerType);
        }

        // The ID fields of one generated reader class and their values per name table
        private class IdTable
        {
            private readonly ConditionalWeakTable<XmlNameTable, string[]> idsByNameTable =
                new ConditionalWeakTable<XmlNameTable, string[]>();
            private readonly Func<object, string[]> read;
            private readonly Action<object, string[]> write;

            public IdTable(Type readerType)
            {
                // Every string field of a generated reader is a name ID assigned by InitIDs
                var fields = readerType
                    .GetFields(
                        BindingFlags.Instance
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.DeclaredOnly
                    )
                    .Where(f => f.FieldType == typeof(string))
                    .ToArray();

                read = EmitRead(readerType, fields);
                write = EmitWrite(readerType, fields);
            }

            public bool TryApply(object reader, XmlNameTable nameTable)
            {
                if (!idsByNameTable.TryGetValue(nameTable, out var ids))
                    return false;

                write(reader, ids);
                return true;
            }

            public void Capture(object reader, XmlNameTable nameTable)
            {
                var ids = read(reader);
                idsByNameTable.GetValue(nameTable, _ => ids);
            }

            // Thousands of fields, so the copies are emitted instead of going through FieldInfo
            private static Func<object, string[]> EmitRead(Type readerType, FieldInfo[] fields)
            {
                var method = new DynamicMethod(
                    "ReadIds",
                    typeof(string[]),
                    new[] { typeof(object) },
                    readerType,
                    true
                );
                var il = method.GetILGenerator();
                var reader = il.DeclareLocal(readerType);
                var ids = il.DeclareLocal(typeof(string[]));
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, readerType);
                il.Emit(OpCodes.Stloc, reader);
                il.Emit(OpCodes.Ldc_I4, fields.Length);
                il.Emit(OpCodes.Newarr, typeof(string));
                il.Emit(OpCodes.Stloc, ids);
                for (var i = 0; i < fields.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, ids);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldloc, reader);
                    il.Emit(OpCodes.Ldfld, fields[i]);
                    il.Emit(OpCodes.Stelem_Ref);
                }
                il.Emit(OpCodes.Ldloc, ids);
                il.Emit(OpCodes.Ret);
                return (Func<object, string[]>)
                    method.CreateDelegate(typeof(Func<object, string[]>));
            }

            private static Action<object, string[]> EmitWrite(Type readerType, FieldInfo[] fields)
            {
                var method = new DynamicMethod(
                    "WriteIds",
                    typeof(void),
                    new[] { typeof(object), typeof(string[]) },
                    readerType,
                    true
                );
                var il = method.GetILGenerator();
                var reader = il.DeclareLocal(readerType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, readerType);
                il.Emit(OpCodes.Stloc, reader);
                for (var i = 0; i < fields.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, reader);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.Emit(OpCodes.Stfld, fields[i]);
                }
                il.Emit(OpCodes.Ret);
                return (Action<object, string[]>)
                    method.CreateDelegate(typeof(Action<object, string[]>));
            }
        }
    }
}
