using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Shared.Patches;

// Proxies between PB scripts and game objects (SE-626 stage 3).
//
// Wrap returns one proxy per game object: an instance of a type emitted at first use that has
// the namespace and name of the object's class and implements exactly its script interfaces
// (the ones IsScriptInterface accepts) by forwarding to the object. So casts, ==, dictionary keys,
// GetType().Name and ToString() behave for a script as with the object itself.
//
// Game objects crossing the boundary are converted on every call: return values, out and ref
// parameters and List<T> contents are wrapped, arguments are unwrapped, and Func<T, TResult> and
// Action<T> arguments are wrapped so the script's delegate sees proxies. Only values typed as a
// script interface (or object) are wrapped: a game class the whitelist exposes by its class, such
// as MyResourceSinkComponent, reaches the script as itself. Value types and strings pass through.
//
// Each interface member gets its ScriptPolicy when the proxy type is emitted. While a ScriptWrites
// log is active on the thread (a script running off the main thread), a queued member records the
// call in the log instead of making it, a getter whose setter is queued first returns the value
// set in this run, and a main-thread member throws. Otherwise every member calls the game.
public static class ScriptProxies
{
    // Decides which interfaces of a game class a proxy implements; set by the plugin
    public static Func<Type, bool> IsScriptInterface = t =>
        t.Namespace?.EndsWith(".Ingame") == true;

    private static readonly ConditionalWeakTable<object, ProxyBase> Proxies = new();
    private static readonly ConcurrentDictionary<Type, Func<object, ProxyBase>> Factories = new();
    private static readonly object EmitLock = new();
    private static ModuleBuilder module;

    public static object Wrap(object target)
    {
        if (target == null || target is ProxyBase || target is string || target is Delegate)
            return target;
        var factory = Factories.GetOrAdd(target.GetType(), Emit);
        return factory == null ? target : Proxies.GetValue(target, t => factory(t));
    }

    // Emits the proxy type of a game class ahead of use; false if the class has no script interfaces
    public static bool Prepare(Type type) => Factories.GetOrAdd(type, Emit) != null;

    public static object Unwrap(object value) => value is ProxyBase proxy ? proxy.Target : value;

    public static T WrapAs<T>(object target) => Wraps(typeof(T)) ? (T)Wrap(target) : (T)target;

    // Values of a script interface type, or of no particular type, become proxies
    private static bool Wraps(Type type) =>
        type == typeof(object) || type.IsInterface && IsScriptInterface(type);

    public static T UnwrapAs<T>(object value) => (T)Unwrap(value);

    // After the game filled a list the script passed in
    public static void WrapList<T>(List<T> list)
    {
        if (list == null || !Wraps(typeof(T)))
            return;
        for (var i = 0; i < list.Count; i++)
            list[i] = (T)Wrap(list[i]);
    }

    public static Func<T, TResult> WrapFunc<T, TResult>(Func<T, TResult> func) =>
        func == null || !Wraps(typeof(T)) ? func : x => func(WrapAs<T>(x));

    public static Action<T> WrapAction<T>(Action<T> action) =>
        action == null || !Wraps(typeof(T)) ? action : x => action(WrapAs<T>(x));

    // The queued members, by the index the proxies record them with
    private static readonly List<(Action<object, object[]> Call, bool Setter)> Writers =
        new List<(Action<object, object[]>, bool)>();

    // Locked: a worker may emit a proxy type, which adds writers, while others record calls
    public static bool IsSetter(int writer)
    {
        lock (Writers)
            return Writers[writer].Setter;
    }

    // Makes a recorded call, see ScriptWrites.Replay
    public static void Write(int writer, object target, object[] args)
    {
        Action<object, object[]> call;
        lock (Writers)
            call = Writers[writer].Call;
        call(target, args);
    }

    private static int AddWriter(Type iface, MethodInfo method)
    {
        var target = Expression.Parameter(typeof(object));
        var args = Expression.Parameter(typeof(object[]));
        var call = Expression.Call(
            Expression.Convert(target, iface),
            method,
            method
                .GetParameters()
                .Select(
                    (p, i) =>
                        Expression.Convert(
                            Expression.ArrayIndex(args, Expression.Constant(i)),
                            p.ParameterType
                        )
                )
        );
        lock (Writers)
        {
            Writers.Add(
                (
                    Expression.Lambda<Action<object, object[]>>(call, target, args).Compile(),
                    method.IsSpecialName && method.Name.StartsWith("set_")
                )
            );
            return Writers.Count - 1;
        }
    }

