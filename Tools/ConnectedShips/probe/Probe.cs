using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using Havok;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using Sandbox.Game.World;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using VRage.ObjectBuilders;
using VRage.Plugins;
using VRageMath;

namespace DockProbe;

// Test hooks of the connected ships rig. The rig has no game client for most of its
// runs, so it talks to the probe through two files in DOCK_PROBE_DIR: it writes one
// command line to `cmd`, and the probe answers in `out` when the command is done.
// Commands run on the game thread as coroutines, see Run.
public class Probe : IPlugin
{
    private static readonly string Dir = Environment.GetEnvironmentVariable("DOCK_PROBE_DIR") ?? "";
    private static readonly List<double> StepMs = new List<double>();
    private static readonly List<double> TickMs = new List<double>();
    private static readonly Stopwatch Step = new Stopwatch();
    private static readonly Stopwatch Tick = new Stopwatch();

    private readonly Harmony harmony = new Harmony("DockProbe");
    private IEnumerator job;
    private int wait;
    private int ticks;
    private StringBuilder reply;

    public void Init(object gameInstance)
    {
        harmony.Patch(
            AccessTools.Method(typeof(MyPhysics), "StepWorlds"),
            new HarmonyMethod(typeof(Probe), nameof(StepStart)),
            new HarmonyMethod(typeof(Probe), nameof(StepEnd))
        );
    }

    private static void StepStart() => Step.Restart();

    private static void StepEnd()
    {
        Step.Stop();
        StepMs.Add(Step.Elapsed.TotalMilliseconds);
    }

    public void Dispose() { }

    public void Update()
    {
        if (Tick.IsRunning)
            TickMs.Add(Tick.Elapsed.TotalMilliseconds);
        Tick.Restart();

        if (MySession.Static == null || !MySession.Static.Ready || Dir.Length == 0)
            return;

        if (job != null)
        {
            Advance();
            return;
        }

        if (++ticks % 15 != 0)
            return;

        var path = Path.Combine(Dir, "cmd");
        if (!File.Exists(path))
            return;

        var line = File.ReadAllText(path).Trim();
        File.Delete(path);
        reply = new StringBuilder();
        var arguments = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        job = Safe(arguments);
        Advance();
    }

    private void Advance()
    {
        if (wait > 0 && --wait > 0)
            return;
        try
        {
            if (job.MoveNext())
            {
                wait = (int)job.Current;
                return;
            }
        }
        catch (Exception e)
        {
            reply.AppendLine("error=" + e.ToString().Replace('\n', ' '));
        }
        job = null;
        var tmp = Path.Combine(Dir, "out.tmp");
        File.WriteAllText(tmp, reply.ToString());
        File.Move(tmp, Path.Combine(Dir, "out"));
    }

    // Run does part of its commands right away, where Advance can't catch an exception
    private IEnumerator Safe(string[] arguments)
    {
        IEnumerator inner = Run(arguments);
        while (inner.MoveNext())
            yield return inner.Current;
    }

    private void Say(string text) => reply.AppendLine(text);

    private IEnumerator Run(string[] a)
    {
        switch (a[0])
        {
            case "spawn":
                Spawn(
                    a[1],
                    a[2] == "1",
                    I(a[3]),
                    I(a[4]),
                    I(a[5]),
                    a[6] == "1",
                    V(a[7], a[8], a[9])
                );
                return Yield(5);
            case "station":
                Station(a[1], I(a[2]), I(a[3]));
                return Yield(5);
            case "rotor":
                return Rotor(a[1]);
            case "dock":
                return Dock(a[1], I(a[2]), a[3], I(a[4]));
            case "undock":
                return Undock(a[1]);
            case "motion":
                return Motion(a[1]);
            case "stats":
                return Stats(I(a[1]));
            case "state":
                State();
                return Yield(1);
            case "kick":
                Kick(a[1], V(a[2], a[3], a[4]), V(a[5], a[6], a[7]));
                return Yield(1);
            case "trace":
                return Trace(a[1], I(a[2]));
            case "series":
                return Series(a[1], I(a[2]));
            case "dampeners":
                AccessTools
                    .Method(typeof(MyCubeGrid), "EnableDampingInternal")
                    .Invoke(Named(a[1]), new object[] { a[2] == "1", false });
                Say($"dampeners={Named(a[1]).DampenersEnabled}");
                return Yield(1);
            case "gyro":
                return Gyro(a[1], a[2]);
            case "planet":
                var planet = MyWorldGenerator.AddPlanet(
                    a[1] + "-" + DateTime.UtcNow.Ticks,
                    a[1],
                    a[1],
                    V(a[2], a[3], a[4]),
                    12345,
                    120000f,
                    false
                );
                var center = planet.PositionComp.WorldVolume.Center;
                Say(
                    $"x={F(center.X)} y={F(center.Y)} z={F(center.Z)} radius={F(planet.MaximumRadius)}"
                );
                return Yield(60);
            case "pilot":
                Pilot(a[1], a[2] == "1");
                return Yield(30);
            case "fly":
                Fly(a[1], a[2] == "1");
                return Yield(5);
            case "input":
                Input(a[1], V(a[2], a[3], a[4]), V(a[5], a[6], a[7]));
                return Yield(1);
            case "save":
                MySession.Static.Save();
                return Yield(30);
            default:
                throw new ArgumentException(a[0]);
        }
    }

