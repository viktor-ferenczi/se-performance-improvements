using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage.Render.Image;

namespace Shared.Patches
{
    // Decodes the game's images (planet height and material maps, terrain blend textures,
    // non-DDS mod textures) with a newer ImageSharp than the 2019 beta the game ships.
    // See ImageSharpRuntime for how the newer library is loaded beside the game's copy.
    //
    // All three MyImage.Load overloads funnel into the Stream one, so a single prefix
    // covers them. When the bundled library is unavailable or a decode throws, the original
    // runs, so the worst case is the game's own behavior.
    [HarmonyPatch(typeof(MyImage))]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class MyImagePatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        public static void Configure()
        {
            enabled = Config.Enabled && Config.UpgradeImageSharp && ImageSharpRuntime.TryLoad();
        }

        static MyImagePatch()
        {
            Config.PropertyChanged += OnConfigChanged;
        }

        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            Configure();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MyImage.Load), typeof(Stream), typeof(bool), typeof(bool), typeof(string))]
        [EnsureCode("90f27ad1")]
        private static bool LoadPrefix(Stream stream, bool oneChannel, bool headerOnly, string debugName, ref IMyImage __result, ref long __state)
        {
            if (!enabled)
            {
                // Lets the debug postfix time the game's own decoder
                __state = Stopwatch.GetTimestamp();
                return true;
            }

            try
            {
                __result = ImageLoader.Load(stream, oneChannel, headerOnly, debugName);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ImageSharp: failed to decode {debugName ?? "<unnamed>"}, falling back to the game's decoder");
                stream.Position = 0;
                return true;
            }
        }

        // Debug builds log the game's own decoder the same way ImageLoader logs the bundled
        // one, so two runs (fix off, fix on) can be compared line by line
#if DEBUG
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyImage.Load), typeof(Stream), typeof(bool), typeof(bool), typeof(string))]
        [EnsureCode("90f27ad1")]
        private static void LoadPostfix(bool headerOnly, string debugName, IMyImage __result, long __state)
        {
            if (__state != 0 && Log.IsDebugEnabled)
                ImageLoader.LogGameDecoder(__result, headerOnly, debugName, (Stopwatch.GetTimestamp() - __state) * 1000.0 / Stopwatch.Frequency);
        }
#endif
    }
}
