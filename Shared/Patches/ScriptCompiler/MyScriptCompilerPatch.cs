using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using HarmonyLib;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage.Scripting;

namespace Shared.Patches
{
    // ReSharper disable once UnusedType.Global
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class MyScriptCompilerPatch
    {
        private static readonly MethodInfo RecallFromCacheInfo = AccessTools.DeclaredMethod(typeof(MyScriptCompilerPatch), nameof(RecallFromCache));
        private static readonly MethodInfo StoreIntoCacheInfo = AccessTools.DeclaredMethod(typeof(MyScriptCompilerPatch), nameof(StoreIntoCache));

        private static string InGameScriptCacheDir => inGameScriptCacheDir ?? (inGameScriptCacheDir = Path.Combine(Common.CacheDir, "CompiledInGameScripts"));
        private static string inGameScriptCacheDir;

        private static string ModScriptCacheDir => modScriptCacheDir ?? (modScriptCacheDir = Path.Combine(Common.CacheDir, "CompiledMods"));
        private static string modScriptCacheDir;

        private static IPluginConfig Config => Common.Config;
        private static IPluginLogger Log => Common.Logger;

        public static void Configure()
        {
            if (RecallFromCacheInfo == null ||
                StoreIntoCacheInfo == null)
            {
                throw new Exception("MyScriptCompilerPatch: Reflection error");
            }
                
            Directory.CreateDirectory(InGameScriptCacheDir);
            Directory.CreateDirectory(ModScriptCacheDir);
        }

        // ReSharper disable once UnusedMember.Local
        private static MethodInfo TargetMethod()
        {
            var ntl = typeof(MyScriptCompiler).GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var nt = ntl.First(st => st.Name.Contains("<Compile>"));
            var m = AccessTools.DeclaredMethod(nt, "MoveNext");
            if(m == null)
                throw new Exception("Could not find MoveNext method used in an iteration inside MyScriptCompiler.Compile");
            return m;
        }

        // ReSharper disable once UnusedMember.Local
        // FIXME: The code in the next comment line does not work. Add support to Harmony for async methods.
        // [HarmonyPatch(typeof(MyScriptCompiler), nameof(MyScriptCompiler.Compile), MethodType.Enumerator)]
        [HarmonyTranspiler]
        // FIXME: Make EnsureCode to work with patches using TargetMethod(s)
        // [EnsureCode("")]
        private static IEnumerable<CodeInstruction> CompileTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // See MyScriptCompiler.Compile.il
            var il = instructions.ToList();
            il.RecordOriginalCode(); // The class is generic, do not pass patchedMethod here!

            // Access to fields of the state object which stores the local variables from the original async method
            var target = il.GetField(fi => fi.Name.Contains("target"));
            var friendlyName = il.GetField(fi => fi.Name.Contains("friendlyName"));
            var assemblyStream = il.GetField(fi => fi.Name.Contains("assemblyStream"));
            var scripts = il.GetField(fi => fi.Name.Contains("scripts"));

            var exit = il.FindLast(i => i.opcode == OpCodes.Ldarg_0 && i.labels.Count > 0).labels[0];

            var j = il.FindIndex(i => i.opcode == OpCodes.Ldfld && i.operand is FieldInfo fi && fi.Name.Contains("loadPDBs")) - 1;
            Debug.Assert(il[j].opcode == OpCodes.Ldarg_0);
            il.Insert(j++, new CodeInstruction(OpCodes.Ldloc_1));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldfld, target));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldfld, scripts));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(j++, new CodeInstruction(OpCodes.Ldfld, assemblyStream));
            il.Insert(j, new CodeInstruction(OpCodes.Call, StoreIntoCacheInfo));

            var k = il.FindIndex(i => i.opcode == OpCodes.Stloc_1) + 1;
            Debug.Assert(il[k].opcode == OpCodes.Ldloc_0);
            il.Insert(k++, new CodeInstruction(OpCodes.Ldloc_1));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldfld, target));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldflda, scripts));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldarg_0));
            il.Insert(k++, new CodeInstruction(OpCodes.Ldfld, friendlyName));
            il.Insert(k++, new CodeInstruction(OpCodes.Call, RecallFromCacheInfo));
            il.Insert(k++, new CodeInstruction(OpCodes.Dup));
            il.Insert(k++, new CodeInstruction(OpCodes.Stloc_2));
            il.Insert(k, new CodeInstruction(OpCodes.Brtrue_S, exit));

            il.RecordPatchedCode(); // The class is generic, do not pass patchedMethod here!
            return il;
        }

        private class ScriptList : IEnumerable<Script>
        {
            public readonly List<Script> Scripts;

            public ScriptList(IEnumerable<Script> scriptEnumerator)
            {
                Scripts = new List<Script>(scriptEnumerator);
            }

            public IEnumerator<Script> GetEnumerator() => Scripts.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private static Assembly RecallFromCache(MyScriptCompiler myScriptCompiler, MyApiTarget target, ref IEnumerable<Script> scripts, string friendlyName)
        {
            var scriptList = new ScriptList(scripts);
            scripts = scriptList;

            if (!GetCachePath(myScriptCompiler, target, scriptList, out var cachePath))
                return null;

            if (!File.Exists(cachePath))
            {
#if DEBUG
                Log.Debug("Compiled script not cached yet: {0}", cachePath);
#endif
                return null;
            }

            if (target == MyApiTarget.Mod)
            {
                if (friendlyName == null)
                    friendlyName = "<No Name>";

                // NOTE: The modId may differ from the time when the mod was compiled
                // This is not a problem, because it is used only for the performance
                // counters this plugin also disables (enabled if mod caching is used).
                var modId = MyModWatchdog.AllocateModId(friendlyName);
#if DEBUG
                Log.Debug("Allocated modId {0} for mod: {1}", modId, friendlyName);
#endif
            }

            for (var retry = 0; retry < 200; retry++)
            {
                try
                {
                    // NOTE: Do not use Assembly.LoadFrom, that results in world load failures
                    // due to unable to unload the assembly on previous world unload.
                    var assembly = Assembly.Load(File.ReadAllBytes(cachePath));
#if DEBUG
                    Log.Debug("Loaded compiled script assembly from cache file: {0}", cachePath);
#endif
                    return assembly;
                }
                catch (IOException)
                {
                    // All I/O errors are retried for 1 second
                    // This exception can happen on trying to compile the same script multiple times concurrently:
                    // HResult = -2147024864
                    // Message = "The process cannot access the file 'C:\Torch\Instance\PerformanceImprovements\Cache\CompiledInGameScripts\9daecd2c41d387772f8edcc792da89a6b776ec2e.cache' because it is being used by another process."
                }
                catch (Exception e1)
                {
                    Log.Error(e1, "Error loading compiled script assembly from cache file: {0}", cachePath);
                    try
                    {
                        File.Move(cachePath, cachePath + ".broken");
                    }
                    catch (Exception e2)
                    {
                        Log.Error(e2, "Error renaming broken compiled script assembly cache file: {0}", cachePath);
                    }

                    break;
                }

                Thread.Sleep(5);
            }

            // Failed to load the compiled script assembly from cache, so it will be compiled
            return null;
        }

        private static void StoreIntoCache(MyScriptCompiler myScriptCompiler, MyApiTarget target, IEnumerable<Script> scripts, MemoryStream assemblyStream)
        {
            var scriptList = (ScriptList)scripts;
            if (!GetCachePath(myScriptCompiler, target, scriptList.Scripts, out var cachePath))
                return;

#if DEBUG
            Log.Debug("Saving compiled script assembly into cache file: {0}", cachePath);
#endif
            try
            {
                // Write a temp file and move it into place, so a crash mid-write or a
                // concurrent compile of the same script never leaves a partial cache file
                var tempPath = $"{cachePath}.{Guid.NewGuid():N}.tmp";
                try
                {
                    using (var fileStream = File.Create(tempPath))
                        assemblyStream.CopyTo(fileStream);

                    // Same key means same assembly, so a file stored meanwhile by a concurrent compile is kept
                    if (!File.Exists(cachePath))
                        File.Move(tempPath, cachePath);
                }
                finally
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
            }
            catch (IOException e)
            {
                Log.Warning("Failed to write compiled script assembly cache file: {0}; Error: {1}", cachePath, e.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to write compiled script assembly cache file: {0}", cachePath);
            }
            finally
            {
                assemblyStream.Seek(0, SeekOrigin.Begin);
            }
        }

        private static bool GetCachePath(MyScriptCompiler myScriptCompiler, MyApiTarget target, IEnumerable<Script> scripts, out string cachePath)
        {
            if (!Config.Enabled ||
                target == MyApiTarget.None ||
                target == MyApiTarget.Mod && !Config.CacheMods ||
                target == MyApiTarget.Ingame && !Config.CacheScripts)
            {
                cachePath = null;
                return false;
            }

            // NOTE: Called twice for new script compilations, a bit of wasted work, but no need to keep state
            var scriptsHash = GetScriptsHash(myScriptCompiler, scripts);
            var cacheFileName = $"{scriptsHash}.cache";
            var cacheDir = target == MyApiTarget.Ingame ? InGameScriptCacheDir : ModScriptCacheDir;
            cachePath = Path.Combine(cacheDir, cacheFileName);
            return true;
        }

        private static string GetScriptsHash(MyScriptCompiler myScriptCompiler, IEnumerable<Script> scripts)
        {
            return ScriptCacheKey.Compute(GetCacheKeyParts(myScriptCompiler, scripts));
        }

        private static IEnumerable<string> GetCacheKeyParts(MyScriptCompiler myScriptCompiler, IEnumerable<Script> scripts)
        {
            // Include the versions of the loaded mod rewriting plugins (currently the implicitly
            // loaded compat ones), so upgrading any of them invalidates the cached assemblies
            var modRewriterVersionsHash = ModRewriterVersions.Hash;
            yield return modRewriterVersionsHash == null ? "" : BitConverter.ToString(modRewriterVersionsHash);

            // Include .NET version in the hash to prevent loading assemblies compiled on different .NET versions
            yield return RuntimeInformation.FrameworkDescription;

            // Include the game's build version (the client build version on the client,
            // the server build version on the dedicated server) to prevent loading assemblies
            // compiled against a different game build after the game has been updated
            yield return Common.GameVersion;

            // The symbols are a set, sort them for a stable order
            var symbols = myScriptCompiler.m_conditionalCompilationSymbols?.OrderBy(symbol => symbol, StringComparer.Ordinal).ToList() ?? new List<string>();
            yield return symbols.Count.ToString();
            foreach (var symbol in symbols)
                yield return symbol;

            // Scripts in compilation order, each with its file name
            foreach (var script in scripts)
            {
                yield return script.Name;
                yield return script.Code;
            }
        }
    }
}