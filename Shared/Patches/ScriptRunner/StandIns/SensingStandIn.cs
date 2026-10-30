using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Sandbox.Engine.Voxels;
using Sandbox.Game.GameSystems;

namespace Shared.Patches;

// Turrets, sensors, cameras and remote controls for scripts off the main thread
// (StandIn.Sensing).
//
// The targets of turrets, turret controllers and sensors are set by the main thread; a script
// gets them as MyDetectedEntityInfo, built from the target's position, velocity and owner. Its
// world box may be computed lazily on that read, an idempotent write of the same values, as
// the game's own targeting tasks already do off the main thread. A camera ray cast updates the
// camera's own scan range and casts a ray in the physics world under the world's lock, which
// the game also does from parallel tasks between steps. Two shared parts are serialized in the
// parallel phase: the antenna system walk of a turret's CanControl (not reached on a dedicated
// server), with ScriptStandIns.AntennaLock, and the voxel geometry a ray is tested against when
// it hits nothing before a planet, which keeps a static scratch list and one shared mesher.
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class SensingStandIn
{
    private static readonly object VoxelLock = new object();

    [HarmonyPatch]
    private static class AntennaPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static IEnumerable<MethodBase> TargetMethods() =>
            typeof(MyAntennaSystem)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.Name == nameof(MyAntennaSystem.CheckConnection));

        private static void Prefix(out bool __state) =>
            __state = ScriptStandIns.Enter(ScriptStandIns.AntennaLock);

        private static void Finalizer(bool __state) =>
            ScriptStandIns.Exit(ScriptStandIns.AntennaLock, __state);
    }

    [HarmonyPatch]
    private static class VoxelPatch
    {
        private static bool Prepare() => ScriptRunner.CounterPerThread;

        private static IEnumerable<MethodBase> TargetMethods() =>
            typeof(MyVoxelGeometry)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.Name == nameof(MyVoxelGeometry.Intersect));

        private static void Prefix(out bool __state) => __state = ScriptStandIns.Enter(VoxelLock);

        private static void Finalizer(bool __state) => ScriptStandIns.Exit(VoxelLock, __state);
    }
}
