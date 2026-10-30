using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI.Ingame;
using VRage.Library.Compiler;

namespace Shared.Patches;

// IMyProgrammableBlock.TryRun for scripts off the main thread (StandIn.TryRun).
//
// The other PB runs right away on the same thread, as in the game, when it is in the same grid
// group and its script may run off the main thread too: its game calls go through its proxies
// and its writes into the running script's log. Its DetailedInfo is set on the main thread
// after the phase, as it raises a network event. Otherwise TryRun returns false, as the game
// does for a PB that is busy.
public static class TryRunStandIn
{
    [StandInFor("Sandbox.ModAPI.Ingame.IMyProgrammableBlock.TryRun")]
    public static bool TryRun(IMyProgrammableBlock block, string argument)
    {
        if (!(block is MyProgrammableBlock pb))
            return false;
        var program = pb.m_instance;
        if (program == null || pb.m_isRunning || !pb.IsWorking || !pb.IsFunctional)
            return false;
        if (
            MyCubeGridGroups.Static.Logical.GetGroup(pb.CubeGrid) != ScriptStandIns.CurrentGroup
            || ScriptRoots.State(program) != 0
            || !ScriptRoots.Prepared(program)
        )
            return false;

        var result = pb.ExecuteCode(argument ?? "", UpdateType.Script, out var response);
        ScriptWrites.Later(() =>
        {
            if (!pb.Closed)
                pb.SetDetailedInfo(response);
        });
        if (result == MyProgrammableBlock.ScriptTerminationReason.InstructionOverflow)
            throw new ScriptOutOfRangeException(
                "This exception crashes the game only when Mod runs programmable block with script that exceeds the stack limit. Consider catching this exception or rewriting your script."
            );
        return result == MyProgrammableBlock.ScriptTerminationReason.None;
    }
}
