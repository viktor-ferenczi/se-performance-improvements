using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.World;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRageMath;

namespace Shared.Patches
{
    // A grid with turrets refreshes its list of target groups once per frame
    // (MyGridTargeting.RefreshGridConnections): every top-most entity in the turrets' range,
    // grouped by physical connection. The game pops entities off the query result and,
    // for every grid, removes each physically connected grid from that list with
    // List.Remove, a linear search, so a scene with N grids in range costs N * N
    // comparisons per turret grid per frame. With hundreds of grids in range that is
    // milliseconds per frame on the main thread, on the server as much as on the client.
    //
    // This does the same grouping with a set for the "not yet grouped" test, which
    // makes the refresh linear in the number of grids. The result (the group lists and
    // their order) is the same.
    [HarmonyPatch(typeof(MyGridTargeting))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyGridTargetingPatch
    {
        private static IPluginConfig Config => Common.Config;

        // The refresh runs on the update thread; a spare set per thread keeps it allocation free
        [System.ThreadStatic]
        private static HashSet<MyEntity> remaining;

        [HarmonyPrefix]
        [HarmonyPatch("RefreshGridConnections")]
        [EnsureCode("9cc69ebe")]
        private static bool RefreshGridConnectionsPrefix(MyGridTargeting __instance)
        {
            if (!Config.FixTargeting)
            {
                return true;
            }

            __instance.m_gridGroupCache = MySession.Static.GameplayFrameCounter;

            var range = __instance.m_queryMaxRange;
            var sphere = new BoundingSphereD(
                Vector3D.Transform(range.Center, __instance.m_grid.WorldMatrix),
                range.Radius
            );

            var connections = __instance.m_targetConnections;
            var pool = MyGridTargeting.m_gridGroupPool;
            foreach (var connection in connections)
            {
                pool.Deallocate(connection);
            }

            connections.Clear();

            var entities = MyEntities.GetTopMostEntitiesInSphere(ref sphere);
            var set = remaining ??= new HashSet<MyEntity>();
            set.Clear();
            foreach (var entity in entities)
            {
                if (entity is MyCubeGrid)
                {
                    set.Add(entity);
                }
            }

            // Same order as the game: from the last entity of the query result backwards
            for (var i = entities.Count - 1; i >= 0; i--)
            {
                if (!(entities[i] is MyCubeGrid grid) || !set.Remove(grid))
                {
                    continue;
                }

                var connected = pool.Allocate();
                grid.GetConnectedGrids(
                    GridLinkTypeEnum.Physical,
                    other =>
                    {
                        set.Remove(other);
                        connected.Add(other.EntityId);
                    }
                );
                connections.Add(connected);
            }

            set.Clear();

            // The game empties the query result while walking it; it is a shared buffer
            entities.Clear();
            return false;
        }
    }
}
