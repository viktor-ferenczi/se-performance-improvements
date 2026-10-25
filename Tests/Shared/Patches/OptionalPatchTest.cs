using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Shared.Patches;
using Xunit;

namespace Shared.Plugin
{
    // Stands in for the plugin's Common, which needs the game
    internal static class Common
    {
        public static readonly TestLogger Logger = new TestLogger();
    }

    internal sealed class TestLogger
    {
        public void Error(Exception ex, string message, params object[] data) => throw ex;
    }
}

namespace Tests.Shared.Patches
{
    public static class Target
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int StartsOff() => 1;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int StartsOn() => 1;
    }

    [HarmonyPatch(typeof(Target), nameof(Target.StartsOff))]
    public static class StartsOffPatch
    {
        public static bool Wanted;
        public static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(StartsOffPatch),
            () => Wanted
        );

        private static bool Prepare() => Optional.Prepare();

        private static bool Prefix(ref int __result)
        {
            __result = 2;
            return false;
        }
    }

    [HarmonyPatch(typeof(Target), nameof(Target.StartsOn))]
    public static class StartsOnPatch
    {
        public static bool Wanted = true;
        public static readonly OptionalPatch Optional = new OptionalPatch(
            typeof(StartsOnPatch),
            () => Wanted
        );

        private static bool Prepare() => Optional.Prepare();

        private static bool Prefix(ref int __result)
        {
            __result = 2;
            return false;
        }
    }

    public class OptionalPatchTests
    {
        [Fact]
        public void OptionalPatch_OffAtStartup()
        {
            // The plugin's own Harmony instance applies every class at startup
            new Harmony("test.startup.off")
                .CreateClassProcessor(typeof(StartsOffPatch))
                .Patch();
            Assert.Equal(1, Target.StartsOff());

            StartsOffPatch.Wanted = true;
            StartsOffPatch.Optional.Update();
            Assert.Equal(2, Target.StartsOff());

            StartsOffPatch.Wanted = false;
            StartsOffPatch.Optional.Update();
            Assert.Equal(1, Target.StartsOff());

            StartsOffPatch.Wanted = true;
            StartsOffPatch.Optional.Update();
            Assert.Equal(2, Target.StartsOff());
        }

        [Fact]
        public void OptionalPatch_OnAtStartup()
        {
            new Harmony("test.startup.on").CreateClassProcessor(typeof(StartsOnPatch)).Patch();
            Assert.Equal(2, Target.StartsOn());

            // Applied by the startup instance, still removed
            StartsOnPatch.Wanted = false;
            StartsOnPatch.Optional.Update();
            Assert.Equal(1, Target.StartsOn());

            StartsOnPatch.Wanted = true;
            StartsOnPatch.Optional.Update();
            Assert.Equal(2, Target.StartsOn());
            Assert.Single(
                Harmony
                    .GetPatchInfo(AccessTools.Method(typeof(Target), nameof(Target.StartsOn)))
                    .Prefixes
            );
        }
    }
}
