using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Shared.Config;
using Shared.Plugin;
using Shared.Stats;
using Shared.Tools;
using VRage.Game.Entity;
using VRage.Game.ObjectBuilders.Components;
using VRageMath;

namespace Shared.Patches
{
    [HarmonyPatch(typeof(MySafeZone))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class MySafeZonePatch
    {
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        public static void Configure()
        {
            enabled = Config.Enabled && Config.FixSafeZone;
            Config.PropertyChanged += OnConfigChanged;
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            enabled = Config.Enabled && Config.FixSafeZone;
            if (!enabled)
            {
                IsSafeCache.Clear();
                IsActionAllowedCache.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update()
        {
            IsSafeCache.Cleanup();
            IsActionAllowedCache.Cleanup();
        }

        // Reads the cache hit-rate counters into the snapshot, resetting them.
        public static void CaptureStatistics(StatisticsSnapshot snapshot)
        {
            var isSafe = IsSafeCache.Sample();
            snapshot.Caches.Add(new CacheStatEntry("SafeZone.IsSafe", isSafe.Lookups, isSafe.Hits, isSafe.Size));

            var isActionAllowed = IsActionAllowedCache.Sample();
            snapshot.Caches.Add(new CacheStatEntry("SafeZone.IsActionAllowed", isActionAllowed.Lookups, isActionAllowed.Hits, isActionAllowed.Size));
        }

        #region "IsSafe fix, see: https://support.keenswh.com/spaceengineers/pc/topic/24146-performance-mysafezone-issafe-is-called-frequently-but-not-cached"

        private static readonly UintCache<long> IsSafeCache = new UintCache<long>(139 * 60, 256, collectStats: true);

        [HarmonyPrefix]
        [HarmonyPatch("IsSafe")]
        [EnsureCode("9cc60b7f")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSafePrefix(MyEntity entity, ref bool __result, ref bool __state)
        {
            if (!enabled)
                return true;

            if (IsSafeCache.TryGetValue(entity.EntityId, out var value))
            {
                __result = value != 0;
                return false;
            }

            __state = true;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("IsSafe")]
        [EnsureCode("9cc60b7f")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IsSafePostfix(MyEntity entity, bool __result, bool __state)
        {
            if (!__state)
                return;

            var entityId = entity.EntityId;
            IsSafeCache.Store(entityId, __result ? 1u : 0u, 120u + (uint) (entityId & 15));
        }

        #endregion

        #region "IsOutside fix"

        [HarmonyPrefix]
        [HarmonyPatch("IsOutside", typeof(BoundingBoxD))]
        [EnsureCode("fe4efdc4")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOutsidePrefix(MySafeZone __instance, BoundingBoxD aabb, ref bool __result)
        {
            var d2 = (aabb.Center - __instance.PositionComp.GetPosition()).LengthSquared();
            if (__instance.Shape == MySafeZoneShape.Sphere)
            {
                var s = __instance.Radius + aabb.HalfExtents.Length();
                if (d2 > s * s)
                {
                    __result = true;
                    return false;
                }
            }
            else
            {
                var s = __instance.PositionComp.LocalAABB.HalfExtents.Length() + aabb.HalfExtents.Length();
                if (d2 > s * s)
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region "IsActionAllowed fix"

        private static readonly UintCache<long> IsActionAllowedCache = new UintCache<long>(27 * 60, collectStats: true);

        [HarmonyPrefix]
        [HarmonyPatch("IsActionAllowed", typeof(MyEntity), typeof(MySafeZoneAction), typeof(long))]
        [EnsureCode("9bd446ec")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsActionAllowedPrefix(MySafeZone __instance, MyEntity entity, MySafeZoneAction action, long sourceEntityId, ref bool __result, ref long __state)
        {
            if (!enabled)
                return true;

            if (entity == null)
                return false;

            if (!__instance.Enabled)
            {
                __result = true;
                return false;
            }

            var entityId = entity.EntityId;
            var key = __instance.EntityId ^ entityId ^ sourceEntityId ^ (long) action;
            if (IsActionAllowedCache.TryGetValue(key, out var value))
            {
                // In case of very rare key collision just run the original
                value ^= (uint) entityId;
                if (value > 1)
                    return true;

                __result = value != 0;
                return false;
            }

            __state = key;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("IsActionAllowed", typeof(MyEntity), typeof(MySafeZoneAction), typeof(long))]
        [EnsureCode("9bd446ec")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IsActionAllowedPostfix(MyEntity entity, bool __result, long __state)
        {
            if (__state == 0)
                return;

            var entityIdLow32Bits = (uint) entity.EntityId;
            IsActionAllowedCache.Store(__state, (__result ? 1u : 0u) ^ entityIdLow32Bits, 120u + (entityIdLow32Bits & 15));
        }

        #endregion
    }
}