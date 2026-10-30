using System.Collections.Generic;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Interfaces;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI;

namespace Shared.Patches;

// IMyGridTerminalSystem.CanAccess for scripts off the main thread (StandIn.Access).
//
// A block's access is a read of ownership, factions and grid groups. A grid's is its big
// owners, which off the main thread the game hands out without the fallback it uses on the
// main thread for a grid owning no blocks, the owners of the grid it hangs on. The grid
// overload here applies that fallback on any thread.
public static class AccessStandIn
{
    [StandInFor("Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.CanAccess")]
    public static bool CanAccess(
        IMyGridTerminalSystem system,
        IMyTerminalBlock block,
        MyTerminalAccessScope scope
    ) => system.CanAccess(block, scope);

    [StandInFor("Sandbox.ModAPI.Ingame.IMyGridTerminalSystem.CanAccess")]
    public static bool CanAccess(
        IMyGridTerminalSystem system,
        VRage.Game.ModAPI.Ingame.IMyCubeGrid grid,
        MyTerminalAccessScope scope
    )
    {
        if (
            !(system is MyProgrammableBlock.MyGridTerminalWrapper wrapper)
            || !(grid is MyCubeGrid cubeGrid)
        )
            return system.CanAccess(grid, scope);
        if (cubeGrid.Closed)
            return false;

        var pb = wrapper.m_programmableBlock;
        if (BigOwners(cubeGrid)?.Contains(pb.OwnerId) != true)
            return false;
        switch (scope)
        {
            case MyTerminalAccessScope.All:
                return ((IMyCubeGrid)cubeGrid).IsInSameLogicalGroupAs(pb.CubeGrid);
            case MyTerminalAccessScope.Construct:
                return cubeGrid.IsSameConstructAs(pb.CubeGrid);
            case MyTerminalAccessScope.Grid:
                return cubeGrid == pb.CubeGrid;
            default:
                return system.CanAccess(grid, scope);
        }
    }

    // MyCubeGrid.BigOwners as on the main thread
    private static List<long> BigOwners(MyCubeGrid grid)
    {
        var owners = grid.BigOwners;
        if (owners == null || owners.Count != 0)
            return owners;
        var parent = MyGridPhysicalHierarchy.Static.GetParent(grid);
        if (parent == null)
            return owners;
        var connection = MyGridPhysicalHierarchy.Static.GetEntityConnectingToParent(grid);
        if (connection == null || connection is IMyLandingGear || connection is IMyShipConnector)
            return owners;
        return BigOwners(parent);
    }
}