    private static IEnumerator Yield(int ticks)
    {
        yield return ticks;
    }

    private static int I(string s) => int.Parse(s, CultureInfo.InvariantCulture);

    private static double D(string s) => double.Parse(s, CultureInfo.InvariantCulture);

    private static Vector3D V(string x, string y, string z) => new Vector3D(D(x), D(y), D(z));

    private static string F(double v) => v.ToString("0.####", CultureInfo.InvariantCulture);

    private static MyCubeGrid Named(string name) =>
        MyEntities
            .GetEntities()
            .OfType<MyCubeGrid>()
            .Where(g => g.DisplayName == name && !g.MarkedForClose)
            .OrderByDescending(g => g.BlocksCount)
            .First();

    private static List<MyCubeGrid> Group(MyCubeGrid grid) =>
        MyCubeGridGroups.Static.Physical.GetGroup(grid)?.Nodes.Select(n => n.NodeData).ToList()
        ?? new List<MyCubeGrid> { grid };

    // ---- building ----

    private static long owner;

    private static long Owner
    {
        get
        {
            if (owner == 0)
                owner = MySession.Static.Players.CreateNewIdentity("DockProbe").IdentityId;
            return owner;
        }
    }

    private static MyObjectBuilder_CubeBlock Block<T>(
        string subtype,
        int x,
        int y,
        int z,
        Base6Directions.Direction forward = Base6Directions.Direction.Forward,
        Base6Directions.Direction up = Base6Directions.Direction.Up
    )
        where T : MyObjectBuilder_CubeBlock, new()
    {
        var block = MyObjectBuilderSerializer.CreateNewObject<T>(subtype);
        block.Min = new SerializableVector3I(x, y, z);
        block.BlockOrientation = new SerializableBlockOrientation(forward, up);
        block.Owner = Owner;
        block.BuiltBy = Owner;
        block.ShareMode = VRage.Game.MyOwnershipShareModeEnum.All;
        return block;
    }

    private static MyObjectBuilder_CubeGrid Grid(string name, bool isStatic, Vector3D at)
    {
        var grid = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_CubeGrid>();
        grid.DisplayName = name;
        grid.GridSizeEnum = MyCubeSize.Large;
        grid.IsStatic = isStatic;
        grid.PersistentFlags = VRage.ObjectBuilders.MyPersistentEntityFlags2.InScene;
        grid.PositionAndOrientation = new MyPositionAndOrientation(at, Vector3.Forward, Vector3.Up);
        grid.DampenersEnabled = true;
        return grid;
    }

