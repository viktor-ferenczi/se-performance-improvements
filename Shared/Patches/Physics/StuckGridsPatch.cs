using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    // Pushes apart two grids stuck inside each other after a collision (SE-753).
    //
    // A ship rammed into a station in a safe zone without damage can end up with blocks inside
    // the station. No block breaks, so nothing lets the grids come apart, and Havok keeps
    // solving the penetration on every step, costing most of the simulation speed for as long
    // as it lasts. Large collisions elsewhere can do the same for a while.
    //
    // The grids' contact point callback reports how deep each contact is. A pair of grids in
    // deep contact for StuckTicks updates is stuck. The grids which may move (not static and not
    // attached to a static grid) are then moved away from the center of their contact points,
    // StepMeters per update, until they no longer touch, and left to the physics at rest.
    // Static grids never move. Only the server moves the grids, the clients follow through the
    // usual position replication. In the client plugin that means offline games and the
    // multiplayer games the client hosts.
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyGridPhysics))]
    public static class StuckGridsPatch
    {
        private static IPluginConfig Config => Common.Config;
        private static IPluginLogger Log => Common.Logger;

        // Applied only while the option is on, see OptionalPatch
        private static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(StuckGridsPatch),
            () => Config.Enabled && Config.SeparateStuckGrids
        );

        // A contact point deeper than this (meters) is a collision the physics has not resolved
        private const float StuckDepth = 0.5f;

        // While a contact point is deeper than this (meters) the grids still collide
        private const float TouchDepth = 0.05f;

        // Updates with deep contact before the pair counts as stuck. The count restarts after
        // ForgetTicks updates without deep contact, so a hard but passing hit never adds up to it.
        private const int StuckTicks = 60;
        private const int ForgetTicks = 30;

        // The grids come apart at 30 m/s. Of two ships the lighter one moves more.
        private const double StepMeters = 0.5;

        // The grids are apart after this many updates without a contact deeper than TouchDepth
        private const int ApartTicks = 5;

        // Gives up after moving this far (300 m), the grids may be stuck some other way
        private const int MaxMoveTicks = 600;

        private struct Touch
        {
            public MyCubeGrid A;
            public MyCubeGrid B;
            public Vector3D Sum;
            public int Count;
            public float Deepest;
        }

        private sealed class Pair
        {
            public MyCubeGrid A;
            public MyCubeGrid B;
            public int DeepTicks;
            public long LastDeep;
            public long LastTouch;
            public Vector3D Center;

            // Separation: how far each grid moves per update, zero for one that stays
            public bool Separating;
            public int MoveTicks;
            public Vector3D StepA;
            public Vector3D StepB;
        }

        // Filled by the contact callback, which may run on the physics worker threads
        private static Dictionary<(long, long), Touch> touches =
            new Dictionary<(long, long), Touch>();
        private static Dictionary<(long, long), Touch> spareTouches =
            new Dictionary<(long, long), Touch>();
        private static readonly object TouchLock = new object();

        // Main thread only
        private static readonly Dictionary<(long, long), Pair> Pairs =
            new Dictionary<(long, long), Pair>();
        private static readonly List<(long, long)> Finished = new List<(long, long)>();
        private static long tick;

        private static bool Prepare() => Optional.Prepare();

        [HarmonyPrefix]
        [HarmonyPatch("RigidBody_ContactPointCallbackImpl")]
        [EnsureCode("94a52fc3")]
        private static void ContactPointCallbackPrefix(
            MyGridPhysics __instance,
            ref HkContactPointEvent value
        )
        {
            var distance = value.ContactPoint.Distance;
            if (distance > -TouchDepth || !Sync.IsServer)
                return;

            var a = value.GetPhysicsBody(0)?.Entity as MyCubeGrid;
            var b = value.GetPhysicsBody(1)?.Entity as MyCubeGrid;

            // Static grids get no callback, so a contact point between two ships is reported twice.
            // That does not change the center or the depth.
            if (a == null || b == null || a == b)
                return;

            var position = __instance.ClusterToWorld(value.ContactPoint.Position);
            var key = a.EntityId < b.EntityId ? (a.EntityId, b.EntityId) : (b.EntityId, a.EntityId);
            lock (TouchLock)
            {
                touches.TryGetValue(key, out var touch);
                touch.A = a;
                touch.B = b;
                touch.Sum += position;
                touch.Count++;
                if (distance < touch.Deepest)
                    touch.Deepest = distance;
                touches[key] = touch;
            }
        }

        // Called on every update from the main thread, between simulation steps
        public static void Update()
        {
            Optional.Update();
            if (!Config.Enabled || !Config.SeparateStuckGrids || !Sync.IsServer)
            {
                Pairs.Clear();
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
                var deep = touch.Deepest <= -StuckDepth;
                if (!Pairs.TryGetValue(item.Key, out var pair))
                {
                    if (!deep)
                        continue;

                    Pairs[item.Key] = pair = new Pair { A = touch.A, B = touch.B };
                }

                pair.LastTouch = tick;
                pair.Center = touch.Sum / touch.Count;
                if (deep)
                {
                    pair.DeepTicks++;
                    pair.LastDeep = tick;
                }
            }

            current.Clear();
            spareTouches = current;

            foreach (var item in Pairs)
            {
                if (!Advance(item.Value))
                    Finished.Add(item.Key);
            }

            foreach (var key in Finished)
                Pairs.Remove(key);

            Finished.Clear();
        }

        // Returns false when the pair is done with
        private static bool Advance(Pair pair)
        {
            var a = pair.A;
            var b = pair.B;
            if (a.Closed || b.Closed || a.Physics == null || b.Physics == null)
                return false;

            if (!pair.Separating)
            {
                if (tick - pair.LastDeep > ForgetTicks)
                    return false;

                return pair.DeepTicks < StuckTicks || Start(pair);
            }

            if (tick - pair.LastTouch >= ApartTicks)
            {
                Stop(a, pair.StepA);
                Stop(b, pair.StepB);
                Log.Info(
                    $"Separated grids {Name(a)} and {Name(b)}, moved them {pair.MoveTicks * StepMeters:F1} m apart"
                );
                return false;
            }

            if (pair.MoveTicks >= MaxMoveTicks)
            {
                Stop(a, pair.StepA);
                Stop(b, pair.StepB);
                Log.Warning(
                    $"Could not separate grids {Name(a)} and {Name(b)}, gave up after moving them {pair.MoveTicks * StepMeters:F1} m"
                );
                return false;
            }

            Move(a, pair.StepA);
            Move(b, pair.StepB);
            pair.MoveTicks++;
            return true;
        }

        private static bool Start(Pair pair)
        {
            var a = pair.A;
            var b = pair.B;

            // Grids joined by rotors, pistons, connectors or landing gears move together
            if (MyCubeGridGroups.Static.Physical.HasSameGroup(a, b))
                return false;

            var staysA = IsAnchored(a);
            var staysB = IsAnchored(b);
            if (staysA && staysB)
                return false;

            var massA = staysA
                ? 0
                : MyGridPhysicalGroupData.GetGroupSharedProperties(a, false).Mass;
            var massB = staysB
                ? 0
                : MyGridPhysicalGroupData.GetGroupSharedProperties(b, false).Mass;
            var shareA =
                staysA ? 0
                : staysB ? 1
                : massA + massB > 0 ? massB / (massA + massB)
                : 0.5f;
            pair.StepA = Away(a, b, pair.Center) * (StepMeters * shareA);
            pair.StepB = Away(b, a, pair.Center) * (StepMeters * (1 - shareA));
            pair.Separating = true;
            Log.Info($"Grids {Name(a)} and {Name(b)} are stuck inside each other, separating them");
            return true;
        }

        // Static grids and the grids attached to one never move
        private static bool IsAnchored(MyCubeGrid grid)
        {
            return grid.IsStatic
                || MyCubeGridGroups
                    .Static.Physical.GetGroup(grid)
                    .Nodes.Any(node => node.NodeData.IsStatic);
        }

        // From the center of the contact points towards the grid's own center. Falls back to the
        // direction from the other grid's center when the contacts are centered on the grid.
        private static Vector3D Away(MyCubeGrid grid, MyCubeGrid other, Vector3D center)
        {
            var own = grid.PositionComp.WorldAABB.Center;
            var direction = own - center;
            if (direction.LengthSquared() < 1)
                direction = own - other.PositionComp.WorldAABB.Center;
            if (direction.LengthSquared() < 1e-6)
                direction = grid.WorldMatrix.Up;
            return Vector3D.Normalize(direction);
        }

        // Teleport moves the grid's whole physical group (subgrids, docked ships) along with it
        private static void Move(MyCubeGrid grid, Vector3D step)
        {
            if (step == Vector3D.Zero)
                return;

            var matrix = grid.WorldMatrix;
            matrix.Translation += step;
            grid.Teleport(matrix);
            Stop(grid, step);
        }

        // The grids would collide again with the speed they had when they got stuck
        private static void Stop(MyCubeGrid grid, Vector3D step)
        {
            if (step == Vector3D.Zero)
                return;

            foreach (var node in MyCubeGridGroups.Static.Physical.GetGroup(grid).Nodes)
            {
                var physics = node.NodeData.Physics;
                if (physics == null)
                    continue;

                physics.LinearVelocity = Vector3.Zero;
                physics.AngularVelocity = Vector3.Zero;
            }
        }

        private static string Name(MyCubeGrid grid) => $"\"{grid.DisplayName}\" ({grid.EntityId})";
    }
}
