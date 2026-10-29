using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Shared.Patches;

// Makes the programmable block instruction counter per thread (FixScriptCounter).
//
// VRage.Library.Compiler.IlInjector keeps the whole counter state of a running script in plain
// static fields: instructions, call depth, stack limit, time baseline, memory, the dead flag and
// one shared counter handle. The game injects calls to it into every block of a PB script, and
// the MemorySafe collections of VRage.Scripting call it on every allocation. Two scripts running
// at the same time on two threads would reset and corrupt each other's counters.
//
// The prepatch marks those fields [ThreadStatic] and replaces the shared handle with one created
// lazily per thread. Script IL does not change, so compiled script caches stay valid, and on the
// main thread the counter behaves exactly as before. m_maxAllowedTicks loses its readonly flag,
// so ScriptRunner can set the per-run time limit (ScriptRunLimitMs).
//
// It runs from the Preloader, before the game assembly loads, and only when the option is on in
// the config file (see PreloadConfig). It does nothing if any field it expects is missing.
public static class IlInjectorPrepatch
{
    private const string TypeName = "VRage.Library.Compiler.IlInjector";
    private const string HandleField = "m_instructionCounterHandle";
    private const string MaxTicksField = "m_maxAllowedTicks";

    private static readonly string[] ThreadStaticFields =
    {
        "m_minAllowedStackPointer",
        "m_isDead",
        "m_numMaxInstructions",
        "m_numMethodCalls",
        "m_maxMethodCalls",
        "m_baselineTimestamp",
        "m_isTimeLimitCheckEnabled",
        "m_ticksSinceStart",
        "m_numInstructions",
        "m_callChainDepth",
        "m_memoryUsed",
        "m_instructionCheckScale",
        "m_methodCheckScale",
    };

    // What happened, for the plugin to log once logging is up
    public static string Result { get; private set; } = "not run";

    public static void Prepatch(AssemblyDefinition assembly)
    {
        var module = assembly.MainModule;
        var type = module.GetType(TypeName);
        var handleType = type?.NestedTypes.FirstOrDefault(t =>
            t.Name == "InstructionCounterHandle"
        );
        var handle = type?.Fields.FirstOrDefault(f => f.Name == HandleField && f.IsStatic);
        var maxTicks = type?.Fields.FirstOrDefault(f => f.Name == MaxTicksField && f.IsStatic);
        var fields = ThreadStaticFields
            .Select(n => type?.Fields.FirstOrDefault(f => f.Name == n && f.IsStatic))
            .ToList();
        var handleCtor = handleType?.Methods.FirstOrDefault(m =>
            m.IsConstructor && !m.IsStatic && !m.HasParameters
        );
        if (
            handleType == null
            || handle == null
            || maxTicks == null
            || handleCtor == null
            || fields.Any(f => f == null)
        )
        {
            Result = "skipped: IlInjector does not have the expected fields (game update?)";
            return;
        }

        var threadStatic = new CustomAttribute(
            module.ImportReference(typeof(ThreadStaticAttribute).GetConstructor(Type.EmptyTypes))
        );
        foreach (var field in fields.Append(handle))
        {
            field.CustomAttributes.Add(threadStatic);
        }

        handle.Attributes &= ~FieldAttributes.InitOnly;
        maxTicks.Attributes &= ~FieldAttributes.InitOnly;

        // The static constructor sets the handle only on the thread that loads the type, so every
        // read of it goes through a getter that creates the thread's own handle on first use
        var getter = new MethodDefinition(
            "GetCounterHandle",
            MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig,
            handleType
        );
        var il = getter.Body.GetILProcessor();
        var ready = il.Create(OpCodes.Ldsfld, handle);
        il.Emit(OpCodes.Ldsfld, handle);
        il.Emit(OpCodes.Brtrue_S, ready);
        il.Emit(OpCodes.Newobj, handleCtor);
        il.Emit(OpCodes.Stsfld, handle);
        il.Append(ready);
        il.Emit(OpCodes.Ret);
        type.Methods.Add(getter);

        var replaced = 0;
        foreach (
            var method in type.Methods.Where(m => m.HasBody && m != getter && !m.IsConstructor)
        )
        {
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldsfld && instruction.Operand == handle)
                {
                    instruction.OpCode = OpCodes.Call;
                    instruction.Operand = getter;
                    replaced++;
                }
            }
        }

        Result =
            $"applied: {fields.Count + 1} fields per thread, {replaced} handle reads redirected";
    }
}