    // A 5x5xlength hull with a connector at each end, a reactor, a cockpit on top, gyros inside
    // and thrust thrusters on each of the six faces
    private void Spawn(
        string name,
        bool isStatic,
        int length,
        int thrust,
        int gyros,
        bool rotor,
        Vector3D at
    )
    {
        var grid = Grid(name, isStatic, at);
        var inner = new HashSet<(int, int, int)>();
        for (int i = 0; i < gyros; i++)
        {
            int z = 2 + i / 3;
            inner.Add((1 + i % 3, 1, z));
            grid.CubeBlocks.Add(Block<MyObjectBuilder_Gyro>("LargeBlockGyro", 1 + i % 3, 1, z));
        }
        inner.Add((2, 2, 1));
        grid.CubeBlocks.Add(Block<MyObjectBuilder_Reactor>("LargeBlockSmallGenerator", 2, 2, 1));
        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
        for (int z = 0; z < length; z++)
            if (!inner.Contains((x, y, z)))
                grid.CubeBlocks.Add(
                    Block<MyObjectBuilder_CubeBlock>("LargeBlockArmorBlock", x, y, z)
                );

        grid.CubeBlocks.Add(
            Block<MyObjectBuilder_ShipConnector>(
                "Connector",
                2,
                2,
                -1,
                Base6Directions.Direction.Forward
            )
        );
        grid.CubeBlocks.Add(
            Block<MyObjectBuilder_ShipConnector>(
                "Connector",
                2,
                2,
                length,
                Base6Directions.Direction.Backward
            )
        );
        grid.CubeBlocks.Add(Block<MyObjectBuilder_Cockpit>("LargeBlockCockpit", 2, 5, 2));
        if (rotor)
            grid.CubeBlocks.Add(
                Block<MyObjectBuilder_MotorAdvancedStator>(
                    "LargeAdvancedStator",
                    2,
                    5,
                    length - 3,
                    Base6Directions.Direction.Forward,
                    Base6Directions.Direction.Up
                )
            );

        // On the sides only: ships meet at their ends, where a block next to the connector would
        // overlap the other ship
        var sides = new List<(int, int, int)>();
        for (int z = 1; z < length - 1; z++)
        for (int u = 0; u < 5; u++)
        {
            sides.Add((-1, u, z));
            sides.Add((5, u, z));
            sides.Add((u, -1, z));
            if (u != 2)
                sides.Add((u, 5, z));
        }
        var directions = Base6Directions.EnumDirections;
        for (int i = 0; i < thrust * directions.Length; i++)
        {
            var forward = directions[i % directions.Length];
            var up = Base6Directions.GetPerpendicular(forward);
            var (x, y, z) = sides[i];
            grid.CubeBlocks.Add(
                Block<MyObjectBuilder_Thrust>("LargeBlockSmallThrust", x, y, z, forward, up)
            );
        }

        MyEntities.CreateFromObjectBuilderAndAdd(grid, false);
    }

    // A static 3x3 beam with a connector on its +x side every `spacing` cells
    private void Station(string name, int connectors, int spacing)
    {
        var grid = Grid(name, true, Vector3D.Zero);
        var length = connectors * spacing;
        for (int x = 0; x < 3; x++)
        for (int y = 0; y < 3; y++)
        for (int z = 0; z < length; z++)
            grid.CubeBlocks.Add(Block<MyObjectBuilder_CubeBlock>("LargeBlockArmorBlock", x, y, z));
        grid.CubeBlocks.Add(Block<MyObjectBuilder_Reactor>("LargeBlockSmallGenerator", 1, -1, 0));
        for (int i = 0; i < connectors; i++)
            grid.CubeBlocks.Add(
                Block<MyObjectBuilder_ShipConnector>(
                    "Connector",
                    3,
                    1,
                    i * spacing + spacing / 2,
                    Base6Directions.Direction.Right
                )
            );
        MyEntities.CreateFromObjectBuilderAndAdd(grid, false);
    }

    private IEnumerator Rotor(string name)
    {
        var stator = Named(name).GetFatBlocks().OfType<MyMotorStator>().First();
        AccessTools
            .Method(typeof(MyMechanicalConnectionBlockBase), "RecreateTop")
            .Invoke(
                stator,
                new object[] { null, MyMechanicalConnectionBlockBase.MyTopBlockSize.Normal, true }
            );
        yield return 30;
        ((Sandbox.ModAPI.IMyMotorStator)stator).TargetVelocityRPM = 10;
        ((Sandbox.ModAPI.IMyMotorStator)stator).Enabled = true;
        Say($"top={stator.TopGrid?.EntityId ?? 0}");
    }

    private static List<MyShipConnector> Connectors(MyCubeGrid grid) =>
        grid.GetFatBlocks().OfType<MyShipConnector>().OrderBy(c => c.Position.Z).ToList();

