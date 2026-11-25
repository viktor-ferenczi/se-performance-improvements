using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Sandbox;
using Sandbox.Engine.Multiplayer;
using Sandbox.Engine.Utils;
using Sandbox.Game;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI.Ingame;
using Shared.Plugin;
using Shared.Tools;
using VRage;
using VRage.FileSystem;
using VRage.Network;
using VRage.Scripting;

namespace Shared.Patches;

[HarmonyPatch(typeof(MyGuiScreenEditor))]
internal static class EditorCompilation
{
    // A client gets one remote code check at a time, so it can't flood the shared queue.
    private static readonly PerKeyGate<ulong> RemoteChecks = new();

    [HarmonyPrefix]
    [HarmonyPatch("CheckCodeButtonClicked")]
    private static bool CheckLocal(MyGuiScreenEditor __instance)
    {
        if (
            !CompilationWorkers.Enabled || !MyVRage.Platform.Scripting.IsRuntimeCompilationSupported
        )
            return true;

        var editor = __instance;
        if (editor.m_progress != null)
            return false;

        string program = editor.Description.Text.ToString();
        var progress = new MyGuiScreenProgress(
            MyTexts.Get(MySpaceTexts.ProgrammableBlock_Editor_CheckingCode)
        );
        editor.m_progress = progress;
        MyScreenManager.AddScreen(progress);
        _ = Check(
            program,
            (success, messages) =>
            {
                if (editor.m_progress != progress)
                    return;
                editor.m_progress = null;
                MyScreenManager.RemoveScreen(progress);
                if (
                    editor.State == MyGuiScreenState.CLOSED
                    || editor.State == MyGuiScreenState.CLOSING
                    || !MyScreenManager.Screens.Contains(editor)
                )
                    return;
                editor.m_compilerErrors.Clear();
                editor.m_compilerErrors.AddRange(messages);
                editor.ProcessCompilationResults(success);
            }
        );
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("CompileProgramServer")]
    private static bool CheckRemote(byte[] program)
    {
        if (
            !CompilationWorkers.Enabled || !MyVRage.Platform.Scripting.IsRuntimeCompilationSupported
        )
            return true;
        if (!MySession.Static.EnableIngameScripts)
            return false;

        string text = StringCompressor.DecompressString(program);
        var sender = MyEventContext.Current.Sender;
        void Reply(bool success, List<string> messages) =>
            MyMultiplayer.RaiseStaticEvent(
                x => MyGuiScreenEditor.ReportCompilationResults,
                success,
                messages,
                sender
            );

        if (text.Length > MyGuiScreenEditor.MAX_NUMBER_CHARACTERS)
            Reply(
                false,
                new List<string>
                {
                    $"Overlimit program characters count. {text.Length}/{MyGuiScreenEditor.MAX_NUMBER_CHARACTERS}",
                }
            );
        else if (!RemoteChecks.TryEnter(sender.Value))
            Reply(
                false,
                new List<string> { "A code check is already running. Try again when it finishes." }
            );
        else
            _ = Check(text, Reply, () => RemoteChecks.Exit(sender.Value));
        return false;
    }

    private static async Task Check(
        string program,
        Action<bool, List<string>> complete,
        Action finished = null
    )
    {
        var session = MySession.Static;
        var game = MySandboxGame.Static;
        var messages = new List<string>();
        bool success = false;
        try
        {
            if (!string.IsNullOrEmpty(program))
            {
                var compile = MyVRage.Platform.Scripting.CompileIngameScriptAsync(
                    Path.Combine(MyFileSystem.UserDataPath, "EditorCode.dll"),
                    program,
                    out var diagnostics,
                    "PB Code editor",
                    "Program",
                    nameof(MyGridProgram),
                    MyFakes.ENABLE_PROGRAMMABLE_BLOCK_MEMORY_LIMIT
                );
                success = await compile.ConfigureAwait(false) != null;
                messages.AddRange(
                    diagnostics
                        .OrderByDescending(message => message.IsError)
                        .Select(message => message.Text)
                );
            }
        }
        catch (Exception error)
        {
            Common.Logger.Error(error, "PB editor compilation failed");
            messages.Add("The compiler could not complete the code check.");
        }

        game.Invoke(
            () =>
            {
                finished?.Invoke();
                if (MySession.Static == session)
                    complete(success, messages);
            },
            nameof(EditorCompilation)
        );
    }
}
