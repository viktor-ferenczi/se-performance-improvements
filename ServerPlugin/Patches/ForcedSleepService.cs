// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Viktor Ferenczi
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Havok;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Groups;
using VRage.ModAPI;
using VRageMath;

namespace ServerPlugin.Patches;

/// <summary>
/// Forced physics sleep for stopped, supported grids (Viktor 2026-09-26).
///
/// Why: vanilla keeps a stopped ship's rigid body awake for good. MyEntityThrustComponent calls
/// ActivatePhysics every frame while dampeners hold against gravity, MyShipController.MoveAndRotate
/// does it every frame while a player sits in control, and a rotor or wheel whose motor has a non-zero
/// target does it too. Havok then keeps the whole island awake - the ship, its subgrids and every
/// wreck touching it. w33 node-2 at 2026-09-25 23:57: 122 of 167 awake grids moved &lt;0.5 m/s and
/// stayed awake 44 s later, in four piles around damaged Polaris copies at the Earth base, 7,167 of
/// ~9,250 awake blocks.
///
/// Rule. A physical group (connectors, landing gear, rotors, pistons) is put to sleep when every
/// non-static member has stayed below <see cref="m_linear"/> m/s and <see cref="m_angular"/> rad/s
/// for <see cref="m_holdSeconds"/>, AND it is supported: a member touches voxel, a static grid or a
/// sleeping non-static grid, or the group holds a static grid. A group stopped in space or hovering
/// mid-air is left alone. Cockpits, thrusters and gyros do not prevent it.
/// The unit Havok sleeps is the simulation island, so the group's active island must be still as a whole:
/// every still grid in it sleeps and wakes with the group, a character or a moving body refuses
/// (<see cref="ExtendToIsland"/>).
///
/// While asleep the game's own wake calls are held shut (ForcedSleepPatch.cs:
/// MyCubeGrid.ActivatePhysics, the rotor and wheel ActivatePhysics, the thrust and gyro updates).
/// It wakes on:
///  - collision: Havok itself activates the body (a hit, an explosion impulse, a body landing on it);
///    the next tick sees IsActive and releases the group;
///  - a control change: pilot input, dampeners toggled, thrust or gyro override changed, control or
///    autopilot thrust/torque changed (the patches call <see cref="Wake"/>);
///  - a structural change (Viktor 2026-09-26): a block added, removed or damaged, a block's integrity
///    changed (welding, grinding), or the grid split. Each slept grid subscribes to its own OnBlockAdded,
///    OnBlockRemoved, OnBlockIntegrityChanged, OnGridBlockDamaged and OnGridSplit while asleep; the
///    block-count poll stays as a backstop. So a ship whose support was ground away does not hang in
///    the air, and a weapon hit (its impulse goes through the held managed Activate) wakes the ship.
/// Rotor/hinge/piston target changes do NOT wake (not in the rule); a script-driven turret on a
/// sleeping ship turns again once something wakes the ship.
///
/// On with PerformanceConfig.SleepSupportedGrids (default off; a cluster stages it on in its profile),
/// tuned by SleepLinearSpeed (m/s, 0.1), SleepAngularSpeed (rad/s, 0.05) and SleepHoldSeconds (2).
/// On a cluster node it waits for the cluster's IsFrozen callback, so a node still running the
/// cluster's own copy of this service keeps that one.
/// Game thread only (Plugin.Update and the cluster's freeze, which calls <see cref="ReleaseForTransfer"/>),
/// except <see cref="IsSlept"/> and <see cref="Wake"/>, which the patches may call from parallel updates.
/// </summary>
public static class ForcedSleepService
{
    private const int TicksPerSecond = 4; // Sample every 15 simulation frames.
    private const int ReportEveryTicks = 240;

    /// <summary>Penetration queries per tick, so a crowd of newly stopped grids cannot stall a frame (checked
    /// before each group; a started group finishes).</summary>
    private const int QueriesPerTick = 8;

    /// <summary>An unsupported (hovering, floating) group is asked again only after this many ticks.</summary>
    private const int UnsupportedRetryTicks = 10 * TicksPerSecond;

    private static Func<long, bool> isFrozen;
    private static bool running;
    private static float m_linear,
        m_angular,
        m_holdSeconds;

    // The .NET Framework target has no built-in reference equality comparer.
    private sealed class ByReference : IEqualityComparer<object>
    {
        internal static readonly ByReference Instance = new();

        bool IEqualityComparer<object>.Equals(object a, object b) => ReferenceEquals(a, b);

