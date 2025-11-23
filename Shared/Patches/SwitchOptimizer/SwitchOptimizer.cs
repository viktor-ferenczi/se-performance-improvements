using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Shared.Tools;

namespace Shared.Patches
{
    [HarmonyPatch]
    public static class SwitchOptimizer
    {
        private static readonly List<Dictionary<Type, int>> LookupTables = new List<Dictionary<Type, int>>();

        public static IEnumerable<MethodInfo> TargetMethods()
        {
            var type1 = AccessTools.TypeByName("Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1") ?? throw new NullReferenceException("Could not find type: Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1");
            yield return AccessTools.Method(type1, "Write138_Object") ?? throw new Exception("Could not find method: Write138_Object");

            // var type2 = AccessTools.TypeByName("VRage.XmlSerializers.Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1") ?? throw new NullReferenceException("Could not find type: VRage.XmlSerializers.Microsoft.Xml.Serialization.GeneratedAssembly.XmlSerializationWriter1");
            // yield return AccessTools.Method(type2, "Write2977_Object") ?? throw new Exception("Could not find method: Write2977_Object");
            // yield return AccessTools.Method(type2, "Write3190_Object") ?? throw new Exception("Could not find method: Write3190_Object");
        }

        private static long callCount = 0;
        public static bool Prefix()
        {
            callCount++;
            return true;
        }

        private static long lastLogged = -1;