    // Moves grid b so that its connector ib meets connector ia of grid a, then locks them
    private IEnumerator Dock(string a, int ia, string b, int ib)
    {
        var fixedGrid = Named(a);
        var movedGrid = Named(b);
        var ca = Connectors(fixedGrid)[ia];
        var cb = Connectors(movedGrid)[ib];

        var worldA = ca.WorldMatrix;
        var localB = cb.PositionComp.LocalMatrixRef;
        var target = MatrixD.CreateWorld(Vector3D.Zero, -worldA.Forward, worldA.Up);
        var rotation =
            MatrixD.Invert(MatrixD.CreateWorld(Vector3D.Zero, localB.Forward, localB.Up)) * target;
        // Turned where it is first: it must not pass through anything on its way
        rotation.Translation = movedGrid.WorldMatrix.Translation;

        movedGrid.WorldMatrix = rotation;
        yield return 2;
        // Connectors lock within 0.6 m. A small gap keeps the two blocks from being pushed apart.
        var placed = rotation;
        placed.Translation +=
            ca.ConstraintPositionWorld() - cb.ConstraintPositionWorld() + worldA.Forward * 0.1;

        // The connector's own search for a partner runs every few seconds, and only if the ship
        // moves into its detector. Name the partner and it's a matter of frames.
        var attach = AccessTools.Method(typeof(MyShipConnector), "TryAttach");
        var api = (Sandbox.ModAPI.IMyShipConnector)cb;
        for (int i = 0; i < 100 && !cb.Connected; i++)
        {
            if (!cb.InConstraint)
            {
                movedGrid.WorldMatrix = placed;
                movedGrid.Physics.LinearVelocity = Vector3.Zero;
                movedGrid.Physics.AngularVelocity = Vector3.Zero;
                attach.Invoke(cb, new object[] { (long?)ca.EntityId });
            }
            api.Connect();
            yield return 5;
        }
        Say(
            $"connected={cb.Connected} status={api.Status} working={ca.IsWorking},{cb.IsWorking} gap={F((ca.ConstraintPositionWorld() - cb.ConstraintPositionWorld()).Length())}"
        );
    }

    private IEnumerator Undock(string name)
    {
        foreach (var connector in Connectors(Named(name)).Where(c => c.Connected))
            ((Sandbox.ModAPI.IMyShipConnector)connector).Disconnect();
        yield return 30;
        Say("undocked");
    }

    // ---- SE-797: what the game's own conversion of rooted grids saves ----

    // `dynamic` undoes MyCubeGrid.CheckEarlyDeactivation on every rooted ship, `fixed` is what
    // the game does. Each keeps the game's own constraint handling through RaisePhysicsChanged.
    private IEnumerator Motion(string mode)
    {
        var raise = AccessTools.Method(typeof(MyEntity), "RaisePhysicsChanged");
        int changed = 0;
        foreach (var grid in MyEntities.GetEntities().OfType<MyCubeGrid>().ToList())
        {
            if (grid.IsStatic || grid.Physics == null || !grid.IsMarkedForEarlyDeactivation)
                continue;
            if (mode == "dynamic" && grid.Physics.IsStatic)
            {
                grid.Physics.ConvertToDynamic(grid.GridSizeEnum == MyCubeSize.Large, false);
                raise.Invoke(grid, null);
                changed++;
            }
            else if (mode == "fixed" && !grid.Physics.IsStatic)
            {
                grid.Physics.ConvertToStatic();
                raise.Invoke(grid, null);
                changed++;
            }
        }
        yield return 30;
        Say($"changed={changed}");
    }

    private static double Percentile(List<double> sorted, double p) =>
        sorted.Count == 0 ? 0 : sorted[Math.Min(sorted.Count - 1, (int)(sorted.Count * p))];

    private IEnumerator Stats(int count)
    {
        StepMs.Clear();
        TickMs.Clear();
        int active = 0,
            islands = 0,
            samples = 0;
        for (int i = 0; i < count; i += 30)
        {
            yield return 30;
            foreach (HkWorld world in MyPhysics.GetClusterList() ?? default)
            {
                active += world.ActiveRigidBodies.Count;
                islands += world.GetActiveSimulationIslandsCount();
            }
            samples++;
        }
        var step = StepMs.OrderBy(x => x).ToList();
        var tick = TickMs.OrderBy(x => x).ToList();
        int bodies = 0;
        foreach (HkWorld world in MyPhysics.GetClusterList() ?? default)
            bodies += world.RigidBodies.Count;
        Say(
            $"steps={step.Count} step_mean={F(step.Average())} step_p50={F(Percentile(step, .5))} step_p99={F(Percentile(step, .99))}"
                + $" tick_mean={F(tick.Average())} tick_p50={F(Percentile(tick, .5))}"
                + $" bodies={bodies} active_bodies={F((double)active / samples)} active_islands={F((double)islands / samples)}"
        );
    }

