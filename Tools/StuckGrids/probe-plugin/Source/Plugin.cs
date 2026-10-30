using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using HarmonyLib;
using Havok;
using Sandbox;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using VRage;
using VRage.Game.ObjectBuilders.Components;
using VRage.ObjectBuilders;
using VRage.Plugins;
using VRageMath;

namespace StuckProbe;

// Measurement probe for SE-753, on the dedicated server or an offline client. Rams a ship
// (STUCK_SHIP, with its whole physical group) into a station (STUCK_STATION) and measures the
// simulation speed.
//
// STUCK_SHOTS lists the shots to try, separated by ';', each "x,y,z,dx,dy,dz,speed[,advance]": the
// station cell aimed at, the direction of flight in the station's own axes, the speed in m/s and
// how far to move the start towards the target (the ship starts 10 m from touching the station). The first
// shot is placed STUCK_WARMUP seconds after the world is ready. A shot places the ship in front of
// the target, launches it 2 s later and watches it for 10 s. If the ship stays in deep contact
// with the station (deeper than 0.25 m for 3 seconds in a row) it is entangled: probe.log says
// ENTANGLED and no further shot is tried. The probe measures STUCK_SECONDS from the launch of the
// entangling or the last shot, then writes DONE into probe.log.
//
// Every second one line goes into STUCK_PROBE_DIR/seconds.tsv: the shot, the frames simulated in
// that second (60 = full speed), the mean MySession.Update time, the ship's distance from the
// station and its speed, and the contact points reported between ship and station that second,
// with their deepest penetration.
//
// Voxel mode (SE-770): STUCK_VOXEL=planet or asteroid spawns a Moon (19 km) or an asteroid prefab
// 60 km from the station when the world is ready. A shot then places the ship with its center
// STUCK_DEPTH meters below the voxel's surface, on the side facing the station, inside a new
// no-damage safe zone when STUCK_SAFEZONE=1. STUCK_UNPOWERED=1 turns off the functional blocks of
// the ship's grids after the placement, so it falls like debris (SE-779). STUCK_HOLE=<meters> cuts
// a sphere of that radius out of the voxel around the placement, like a mined cavern, and
// STUCK_SPIN=1 sets the ship turning every second, so its physics never sleeps. STUCK_STATIC=1
// converts the placed ship to a static grid. The distance column is the ship's distance from
// where it was placed, the contact columns count ship-voxel contacts.
public class Plugin : IPlugin
{
    public void Init(object gameInstance)
    {
        new Harmony("StuckProbe").PatchAll(typeof(Plugin).Assembly);
        Probe.Log("loaded");
    }

    public void Update() { }

    public void Dispose() { }
}

public static class Probe
{
    private const double LaunchDelay = 2;
    private const double Watch = 10;
    private const float EntangledDepth = -0.25f;
    private const int EntangledSeconds = 3;

