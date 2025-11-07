using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Havok;
using Sandbox.Engine.Voxels.Planet;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage;

namespace Shared.Patches
{
    // This patch has been ported with the permission of the author:
    // https://github.com/zznty/Torch/blob/master/Torch/Patches/GcCollectPatch.cs

    // ReSharper disable once UnusedType.Global
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public static class GcCollectPatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;

        // These methods freeze for seconds due to forcing a full GC
        static IEnumerable<MethodBase> TargetMethods()
        {
            if (!Config.FixGarbageCollection)
                yield break;
            
            yield return AccessTools.DeclaredMethod(typeof(MyPlanetTextureMapProvider), nameof(MyPlanetTextureMapProvider.GetHeightmap));
            yield return AccessTools.DeclaredMethod(typeof(MyPlanetTextureMapProvider), nameof(MyPlanetTextureMapProvider.GetDetailMap));
            yield return AccessTools.DeclaredMethod(typeof(MyPlanetTextureMapProvider), nameof(MyPlanetTextureMapProvider.GetMaps));
            yield return AccessTools.DeclaredMethod(typeof(MySession), nameof(MySession.Unload));
            yield return AccessTools.DeclaredMethod(typeof(HkBaseSystem), nameof(HkBaseSystem.Quit));
            yield return AccessTools.DeclaredMethod(typeof(MySimpleProfiler), nameof(MySimpleProfiler.LogPerformanceTestResults));
            yield return AccessTools.Constructor(typeof(MySession), new[] { typeof(MySyncLayer), typeof(bool) });
        }

        // Remove all GC calls from the bytecode of the above methods
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CollectRemovalTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var original = instructions.ToList();
            var patched = new List<CodeInstruction>(original.Count + 16);

            var replaced = 0;
            foreach (var instruction in original)
            {
                var isGcCollect = instruction.opcode == OpCodes.Call &&
                        instruction.operand is MethodInfo o1 &&
                        o1.DeclaringType == typeof(GC) && 
                        o1.Name == "Collect";
                
                var isCollectGc = instruction.opcode == OpCodes.Callvirt &&
                        instruction.operand is MethodInfo o2 &&
                        o2.DeclaringType == typeof(IVRageSystem) && 
                        o2.Name == "CollectGC";
                
                if (isGcCollect || isCollectGc)
                {
                    var nop = new CodeInstruction(OpCodes.Nop);
                    nop.labels.AddRange(instruction.labels);
                    patched.Add(nop);
                    
                    var methodInfo = (MethodInfo)instruction.operand;
                    var parameterCount = methodInfo.GetParameters().Length;
                    for (var i = methodInfo.IsStatic ? 1 : 0; i <= parameterCount; i++)
                    {
                        var pop = new CodeInstruction(OpCodes.Pop);
                        patched.Add(pop);
                    }
                    
                    replaced += 1;
                    continue;
                }

                patched.Add(instruction);
            }

            var typeName = originalMethod.DeclaringType?.Name ?? "<N/A>";
            var methodName = originalMethod.Name;
            if (methodName == ".ctor")
            {
                methodName = typeName;
            }
            
            original.RecordCustomCode($"{typeName}.{methodName}.original");
            patched.RecordCustomCode($"{typeName}.{methodName}.patched");

            if (replaced == 0)
            {
                Log.Warning($"GcCollectPatch(): No GC Collect calls found in method: {originalMethod.FullDescription()}");
            }
            
            foreach(var instruction in patched)
            {
                yield return instruction;
            }
        }
    }
}