    // ---- state ----

    private void State()
    {
        foreach (
            var grid in MyEntities.GetEntities().OfType<MyCubeGrid>().OrderBy(g => g.DisplayName)
        )
        {
            var physics = grid.Physics;
            if (physics == null)
                continue;
            var thrust = grid.Components.Get<MyEntityThrustComponent>();
            Say(
                $"grid={grid.DisplayName.Replace(' ', '_')} blocks={grid.BlocksCount} static={grid.IsStatic} physics_static={physics.IsStatic}"
                    + $" motion={physics.RigidBody.GetMotionType()} active={physics.RigidBody.IsActive} rooted={MyFixedGrids.IsRooted(grid)}"
                    + $" early={grid.IsMarkedForEarlyDeactivation} v={F(physics.LinearVelocity.Length())} w={F(physics.AngularVelocity.Length())}"
                    + $" dampeners={grid.DampenersEnabled} comp_dampeners={thrust?.DampenersEnabled} thrust={F(Total(grid))}"
                    + $" gravity={F(physics.Gravity.Length())} mass={F(physics.Mass)} controlled={grid.GridSystems.ControlSystem.IsControlled}"
                    + $" quiet_thrust={Quiet("IsThrustQuiet", grid)} quiet_gyro={Quiet("IsGyroQuiet", grid)}"
                    + $" torque={F(Traverse.Create(grid.GridSystems.GyroSystem).Field<float>("m_maxGyroForce").Value)}"
            );
        }
    }

    private static Type patch;

    // False while Performance is not loaded
    private static bool Quiet(string method, MyCubeGrid grid)
    {
        patch =
            patch
            ?? AppDomain
                .CurrentDomain.GetAssemblies()
                .Select(a => a.GetType("Shared.Patches.ConnectedShipsPatch"))
                .FirstOrDefault(t => t != null);
        return patch != null && (bool)patch.GetMethod(method).Invoke(null, new object[] { grid });
    }

    private static double Total(MyCubeGrid grid)
    {
        var thrust = grid.Components.Get<MyEntityThrustComponent>();
        if (thrust == null)
            return 0;
        return (
            Vector3.Abs(Traverse.Create(thrust).Property<Vector3>("TotalMaxPositiveThrust").Value)
            + Vector3.Abs(Traverse.Create(thrust).Property<Vector3>("TotalMaxNegativeThrust").Value)
        ).Sum;
    }

    // ---- SE-798: how much connected ships fight ----

    private void Kick(string name, Vector3D velocity, Vector3D spin)
    {
        var center = Named(name).Physics.CenterOfMassWorld;
        foreach (var grid in Group(Named(name)))
        {
            grid.Physics.LinearVelocity =
                velocity + Vector3D.Cross(spin, grid.Physics.CenterOfMassWorld - center);
            grid.Physics.AngularVelocity = spin;
            grid.Physics.RigidBody.Activate();
        }
    }

    // Seats the first online player in the ship's cockpit
    private void Pilot(string name, bool on)
    {
        var cockpit = Named(name).GetFatBlocks().OfType<MyCockpit>().First();
        if (on)
            ((Sandbox.ModAPI.IMyCockpit)cockpit).AttachPilot(
                MySession.Static.Players.GetOnlinePlayers().First().Character
            );
        else
            ((Sandbox.ModAPI.IMyCockpit)cockpit).RemovePilot();
    }

    // Makes the ship's cockpit the group's controller, as a pilot sitting down does
    private void Fly(string name, bool on)
    {
        var grid = Named(name);
        var cockpit = grid.GetFatBlocks().OfType<MyCockpit>().First();
        if (on)
            grid.GridSystems.ControlSystem.AddControllerBlock(cockpit);
        else
            grid.GridSystems.ControlSystem.RemoveControllerBlock(cockpit);
    }