    private static readonly string Dir =
        Environment.GetEnvironmentVariable("STUCK_PROBE_DIR") ?? ".";
    private static readonly long ShipId = long.Parse(
        Environment.GetEnvironmentVariable("STUCK_SHIP") ?? "0"
    );
    private static readonly long StationId = long.Parse(
        Environment.GetEnvironmentVariable("STUCK_STATION") ?? "0"
    );
    private static readonly double Warmup = Number("STUCK_WARMUP", 20);
    private static readonly string VoxelKind = Environment.GetEnvironmentVariable("STUCK_VOXEL");
    private static readonly double Depth = Number("STUCK_DEPTH", 0);
    private static readonly bool SafeZone = Environment.GetEnvironmentVariable("STUCK_SAFEZONE") == "1";
    private static readonly bool Unpowered = Environment.GetEnvironmentVariable("STUCK_UNPOWERED") == "1";
    private static readonly double Hole = Number("STUCK_HOLE", 0);
    private static readonly bool Spin = Environment.GetEnvironmentVariable("STUCK_SPIN") == "1";
    private static readonly bool Static = Environment.GetEnvironmentVariable("STUCK_STATIC") == "1";
    private static MyVoxelBase voxel;
    private static Vector3D placedCenter;
    private static readonly double Seconds = Number("STUCK_SECONDS", 180);
    private static readonly double[][] Shots = (
        Environment.GetEnvironmentVariable("STUCK_SHOTS") ?? "0,14,0,0,-1,0,200"
    )
        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s =>
            s.Split(',').Select(v => double.Parse(v, CultureInfo.InvariantCulture)).ToArray()
        )
        .ToArray();

    private static readonly Stopwatch Clock = new Stopwatch();
    private static int shot = -1;
    private static double placedAt;
    private static double launchedAt = -1;
    private static bool entangled;
    private static int deepSeconds;
    private static double windowStart;
    private static int frames;
    private static long updateTicks;
    private static bool done;
    private static StreamWriter table;

    private static readonly object ContactLock = new object();
    private static int contacts;
    private static int deep;
    private static float deepest;

    private static double Number(string name, double fallback) =>
        double.TryParse(
            Environment.GetEnvironmentVariable(name),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var value
        )
            ? value
            : fallback;

    public static void Log(string message)
    {
        Directory.CreateDirectory(Dir);
        File.AppendAllText(
            Path.Combine(Dir, "probe.log"),
            $"{DateTime.UtcNow:HH:mm:ss.fff} {message}\n"
        );
    }

    public static void EndFrame(long ticks)
    {
        if (done)
            return;

        if (!Clock.IsRunning)
        {
            Clock.Start();
            Log(
                $"world ready, {Shots.Length} shots, the first in {Warmup} s, measuring {Seconds} s after the last launch"
            );
            table = new StreamWriter(Path.Combine(Dir, "seconds.tsv"));
            if (VoxelKind != null)
                SpawnVoxel();
            table.WriteLine(
                "shot\tsecond\tframes\tupdate_ms\tphysics_ratio\tdistance_m\tspeed_mps\tcontacts\tdeep\tdeepest_m\tactive"
            );
        }

        // The world has no player, and the respawn screen pauses an offline game
        if (MySandboxGame.IsPaused)
            MySandboxGame.IsPaused = false;

        frames++;
        updateTicks += ticks;
        var now = Clock.Elapsed.TotalSeconds;

        if (shot < 0 && now >= Warmup)
            Place(0, now);

        if (shot >= 0 && launchedAt < placedAt && now >= placedAt + LaunchDelay)
            Launch(now);

        if (now - windowStart < 1)
            return;

        int c,
            d;
        float dd;
        lock (ContactLock)
        {
            c = contacts;
            d = deep;
            dd = deepest;
            contacts = deep = 0;
            deepest = 0;
        }

        MyEntities.TryGetEntityById(ShipId, out MyCubeGrid ship);
        MyEntities.TryGetEntityById(StationId, out MyCubeGrid station);
        var distance =
            VoxelKind != null && ship != null
                ? Vector3D.Distance(ship.PositionComp.WorldAABB.Center, placedCenter)
            : ship != null && station != null
                ? Vector3D.Distance(
                    ship.PositionComp.WorldAABB.Center,
                    station.PositionComp.WorldAABB.Center
                )
                : -1;
        var speed = ship?.Physics?.LinearVelocity.Length() ?? -1;
        var active = ship?.Physics == null ? -1 : ship.Physics.IsActive ? 1 : 0;
        if (Spin && ship?.Physics != null && launchedAt >= placedAt && shot >= 0)
            ship.Physics.AngularVelocity = Vector3.Up;
        var launched = shot >= 0 && launchedAt >= placedAt;
        table.WriteLine(
            string.Join(
                "\t",
                shot,
                (launched ? now - launchedAt : now - Warmup).ToString(
                    "F1",
                    CultureInfo.InvariantCulture
                ),
                frames,
                (updateTicks * 1000.0 / Stopwatch.Frequency / frames).ToString(
                    "F2",
                    CultureInfo.InvariantCulture
                ),
                MyPhysics.SimulationRatio.ToString("F2", CultureInfo.InvariantCulture),
                distance.ToString("F1", CultureInfo.InvariantCulture),
                speed.ToString("F1", CultureInfo.InvariantCulture),
                c,
                d,
                dd.ToString("F3", CultureInfo.InvariantCulture),
                active
            )
        );
        table.Flush();
        windowStart = now;
        frames = 0;
        updateTicks = 0;

        if (!launched)
            return;

        deepSeconds = dd < EntangledDepth ? deepSeconds + 1 : 0;
        if (!entangled && deepSeconds >= EntangledSeconds)
        {
            entangled = true;
            Log(
                $"ENTANGLED shot {shot} ({string.Join(",", Shots[shot].Select(v => v.ToString(CultureInfo.InvariantCulture)))})"
            );
        }

        var last = entangled || shot == Shots.Length - 1;
        if (!last && now - launchedAt >= Watch)
        {
            Place(shot + 1, now);
            return;
        }

        if (last && now - launchedAt >= Seconds)
        {
            done = true;
            table.Dispose();
            Log("DONE");
        }
    }

    private static void Place(int index, double now)
    {
        shot = index;
        placedAt = now;
        deepSeconds = 0;
        if (
            !MyEntities.TryGetEntityById(ShipId, out MyCubeGrid ship)
            || !MyEntities.TryGetEntityById(StationId, out MyCubeGrid station)
        )
        {
            Log("ship or station not found");
            return;
        }

        if (VoxelKind != null)
        {
            PlaceInVoxel(ship, station);
            return;
        }

        var s = Shots[index];
        var matrix = station.WorldMatrix;
        var target = Vector3D.Transform(new Vector3D(s[0], s[1], s[2]) * station.GridSize, matrix);
        var direction = Vector3D.Normalize(
            Vector3D.TransformNormal(new Vector3D(s[3], s[4], s[5]), matrix)
        );

        Stop(ship);
        var offset = ship.PositionComp.WorldAABB.Center - ship.PositionComp.GetPosition();
        // The optional 8th value moves the start that many meters further, into the station when
        // it is more than the 10 m gap
        var advance = s.Length > 7 ? s[7] : 0;
        var center = target - direction * (ship.PositionComp.WorldVolume.Radius + 10 - advance);
        var world = ship.WorldMatrix;
        world.Translation = center - offset;
        ship.Teleport(world);
        Stop(ship);
        Log($"shot {index}: placed {Vector3D.Distance(center, target):F0} m from the target");
    }

    private static void Launch(double now)
    {
        launchedAt = now;
        if (
            !MyEntities.TryGetEntityById(ShipId, out MyCubeGrid ship)
            || !MyEntities.TryGetEntityById(StationId, out MyCubeGrid station)
        )
            return;

        var s = Shots[shot];
        var speed = (float)s[6];
        var direction = (Vector3)
            Vector3D.Normalize(
                Vector3D.TransformNormal(new Vector3D(s[3], s[4], s[5]), station.WorldMatrix)
            );

        // Lifts the vanilla 100 m/s cap, the Octahedron server runs with a higher one
        MySector.EnvironmentDefinition.LargeShipMaxSpeed = Math.Max(
            MySector.EnvironmentDefinition.LargeShipMaxSpeed,
            speed + 10
        );
        MySector.EnvironmentDefinition.SmallShipMaxSpeed = Math.Max(
            MySector.EnvironmentDefinition.SmallShipMaxSpeed,
            speed + 10
        );

        var grids = MyCubeGridGroups
            .Static.Physical.GetGroup(ship)
            .Nodes.Select(n => n.NodeData)
            .ToList();
        foreach (var grid in grids)
        {
            if (grid.Physics == null)
                continue;

            // Havok's own cap comes from the limit when the body was made
            ((MyGridPhysics)grid.Physics).SetDefaultRigidBodyMaxVelocities();
            grid.Physics.LinearVelocity = direction * speed;
            grid.Physics.AngularVelocity = Vector3.Zero;
        }

        Log($"shot {shot}: launched {grids.Count} grids at {speed} m/s");
    }

    private static void Stop(MyCubeGrid ship)
    {
        foreach (var node in MyCubeGridGroups.Static.Physical.GetGroup(ship).Nodes)
        {
            if (node.NodeData.Physics == null)
                continue;

            node.NodeData.Physics.LinearVelocity = Vector3.Zero;
            node.NodeData.Physics.AngularVelocity = Vector3.Zero;
        }
    }

    private static void SpawnVoxel()
    {
        var at = MyEntities.GetEntityById(StationId).PositionComp.GetPosition() + new Vector3D(0, 0, 60000);
        voxel =
            VoxelKind == "planet"
                ? (MyVoxelBase)MyAPIGateway.Session.VoxelMaps.SpawnPlanet("Moon", 19000, 770, at)
                : (MyVoxelBase)MyAPIGateway.Session.VoxelMaps.CreateVoxelMapFromStorageName("SE770Asteroid", "Asteroid256-001", at);
        Log($"spawned {VoxelKind} {voxel?.EntityId} at {voxel?.PositionComp.WorldAABB.Center}");
    }

    private static void PlaceInVoxel(MyCubeGrid ship, MyCubeGrid station)
    {
        var center = voxel.PositionComp.WorldAABB.Center;
        var up = Vector3D.Normalize(station.PositionComp.GetPosition() - center);
        Vector3D surface;
        if (voxel is MyPlanet planet)
            surface = planet.GetClosestSurfacePointGlobal(center + up * 100000);
        else
        {
            var line = new LineD(center + up * 1000, center);
            if (!voxel.GetIntersectionWithLine(ref line, out Vector3D? hit) || !hit.HasValue)
            {
                Log("no asteroid surface found");
                return;
            }
            surface = hit.Value;
        }

        placedCenter = surface - up * Depth;
        if (Hole > 0)
        {
            var sphere = MyAPIGateway.Session.VoxelMaps.GetSphereVoxelHand();
            sphere.Center = placedCenter;
            sphere.Radius = (float)Hole;
            MyAPIGateway.Session.VoxelMaps.CutOutShape(voxel, sphere);
            Log($"cut a {Hole} m hole");
        }
        Stop(ship);
        var offset = ship.PositionComp.WorldAABB.Center - ship.PositionComp.GetPosition();
        var world = ship.WorldMatrix;
        world.Translation = placedCenter - offset;
        ship.Teleport(world);
        Stop(ship);

        if (Static)
            ship.ConvertToStatic();

        // Debris has no power: no dampeners hold it against the gravity
        if (Unpowered)
        {
            foreach (var node in MyCubeGridGroups.Static.Physical.GetGroup(ship).Nodes)
            {
                foreach (var block in node.NodeData.GetFatBlocks().OfType<MyFunctionalBlock>())
                    block.Enabled = false;
            }
        }

        if (SafeZone)
        {
            var zone = new MyObjectBuilder_SafeZone
            {
                PositionAndOrientation = new MyPositionAndOrientation(MatrixD.CreateTranslation(surface)),
                Radius = 500,
                PersistentFlags = MyPersistentEntityFlags2.InScene,
                Shape = MySafeZoneShape.Sphere,
                AccessTypePlayers = MySafeZoneAccess.Blacklist,
                AccessTypeFactions = MySafeZoneAccess.Blacklist,
                AccessTypeGrids = MySafeZoneAccess.Blacklist,
                AccessTypeFloatingObjects = MySafeZoneAccess.Blacklist,
                AllowedActions = MySafeZoneAction.All & ~MySafeZoneAction.Damage,
                Enabled = true,
                IsVisible = true,
            };
            MyEntities.CreateFromObjectBuilderAndAdd(zone, false);
        }

        Log($"shot {shot}: placed the ship {Depth} m into the {VoxelKind}, safe zone {SafeZone}, unpowered {Unpowered}, spin {Spin}, static {ship.IsStatic}");
    }

    public static void Contact(ref HkContactPointEvent value)
    {
        if (VoxelKind != null)
        {
            var e0 = value.GetPhysicsBody(0)?.Entity;
            var e1 = value.GetPhysicsBody(1)?.Entity;
            if (!(e0 is MyVoxelBase && e1 is MyCubeGrid || e1 is MyVoxelBase && e0 is MyCubeGrid))
                return;
            lock (ContactLock)
            {
                contacts++;
                if (value.ContactPoint.Distance < -0.1f)
                    deep++;
                if (value.ContactPoint.Distance < deepest)
                    deepest = value.ContactPoint.Distance;
            }
            return;
        }

        var a = value.GetPhysicsBody(0)?.Entity as MyCubeGrid;
        var b = value.GetPhysicsBody(1)?.Entity as MyCubeGrid;
        if (a == null || b == null || (a.EntityId == StationId) == (b.EntityId == StationId))
            return;

        var distance = value.ContactPoint.Distance;
        lock (ContactLock)
        {
            contacts++;
            if (distance < -0.1f)
                deep++;
            if (distance < deepest)
                deepest = distance;
        }
    }
}

[HarmonyPatch(typeof(MySession), nameof(MySession.Update))]
public static class SessionUpdatePatch
{
    public static void Prefix(out long __state) => __state = Stopwatch.GetTimestamp();

    public static void Postfix(long __state) => Probe.EndFrame(Stopwatch.GetTimestamp() - __state);
}

[HarmonyPatch(typeof(MyGridPhysics), "RigidBody_ContactPointCallbackImpl")]
public static class ContactPatch
{
    public static void Prefix(ref HkContactPointEvent value) => Probe.Contact(ref value);
}
