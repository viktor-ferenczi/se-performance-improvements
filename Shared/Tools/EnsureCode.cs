using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Shared.Tools;

// Attribute to mark prefix and suffix patch methods
// For usage examples please search this repo:
// https://github.com/viktor-ferenczi/performance-improvements
//
// ReSharper disable once ClassNeverInstantiated.Global
[AttributeUsage(AttributeTargets.Method)]
public class EnsureCode : Attribute
{
    // Allowed method body hashes in hexadecimal, multiple entries can be separated by a | (pipe) character
    private readonly string allowedHashes;

    private bool IsAllowed(string hash) => $"|{allowedHashes}|".Contains($"|{hash}|");

    public EnsureCode(string allowedHashes)
    {
        this.allowedHashes = allowedHashes;
    }

    public static IEnumerable<CodeChange> Verify()
    {
        var reflectedType = new StackTrace().GetFrame(1).GetMethod().ReflectedType;
        if (reflectedType == null)
            throw new Exception("Cannot determine the caller's assembly");

        var callingAssembly = reflectedType.Assembly;
        return Verify(callingAssembly);
    }

    private static IEnumerable<CodeChange> Verify(Assembly pluginAssembly)
    {
        return AccessTools.GetTypesFromAssembly(pluginAssembly).SelectMany(Verify);
    }

    // Verify only the patch types in the given Harmony patch category. Used when patches are
    // applied in phases (Harmony.PatchCategory): each phase verifies only the patches it is about
    // to apply, so patches whose target assembly is not loaded yet (handled in another phase) are
    // not resolved here. See Shared.Patches.PatchHelpers.
    public static IEnumerable<CodeChange> VerifyCategory(string category)
    {
        return VerifyFiltered(type => GetCategory(type) == category);
    }

    // Verify only the uncategorized patch types, mirroring Harmony.PatchAllUncategorized.
    // Companion to VerifyCategory for the dedicated server's early phase.
    public static IEnumerable<CodeChange> VerifyUncategorized()
    {
        return VerifyFiltered(type => string.IsNullOrEmpty(GetCategory(type)));
    }

    private static IEnumerable<CodeChange> VerifyFiltered(Func<Type, bool> typeFilter)
    {
        // The patches live in the same assembly as this class (the Shared project is compiled
        // into the plugin assembly), so resolve it directly rather than via the call stack.
        return AccessTools.GetTypesFromAssembly(typeof(EnsureCode).Assembly).Where(typeFilter).SelectMany(Verify);
    }

    private static string GetCategory(Type type)
    {
        return type.GetCustomAttributes<HarmonyPatchCategory>().FirstOrDefault()?.info.category;
    }

    private static IEnumerable<CodeChange> Verify(Type patchType)
    {
        return AccessTools.GetDeclaredMethods(patchType).SelectMany(Verify);
    }

    private static IEnumerable<CodeChange> Verify(MethodInfo patchMethod)
    {
        var validateAttribute = patchMethod.GetCustomAttributes<EnsureCode>().FirstOrDefault();
        if (validateAttribute == null)
            return Enumerable.Empty<CodeChange>();

        return validateAttribute.VerifyMethod(patchMethod);
    }

    private IEnumerable<CodeChange> VerifyMethod(MethodInfo patchMethod)
    {
        Plugin.Common.Logger.Debug($"Verifying patch method: {patchMethod.DeclaringType.Name}.{patchMethod.Name}");

        var methodPatch = patchMethod.GetCustomAttributes<HarmonyPatch>().FirstOrDefault();
        if (methodPatch == null)
            yield break;

        var patchType = patchMethod.DeclaringType;
        if (patchType == null)
            throw new Exception($"Method info has no DeclaringType: {patchMethod.Name}");

        var patchedType = methodPatch.info.declaringType ?? patchType.GetCustomAttributes<HarmonyPatch>().FirstOrDefault()?.info.declaringType;
        if (patchedType == null)
            throw new Exception($"Could not determine the patched type for patch method {patchType.Name}.{patchMethod.Name}");

        MethodInfo patchedMethod = null;
        ConstructorInfo patchedConstructor = null;
        switch (methodPatch.info.methodType)
        {
            case MethodType.Getter:
                patchedMethod = AccessTools.PropertyGetter(patchedType, methodPatch.info.methodName);
                break;

            case MethodType.Setter:
                patchedMethod = AccessTools.PropertySetter(patchedType, methodPatch.info.methodName);
                break;

            case MethodType.Constructor:
                patchedConstructor = AccessTools.Constructor(patchedType, methodPatch.info.argumentTypes);
                break;

            case MethodType.StaticConstructor:
                patchedConstructor = AccessTools.Constructor(patchedType, methodPatch.info.argumentTypes, true);
                break;

            default:
                patchedMethod = AccessTools.DeclaredMethod(patchedType, methodPatch.info.methodName, methodPatch.info.argumentTypes);
                break;
        }

        if (patchedMethod == null && patchedConstructor == null)
            throw new Exception($"Could not get patched method information for {patchType.Name}.{patchMethod.Name}");

        var methodBodyHash = (patchedMethod != null ? patchedMethod.HashBody() : patchedConstructor.HashBody()).ToString("x8");
        if (IsAllowed(methodBodyHash))
            yield break;

        yield return new CodeChange(patchedMethod, patchedConstructor, allowedHashes, methodBodyHash);
    }
}