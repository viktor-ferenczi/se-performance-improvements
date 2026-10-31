using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using VRage;
using VRage.Game.ModAPI.Ingame;

namespace Shared.Patches;

// Item transfers between inventories for scripts on a worker thread (StandIn.Transfers).
//
// Whether an item can go from one inventory to the other is worked out on the worker, as the
// game does it: the conveyor reachability check has its own lock and its own per inventory
// cache, and the game's background tasks run it too. So the script gets the game's own result.
// The move itself raises inventory events with listeners all over the game (terminal screens,
// contracts, mods), so it is made on the main thread after the phase, by item id, in order with
// the script's other writes. Until then the script sees the inventories as they were: a loop
// moving items until an inventory is empty does not end in that run.
//
// Both inventories must belong to blocks of the script's own grid group. One on a grid behind a
// trading connector, or on a grid disconnected meanwhile, is unreachable for the game too.
public static class TransferStandIn
{
    [StandInFor("VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemTo")]
    public static bool TransferItemTo(
        IMyInventory source,
        IMyInventory destination,
        int sourceItemIndex,
        int? targetItemIndex,
        bool? stackIfPossible,
        MyFixedPoint? amount
    ) =>
        Move(
            source as MyInventory,
            destination as MyInventory,
            sourceItemIndex,
            targetItemIndex,
            amount
        );

    [StandInFor("VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemFrom")]
    public static bool TransferItemFrom(
        IMyInventory destination,
        IMyInventory source,
        int sourceItemIndex,
        int? targetItemIndex,
        bool? stackIfPossible,
        MyFixedPoint? amount
    ) =>
        Move(
            source as MyInventory,
            destination as MyInventory,
            sourceItemIndex,
            targetItemIndex,
            amount
        );

    [StandInFor("VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemTo")]
    public static bool TransferItemTo(
        IMyInventory source,
        IMyInventory destination,
        MyInventoryItem item,
        MyFixedPoint? amount
    ) => Move(source as MyInventory, destination as MyInventory, item, amount);

    [StandInFor("VRage.Game.ModAPI.Ingame.IMyInventory.TransferItemFrom")]
    public static bool TransferItemFrom(
        IMyInventory destination,
        IMyInventory source,
        MyInventoryItem item,
        MyFixedPoint? amount
    ) => Move(source as MyInventory, destination as MyInventory, item, amount);

    private static bool Move(
        MyInventory source,
        MyInventory destination,
        MyInventoryItem item,
        MyFixedPoint? amount
    )
    {
        if (source == null || !InGroup(source))
            return false;
        var index = source.GetItemIndexById(item.ItemId);
        return index >= 0 && Move(source, destination, index, null, amount);
    }

    // MyInventory.TransferItemsFrom with the move made later
    private static bool Move(
        MyInventory source,
        MyInventory destination,
        int sourceItemIndex,
        int? targetItemIndex,
        MyFixedPoint? amount
    )
    {
        if (destination == null)
            return false;
        if (amount.HasValue && amount.Value <= 0)
            return true;
        if (source == null || !InGroup(source) || !InGroup(destination))
            return false;
        if (!source.IsItemAt(sourceItemIndex))
            return false;

        var item = source.GetItems()[sourceItemIndex];
        if (!source.CanTransferTo(destination, item.Content.GetObjectId()))
            return false;

        var itemId = item.ItemId;
        var target = targetItemIndex ?? -1;
        ScriptWrites.Later(() => MyInventory.Transfer(source, destination, itemId, target, amount));
        return true;
    }

    private static bool InGroup(MyInventory inventory) =>
        inventory.Owner is MyCubeBlock block
        && block.CubeGrid != null
        && MyCubeGridGroups.Static.Logical.GetGroup(block.CubeGrid) == ScriptStandIns.CurrentGroup;
}
