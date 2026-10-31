using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using SpaceEngineers.Game.Entities.Blocks;

namespace Shared.Patches;

// DetailedInfo of blocks for scripts on a worker thread (StandIn.DetailedInfo).
//
// The getter refreshes the text if the block marked it dirty, by the block's UpdateDetailedInfo.
// Almost all of them read their own block and format numbers into the text with the game's
// shared formatter, so the refresh runs under ScriptStandIns.FormatLock. A few write shared
// state or raise events while refreshing: the piston raises PropertiesChanged, the flight
// movement block writes synced values, the path recorder updates IsWorking and emissives, the
// jump drive walks the physical grid group with a static scratch list and the projector updates
// its statistics. On a worker thread those return their last text and are refreshed on the
// main thread after the phase, so a script sees their text one run late.
public static class DetailedInfoStandIn
{
    private static readonly AccessTools.FieldRef<MyTerminalBlock, bool> Dirty =
        AccessTools.FieldRefAccess<MyTerminalBlock, bool>("m_detailedInfoDirty");

    private static readonly AccessTools.FieldRef<MyTerminalBlock, StringBuilder> Text =
        AccessTools.FieldRefAccess<MyTerminalBlock, StringBuilder>("m_detailedInfo");

    [StandInFor("Sandbox.ModAPI.Ingame.IMyTerminalBlock.DetailedInfo")]
    public static string DetailedInfo(Sandbox.ModAPI.Ingame.IMyTerminalBlock block)
    {
        if (!(block is MyTerminalBlock terminalBlock) || !Dirty(terminalBlock))
            return block.DetailedInfo;

        if (RefreshedOnMain(terminalBlock))
        {
            ScriptWrites.Later(() => _ = terminalBlock.DetailedInfo);
            return Text(terminalBlock).ToString();
        }

        lock (ScriptStandIns.FormatLock)
            return block.DetailedInfo;
    }

    private static bool RefreshedOnMain(MyTerminalBlock block) =>
        block is MyPistonBase
        || block is MyFlightMovementBlock
        || block is MyPathRecorderBlock
        || block is MyJumpDrive
        || block is MyProjectorBase;

    [StandInFor("Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.GetDetailedInfoWithoutRequiredInput")]
    public static string FarmPlotInfo(Sandbox.ModAPI.Ingame.IMyFarmPlotLogic farmPlot)
    {
        lock (ScriptStandIns.FormatLock)
            return farmPlot.GetDetailedInfoWithoutRequiredInput();
    }
}