    // Sets the control input of a ship, as a pilot or autopilot would
    private void Input(string name, Vector3D move, Vector3D rotate)
    {
        var grid = Named(name);
        var thrust = grid.Components.Get<MyEntityThrustComponent>();
        if (thrust != null)
            thrust.ControlThrust = move;
        grid.GridSystems.GyroSystem.ControlTorque = rotate;
        grid.ActivatePhysics();
    }

    private IEnumerator Gyro(string name, string on)
    {
        foreach (var gyro in Named(name).GetFatBlocks().OfType<MyGyro>())
            ((Sandbox.ModAPI.IMyGyro)gyro).GyroOverride = on == "1";
        yield return 1;
    }

    // Raw values of each ship per tick, to see what a wobble is made of
    private IEnumerator Series(string name, int count)
    {
        var grids = Group(Named(name)).OrderBy(g => g.DisplayName).ToList();
        for (int i = 0; i < count; i++)
        {
            var line = new StringBuilder($"t={i}");
            foreach (var g in grids)
            {
                var v = g.Physics.LinearVelocity;
                var w = g.Physics.AngularVelocity;
                var f = Vector3D.TransformNormal(
                    g.Components.Get<MyEntityThrustComponent>().FinalThrust,
                    g.WorldMatrix
                );
                var gyro = g.GridSystems.GyroSystem;
                line.Append(
                    $" {g.DisplayName}: v={F(v.X)},{F(v.Y)},{F(v.Z)} w={F(w.X)},{F(w.Y)},{F(w.Z)} f={F(f.X)},{F(f.Y)},{F(f.Z)} slow={F(gyro.SlowdownTorque.Length())}"
                );
            }
            Say(line.ToString());
            yield return 1;
        }
    }

    // Follows a group for count ticks, for its speed and for what its ships apply to it. A force
    // or torque a ship applies against another ship's adds to `applied` but not to `net`, so
    // applied / net is how much of the work goes into fighting.
    private IEnumerator Trace(string name, int count)
    {
        var grids = Group(Named(name));
        double thrustApplied = 0,
            thrustNet = 0,
            torqueApplied = 0,
            torqueNet = 0,
            jitter = 0,
            lastW = 0;
        int settled = -1,
            samples = 0;
        var reference = grids.OrderByDescending(g => g.BlocksCount).First();
        var relative = grids
            .Where(g => g != reference)
            .ToDictionary(g => g, g => g.WorldMatrix * MatrixD.Invert(reference.WorldMatrix));
        var maxOffset = 0.0;
        for (int i = 0; i < count; i++)
        {
            var thrustSum = Vector3D.Zero;
            var torqueSum = Vector3D.Zero;
            foreach (var grid in grids)
            {
                var t = grid.Components.Get<MyEntityThrustComponent>();
                if (t != null)
                {
                    var world = Vector3D.TransformNormal(t.FinalThrust, grid.WorldMatrix);
                    thrustApplied += world.Length() / 60;
                    thrustSum += world;
                }
                var gyro = grid.GridSystems.GyroSystem;
                var torque = Vector3D.TransformNormal(
                    gyro.SlowdownTorque + gyro.Torque,
                    grid.WorldMatrix
                );
                torqueApplied += torque.Length() / 60;
                torqueSum += torque;
            }
            thrustNet += thrustSum.Length() / 60;
            torqueNet += torqueSum.Length() / 60;

            var v = reference.Physics.LinearVelocity.Length();
            var w = reference.Physics.AngularVelocity.Length();
            if (v < 0.02 && w < 0.002)
            {
                if (settled < 0)
                    settled = i;
            }
            else
                settled = -1;
            jitter += Math.Abs(w - lastW);
            lastW = w;
            foreach (var pair in relative)
            {
                var now = pair.Key.WorldMatrix * MatrixD.Invert(reference.WorldMatrix);
                maxOffset = Math.Max(
                    maxOffset,
                    (now.Translation - pair.Value.Translation).Length()
                );
            }
            samples++;
            yield return 1;
        }
        Say(
            $"settle_s={F(settled < 0 ? -1 : settled / 60.0)} v={F(reference.Physics.LinearVelocity.Length())} w={F(reference.Physics.AngularVelocity.Length())}"
                + $" thrust_applied={F(thrustApplied)} thrust_net={F(thrustNet)} torque_applied={F(torqueApplied)} torque_net={F(torqueNet)}"
                + $" jitter={F(jitter)} max_offset={F(maxOffset)} samples={samples}"
        );
    }
}
