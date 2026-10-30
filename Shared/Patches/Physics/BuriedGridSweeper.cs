using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Planet;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using VRage.Game.Entity;
using VRage.Groups;
using VRageMath;

namespace Shared.Patches
{
    // Removes dynamic grids lying entirely inside a planet (SE-779, moved from the cluster
    // plugin).
    //
    // Debris that falls through a planet's surface never settles. It sinks towards the planet's
    // center and jitters there, and the physics step keeps paying for it. Every few minutes this
    // closes the dynamic grids that are not sleeping and lie entirely inside a planet: the whole
    // bounding sphere more than MarginMeters under the heightmap, and every sampled block center
    // inside the voxel content. Every grid connected to one by rotors, pistons or connectors must
    // be buried as well, and none of them static. What is on the grid (power, inventory, a pilot)
    // does not matter. A sleeping grid costs nothing per step, so it stays. Grids only partly
    // inside are pushed out by StuckInVoxelPatch instead. A sweep is spread over updates,
    // GridsPerUpdate grids each. Only the server removes grids.
    public static class BuriedGridSweeper
    {
        private static IPluginConfig Config => Common.Config;
        private static IPluginLogger Log => Common.Logger;

        // A ship resting on the ground dips below the heightmap, one that fell through is far under
        private const double MarginMeters = 100.0;

        // A sweep checks this many grids per update, so a world with thousands of grids does not
        // get one long frame
        private const int GridsPerUpdate = 20;

        private static double nextSweep;
        private static readonly List<MyCubeGrid> Pending = new List<MyCubeGrid>();

        // Called on every update from the main thread, between simulation steps
        public static void Update()
        {
            if (
                !Config.Enabled
                || !Config.RemoveBuriedGrids
                || !Sync.IsServer
                || MySession.Static?.Ready != true
            )
            {
                // ElapsedGameTime starts over with the next world
                Pending.Clear();
                nextSweep = 0;
                return;
            }

            if (Pending.Count == 0)
            {
                var now = MySession.Static.ElapsedGameTime.TotalSeconds;
                if (now < nextSweep)
                    return;

                nextSweep = now + 60.0 * Math.Max(1, Config.BuriedGridSweepMinutes);
                foreach (MyEntity entity in MyEntities.GetEntities())
                {
                    if (entity is MyCubeGrid grid && !grid.IsStatic)
                        Pending.Add(grid);
                }
            }

            var count = Math.Min(GridsPerUpdate, Pending.Count);
            for (var i = Pending.Count - count; i < Pending.Count; i++)
                Check(Pending[i]);

            Pending.RemoveRange(Pending.Count - count, count);
        }

        private static void Check(MyCubeGrid grid)
        {
            if (grid.MarkedForClose || grid.Physics == null || !grid.Physics.IsActive)
                return;

            var planet = BuriedIn(grid);
            if (planet == null)
                return;

            // A buried drill head or wheel of a ship still on the surface is not debris
            var group = MyCubeGridGroups.Static.Physical.GetGroup(grid);
            foreach (
                var node in group?.Nodes
                    ?? Enumerable.Empty<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node>()
            )
            {
                var other = node.NodeData;
                if (other.IsStatic || other != grid && BuriedIn(other) == null)
                    return;
            }

            var position = grid.PositionComp.GetPosition();
            Log.Info(
                $"Removed grid {StuckGridsPatch.Name(grid)} buried inside planet \"{planet.StorageName}\", {grid.BlocksCount} blocks at {position.X:F0}, {position.Y:F0}, {position.Z:F0}"
            );
            grid.Close();
        }

        // The planet whose surface is above the grid's whole bounding sphere and whose voxel
        // content holds every sampled block center, or null. GetClosestSurfacePointGlobal projects
        // onto the heightmap along the ray from the planet's center, so the depth is measured
        // straight down, not against a nearby canyon wall. The heightmap knows nothing about mined
        // tunnels and shafts, so a grid under it is checked against the voxel content too.
        private static MyPlanet BuriedIn(MyCubeGrid grid)
        {
            var position = grid.PositionComp.GetPosition();
            var planet = MyPlanets.Static?.GetClosestPlanet(position);
            if (planet == null)
                return null;

            var center = planet.PositionComp.GetPosition();
            var fromCenter = Vector3D.Distance(position, center);
            if (fromCenter > planet.AtmosphereRadius)
                return null;

            var surface = planet.GetClosestSurfacePointGlobal(position);
            var depth = fromCenter - Vector3D.Distance(surface, center);
            var radius = grid.PositionComp.LocalVolume.Radius;
            if (depth + radius + MarginMeters >= 0.0)
                return null;

            var (sampled, inside) = StuckInVoxelPatch.CountInside(grid, planet);
            return sampled > 0 && inside == sampled ? planet : null;
        }
    }
}
