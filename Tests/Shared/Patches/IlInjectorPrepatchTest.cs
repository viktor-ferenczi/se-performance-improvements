using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Mono.Cecil;
using Shared.Patches;
using Xunit;

namespace Tests.Shared.Patches;

// Patches the game's VRage.Library.dll in memory and checks that two scripts counting on two
// threads no longer share their instruction counter. Skipped when the game is not installed;
// SE_BIN64 overrides the Bin64 folder.
public class IlInjectorPrepatchTest
{
    private static readonly string Bin64 =
        Environment.GetEnvironmentVariable("SE_BIN64")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            OperatingSystem.IsWindows()
                ? @"C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64"
                : ".steam/steam/steamapps/common/SpaceEngineers/Bin64"
        );

    private static Type LoadPatched()
    {
        var path = Path.Combine(Bin64, "VRage.Library.dll");
        if (!File.Exists(path))
            return null;

        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(Bin64);
        using var assembly = AssemblyDefinition.ReadAssembly(
            path,
            new ReaderParameters { AssemblyResolver = resolver }
        );
        IlInjectorPrepatch.Prepatch(assembly);
        Assert.StartsWith("applied", IlInjectorPrepatch.Result);

        var image = new MemoryStream();
        assembly.Write(image);
        var context = new AssemblyLoadContext("IlInjectorPrepatchTest", isCollectible: true);
        context.Resolving += (c, name) =>
        {
            var file = Path.Combine(Bin64, name.Name + ".dll");
            return File.Exists(file) ? c.LoadFromAssemblyPath(file) : null;
        };
        image.Position = 0;
        return context.LoadFromStream(image).GetType("VRage.Library.Compiler.IlInjector", true);
    }

    [Fact]
    public void CountersArePerThread()
    {
        var injector = LoadPatched();
        if (injector == null)
            return;

        var begin = injector.GetMethod("BeginRunBlock");
        var count = (Action)
            Delegate.CreateDelegate(typeof(Action), injector.GetMethod("CountInstructions"));
        var isDead =
            (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), injector.GetMethod("IsDead"));
        var within =
            (Func<bool>)
                Delegate.CreateDelegate(typeof(Func<bool>), injector.GetMethod("IsWithinRunBlock"));

        // Both threads hold an open run block at the same time
        using var bothStarted = new Barrier(2);
        using var overflowDone = new ManualResetEventSlim();
        Exception smallError = null;
        bool smallDead = false,
            bigDead = true;
        int bigCount = -1,
            bigDepth = -1;

        var big = new Thread(() =>
        {
            var handle = (IDisposable)begin.Invoke(null, new object[] { 1000, 10000, false });
            bothStarted.SignalAndWait();
            for (var i = 0; i < 500; i++)
                count();
            overflowDone.Wait();
            for (var i = 0; i < 400; i++)
                count();
            bigDead = isDead();
            bigCount = (int)handle.GetType().GetProperty("InstructionCount").GetValue(handle);
            bigDepth = (int)handle.GetType().GetProperty("Depth").GetValue(handle);
            handle.Dispose();
        });
        var small = new Thread(() =>
        {
            var handle = (IDisposable)begin.Invoke(null, new object[] { 100, 10000, false });
            bothStarted.SignalAndWait();
            try
            {
                for (var i = 0; i < 200; i++)
                    count();
            }
            catch (Exception e)
            {
                smallError = e;
            }
            smallDead = isDead();
            handle.Dispose();
            overflowDone.Set();
        });
        big.Start();
        small.Start();
        big.Join();
        small.Join();

        Assert.Equal("ScriptOutOfInstructionsException", smallError?.GetType().Name);
        Assert.True(smallDead);
        Assert.False(bigDead);
        Assert.Equal(900, bigCount);
        Assert.Equal(1, bigDepth);
        // The test thread never began a run block
        Assert.False(within());
    }

    [Fact]
    public void TimeLimitIsWritable()
    {
        var injector = LoadPatched();
        if (injector == null)
            return;

        var field = injector.GetField(
            "m_maxAllowedTicks",
            BindingFlags.NonPublic | BindingFlags.Static
        );
        field.SetValue(null, 1234L);
        Assert.Equal(1234L, field.GetValue(null));
    }
}