    // A queued member must return nothing and take plain arguments; anything else stays on the
    // main thread
    // The policy the proxies apply: a queued member must be an interface member the proxy can
    // record, and a queued method returning bool returns true, as the game's WriteText does
    public static MemberPolicy PolicyOf(MethodInfo method)
    {
        var policy = ScriptPolicy.Of(method);
        if (
            policy == MemberPolicy.Queue
            && (
                method.DeclaringType?.IsInterface != true
                || (method.ReturnType != typeof(void) && method.ReturnType != typeof(bool))
                || method.IsGenericMethodDefinition
                || method
                    .GetParameters()
                    .Any(p =>
                        p.ParameterType.IsByRef
                        || typeof(Delegate).IsAssignableFrom(p.ParameterType)
                    )
            )
        )
            return MemberPolicy.Main;
        if (
            policy == MemberPolicy.StandIn
            && (
                method.DeclaringType?.IsInterface != true
                || method.IsGenericMethodDefinition
                || StandInMethods.Of(method) == null
            )
        )
            return MemberPolicy.Main;
        return policy;
    }

    private static Func<object, ProxyBase> Emit(Type type)
    {
        if (!type.IsClass)
            return null;
        var interfaces = type.GetInterfaces()
            .Where(i => i.IsPublic && IsScriptInterface(i))
            .ToArray();
        if (interfaces.Length == 0)
            return null;

        lock (EmitLock)
        {
            module ??= AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName("ScriptProxies"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("ScriptProxies");
            var proxy = module.DefineType(
                UniqueName(type.FullName),
                TypeAttributes.Public | TypeAttributes.Sealed,
                typeof(ProxyBase),
                interfaces
            );

            var baseCtor = typeof(ProxyBase).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(object) },
                null
            );
            var ctor = proxy.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(object) }
            );
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseCtor);
            il.Emit(OpCodes.Ret);

            // Queued setters first, so their getters know where to read the value back from
            var writers = new Dictionary<MethodInfo, int>();
            foreach (var iface in interfaces)
            foreach (var method in iface.GetMethods())
            {
                if (PolicyOf(method) == MemberPolicy.Queue)
                    writers[method] = AddWriter(iface, method);
            }

            foreach (var iface in interfaces)
            foreach (var method in iface.GetMethods())
                Forward(proxy, iface, method, writers);

            var created = proxy.CreateTypeInfo().AsType();
            var constructor = created.GetConstructor(new[] { typeof(object) });
            return target => (ProxyBase)constructor.Invoke(new[] { target });
        }
    }

    // The original full name, so GetType().Name and GetType().ToString() read the same;
    // a second class of the same name (another assembly) gets a suffix
    private static readonly HashSet<string> UsedNames = new();

    private static string UniqueName(string name)
    {
        var unique = name;
        for (var n = 2; !UsedNames.Add(unique); n++)
            unique = $"{name}`{n}";
        return unique;
    }

    // An explicit implementation of the interface method that converts the arguments, calls the
    // target and converts the result
    private static void Forward(
        TypeBuilder proxy,
        Type iface,
        MethodInfo method,
        Dictionary<MethodInfo, int> writers
    )
    {
        var parameters = method.GetParameters();
        var impl = proxy.DefineMethod(
            $"{iface.FullName}.{method.Name}",
            MethodAttributes.Private
                | MethodAttributes.Virtual
                | MethodAttributes.Final
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
        );

        // A generic method gets the same type parameters, and its signature uses them
        Type[] generics = Type.EmptyTypes;
        if (method.IsGenericMethodDefinition)
        {
            var args = method.GetGenericArguments();
            var builders = impl.DefineGenericParameters(args.Select(a => a.Name).ToArray());
            for (var i = 0; i < args.Length; i++)
            {
                builders[i].SetGenericParameterAttributes(args[i].GenericParameterAttributes);
                var constraints = args[i].GetGenericParameterConstraints();
                var baseConstraint = constraints.FirstOrDefault(c => !c.IsInterface);
                if (baseConstraint != null)
                    builders[i].SetBaseTypeConstraint(baseConstraint);
                builders[i]
                    .SetInterfaceConstraints(constraints.Where(c => c.IsInterface).ToArray());
            }
            generics = builders.Cast<Type>().ToArray();
        }

        Type Bind(Type t) =>
            Substitute(
                t,
                method.IsGenericMethodDefinition ? method.GetGenericArguments() : null,
                generics
            );
        var parameterTypes = parameters.Select(p => Bind(p.ParameterType)).ToArray();
        var returnType = Bind(method.ReturnType);
        impl.SetParameters(parameterTypes);
        impl.SetReturnType(returnType);
        for (var i = 0; i < parameters.Length; i++)
            impl.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);

        var target = method.IsGenericMethodDefinition ? method.MakeGenericMethod(generics) : method;
        var il = impl.GetILGenerator();
        EmitPolicy(il, iface, method, parameterTypes, returnType, writers);

        // By-ref arguments go through locals holding the unwrapped value
        var locals = new LocalBuilder[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            if (!parameterTypes[i].IsByRef)
                continue;
            var element = parameterTypes[i].GetElementType();
            locals[i] = il.DeclareLocal(element);
            if (!parameters[i].IsOut)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Ldobj, element);
                ConvertIn(il, element);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
        }

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, typeof(ProxyBase).GetField(nameof(ProxyBase.Target)));
        il.Emit(OpCodes.Castclass, iface);
        for (var i = 0; i < parameters.Length; i++)
        {
            if (locals[i] != null)
            {
                il.Emit(OpCodes.Ldloca, locals[i]);
                continue;
            }
            il.Emit(OpCodes.Ldarg, i + 1);
            ConvertIn(il, parameterTypes[i]);
        }
        // Off the main thread a stand-in takes the same arguments, the game object first
        if (PolicyOf(method) == MemberPolicy.StandIn && !writers.ContainsKey(method))
        {
            var standIn = StandInMethods.Of(method);
            var virtualCall = il.DefineLabel();
            var called = il.DefineLabel();
            il.Emit(
                OpCodes.Call,
                typeof(ScriptWrites).GetProperty(nameof(ScriptWrites.Active)).GetMethod
            );
            il.Emit(OpCodes.Brfalse, virtualCall);
            il.Emit(OpCodes.Call, standIn);
            il.Emit(OpCodes.Br, called);
            il.MarkLabel(virtualCall);
            il.Emit(OpCodes.Callvirt, target);
            il.MarkLabel(called);
        }
        else
            il.Emit(OpCodes.Callvirt, target);

        var result = returnType == typeof(void) ? null : il.DeclareLocal(returnType);
        if (result != null)
        {
            ConvertOut(il, returnType);
            il.Emit(OpCodes.Stloc, result);
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            if (locals[i] != null)
            {
                var element = parameterTypes[i].GetElementType();
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Ldloc, locals[i]);
                ConvertOut(il, element);
                il.Emit(OpCodes.Stobj, element);
            }
            else if (ListElement(parameterTypes[i]) is { } element)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Call, Helper(nameof(WrapList), element));
            }
        }

        if (result != null)
            il.Emit(OpCodes.Ldloc, result);
        il.Emit(OpCodes.Ret);
        proxy.DefineMethodOverride(impl, method);
    }

    // The part before the call to the game, which runs only while a ScriptWrites log is active
    private static void EmitPolicy(
        ILGenerator il,
        Type iface,
        MethodInfo method,
        Type[] parameterTypes,
        Type returnType,
        Dictionary<MethodInfo, int> writers
    )
    {
        var policy = writers.ContainsKey(method) ? MemberPolicy.Queue : PolicyOf(method);
        var readBack = policy == MemberPolicy.Pass ? ReadBackSetter(iface, method, writers) : -1;
        if (policy == MemberPolicy.Pass && readBack < 0 || policy == MemberPolicy.StandIn)
            return;

        var forward = il.DefineLabel();
        il.Emit(
            OpCodes.Call,
            typeof(ScriptWrites).GetProperty(nameof(ScriptWrites.Active)).GetMethod
        );
        il.Emit(OpCodes.Brfalse, forward);

        switch (policy)
        {
            case MemberPolicy.Main:
                il.Emit(OpCodes.Ldstr, $"{ScriptPolicy.TypeName(iface)}.{method.Name}");
                il.Emit(OpCodes.Call, typeof(ScriptWrites).GetMethod(nameof(ScriptWrites.Refuse)));
                il.Emit(OpCodes.Throw);
                break;

            case MemberPolicy.Queue:
                il.Emit(OpCodes.Ldc_I4, writers[method]);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, typeof(ProxyBase).GetField(nameof(ProxyBase.Target)));
                il.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    ConvertIn(il, parameterTypes[i]);
                    if (parameterTypes[i].IsValueType)
                        il.Emit(OpCodes.Box, parameterTypes[i]);
                    il.Emit(OpCodes.Stelem_Ref);
                }
                il.Emit(OpCodes.Call, typeof(ScriptWrites).GetMethod(nameof(ScriptWrites.Add)));
                if (returnType == typeof(bool))
                    il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                break;

            default:
                // A getter of a queued setter: the value set in this run, if any
                var value = il.DeclareLocal(typeof(object));
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, typeof(ProxyBase).GetField(nameof(ProxyBase.Target)));
                il.Emit(OpCodes.Ldc_I4, readBack);
                il.Emit(OpCodes.Ldloca, value);
                il.Emit(OpCodes.Call, typeof(ScriptWrites).GetMethod(nameof(ScriptWrites.TryRead)));
                il.Emit(OpCodes.Brfalse, forward);
                il.Emit(OpCodes.Ldloc, value);
                il.Emit(OpCodes.Unbox_Any, returnType);
                ConvertOut(il, returnType);
                il.Emit(OpCodes.Ret);
                break;
        }

        il.MarkLabel(forward);
    }

    // The writer index of the queued setter of the property a getter belongs to, or -1
    private static int ReadBackSetter(
        Type iface,
        MethodInfo getter,
        Dictionary<MethodInfo, int> writers
    )
    {
        if (
            !getter.IsSpecialName
            || !getter.Name.StartsWith("get_")
            || getter.GetParameters().Length != 0
        )
            return -1;
        var setter = iface.GetProperty(getter.Name.Substring(4))?.SetMethod;
        return setter != null && writers.TryGetValue(setter, out var index) ? index : -1;
    }

    // Replaces the interface method's type parameters with the proxy method's own
    private static Type Substitute(Type type, Type[] from, Type[] to)
    {
        if (from == null)
            return type;
        if (type.IsGenericParameter)
            return to[Array.IndexOf(from, type)];
        if (type.IsByRef)
            return Substitute(type.GetElementType(), from, to).MakeByRefType();
        if (type.IsArray)
            return Substitute(type.GetElementType(), from, to).MakeArrayType();
        if (type.IsGenericType)
            return type.GetGenericTypeDefinition()
                .MakeGenericType(
                    type.GetGenericArguments().Select(a => Substitute(a, from, to)).ToArray()
                );
        return type;
    }

    // Converts the value on the stack from the script's side to the game's side
    private static void ConvertIn(ILGenerator il, Type type)
    {
        // A delegate over a value type sees no game objects
        if (IsDelegate(type, typeof(Func<,>)) is { } func)
        {
            if (Converts(func[0]))
                il.Emit(OpCodes.Call, Helper(nameof(WrapFunc), func));
        }
        else if (IsDelegate(type, typeof(Action<>)) is { } action)
        {
            if (Converts(action[0]))
                il.Emit(OpCodes.Call, Helper(nameof(WrapAction), action));
        }
        else if (Converts(type))
            il.Emit(OpCodes.Call, Helper(nameof(UnwrapAs), type));
    }

    // Converts the value on the stack from the game's side to the script's side
    private static void ConvertOut(ILGenerator il, Type type)
    {
        if (Converts(type))
            il.Emit(OpCodes.Call, Helper(nameof(WrapAs), type));
    }

    // Reference types other than strings, delegates and the lists handled after the call
    private static bool Converts(Type type) =>
        type.IsGenericParameter
        || !type.IsValueType
            && type != typeof(string)
            && !typeof(Delegate).IsAssignableFrom(type)
            && ListElement(type) == null;

    private static Type ListElement(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
            ? type.GetGenericArguments()[0]
            : null;

    private static Type[] IsDelegate(Type type, Type definition) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == definition
            ? type.GetGenericArguments()
            : null;

    private static MethodInfo Helper(string name, params Type[] args) =>
        typeof(ScriptProxies).GetMethod(name).MakeGenericMethod(args);

    private static MethodInfo Helper(string name, Type arg) => Helper(name, new[] { arg });
}

// The base of every emitted proxy type
public abstract class ProxyBase
{
    public readonly object Target;

    protected ProxyBase(object target)
    {
        Target = target;
    }

    public override string ToString() => Target.ToString();
}
