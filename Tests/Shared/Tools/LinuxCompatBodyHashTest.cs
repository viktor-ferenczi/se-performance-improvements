using System.Diagnostics;
using System.Reflection.Emit;
using HarmonyLib;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public class LinuxCompatBodyHashTest
{
    // LinuxCompat's preloader replaces MyWindowsSystem.get_ProcessPrivateMemory with this body on
    // Linux. MyWindowsSystemPatchForStats must allow its hash, or the failed EnsureCode check
    // skips every uncategorized patch of the plugin there (SE-1373).
    [Fact]
    public void ProcessPrivateMemory_LinuxCompatBody()
    {
        var body = new[]
        {
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(Process), nameof(Process.GetCurrentProcess))
            ),
            new CodeInstruction(
                OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Process), nameof(Process.PrivateMemorySize64))
            ),
            new CodeInstruction(OpCodes.Ret),
        };

        Assert.Equal("4a3cb737", body.HashInstructions().CombineHashCodes().ToString("x8"));
    }
}