        public int GetHashCode(object value) =>
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);
    }

    private sealed class Slept
    {
        public long Group;
        public int Blocks;
        public long SleptTick;
        public HkEntity[] Bodies;

        /// <summary>The structural-change handlers subscribed on the grid while it sleeps.</summary>
        public MyCubeGrid Grid;
        public Action<MySlimBlock> OnAdded,
            OnRemoved,
            OnIntegrity;
        public Action<IMySlimBlock, float, MyHitInfo?, long> OnDamaged;
        public Action<MyCubeGrid, MyCubeGrid> OnSplit;

        public void Subscribe()
        {
            Grid.OnBlockAdded += OnAdded;
            Grid.OnBlockRemoved += OnRemoved;
            Grid.OnBlockIntegrityChanged += OnIntegrity;
            Grid.OnGridBlockDamaged += OnDamaged;
            Grid.OnGridSplit += OnSplit;
        }

        public void Unsubscribe()
        {
            if (Grid == null)
                return;
            Grid.OnBlockAdded -= OnAdded;
            Grid.OnBlockRemoved -= OnRemoved;
            Grid.OnBlockIntegrityChanged -= OnIntegrity;
            Grid.OnGridBlockDamaged -= OnDamaged;
            Grid.OnGridSplit -= OnSplit;
        }
    }

    /// <summary>
    /// The rigid bodies of slept grids, for the Havok-level hold (ForcedSleepPatch on HkEntity.Activate):
    /// any managed Activate on one of these is skipped and its caller counted.
    /// </summary>
    private static readonly ConcurrentDictionary<HkEntity, long> m_sleptBodies =
        new ConcurrentDictionary<HkEntity, long>(ByReference.Instance);
    private static readonly ConcurrentDictionary<string, long> m_heldActivations =
        new ConcurrentDictionary<string, long>();
    private static int m_heldSamples;

    /// <summary>True for a body held asleep; counts the caller (sampled) of the Activate it replaces.</summary>
    public static bool HoldActivation(HkEntity body)
    {
        if (body == null || m_sleptBodies.IsEmpty || !m_sleptBodies.ContainsKey(body))
            return false;
        string caller = "sampled-out";
        if (Interlocked.Increment(ref m_heldSamples) <= 200)
        {
            var trace = new System.Diagnostics.StackTrace(2, false);
            var frames = new List<string>();
            for (int i = 0; i < trace.FrameCount && frames.Count < 2; i++)
            {
                System.Reflection.MethodBase method = trace.GetFrame(i)?.GetMethod();
                if (method == null)
                    continue;
                string name = (method.DeclaringType?.Name ?? "?") + "." + method.Name;
                if (
                    name.StartsWith("HkEntity.", StringComparison.Ordinal)
                    || name.StartsWith("ForcedSleepPatch", StringComparison.Ordinal)
                    || name.Contains("DMD<")
                )
                    continue;
                frames.Add(name);
            }
            caller = frames.Count == 0 ? "?" : string.Join("<", frames);
        }
        m_heldActivations.AddOrUpdate(caller, 1, (_, n) => n + 1);
        return true;
    }

    /// <summary>Slept grid id -> its sleep record. Read by the patches from any thread.</summary>
    private static readonly ConcurrentDictionary<long, Slept> m_slept =
        new ConcurrentDictionary<long, Slept>();

    /// <summary>Grid id -> first tick it was seen still (continuously).</summary>
    private static readonly Dictionary<long, long> m_stillSince = new Dictionary<long, long>();

    /// <summary>Group key -> tick before which it is not queried again (unsupported last time).</summary>
    private static readonly Dictionary<long, long> m_retryAfter = new Dictionary<long, long>();
    private static readonly HashSet<long> m_seen = new HashSet<long>();
    private static readonly List<HkBodyCollision> m_hits = new List<HkBodyCollision>();
    private static readonly List<MyCubeGrid> m_members = new List<MyCubeGrid>();
    private static readonly List<long> m_drop = new List<long>();

    private static long m_tick;
    private static long m_sleeps,
        m_wakeCollision,
        m_wakeControl,
        m_wakeStructure,
        m_wakeGone,
        m_didNotSleep,
        m_unsupported,
        m_queries;
    private static readonly ConcurrentDictionary<string, long> m_controlReasons =
        new ConcurrentDictionary<string, long>();
    private static readonly ConcurrentDictionary<string, long> m_structureReasons =
        new ConcurrentDictionary<string, long>();

    /// <summary>Which query layer found the support ("own" = the grid body's layer).</summary>
    private static readonly ConcurrentDictionary<string, long> m_supportByLayer =
        new ConcurrentDictionary<string, long>();

    private static bool Frozen(long id) => isFrozen?.Invoke(id) == true;

    public static void ReleaseForTransfer(long id)
    {
        if (m_slept.TryGetValue(id, out var record))
            Release(record.Group);
    }

    public static void Stop() => Stop(wake: false);

    private static void Stop(bool wake)
    {
        if (!running)
            return;
        running = false;
        MySession.OnUnloading -= Stop;
        AppDomain.CurrentDomain.SetData("Performance.ForcedSleep.ReleaseForTransfer", null);
        foreach (var record in m_slept.Values)
        foreach (var grid in Release(record.Group))
            if (wake && grid.Physics?.Enabled == true && !Frozen(grid.EntityId))
                grid.ActivatePhysics();
        m_rayHits.Clear();
        m_islandOfBody.Clear();
        m_stillSince.Clear();
        m_retryAfter.Clear();
        m_members.Clear();
        m_hits.Clear();
        m_seen.Clear();
        m_drop.Clear();
        isFrozen = null;
    }

    /// <summary>True while the grid is held asleep; the patches skip the game's wake calls for it.</summary>
    public static bool IsSlept(MyCubeGrid grid) =>
        grid != null && !m_slept.IsEmpty && m_slept.ContainsKey(grid.EntityId);

    /// <summary>A control changed on this grid: release its whole sleeping group and wake it.</summary>
    public static void Wake(MyCubeGrid grid, string reason) =>
        WakeGroup(grid, reason, ref m_wakeControl, m_controlReasons);

    /// <summary>A block of this grid was added, removed, damaged or changed integrity, or the grid split:
    /// release its whole sleeping group and wake it.</summary>
    private static void WakeStructure(MyCubeGrid grid, string reason) =>
        WakeGroup(grid, reason, ref m_wakeStructure, m_structureReasons);

    private static void WakeGroup(
        MyCubeGrid grid,
        string reason,
        ref long counter,
        ConcurrentDictionary<string, long> reasons
    )
    {
        if (
            grid == null
            || m_slept.IsEmpty
            || !m_slept.TryGetValue(grid.EntityId, out Slept record)
        )
            return;
        Interlocked.Increment(ref counter);
        reasons.AddOrUpdate(reason, 1, (_, n) => n + 1);
        foreach (MyCubeGrid member in Release(record.Group))
            member.ActivatePhysics();
    }

    private static List<MyCubeGrid> Release(long group)
    {
        var released = new List<MyCubeGrid>();
        foreach (KeyValuePair<long, Slept> entry in m_slept)
        {
            if (entry.Value.Group != group || !m_slept.TryRemove(entry.Key, out Slept removed))
                continue;
            removed.Unsubscribe();
            if (removed.Bodies != null)
                foreach (HkEntity body in removed.Bodies)
                    m_sleptBodies.TryRemove(body, out _);
            if (MyEntities.GetEntityById(entry.Key) is MyCubeGrid grid && !grid.MarkedForClose)
                released.Add(grid);
        }
        return released;
    }

    public static void Update(Config.PerformanceConfig config)
    {
        var domain = AppDomain.CurrentDomain;
        isFrozen = domain.GetData("CometWorks.Cluster.IsFrozen") as Func<long, bool>;
        // Older cluster versions still own their sleep service. Wait for the transfer guard
        // on a cluster node; a standalone server needs no guard.
        if (
            config?.Enabled != true
            || !config.SleepSupportedGrids
            || !Sync.IsServer
            || MySession.Static?.Ready != true
            || (PluginSdk.Clustering.PluginCluster.IsClusterProcess && isFrozen == null)
        )
        {
            Stop(wake: MySession.Static?.Ready == true);
            return;
        }
        if (!running)
        {
            running = true;
            MySession.OnUnloading += Stop;
            domain.SetData(
                "Performance.ForcedSleep.ReleaseForTransfer",
                (Action<long>)ReleaseForTransfer
            );
        }
        m_linear = config.SleepLinearSpeed;
        m_angular = config.SleepAngularSpeed;
        m_holdSeconds = config.SleepHoldSeconds;
        m_tick++;
        CheckSlept();
        FindCandidates();
        if (m_tick % ReportEveryTicks == 0)
            Report();
    }

    /// <summary>Releases groups Havok woke (collision), that changed shape, or that left the node, turned
    /// static (converting back to dynamic must not find ActivatePhysics held shut) or went into a handover.</summary>
    private static void CheckSlept()
    {
        if (m_slept.IsEmpty)
            return;
        m_drop.Clear();
        foreach (KeyValuePair<long, Slept> entry in m_slept)
        {
            MyCubeGrid grid = MyEntities.GetEntityById(entry.Key) as MyCubeGrid;
            if (
                grid == null
                || grid.MarkedForClose
                || grid.IsStatic
                || grid.Physics == null
                || !grid.Physics.Enabled
                || Frozen(grid.EntityId)
            )
            {
                m_wakeGone++;
                m_drop.Add(entry.Value.Group);
            }
            else if (grid.BlocksCount != entry.Value.Blocks)
            {
                // Backstop for a change no subscribed event reported.
                Interlocked.Increment(ref m_wakeStructure);
                m_structureReasons.AddOrUpdate("block-count", 1, (_, n) => n + 1);
                m_drop.Add(entry.Value.Group);
            }
            else if (grid.Physics.IsActive)
            {
                // Right after the sleep call it is our failure, later it is Havok waking it.
                if (m_tick - entry.Value.SleptTick <= 1)
                {
                    // Back off: a group Havok would not deactivate must not be re-queried every 2 s.
                    m_didNotSleep++;
                    m_retryAfter[entry.Value.Group] = m_tick + UnsupportedRetryTicks;
                    DiagnoseDidNotSleep(grid, entry.Value.Group);
                }
                else
                    m_wakeCollision++;
                m_drop.Add(entry.Value.Group);
            }
        }
        foreach (long group in m_drop)
            Release(group);
    }

    private static void FindCandidates()
    {
        long holdTicks = Math.Max(1, (long)Math.Ceiling(m_holdSeconds * TicksPerSecond));
        float linearSq = m_linear * m_linear,
            angularSq = m_angular * m_angular;
        m_seen.Clear();
        int queries = 0;
        foreach (MyEntity entity in MyEntities.GetEntities())
        {
            if (
                entity is not MyCubeGrid grid
                || grid.IsStatic
                || grid.MarkedForClose
                || grid.Physics == null
            )
                continue;
            m_seen.Add(grid.EntityId);
            bool still =
                grid.Physics.Enabled
                && grid.Physics.IsActive
                && grid.Physics.LinearVelocity.LengthSquared() < linearSq
                && grid.Physics.AngularVelocity.LengthSquared() < angularSq;
            if (!still || m_slept.ContainsKey(grid.EntityId))
            {
                m_stillSince.Remove(grid.EntityId);
                continue;
            }
            if (!m_stillSince.TryGetValue(grid.EntityId, out long since))
                m_stillSince[grid.EntityId] = since = m_tick;
            if (m_tick - since < holdTicks || queries >= QueriesPerTick)
                continue;

            if (!CollectGroup(grid, holdTicks, out long key, out bool hasStatic))
                continue;
            if (m_retryAfter.TryGetValue(key, out long retry) && m_tick < retry)
                continue;
            bool supported = hasStatic;
            // A started group is queried to the end: stopping at the budget mid-group used to count it
            // unsupported with members never asked, and park it for UnsupportedRetryTicks.
            for (int i = 0; !supported && i < m_members.Count; i++)
            {
                queries++;
                supported = Touches(m_members[i], key);
            }
            if (!supported)
            {
                DiagnoseUnsupported(grid);
                m_unsupported++;
                m_retryAfter[key] = m_tick + UnsupportedRetryTicks;
                continue;
            }
            if (!ExtendToIsland(holdTicks, out string refusal, out long readyInTicks))
            {
                m_islandRefusals.AddOrUpdate(refusal, 1, (_, n) => n + 1);
                // A neighbour that is already still only needs to finish its hold: retry when it can have (61fbda6
                // 09:4x: the first 20 island_blocked_by lines were settling neighbours at still_ticks 4-6 of 8, and
                // each refusal parked the group for the full 10 s). Anything else waits the usual retry.
                m_retryAfter[key] =
                    m_tick + (readyInTicks > 0 ? readyInTicks : UnsupportedRetryTicks);
                continue;
            }
            m_retryAfter.Remove(key);
            Sleep(key);
        }
        m_queries += queries;
        // Forget grids that left the node or closed.
        if (m_stillSince.Count > m_seen.Count)
        {
            m_drop.Clear();
            foreach (long id in m_stillSince.Keys)
                if (!m_seen.Contains(id))
                    m_drop.Add(id);
            foreach (long id in m_drop)
                m_stillSince.Remove(id);
        }
        if (m_retryAfter.Count > 4096)
            m_retryAfter.Clear();
    }

    /// <summary>
    /// Fills <see cref="m_members"/> with the non-static grids of the physical group when all of them
    /// qualify: still for the hold time, physics on, not already asleep, not mid-handover.
    /// </summary>
    private static bool CollectGroup(
        MyCubeGrid grid,
        long holdTicks,
        out long key,
        out bool hasStatic
    )
    {
        m_members.Clear();
        key = grid.EntityId;
        hasStatic = false;
        MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group group =
            MyCubeGridGroups.Static?.Physical.GetGroup(grid);
        if (group == null)
        {
            m_members.Add(grid);
        }
        else
        {
            foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node node in group.Nodes)
            {
                MyCubeGrid member = node.NodeData;
                if (member == null || member.MarkedForClose)
                    return false;
                if (member.IsStatic)
                {
                    hasStatic = true;
                    continue;
                }
                m_members.Add(member);
                if (member.EntityId < key)
                    key = member.EntityId;
            }
        }
        foreach (MyCubeGrid member in m_members)
        {
            // A transfer freeze disables physics, so ask about the freeze first: a group with a member in
            // transit must not sleep. Any other member without enabled physics is skipped, as before.
            if (Frozen(member.EntityId) || m_slept.ContainsKey(member.EntityId))
                return false;
            if (member.Physics == null || !member.Physics.Enabled)
                continue;
            if (
                !m_stillSince.TryGetValue(member.EntityId, out long since)
                || m_tick - since < holdTicks
            )
                return false;
        }
        return m_members.Count > 0;
    }

    /// <summary>
    /// True when the grid's collision shape, where it stands, overlaps (within Havok's collision
    /// tolerance, i.e. resting contact) voxel, a static grid, or a sleeping non-static grid outside
    /// its own group.
    /// </summary>
    private static bool Touches(MyCubeGrid grid, long groupKey)
    {
        HkRigidBody body = grid.Physics?.RigidBody;
        if (body == null)
            return false;
        MatrixD world = grid.WorldMatrix;
        Vector3D translation = world.Translation;
        Quaternion rotation = Quaternion.CreateFromRotationMatrix(world);
        m_hits.Clear();
        try
        {
            MyPhysics.GetPenetrationsShape(
                body.GetShape(),
                ref translation,
                ref rotation,
                m_hits,
                body.Layer
            );
        }
        catch (Exception e)
        {
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep query failed grid="
                    + grid.EntityId
                    + ": "
                    + e.GetType().Name
                    + ": "
                    + e.Message
            );
            return false;
        }
        if (Supports(grid))
        {
            m_supportByLayer.AddOrUpdate("own", 1, (_, n) => n + 1);
            return true;
        }
        // The penetration query never returns VOXEL for a grid shape, on any layer: 7063c30 (2026-09-26
        // 06:5x-07:0x, P517 loaded) asked the body's layer, 15 and 30 - every sleep came from 'own' (grid
        // support) and 41 unsupported_in_gravity lines on node-2/3 still had voxel 0.34 m to -0.84 m below
        // (e41604a: the same with the body layer alone). A ray does see voxel, so ground support in
        // gravity is measured along gravity instead.
        if (GroundBelow(grid))
        {
            m_supportByLayer.AddOrUpdate("ray", 1, (_, n) => n + 1);
            return true;
        }
        return false;
    }

    /// <summary>Resting contact tolerance for <see cref="GroundBelow"/>: the grid's box bottom may be this far above
    /// the hit (a box is never tighter than the blocks, so resting grids read 0 or below; a hover reads metres).</summary>
    private const double GroundContactMeters = 0.3;

    /// <summary>
    /// In natural gravity: the first foreign hit straight down from the centre of the grid's box is voxel, a
    /// static grid or a sleeping non-member grid, no more than <see cref="GroundContactMeters"/> below the box.
    /// </summary>
    private static bool GroundBelow(MyCubeGrid grid)
    {
        Vector3D center = grid.PositionComp.WorldAABB.Center;
        Vector3 gravity =
            Sandbox.Game.GameSystems.MyGravityProviderSystem.CalculateNaturalGravityInPoint(center);
        if (gravity.LengthSquared() < 0.01f)
            return false;
        Vector3D down = Vector3D.Normalize((Vector3D)gravity);
        MatrixD world = grid.WorldMatrix;
        Vector3 half = grid.PositionComp.LocalAABB.HalfExtents;
        double extent =
            Math.Abs(half.X * Vector3D.Dot(world.Right, down))
            + Math.Abs(half.Y * Vector3D.Dot(world.Up, down))
            + Math.Abs(half.Z * Vector3D.Dot(world.Backward, down));
        Vector3D from = Vector3D.Transform(grid.PositionComp.LocalAABB.Center, world);
        m_rayHits.Clear();
        MyPhysics.CastRay(from, from + down * (extent + GroundContactMeters + 0.5), m_rayHits);
        foreach (MyPhysics.HitInfo hit in m_rayHits)
        {
            IMyEntity entity = hit.HkHitInfo.GetHitEntity();
            if (entity == null || entity.GetTopMostParent() == grid)
                continue;
            if (entity.GetTopMostParent() is MyCubeGrid other && m_members.Contains(other))
                continue;
            if (Vector3D.Distance(from, hit.Position) - extent > GroundContactMeters)
                return false;
            return entity is MyVoxelBase
                || entity.GetTopMostParent() is MyCubeGrid hitGrid
                    && (
                        hitGrid.IsStatic
                        || m_slept.ContainsKey(hitGrid.EntityId)
                        || hitGrid.Physics != null && !hitGrid.Physics.IsActive
                    );
        }
        return false;
    }

    /// <summary>Voxel, a static grid, or a sleeping non-static grid outside the group among <see cref="m_hits"/>.</summary>
    private static bool Supports(MyCubeGrid grid)
    {
        foreach (HkBodyCollision hit in m_hits)
        {
            IMyEntity other = hit.GetCollisionEntity();
            if (other == null || other == grid)
                continue;
            if (other is MyVoxelBase)
                return true;
            if (other.GetTopMostParent() is not MyCubeGrid otherGrid || otherGrid == grid)
                continue;
            if (otherGrid.IsStatic)
                return true;
            if (m_members.Contains(otherGrid))
                continue;
            if (
                m_slept.ContainsKey(otherGrid.EntityId)
                || otherGrid.Physics != null && !otherGrid.Physics.IsActive
            )
                return true;
        }
        return false;
    }

    /// <summary>Unsupported groups explained so far, and the tick after which the next one may be (budget).</summary>
    private static int m_diagnosed;
    private static long m_nextDiagnosisTick;
    private static readonly List<MyPhysics.HitInfo> m_rayHits = new List<MyPhysics.HitInfo>();

    /// <summary>
    /// Instrument for the Touches() zero (c1cb94b: unsupported_total 308/313/0/68/161 and sleeps_total=0 on
    /// every node over ~70 min). A still grid IN GRAVITY that nothing supports can only be hovering, so for
    /// those this logs what the penetration query returned and the ground clearance along gravity: a
    /// clearance near 0 m with no voxel/static hit means Touches misses resting contact. The first 20 per
    /// process, then one per 5 minutes.
    /// </summary>
    private static void DiagnoseUnsupported(MyCubeGrid grid)
    {
        if (m_diagnosed >= 20 && m_tick < m_nextDiagnosisTick)
            return;
        try
        {
            Vector3D center = grid.PositionComp.WorldAABB.Center;
            Vector3 gravity =
                Sandbox.Game.GameSystems.MyGravityProviderSystem.CalculateNaturalGravityInPoint(
                    center
                );
            float g = gravity.Length();
            if (g < 0.1f)
                return;
            m_diagnosed++;
            m_nextDiagnosisTick = m_tick + 5 * 60 * TicksPerSecond;
            // m_hits still holds the last Touches() query: the group's last member, on the LAST layer tried.
            var kinds = new SortedDictionary<string, int>();
            foreach (HkBodyCollision hit in m_hits)
            {
                IMyEntity other = hit.GetCollisionEntity();
                string kind =
                    other == null ? "null"
                    : other is MyVoxelBase ? "voxel"
                    : other.GetTopMostParent() is MyCubeGrid otherGrid
                        ? otherGrid == grid ? "self"
                            : otherGrid.IsStatic ? "static"
                            : m_members.Contains(otherGrid) ? "member"
                            : otherGrid.Physics != null && !otherGrid.Physics.IsActive
                                ? "grid-asleep"
                            : "grid-active"
                    : other.GetType().Name;
                kinds[kind] = kinds.TryGetValue(kind, out int n) ? n + 1 : 1;
            }
            // Extent of the grid's box along gravity, then the first foreign hit straight down from its centre.
            Vector3D down = Vector3D.Normalize((Vector3D)gravity);
            MatrixD world = grid.WorldMatrix;
            Vector3 half = grid.PositionComp.LocalAABB.HalfExtents;
            double extent =
                Math.Abs(half.X * Vector3D.Dot(world.Right, down))
                + Math.Abs(half.Y * Vector3D.Dot(world.Up, down))
                + Math.Abs(half.Z * Vector3D.Dot(world.Backward, down));
            Vector3D localCenter = Vector3D.Transform(grid.PositionComp.LocalAABB.Center, world);
            m_rayHits.Clear();
            MyPhysics.CastRay(localCenter, localCenter + down * (extent + 20), m_rayHits);
            string ground =
                "none within " + (extent + 20).ToString("0.0", CultureInfo.InvariantCulture) + "m";
            foreach (MyPhysics.HitInfo hit in m_rayHits)
            {
                IMyEntity entity = hit.HkHitInfo.GetHitEntity();
                if (entity == null || entity.GetTopMostParent() == grid)
                    continue;
                string kind =
                    entity is MyVoxelBase ? "voxel"
                    : entity.GetTopMostParent() is MyCubeGrid hitGrid
                        ? hitGrid.IsStatic ? "static"
                            : "grid"
                    : entity.GetType().Name;
                double distance = Vector3D.Distance(localCenter, hit.Position);
                ground =
                    kind
                    + " clearance="
                    + (distance - extent).ToString("0.00", CultureInfo.InvariantCulture)
                    + "m";
                break;
            }
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep unsupported_in_gravity grid="
                    + grid.EntityId
                    + " name='"
                    + grid.DisplayName
                    + "' blocks="
                    + grid.BlocksCount
                    + " members="
                    + m_members.Count
                    + " g="
                    + g.ToString("0.00", CultureInfo.InvariantCulture)
                    + " penetrations="
                    + (
                        kinds.Count == 0
                            ? "-"
                            : string.Join(",", kinds.Select(pair => pair.Key + ":" + pair.Value))
                    )
                    + " ground="
                    + ground
            );
        }
        catch (Exception e)
        {
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep diagnosis failed grid="
                    + grid.EntityId
                    + ": "
                    + e.GetType().Name
                    + ": "
                    + e.Message
            );
        }
    }

    /// <summary>An island above this many bodies is not put to sleep (cost of the listing, and a pile that big is rarely all still).</summary>
    private const int MaxIslandBodies = 512;
    private static readonly ConcurrentDictionary<string, long> m_islandRefusals =
        new ConcurrentDictionary<string, long>();
    private static long m_islandGrids;
    private static long m_islandsTick = -1;

    /// <summary>This tick: body -> the bodies of the island that holds it (each island listed once per tick).</summary>
    private static readonly Dictionary<HkEntity, List<HkRigidBody>> m_islandOfBody = new Dictionary<
        HkEntity,
        List<HkRigidBody>
    >(ByReference.Instance);

    /// <summary>
    /// The bodies of the island that holds <paramref name="body"/>, or null when it has none. Read through the
    /// island itself (HkSimulationIsland_GetEntityCount/GetEntity; vanilla's HkEntity.IsActive reads the same
    /// island): HkWorld.GetActiveSimulationIslandRigidBodies imports HkWorld_GetActiveSimulationIslandEntities,
    /// which Havok.dll does not export (it exports ...IslandRigidBodies). 3544695 called it and every node
    /// that tried aborted natively (SIGABRT "Failed to load function", 2026-09-26 07:50); no managed catch
    /// can stop that, so no HkWorld island listing may be used here.
    /// </summary>
    private static List<HkRigidBody> IslandOf(HkRigidBody body)
    {
        if (m_islandsTick != m_tick)
        {
            m_islandOfBody.Clear();
            m_islandsTick = m_tick;
        }
        if (m_islandOfBody.TryGetValue(body, out List<HkRigidBody> known))
            return known;
        HkSimulationIslandRef island = body.GetSimulationIsland();
        if (!island.IsValid)
            return null;
        int count = island.GetEntityCount();
        var bodies = new List<HkRigidBody>(Math.Min(count, MaxIslandBodies + 1));
        for (int i = 0; i < count && bodies.Count <= MaxIslandBodies; i++)
        {
            HkRigidBody member = island.GetEntity(i);
            if (member != null)
                bodies.Add(member);
        }
        foreach (HkRigidBody member in bodies)
            m_islandOfBody[member] = bodies;
        m_islandOfBody[body] = bodies;
        return bodies;
    }

    /// <summary>
    /// Havok sleeps an island, not a group: 618b165 at 07:22 (P517 on node-1) made 157 sleeps and 346
    /// did_not_sleep, and the unsupported lines were grids lying on ACTIVE grids - wreck piles. A pile member
    /// outside the group is not held (its ActivatePhysics goes through) and wakes the whole island at once.
    /// So the group sleeps only when every other body in its active island is still too: still non-static
    /// grids (for the hold time, not frozen, not already asleep) join <see cref="m_members"/> and sleep and
    /// wake with the group; a still floating object or other body is deactivated with the island; a character
    /// or anything moving refuses. Fills m_members further; false with the reason on a refusal.
    /// </summary>
    private static bool ExtendToIsland(long holdTicks, out string refusal, out long readyInTicks)
    {
        refusal = null;
        readyInTicks = 0;
        float linearSq = m_linear * m_linear;
        var visited = new HashSet<List<HkRigidBody>>(ByReference.Instance);
        int before = m_members.Count;
        try
        {
            for (int m = 0; m < m_members.Count; m++)
            {
                MyPhysicsBody physics = m_members[m].Physics;
                if (physics?.RigidBody == null || physics.HavokWorld == null)
                    continue;
                List<HkRigidBody> island = IslandOf(physics.RigidBody);
                if (island == null || !visited.Add(island))
                    continue;
                if (island.Count > MaxIslandBodies)
                {
                    refusal = "too-big";
                    return false;
                }
                foreach (HkRigidBody other in island)
                {
                    IMyEntity entity = other.GetEntity(0)?.GetTopMostParent();
                    if (entity is MyCubeGrid grid)
                    {
                        if (grid.IsStatic || m_members.Contains(grid))
                            continue;
                        if (
                            grid.MarkedForClose
                            || grid.Physics == null
                            || Frozen(grid.EntityId)
                            || m_slept.ContainsKey(grid.EntityId)
                        )
                        {
                            refusal = "grid-busy";
                            return false;
                        }
                        if (
                            !m_stillSince.TryGetValue(grid.EntityId, out long since)
                            || m_tick - since < holdTicks
                        )
                        {
                            refusal = "grid-moving";
                            long stillTicks = m_stillSince.TryGetValue(grid.EntityId, out long seen)
                                ? m_tick - seen
                                : -1;
                            if (stillTicks >= 0)
                                readyInTicks = holdTicks - stillTicks;
                            DiagnoseMovingNeighbour(grid, stillTicks, island.Count);
                            return false;
                        }
                        m_members.Add(grid);
                    }
                    else if (entity is Sandbox.Game.Entities.Character.MyCharacter)
                    {
                        refusal = "character";
                        return false;
                    }
                    else if (other.LinearVelocity.LengthSquared() >= linearSq)
                    {
                        refusal = (entity?.GetType().Name ?? "null") + "-moving";
                        return false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            refusal = "error-" + e.GetType().Name;
            return false;
        }
        m_islandGrids += m_members.Count - before;
        return true;
    }

    private static int m_movingDiagnosed;
    private static long m_nextMovingDiagnosisTick;

    /// <summary>
    /// Instrument for the island coverage gap (815fb98 2026-09-26: node-1 slept its first Earth-base pile in under a
    /// minute and the second only ~20 min later, with island_refusals grid-moving:1107 in between). Logs the grid
    /// that blocked an island: its speeds against the thresholds, how long it has been still (-1 = not still now),
    /// whether it is controlled, dampened or thrusting. The first 20 per process, then one per 5 minutes.
    /// </summary>
    private static void DiagnoseMovingNeighbour(MyCubeGrid grid, long stillTicks, int islandBodies)
    {
        if (m_movingDiagnosed >= 20 && m_tick < m_nextMovingDiagnosisTick)
            return;
        m_movingDiagnosed++;
        m_nextMovingDiagnosisTick = m_tick + 5 * 60 * TicksPerSecond;
        try
        {
            MyPhysicsBody physics = grid.Physics;
            var thrust = grid.Components.Get<Sandbox.Game.GameSystems.MyEntityThrustComponent>();
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep island_blocked_by grid="
                    + grid.EntityId
                    + " name='"
                    + grid.DisplayName
                    + "' blocks="
                    + grid.BlocksCount
                    + " active="
                    + (physics?.IsActive == true)
                    + " v="
                    + (physics?.LinearVelocity.Length() ?? 0).ToString(
                        "0.000",
                        CultureInfo.InvariantCulture
                    )
                    + " w="
                    + (physics?.AngularVelocity.Length() ?? 0).ToString(
                        "0.000",
                        CultureInfo.InvariantCulture
                    )
                    + " still_ticks="
                    + stillTicks
                    + " island="
                    + islandBodies
                    + " controlled="
                    + (grid.GridSystems?.ControlSystem?.IsControlled == true)
                    + " dampeners="
                    + (thrust?.DampenersEnabled == true)
                    + " thrust="
                    + (thrust?.FinalThrust.Length() ?? 0).ToString(
                        "0",
                        CultureInfo.InvariantCulture
                    )
                    + " frozen="
                    + Frozen(grid.EntityId)
            );
        }
        catch (Exception e)
        {
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep blocker diagnosis failed grid="
                    + grid.EntityId
                    + ": "
                    + e.GetType().Name
                    + ": "
                    + e.Message
            );
        }
    }

    private static int m_islandDiagnosed;
    private static long m_nextIslandDiagnosisTick;

    /// <summary>
    /// Instrument for did_not_sleep (7063c30 07:02: node-2 sleeps 354 / did_not_sleep 477, node-3 115 / 169):
    /// Havok re-activated a group within one tick of DeactivateSimulationIslandRigidBodies. Logs the active
    /// island that holds the woken body and every body in it that is not the slept group's, fastest first.
    /// Foreign active bodies = something outside the group (a wreck pile, a character) keeps the island awake;
    /// none = the group woke itself. The first 20 per process, then one per 5 minutes.
    /// </summary>
    private static void DiagnoseDidNotSleep(MyCubeGrid grid, long group)
    {
        if (m_islandDiagnosed >= 20 && m_tick < m_nextIslandDiagnosisTick)
            return;
        m_islandDiagnosed++;
        m_nextIslandDiagnosisTick = m_tick + 5 * 60 * TicksPerSecond;
        try
        {
            HkRigidBody body = grid.Physics.RigidBody;
            if (body == null)
                return;
            List<HkRigidBody> island = IslandOf(body);
            int size = island?.Count ?? -1,
                own = 0;
            var foreign = new List<KeyValuePair<float, string>>();
            foreach (HkRigidBody other in island ?? new List<HkRigidBody>())
            {
                IMyEntity entity = other.GetEntity(0)?.GetTopMostParent();
                if (
                    entity is MyCubeGrid otherGrid
                    && m_slept.TryGetValue(otherGrid.EntityId, out Slept record)
                    && record.Group == group
                )
                {
                    own++;
                    continue;
                }
                string kind =
                    entity == null ? "null"
                    : entity is MyCubeGrid slept && m_slept.ContainsKey(slept.EntityId)
                        ? "grid-slept"
                    : entity is MyCubeGrid ? "grid"
                    : entity.GetType().Name;
                float speed = other.LinearVelocity.Length();
                foreign.Add(
                    new KeyValuePair<float, string>(
                        speed,
                        kind
                            + ":"
                            + (entity?.EntityId ?? 0)
                            + ":"
                            + speed.ToString("0.00", CultureInfo.InvariantCulture)
                            + (other.IsActive ? "" : ":inactive")
                    )
                );
            }
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep did_not_sleep_island grid="
                    + grid.EntityId
                    + " name='"
                    + grid.DisplayName
                    + "' blocks="
                    + grid.BlocksCount
                    + " speed="
                    + grid.Physics.LinearVelocity.Length()
                        .ToString("0.00", CultureInfo.InvariantCulture)
                    + " island="
                    + (size < 0 ? "none" : size.ToString())
                    + " group_bodies="
                    + own
                    + " foreign="
                    + foreign.Count
                    + " "
                    + (
                        foreign.Count == 0
                            ? "-"
                            : string.Join(
                                ",",
                                foreign
                                    .OrderByDescending(pair => pair.Key)
                                    .Take(8)
                                    .Select(pair => pair.Value)
                            )
                    )
            );
        }
        catch (Exception e)
        {
            Plugin.Instance.Log.Info(
                "[Performance] forced_sleep island diagnosis failed grid="
                    + grid.EntityId
                    + ": "
                    + e.GetType().Name
                    + ": "
                    + e.Message
            );
        }
    }

    private static void Sleep(long key)
    {
        // Zero the drift first (setting a velocity activates the body), then mark, then deactivate.
        foreach (MyCubeGrid member in m_members)
        {
            MyPhysicsBody physics = member.Physics;
            if (physics?.RigidBody == null)
                continue;
            physics.LinearVelocity = Vector3.Zero;
            physics.AngularVelocity = Vector3.Zero;
        }
        foreach (MyCubeGrid member in m_members)
        {
            HkEntity[] bodies =
                member.Physics?.RigidBody2 != null
                    ? new HkEntity[] { member.Physics.RigidBody, member.Physics.RigidBody2 }
                : member.Physics?.RigidBody != null ? new HkEntity[] { member.Physics.RigidBody }
                : Array.Empty<HkEntity>();
            MyCubeGrid grid = member;
            var record = new Slept
            {
                Group = key,
                Blocks = member.BlocksCount,
                SleptTick = m_tick,
                Bodies = bodies,
                Grid = grid,
                OnAdded = _ => WakeStructure(grid, "block-added"),
                OnRemoved = _ => WakeStructure(grid, "block-removed"),
                OnIntegrity = _ => WakeStructure(grid, "integrity"),
                OnDamaged = (_, _, _, _) => WakeStructure(grid, "damage"),
                OnSplit = (_, _) => WakeStructure(grid, "split"),
            };
            m_slept[member.EntityId] = record;
            record.Subscribe();
            foreach (HkEntity body in bodies)
                m_sleptBodies[body] = key;
            m_stillSince.Remove(member.EntityId);
        }
        // HkEntity.Deactivate is only hkpEntity::requestDeactivation, which Havok ignores while the
        // island is being pushed (dampeners): on e1ae22a node-1 made 291 sleeps of which 472 grids were
        // awake again 250 ms later. The world call deactivates the island outright.
        foreach (MyCubeGrid member in m_members)
        {
            MyPhysicsBody physics = member.Physics;
            if (physics?.RigidBody == null)
                continue;
            HkWorld world = physics.HavokWorld;
            if (world != null)
                world.DeactivateSimulationIslandRigidBodies(physics.RigidBody);
            else
                physics.RigidBody.Deactivate();
        }
        m_sleeps++;
    }

    private static void Report()
    {
        var groups = new HashSet<long>();
        foreach (Slept record in m_slept.Values)
            groups.Add(record.Group);
        var reasons = new List<string>();
        foreach (KeyValuePair<string, long> entry in m_controlReasons)
            reasons.Add(entry.Key + ":" + entry.Value);
        Plugin.Instance.Log.Info(
            "[Performance] forced_sleep slept_grids="
                + m_slept.Count
                + " slept_groups="
                + groups.Count
                + " sleeps_total="
                + m_sleeps
                + " did_not_sleep_total="
                + m_didNotSleep
                + " wake_collision_total="
                + m_wakeCollision
                + " wake_control_total="
                + Interlocked.Read(ref m_wakeControl)
                + " wake_structure_total="
                + Interlocked.Read(ref m_wakeStructure)
                + " released_gone_total="
                + m_wakeGone
                + " unsupported_total="
                + m_unsupported
                + " queries_total="
                + m_queries
                + " island_grids_total="
                + m_islandGrids
                + " island_refusals="
                + (
                    m_islandRefusals.IsEmpty
                        ? "-"
                        : string.Join(
                            ",",
                            m_islandRefusals.Select(pair => pair.Key + ":" + pair.Value)
                        )
                )
                + " support_by_layer="
                + (
                    m_supportByLayer.IsEmpty
                        ? "-"
                        : string.Join(
                            ",",
                            m_supportByLayer.Select(pair => pair.Key + ":" + pair.Value)
                        )
                )
                + " control_reasons="
                + (reasons.Count == 0 ? "-" : string.Join(",", reasons))
                + " structure_reasons="
                + (
                    m_structureReasons.IsEmpty
                        ? "-"
                        : string.Join(
                            ",",
                            m_structureReasons.Select(pair => pair.Key + ":" + pair.Value)
                        )
                )
                + " held_activations="
                + (
                    m_heldActivations.IsEmpty
                        ? "-"
                        : string.Join(
                            ",",
                            m_heldActivations
                                .OrderByDescending(pair => pair.Value)
                                .Take(8)
                                .Select(pair => pair.Key + ":" + pair.Value)
                        )
                )
        );
        m_heldSamples = 0;
    }
}
