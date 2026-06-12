using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Shared.Tools;

public static class PreloaderHelpers
{
    public delegate bool CodeInstructionPredicate(Instruction ci);

    public static int FindFirstIndex(this Collection<Instruction> il, CodeInstructionPredicate predicate)
    {
        return il.Select((instruction, index) => new { Instruction = instruction, Index = index })
            .Where(pair => predicate(pair.Instruction))
            .Select(pair => pair.Index)
            .FirstOrDefault(i => i == -1); // Do NOT simplify this call on .NET 10, because that won't build on .Net Framework 4.8
    }

    public static int FindLastIndex(this Collection<Instruction> il, CodeInstructionPredicate predicate)
    {
        return il.Select((instruction, index) => new { Instruction = instruction, Index = index })
            .Where(pair => predicate(pair.Instruction))
            .Select(pair => pair.Index)
            .LastOrDefault(i => i == -1); // Do NOT simplify this call on .NET 10, because that won't build on .Net Framework 4.8
    }

    public static List<int> FindAllIndex(this Collection<Instruction> il, CodeInstructionPredicate predicate)
    {
        return il.Select((instruction, index) => new { Instruction = instruction, Index = index })
            .Where(pair => predicate(pair.Instruction))
            .Select(pair => pair.Index)
            .ToList();
    }

    public static string Hash(this Collection<Instruction> instructions)
    {
        return instructions.HashInstructions().CombineHashCodes().ToString("x8");
    }

    public static void VerifyCodeHash(this Collection<Instruction> il, MethodDefinition patchedMethod, string expected)
    {
        var actual = il.Hash();
        if (actual != expected) throw new Exception($"Prepatch: Detected code change in {patchedMethod.Name}: expected {expected}, actual {actual}");
    }

    private static string FormatCode(this Collection<Instruction> instructions)
    {
        var sb = new StringBuilder();

        var hash = instructions.Hash();
        sb.Append($"// {hash}\r\n");

        foreach (var instr in instructions)
        {
            sb.Append(instr.ToCodeLine());
            sb.Append("\r\n");
        }

        return sb.ToString();
    }

    private static string ToCodeLine(this Instruction instr)
    {
        var sb = new StringBuilder();

        sb.Append($"IL_{instr.Offset:0000}: {instr.OpCode}");

        var arg = FormatOperand(instr.Operand);
        if (arg.Length > 0)
        {
            sb.Append(' ');
            sb.Append(arg);
        }

        return sb.ToString();
    }

    private static string FormatOperand(object operand)
    {
        switch (operand)
        {
            case null:
                return "";

            case MethodReference methodRef:
                return FormatMethodReference(methodRef);

            case TypeReference typeRef:
                return FormatTypeReference(typeRef);

            case FieldReference fieldRef:
                return FormatFieldReference(fieldRef);

            case Instruction targetInstr:
                return $"IL_{targetInstr.Offset:x4}";

            case Instruction[] instructions:
                return string.Join(", ", instructions.Select(i => $"IL_{i.Offset:x4}"));

            case VariableDefinition varDef:
                return $"{varDef.Index} ({FormatTypeReference(varDef.VariableType)})";

            case ParameterDefinition paramDef:
                return $"{paramDef.Name} ({FormatTypeReference(paramDef.ParameterType)})";

            case string s:
                return s.ToLiteral();

            case float f:
                return f.ToString(CultureInfo.InvariantCulture);

            case double d:
                return d.ToString(CultureInfo.InvariantCulture);

            case int i:
                return i.ToString(CultureInfo.InvariantCulture);

            case long l:
                return l.ToString(CultureInfo.InvariantCulture);

            case sbyte sb:
                return sb.ToString(CultureInfo.InvariantCulture);

            case byte b:
                return b.ToString(CultureInfo.InvariantCulture);

            default:
#if NETCOREAPP
                return operand.ToString()?.Trim() ?? "null";
#else
                return operand.ToString().Trim();
#endif
        }
    }

