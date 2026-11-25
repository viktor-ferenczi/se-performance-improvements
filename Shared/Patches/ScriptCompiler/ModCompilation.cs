using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Scripting;
using VRage.Utils;

namespace Shared.Patches;

[HarmonyPatch(typeof(MyScriptManager))]
internal static class ModCompilation
{
    // The loading thread owns the batch and the script manager's registries.
    [ThreadStatic]
    private static List<(Task<Assembly> Compilation, Action Register)> batch;

    [HarmonyPrefix]
    [HarmonyPatch("LoadData")]
    private static void Begin(out List<(Task<Assembly>, Action)> __state)
    {
        __state = batch;
        batch = CompilationWorkers.Enabled ? new() : null;
    }

    [HarmonyFinalizer]
    [HarmonyPatch("LoadData")]
    private static void End(List<(Task<Assembly>, Action)> __state)
    {
        var remaining = batch;
        batch = __state;
        // If discovery or registration failed, outstanding compilations still have to finish
        // before the loader tears down their session. Keep its original exception.
        if (remaining == null)
            return;
        foreach (var job in remaining)
        {
            try
            {
                job.Compilation.GetAwaiter().GetResult();
            }
            catch
            { /* The loader is already unwinding. */
            }
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch("Compile")]
    private static IEnumerable<CodeInstruction> Collect(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var code = instructions.ToList();
        var compile = AccessTools.Method(
            typeof(IVRageScripting),
            nameof(IVRageScripting.CompileAsync)
        );
        var call = code.FindIndex(i => i.Calls(compile));
        var result = AccessTools.PropertyGetter(
            typeof(Task<Assembly>),
            nameof(Task<Assembly>.Result)
        );
        if (call < 0 || !code[call + 1].Calls(result))
            throw new InvalidOperationException("Mod loader compile/result sequence changed");
        var messages = code.Take(call)
            .Last(i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S);
        var sync = generator.DefineLabel();
        code[call + 1].labels.Add(sync);
        code.InsertRange(
            call + 1,
            new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Ldloc, messages.operand),
                CodeInstruction.Call(typeof(ModCompilation), nameof(Defer)),
                new CodeInstruction(OpCodes.Brfalse, sync),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ret),
            }
        );
        return code;
    }

    private static bool Defer(
        Task<Assembly> compilation,
        MyScriptManager manager,
        string name,
        MyModContext context,
        List<Message> diagnostics
    )
    {
        if (batch == null)
            return false;
        // The platform entry point has already captured the script contents (also for ZIP mods).
        manager.m_cachedFiles.Clear();
        batch.Add(
            (
                compilation,
                () =>
                {
                    var assembly = compilation.GetAwaiter().GetResult();
                    if (assembly != null)
                    {
                        manager.AddAssembly(context, MyStringId.GetOrCompute(name), assembly);
                        return;
                    }
                    MyDefinitionErrors.Add(
                        context,
                        "Compilation of " + name + " failed:",
                        TErrorSeverity.Error
                    );
                    foreach (var message in diagnostics)
                        MyDefinitionErrors.Add(
                            context,
                            message.Text,
                            message.IsError ? TErrorSeverity.Error : TErrorSeverity.Warning
                        );
                    manager.m_errors.Clear();
                }
            )
        );
        return true;
    }

    [HarmonyTranspiler]
    [HarmonyPatch("LoadData")]
    private static IEnumerable<CodeInstruction> FinishBeforeRegistration(
        IEnumerable<CodeInstruction> instructions
    )
    {
        var code = instructions.ToList();
        var scripts = AccessTools.Field(typeof(MyScriptManager), nameof(MyScriptManager.Scripts));
        var values = AccessTools.PropertyGetter(typeof(Dictionary<MyStringId, Assembly>), "Values");
        var index = code.FindIndex(i => i.Calls(values));
        if (index < 2 || !code[index - 1].LoadsField(scripts) || !code[index - 2].IsLdarg(0))
            throw new InvalidOperationException("Mod registration barrier changed");
        code.Insert(
            index - 2,
            CodeInstruction
                .Call(typeof(ModCompilation), nameof(Register))
                .MoveLabelsFrom(code[index - 2])
        );
        return code;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MyScriptManager.UnloadData))]
    private static void Unload() => CompilationWorkers.WaitForIdle(cancelPending: true);

    private static void Register()
    {
        if (batch == null)
            return;
        foreach (var job in batch)
            job.Register();
        // The session warmup PB also shares the compiler. Finish it before loader
        // postfixes change the conditional symbols for their own scripts.
        CompilationWorkers.WaitForIdle();
        batch = null; // Loader postfixes may compile more scripts under temporary symbols.
    }
}
