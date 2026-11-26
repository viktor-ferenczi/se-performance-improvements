using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.Entities.Blocks;
using Shared.Tools;
using VRage.Utils;

namespace Shared.Patches
{
    // A rotor, hinge, piston or wheel top marks its stator for reattach in Init. Grids that a
    // client streams in, and grids made with CreateFromObjectBuilderParallel, run Init on a worker
    // thread, while the stator's grid may already be in the scene. MarkForReattach then adds the
    // stator to its grid's BlocksWaitingOnAttachUpdate from the worker, racing the main thread,
    // which changes the same HashSet from the stators' UpdateOnceBeforeFrame. A HashSet torn by
    // the race throws NullReferenceException in AddIfNotPresent on the main thread and crashes
    // the game (issue #161). The setter also registers the stator for the next frame update from
    // the worker thread.
    //
    // The call is redirected to run on the main thread instead. Init queues it there before the
    // entity's own completion callback, so the stator is still marked before the new top is
    // added to the scene.
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [HarmonyPatch(typeof(MyAttachableTopBlockBase))]
    public static class MyAttachableTopBlockBasePatch
    {
        [HarmonyPatch("Init")]
        [HarmonyTranspiler]
        [EnsureCode("e350b93b")]
        private static IEnumerable<CodeInstruction> InitTranspiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var markForReattach = AccessTools.DeclaredMethod(
                typeof(MyMechanicalConnectionBlockBase),
                nameof(MyMechanicalConnectionBlockBase.MarkForReattach)
            );
            var replacement = AccessTools.DeclaredMethod(
                typeof(MyAttachableTopBlockBasePatch),
                nameof(MarkForReattach)
            );
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(markForReattach))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = replacement;
                }
                yield return instruction;
            }
        }

        private static void MarkForReattach(MyMechanicalConnectionBlockBase stator)
        {
            if (Thread.CurrentThread == MyUtils.MainThread)
            {
                stator.MarkForReattach();
                return;
            }

            MySandboxGame.Static.Invoke(
                () =>
                {
                    if (!stator.Closed)
                        stator.MarkForReattach();
                },
                "Performance: MarkForReattach"
            );
        }
    }
}