    private static string FormatMethodReference(MethodReference methodRef)
    {
        var sb = new StringBuilder();

        // Return type
        sb.Append(FormatTypeReference(methodRef.ReturnType));
        sb.Append(' ');

        // Declaring type
        if (methodRef.DeclaringType != null)
        {
            sb.Append(FormatTypeReference(methodRef.DeclaringType));
            sb.Append("::");
        }

        // Method name
        sb.Append(methodRef.Name);

        // Generic parameters
        if (methodRef is GenericInstanceMethod genericMethod && genericMethod.GenericArguments.Count > 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", genericMethod.GenericArguments.Select(FormatTypeReference)));
            sb.Append('>');
        }

        // Parameters
        sb.Append('(');
        if (methodRef.HasParameters) sb.Append(string.Join(", ", methodRef.Parameters.Select(p => FormatTypeReference(p.ParameterType))));
        sb.Append(')');

        return sb.ToString();
    }

    private static string FormatTypeReference(TypeReference typeRef)
    {
        if (typeRef == null)
            return "void";

        // Handle generic instances
        if (typeRef is GenericInstanceType genericType)
        {
            var baseName = genericType.ElementType.Name;
            // Remove `1, `2 etc from generic type names
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex > 0)
                baseName = baseName.Substring(0, tickIndex);

            return $"{genericType.Namespace}.{baseName}<{string.Join(", ", genericType.GenericArguments.Select(FormatTypeReference))}>";
        }

        // Handle arrays
        if (typeRef is ArrayType arrayType) return FormatTypeReference(arrayType.ElementType) + "[]";

        // Handle by reference
        if (typeRef is ByReferenceType byRefType) return FormatTypeReference(byRefType.ElementType) + "&";

        // Handle pointers
        if (typeRef is PointerType pointerType) return FormatTypeReference(pointerType.ElementType) + "*";

        // Simple type
        if (!string.IsNullOrEmpty(typeRef.Namespace))
            return $"{typeRef.Namespace}.{typeRef.Name}";

        return typeRef.Name;
    }

    private static string FormatFieldReference(FieldReference fieldRef)
    {
        var sb = new StringBuilder();
        sb.Append(FormatTypeReference(fieldRef.FieldType));
        sb.Append(' ');
        if (fieldRef.DeclaringType != null)
        {
            sb.Append(FormatTypeReference(fieldRef.DeclaringType));
            sb.Append("::");
        }

        sb.Append(fieldRef.Name);
        return sb.ToString();
    }

    public static void RecordOriginalCode(this Collection<Instruction> instructions, MethodDefinition method, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        RecordCode(instructions, method, callerFilePath, callerMemberName, "original");
    }

    public static void RecordPatchedCode(this Collection<Instruction> instructions, MethodDefinition method, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        RecordCode(instructions, method, callerFilePath, callerMemberName, "patched");
    }

    public static void RecordCustomCode(this Collection<Instruction> instructions, MethodDefinition method, string suffix, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        RecordCode(instructions, method, callerFilePath, callerMemberName, suffix);
    }

    private static void RecordCode(Collection<Instruction> instructions, MethodDefinition method, string callerFilePath, string callerMemberName, string suffix)
    {
#if DEBUG
        Debug.Assert(callerFilePath.Length > 0);

        if (!File.Exists(callerFilePath))
            return;

        var dir = Path.GetDirectoryName(callerFilePath);
        if (dir == null)
            return;

        var name = method == null
            ? callerMemberName.EndsWith("Prepatch")
                ? callerMemberName.Substring(0, callerMemberName.Length - "Prepatch".Length)
                : callerMemberName
            : method.DeclaringType.Name.Split('`')[0] + "." + method.Name.Replace(".ctor", "Constructor").Replace(".cctor", "StaticConstructor");

        // For compiler-generated methods (containing non-alphanumeric chars other than _),
        // extract just the local function name, e.g., "<LoadFromFile>g__PerformLoad|0" -> "PerformLoad"
        if (name.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '.'))
        {
            var match = System.Text.RegularExpressions.Regex.Match(name, @"g__(\w+)");
            name = match.Success
                ? match.Groups[1].Value
                : new string(name.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '.').ToArray());
        }
        
        var path = Path.Combine(dir, $"{name}.{suffix}.il");

        var text = instructions.FormatCode();

        if (File.Exists(path) && File.ReadAllText(path) == text)
            return;

        File.WriteAllText(path, text);
#endif
    }
}