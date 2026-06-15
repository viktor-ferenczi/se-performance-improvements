using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRageMath;
using VRageMath.Spatial;

namespace Shared.Patches
{
    [HarmonyPatch(typeof(MyClusterTree))]
    public static class MyClusterTreePatch
    {
        private static IPluginLogger Logger => Common.Plugin.Log;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        private static readonly MethodInfo OptimizedImplementationMethodInfo = AccessTools.DeclaredMethod(typeof(MyClusterTreePatch), nameof(OptimizedImplementation));

        public static void Configure()
        {
            enabled = Config.Enabled && Config.FixPhysics;
        }

        // ReSharper disable once UnusedMember.Local
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(MyClusterTree.ReorderClusters))]
        [EnsureCode("a129d62a")]
        private static IEnumerable<CodeInstruction> ReorderClustersTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase patchedMethod)
        {
            if (!enabled)
                return instructions;

            var il = instructions.ToList();
            il.RecordOriginalCode(patchedMethod);

            // Find the nested loop
            var i = il.FindAllIndex(ci => ci.opcode == OpCodes.Ldloc_2)[1];
            while (i < il.Count && il[i].opcode != OpCodes.Pop) i++;
            i++;

            // Remove the nested loop
            var j = il.FindAllIndex(ci => ci.opcode == OpCodes.Endfinally)[1];
            var nop = new CodeInstruction(OpCodes.Nop);
            nop.labels = il.Skip(i).Take(j + 1 - i).Select(ci => ci.labels).SelectMany(l => l).ToList();
            il.RemoveRange(i, j + 1 - i);
            il.Insert(i++, nop);

            // Call the optimized implementation instead
            var resultListGetter = il.FindPropertyGetter("m_resultList");
            il.Insert(i++, new CodeInstruction(OpCodes.Call, resultListGetter)); // static MyClusterTree.m_resultList
            var objectsDataField = il.GetField(fi => fi.Name == "m_objectsData");
            il.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0)); // this
            il.Insert(i++, new CodeInstruction(OpCodes.Ldfld, objectsDataField)); // this.m_objectsData
            il.Insert(i++, new CodeInstruction(OpCodes.Ldloc_2)); // source
            il.Insert(i++, new CodeInstruction(OpCodes.Ldloca_S, (byte)1)); // ref inflated1
            il.Insert(i, new CodeInstruction(OpCodes.Call, OptimizedImplementationMethodInfo));

            il.RecordPatchedCode(patchedMethod);
            return il;
        }

        // Reuse the hashsets used by the optimized implementation
        private static readonly ThreadLocal<HashSet<ulong>> CollidedObjectKeysPool = new ThreadLocal<HashSet<ulong>>();

        private static void OptimizedImplementation(List<MyClusterTree.MyCluster> resultList, Dictionary<ulong, MyClusterTree.MyObjectData> objectsData, HashSet<MyClusterTree.MyObjectData> source, ref BoundingBoxD inflated1)
        {
            // Original nested loop for reference:
            // foreach (MyClusterTree.MyCluster mResult in resultList)
            // {
            //     foreach (MyObjectData myObjectData
            //              in objectsData
            //                  .Where(x => mResult.Objects.Contains(x.Key))
            //                  .Select(x => x.Value))
            //     {
            //         source.Add(myObjectData);
            //         inflated1.Include(myObjectData.AABB.GetInflated(MyClusterTree.IdealClusterSize / 2f));
            //     }
            // }

            var collidedObjectKeys = CollidedObjectKeysPool.IsValueCreated ? CollidedObjectKeysPool.Value : CollidedObjectKeysPool.Value = new HashSet<ulong>(4096);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"{nameof(MyClusterTreePatch)}.{nameof(OptimizedImplementation)}: objectsData.Count={objectsData.Count}, collidedObjectKeys.Count={collidedObjectKeys.Count}");

            foreach (MyClusterTree.MyCluster collidedCluster in resultList)
            {
                collidedObjectKeys.UnionWith(collidedCluster.Objects);
            }

            foreach (var objectKey in collidedObjectKeys)
            {
                if (objectsData.TryGetValue(objectKey, out var myObjectData))
                {
                    source.Add(myObjectData);
                    inflated1.Include(myObjectData.AABB.GetInflated(MyClusterTree.IdealClusterSize / 2f));
                }
            }

            collidedObjectKeys.Clear();

#if DEBUG
            var original = resultList.Count * objectsData.Count;

            var optimized = collidedObjectKeys.Count;
            foreach (var collidedCluster in resultList)
            {
                optimized += collidedCluster.Objects.Count;
            }

            Logger.Info($"ReorderClusters: original {original}, optimized {optimized} ({100.0 * optimized / Math.Max(1, original):F2}%)");
#endif
        }
    }
}
