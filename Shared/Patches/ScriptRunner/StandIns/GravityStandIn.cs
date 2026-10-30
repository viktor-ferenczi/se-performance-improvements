using System.Collections.Generic;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems;
using SpaceEngineers.Game.Entities.Blocks;

namespace Shared.Patches;

// Artificial and total gravity of ship controllers and parachutes for scripts off the main
// thread (StandIn.Gravity).
//
// The artificial gravity at a point is summed over the gravity generators around it, of any
// grid, from their world matrix and box. Those are computed lazily on read, and two workers may
// read the same generator. So right before the phase the main thread brings every generator's
// up to date, and the reads in the phase only read. A gravity provider that is not a gravity
// generator block, added by a mod, may do anything: with one in the world the scripts reading
// gravity stay on the main thread.
public static class GravityStandIn
{
    private static readonly Dictionary<IMyGravityProvider, int> Providers =
        AccessTools.StaticFieldRefAccess<Dictionary<IMyGravityProvider, int>>(
            typeof(MyGravityProviderSystem),
            "m_proxyIdMap"
        );

    // On the main thread before the phase; false if gravity cannot be read off it now
    public static bool BeforePhase()
    {
        foreach (var provider in Providers.Keys)
        {
            if (!(provider is MyGravityGeneratorBase generator))
                return false;
            _ = generator.PositionComp.WorldAABB;
        }
        return true;
    }
}