        public static void LogCount()
        {
            if (lastLogged == callCount)
                return;
            
            Plugin.Common.Logger.Info($"SwitchOptimizer: Prefix call count: {callCount}");

            lastLogged = callCount;
        }

/*
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes.RecordCustomCode($"{originalMethod.Name}.original");

            var typeChecks = new List<TypeCheckInfo>();

            // 1. Identify the local variable holding the Type
            // Look for: Type type = o.GetType();
            // IL: ldarg.X, callvirt GetType, stloc.S V_X
            int typeLocalIndex = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo)?.Name == "GetType")
                {
                    if (i + 1 < codes.Count && IsStoreLocal(codes[i + 1]))
                    {
                        typeLocalIndex = GetLocalIndex(codes[i + 1]);
                        break;
                    }
                }
            }

            if (typeLocalIndex == -1)
            {
                // Fallback: try to find the most common local variable used in typeof checks
                // This is a heuristic if the GetType pattern isn't found exactly as expected
            }

            // 2. Scan for if-else chain pattern
            // Pattern:
            // ldloc.s V_type
            // ldtoken T
            // call GetTypeFromHandle
            // call op_Equality
            // brfalse L_Next

            for (int i = 0; i < codes.Count; i++)
            {
                if (IsLoadLocal(codes[i], typeLocalIndex) &&
                    i + 4 < codes.Count &&
                    codes[i + 1].opcode == OpCodes.Ldtoken &&
                    codes[i + 2].opcode == OpCodes.Call && ((MethodInfo)codes[i + 2].operand).Name == "GetTypeFromHandle" &&
                    codes[i + 3].opcode == OpCodes.Call && ((MethodInfo)codes[i + 3].operand).Name == "op_Equality" &&
                    (codes[i + 4].opcode == OpCodes.Brfalse || codes[i + 4].opcode == OpCodes.Brfalse_S))
                {
                    var type = (Type)codes[i + 1].operand;
                    var jumpLabel = (Label)codes[i + 4].operand;

                    // The body starts at i + 5
                    var bodyStartInstruction = codes[i + 5];

                    // Ensure the body start instruction has a label
                    var bodyLabel = new Label();
                    if (bodyStartInstruction.labels.Count > 0)
                        bodyLabel = bodyStartInstruction.labels[0];
                    else
                        bodyStartInstruction.labels.Add(bodyLabel);

                    typeChecks.Add(new TypeCheckInfo
                    {
                        StartIndex = i,
                        EndIndex = i + 4, // Inclusive of brfalse
                        CheckedType = type,
                        BodyLabel = bodyLabel,
                        FailLabel = jumpLabel
                    });

                    // Skip the check instructions
                    i += 4;
                }
            }

            if (typeChecks.Count < 10) // Threshold to apply optimization
                return codes;

            // 3. Build the LUT
            var jumpTable = new List<Label>();

            // We need to group them by contiguous chains
            // But for now, let's assume one big chain or handle the first big one
            // A chain is contiguous if the FailLabel of one check points to the StartIndex of the next check

            var chains = new List<List<TypeCheckInfo>>();
            List<TypeCheckInfo> currentChain = null;

            for (int i = 0; i < typeChecks.Count; i++)
            {
                if (currentChain == null)
                {
                    currentChain = new List<TypeCheckInfo> { typeChecks[i] };
                    chains.Add(currentChain);
                }
                else
                {
                    var currentChainLastCheck = currentChain.Last();
                    // Check if lastCheck.FailLabel points to typeChecks[i].StartIndex
                    if (LabelsMatch(codes, currentChainLastCheck.FailLabel, typeChecks[i].StartIndex))
                    {
                        currentChain.Add(typeChecks[i]);
                    }
                    else
                    {
                        currentChain = new List<TypeCheckInfo> { typeChecks[i] };
                        chains.Add(currentChain);
                    }
                }
            }

            // Process the largest chain
            var targetChain = chains.OrderByDescending(c => c.Count).FirstOrDefault();
            if (targetChain == null || targetChain.Count < 10)
                return codes;

            var lut = new Dictionary<Type, int>();
            for (int i = 0; i < targetChain.Count; i++)
            {
                lut[targetChain[i].CheckedType] = i;
                jumpTable.Add(targetChain[i].BodyLabel);
            }

            var lutId = LookupTables[lut.Count].Count;
            LookupTables.Add(lut);

            // 4. Emit the switch code
            // We replace the FIRST check with the switch logic
            // And NOP out the rest of the checks in the chain

            var firstCheck = targetChain.First();
            var lastCheck = targetChain.Last();
            var defaultLabel = lastCheck.FailLabel; // Where the last check jumps if it fails

            var newInstructions = new List<CodeInstruction>();

            // Load type
            newInstructions.Add(new CodeInstruction(OpCodes.Ldloc, typeLocalIndex)); // Or correct ldloc variant
            // Load lutId
            newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4, lutId));
            // Call GetTypeIndex
            newInstructions.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SwitchOptimizer), nameof(GetTypeIndex))));
            // Switch
            newInstructions.Add(new CodeInstruction(OpCodes.Switch, jumpTable.ToArray()));
            // Default case (jump to where the chain would go if all failed)
            newInstructions.Add(new CodeInstruction(OpCodes.Br, defaultLabel));

            // Insert new instructions at the start of the chain
            codes.InsertRange(firstCheck.StartIndex, newInstructions);

            // NOP out the original checks
            // We need to be careful about labels.
            // The labels jumping TO the start of the chain (if any) should now point to our new code.
            // The labels jumping TO the intermediate checks (from previous checks) are now irrelevant because we are bypassing them.
            // BUT, if there are other jumps into the middle of the chain (unlikely for if-else), we might break things.
            // Assuming standard if-else chain.

            // Adjust indices because we inserted code
            int offset = newInstructions.Count;

            foreach (var check in targetChain)
            {
                // We NOP out the instructions from StartIndex to EndIndex
                // But we must preserve labels on the StartIndex instruction, moving them to the NOP?
                // Actually, for the first check, the labels are already on the first instruction of our inserted code (because InsertRange puts them before).
                // Wait, InsertRange inserts *before* the index. So the original instruction at StartIndex is pushed down.
                // The labels attached to the original instruction at StartIndex stay with it.
                // We want the labels to be on the *new* first instruction.

                // Fix labels for the very first check
                if (check == firstCheck)
                {
                    var originalFirst = codes[firstCheck.StartIndex + offset];
                    if (originalFirst.labels.Count > 0)
                    {
                        codes[firstCheck.StartIndex].labels.AddRange(originalFirst.labels);
                        originalFirst.labels.Clear();
                    }
                }

                // NOP out the check instructions
                for (int k = check.StartIndex + offset; k <= check.EndIndex + offset; k++)
                {
                    codes[k].opcode = OpCodes.Nop;
                    codes[k].operand = null;
                }
            }

            codes.RecordCustomCode($"{originalMethod.Name}.patched");
            return codes;
        }

        // Helper method called by the injected IL
        public static int GetTypeIndex(Type type, int lutId)
        {
            return LookupTables[lutId].TryGetValue(type, out var index) ? index : -1;
        }

        private static bool IsStoreLocal(CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Stloc_0 ||
                   instruction.opcode == OpCodes.Stloc_1 ||
                   instruction.opcode == OpCodes.Stloc_2 ||
                   instruction.opcode == OpCodes.Stloc_3 ||
                   instruction.opcode == OpCodes.Stloc_S ||
                   instruction.opcode == OpCodes.Stloc;
        }

        private static bool IsLoadLocal(CodeInstruction instruction, int index)
        {
            if (index == 0 && instruction.opcode == OpCodes.Ldloc_0) return true;
            if (index == 1 && instruction.opcode == OpCodes.Ldloc_1) return true;
            if (index == 2 && instruction.opcode == OpCodes.Ldloc_2) return true;
            if (index == 3 && instruction.opcode == OpCodes.Ldloc_3) return true;

            if ((instruction.opcode == OpCodes.Ldloc_S || instruction.opcode == OpCodes.Ldloc) &&
                GetLocalIndex(instruction) == index) return true;

            return false;
        }

        private static int GetLocalIndex(CodeInstruction instruction)
        {
            if (instruction.opcode == OpCodes.Stloc_0 || instruction.opcode == OpCodes.Ldloc_0) return 0;
            if (instruction.opcode == OpCodes.Stloc_1 || instruction.opcode == OpCodes.Ldloc_1) return 1;
            if (instruction.opcode == OpCodes.Stloc_2 || instruction.opcode == OpCodes.Ldloc_2) return 2;
            if (instruction.opcode == OpCodes.Stloc_3 || instruction.opcode == OpCodes.Ldloc_3) return 3;

            if (instruction.operand is LocalBuilder lb) return lb.LocalIndex;
            if (instruction.operand is int i) return i;
            if (instruction.operand is byte b) return b;

            return -1;
        }

        private static bool LabelsMatch(List<CodeInstruction> codes, Label label, int targetIndex)
        {
            if (targetIndex >= codes.Count) return false;
            return codes[targetIndex].labels.Contains(label);
        }

        private class TypeCheckInfo
        {
            public int StartIndex;
            public int EndIndex;
            public Type CheckedType;
            public Label BodyLabel;
            public Label FailLabel;
        }
*/
    }
}