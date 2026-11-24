using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Sandbox;
using Sandbox.Engine.Utils;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Localization;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Sandbox.ModAPI.Ingame;
using Shared.Plugin;
using VRage;
using VRage.FileSystem;
using VRage.Game;
using VRage.ModAPI;
using VRage.Scripting;
using VRage.Utils;

namespace Shared.Patches;

[HarmonyPatch(typeof(MyProgrammableBlock))]
internal static class ProgrammableBlockCompilation
{
    private sealed class Request { }

    private static readonly ConditionalWeakTable<MyProgrammableBlock, Request> Pending = new();

    [HarmonyPrefix]
    [HarmonyPatch("Closing")]
    private static void Forget(MyProgrammableBlock __instance) => Pending.Remove(__instance);

    [HarmonyPrefix]
    [HarmonyPatch("OnProgramTermination")]
    private static void Terminated(MyProgrammableBlock __instance) => Pending.Remove(__instance);

    [HarmonyPrefix]
    [HarmonyPatch("Compile")]
    private static bool Compile(
        MyProgrammableBlock __instance,
        string program,
        string storage,
        bool instantiate
    )
    {
        var block = __instance;
        Pending.Remove(block);
        if (!Common.Config.Enabled || !Sync.IsServer)
            return true;

        var session = MySession.Static;
        block.m_scriptComponent.NeedsUpdate = MyEntityUpdateEnum.NONE;
        block.m_scriptComponent.UpdateFrequency = UpdateFrequency.None;
        if (
            !session.EnableIngameScripts
            || block.CubeGrid.IsPreview
            || !block.CubeGrid.CreatePhysics
        )
            return false;

        var request = new Request();
        Pending.Add(block, request);
        // Init and the first physical simulation update both request a deferred instance.
        // The latter has already cleared m_needsInstantiation when it calls Recompile(false).
        bool needsInstance = !string.IsNullOrEmpty(program);
        // A queued compile is the only owner of activation until its result arrives.
        block.m_needsInstantiation = false;
        block.CurrentAssembly = null;
        block.m_instance = null;
        block.m_terminationReason = MyProgrammableBlock.ScriptTerminationReason.None;
        var game = MySandboxGame.Static;
        var file = Path.Combine(MyFileSystem.UserDataPath, block.GetAssemblyName());
        var name = "PB: " + block.DisplayName + " (" + block.EntityId + ")";
        try
        {
            var compilation = MyVRage.Platform.Scripting.CompileIngameScriptAsync(
                file,
                program,
                out var diagnostics,
                name,
                "Program",
                "MyGridProgram",
                MyFakes.ENABLE_PROGRAMMABLE_BLOCK_MEMORY_LIMIT
            );
            _ = compilation.ContinueWith(
                finished =>
                {
                    var error = finished.Exception?.GetBaseException();
                    game.Invoke(
                        () =>
                        {
                            if (!Pending.TryGetValue(block, out var current) || current != request)
                                return;
                            Pending.Remove(block);
                            if (
                                block.Closed
                                || block.MarkedForClose
                                || MySession.Static != session
                                || !session.EnableIngameScripts
                                || block.CubeGrid.IsPreview
                                || !block.CubeGrid.CreatePhysics
                            )
                                return;
                            try
                            {
                                if (error != null)
                                    throw error;
                                block.CurrentAssembly = finished.GetAwaiter().GetResult();
                                block.m_compilerErrors.Clear();
                                block.m_compilerErrors.AddRange(diagnostics.Select(m => m.Text));
                                if (block.CurrentAssembly == null)
                                    block.SetDetailedInfo(
                                        string.Join(Environment.NewLine, block.m_compilerErrors)
                                    );
                                else if (instantiate)
                                    block.CreateInstance(
                                        block.CurrentAssembly,
                                        block.m_compilerErrors,
                                        storage
                                    );
                                else if (needsInstance)
                                {
                                    block.m_needsInstantiation = true;
                                    block.NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
                                }
                            }
                            catch (Exception failure)
                            {
                                ShowFailure(block, failure);
                            }
                        },
                        nameof(ProgrammableBlockCompilation)
                    );
                },
                System.Threading.Tasks.TaskScheduler.Default
            );
        }
        catch (Exception error)
        {
            Pending.Remove(block);
            ShowFailure(block, error);
        }
        return false;
    }

    private static void ShowFailure(MyProgrammableBlock block, Exception error)
    {
        Common.Logger.Error(
            error,
            "Could not finish compilation for programmable block {0}",
            block.EntityId
        );
        block.SetDetailedInfo(
            MyTexts.GetString(MySpaceTexts.ProgrammableBlock_Exception_ExceptionCaught)
                + error.Message
        );
    }
}
