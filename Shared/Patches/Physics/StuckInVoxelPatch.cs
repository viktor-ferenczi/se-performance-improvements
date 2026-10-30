using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Havok;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRageMath;

namespace Shared.Patches
{
    // Pushes a grid stuck inside a planet or an asteroid out of it (SE-770).
    //
    // The voxel counterpart of StuckGridsPatch. A ship teleported or rammed into the ground, in
    // a safe zone without damage or with damage, can end up partly inside the voxel. The
    // overlap keeps Havok busy on every step and costs most of the simulation speed for as long
    // as it lasts, even where the grid is inside the voxel and has no contact with its surface.
    //
    // Havok reports the contacts with a voxel body less than 1 m deep even for a ship half inside
    // an asteroid, and the depth comes and goes, so it does not tell a stuck grid from a landed
    // one. A grid in contact with a voxel body is checked every CheckTicks updates instead: when
    // at least one in InsideShare of its sampled block centers is inside the voxel on StuckChecks
    // checks in a row, it is stuck. Unless it is static or attached to a static grid, it is then
    // moved away from the voxel body's center, StepMeters per update, until it neither touches
    // the voxel nor has a sampled block center inside it, and left to the physics at rest. Only
    // the server moves the grids, the clients follow through the usual position replication.
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyGridPhysics))]
    public static class StuckInVoxelPatch
    {
        private static IPluginConfig Config => Common.Config;
        private static IPluginLogger Log => Common.Logger;

        // Applied only while the option is on, see OptionalPatch
        private static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(StuckInVoxelPatch),
            () => Config.Enabled && Config.PushGridsOutOfVoxels
        );

        // While a contact point is deeper than this (meters) the grid touches the voxel
        private const float TouchDepth = 0.05f;

        // Forgets a grid after this many updates without touching
        private const int ForgetTicks = 60;

        // A landed grid has no block center inside the voxel
        private const int CheckTicks = 10;
        private const int StuckChecks = 3;
        private const int SampledBlocks = 64;
        private const int InsideShare = 20;

        // Moves out at 30 m/s, gives up after 300 m
        private const double StepMeters = 0.5;
        private const int ApartTicks = 5;
        private const int MaxMoveTicks = 600;

        private struct Touch
        {
            public MyCubeGrid Grid;
            public MyVoxelBase Voxel;
        }

        private sealed class Stuck
        {
            public MyCubeGrid Grid;
            public MyVoxelBase Voxel;
            public long LastTouch;
            public long NextCheck;
            public int InsideChecks;
            public bool Moving;
            public int MoveTicks;
            public Vector3D Step;
        }

        // Filled by the contact callback, which may run on the physics worker threads
        private static Dictionary<(long, long), Touch> touches =
            new Dictionary<(long, long), Touch>();
        private static Dictionary<(long, long), Touch> spareTouches =
            new Dictionary<(long, long), Touch>();
        private static readonly object TouchLock = new object();

        // Main thread only
        private static readonly Dictionary<(long, long), Stuck> Stucks =
            new Dictionary<(long, long), Stuck>();
        private static readonly List<(long, long)> Finished = new List<(long, long)>();
        private static long tick;
        private static readonly Vector3D[] Point = new Vector3D[1];

        private static bool Prepare() => Optional.Prepare();

        [HarmonyPrefix]
        [HarmonyPatch("RigidBody_ContactPointCallbackImpl")]
        [EnsureCode("94a52fc3")]
        private static void ContactPointCallbackPrefix(ref HkContactPointEvent value)
        {
            var distance = value.ContactPoint.Distance;
            if (distance > -TouchDepth || !Sync.IsServer)
                return;

            var a = value.GetPhysicsBody(0)?.Entity;
            var b = value.GetPhysicsBody(1)?.Entity;
            var grid = a as MyCubeGrid ?? b as MyCubeGrid;
            var voxel = (a as MyVoxelBase ?? b as MyVoxelBase)?.RootVoxel;
            if (grid == null || voxel == null || grid.IsStatic)
                return;

            var key = (grid.EntityId, voxel.EntityId);
            lock (TouchLock)
            {
                touches.TryGetValue(key, out var touch);
                touch.Grid = grid;
                touch.Voxel = voxel;
                touches[key] = touch;
            }
        }

        // Called on every update from the main thread, between simulation steps
        public static void Update()
        {
            Optional.Update();
            if (!Config.Enabled || !Config.PushGridsOutOfVoxels || !Sync.IsServer)
            {
                Stucks.Clear();
                return;
            }

            tick++;

            Dictionary<(long, long), Touch> current;
            lock (TouchLock)
            {
                current = touches;
                touches = spareTouches;
            }

            foreach (var item in current)
            {
                var touch = item.Value;
                if (!Stucks.TryGetValue(item.Key, out var stuck))
                    Stucks[item.Key] = stuck = new Stuck { Grid = touch.Grid, Voxel = touch.Voxel };

                stuck.LastTouch = tick;
            }

            current.Clear();
            spareTouches = current;

            foreach (var item in Stucks)
            {
                if (!Advance(item.Value))
                    Finished.Add(item.Key);
            }

            foreach (var key in Finished)
                Stucks.Remove(key);

            Finished.Clear();
        }

        // Returns false when the grid is done with
        private static bool Advance(Stuck stuck)
        {
            var grid = stuck.Grid;
            if (grid.Closed || grid.Physics == null || stuck.Voxel.Closed)
                return false;

            if (!stuck.Moving)
            {
                if (tick - stuck.LastTouch > ForgetTicks)
                    return false;

                if (tick < stuck.NextCheck)
                    return true;

                stuck.NextCheck = tick + CheckTicks;
                stuck.InsideChecks = IsStuck(grid, stuck.Voxel) ? stuck.InsideChecks + 1 : 0;
                return stuck.InsideChecks < StuckChecks || Start(stuck);
            }

            var meters = stuck.MoveTicks * StepMeters;
            // A grid inside the voxel has no contact with its surface, but still costs as much
            if (tick - stuck.LastTouch >= ApartTicks && CountInside(grid, stuck.Voxel).Inside == 0)
            {
                StuckGridsPatch.Stop(grid, stuck.Step);
                Log.Info(
                    $"Pushed grid {StuckGridsPatch.Name(grid)} out of {Name(stuck.Voxel)}, moved it {meters:F1} m"
                );
                return false;
            }

            if (stuck.MoveTicks >= MaxMoveTicks)
            {
                StuckGridsPatch.Stop(grid, stuck.Step);
                Log.Warning(
                    $"Could not push grid {StuckGridsPatch.Name(grid)} out of {Name(stuck.Voxel)}, gave up after moving it {meters:F1} m"
                );
                return false;
            }

            StuckGridsPatch.Move(grid, stuck.Step);
            stuck.MoveTicks++;
            return true;
        }

        private static bool Start(Stuck stuck)
        {
            var grid = stuck.Grid;
            if (StuckGridsPatch.IsAnchored(grid))
                return false;

            // Another grid of the same physical group is already moving the whole group
            foreach (var other in Stucks.Values)
            {
                if (other.Moving && MyCubeGridGroups.Static.Physical.HasSameGroup(grid, other.Grid))
                    return false;
            }

            // Away from the center works for planets and for the mostly round asteroids
            var direction =
                grid.PositionComp.WorldAABB.Center - stuck.Voxel.PositionComp.WorldAABB.Center;
            if (direction.LengthSquared() < 1e-6)
                direction = grid.WorldMatrix.Up;

            stuck.Step = Vector3D.Normalize(direction) * StepMeters;
            stuck.Moving = true;
            Log.Info(
                $"Grid {StuckGridsPatch.Name(grid)} is stuck inside {Name(stuck.Voxel)}, pushing it out"
            );
            return true;
        }

        // At least one in InsideShare of the sampled block centers is inside the voxel
        private static bool IsStuck(MyCubeGrid grid, MyVoxelBase voxel)
        {
            var (sampled, inside) = CountInside(grid, voxel);
            return inside > 0 && inside * InsideShare >= sampled;
        }

        // Samples up to SampledBlocks block centers spread over the grid
        internal static (int Sampled, int Inside) CountInside(MyCubeGrid grid, MyVoxelBase voxel)
        {
            var blocks = grid.GetBlocks();
            var stride = blocks.Count / SampledBlocks + 1;
            int index = 0,
                sampled = 0,
                inside = 0;
            foreach (var block in blocks)
            {
                if (index++ % stride != 0)
                    continue;

                block.ComputeWorldCenter(out Point[0]);
                sampled++;
                if (voxel.IsAnyOfPointInside(Point))
                    inside++;
            }

            return (sampled, inside);
        }

        private static string Name(MyVoxelBase voxel) =>
            $"\"{voxel.StorageName}\" ({voxel.EntityId})";
    }
}
