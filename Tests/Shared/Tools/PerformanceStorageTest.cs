using System;
using System.IO;
using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools;

public sealed class PerformanceStorageTest : IDisposable
{
    private readonly string root = Directory.CreateTempSubdirectory().FullName;
    private const string Key = "0123456789abcdef0123456789abcdef01234567.cache";

    public void Dispose() => Directory.Delete(root, recursive: true);

    private static void Write(string folder, string name, string content)
    {
        var path = Path.Combine(folder, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, content);
    }

    private string Prepare(string instance) =>
        PerformanceStorage.Prepare(Path.Combine(root, instance), message => Assert.Fail(message));

    [Theory]
    [InlineData("SpaceEngineers")]
    [InlineData("SpaceEngineersDedicated")]
    public void FreshInstallCreatesStorageAndBothCaches(string instance)
    {
        var storage = Prepare(instance);
        Assert.True(Directory.Exists(storage));
        Assert.Equal("Performance", PerformanceStorage.DefaultRoot);
        var cache = PerformanceStorage.CompilationCache(storage, PerformanceStorage.DefaultRoot);
        Assert.Equal(Path.Combine(root, instance, "Storage", "Performance", "Cache"), cache);
        foreach (var name in new[] { "CompiledMods", "CompiledInGameScripts" })
            Assert.True(Directory.Exists(PerformanceStorage.OpenCache(cache, name)));
    }

    [Fact]
    public void ConfiguredRootIsRelativeToStorageOrAbsolute()
    {
        var storage = Prepare("node");
        Assert.Equal(
            Path.Combine(storage, "Custom Folder", "Cache"),
            PerformanceStorage.CompilationCache(storage, "Custom Folder")
        );
        var shared = Path.Combine(root, "shared");
        Assert.Equal(
            Path.Combine(shared, "Cache"),
            PerformanceStorage.CompilationCache(storage, shared)
        );
        Assert.Equal(
            Path.Combine(storage, "Performance", "Cache"),
            PerformanceStorage.CompilationCache(storage, "")
        );
    }

    [Fact]
    public void MovesEntireLegacyTreeOnlyWhenDestinationIsMissing()
    {
        var legacy = Path.Combine(root, "client", "Performance");
        Write(legacy, "Cache/CompiledMods/" + Key, "warm");
        Write(legacy, "my notes.txt", "keep");
        var storage = Prepare("client");
        Assert.False(Directory.Exists(legacy));
        Assert.Equal(
            "warm",
            File.ReadAllText(Path.Combine(storage, "Performance/Cache/CompiledMods", Key))
        );
        Assert.Equal("keep", File.ReadAllText(Path.Combine(storage, "Performance/my notes.txt")));
        Assert.Equal(storage, Prepare("client"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExistingDestinationOnlyCleansPluginFilesFromLegacyTree(bool populateDestination)
    {
        var legacy = Path.Combine(root, "ds", "Performance");
        var destination = Directory
            .CreateDirectory(Path.Combine(root, "ds/Storage/Performance"))
            .FullName;
        if (populateDestination)
            Write(destination, "Cache/CompiledMods/" + Key, "new");
        var generated = new[]
        {
            "Cache/GameVersion.txt",
            "Debug/PluginVersion.txt",
            "Cache/CompiledMods/" + Key,
            "Cache/CompiledMods/" + Key + ".broken",
            "Cache/CompiledInGameScripts/" + Key + "." + Guid.NewGuid().ToString("N") + ".tmp",
            "Debug/Patch.Method.original.il",
            "Debug/Patch.Method.patched.il",
            "Cache/ImageSharp/SixLabors.ImageSharp.Performance.2.0.0.0.dll.tmp",
            "Cache/ImageSharp/System.Memory.dll",
        };
        var userFiles = new[]
        {
            "notes.txt",
            "Cache/CompiledMods/personal.cache",
            "Cache/CompiledMods/" + Key + ".my-backup.tmp",
            "Debug/notes.il",
            "Cache/ImageSharp/my.dll",
            "Other/" + Key,
        };
        foreach (var name in generated)
            Write(legacy, name, "old");
        foreach (var name in userFiles)
            Write(legacy, name, "keep");

        Prepare("ds");
        Prepare("ds");
        Assert.All(generated, name => Assert.False(File.Exists(Path.Combine(legacy, name))));
        Assert.All(
            userFiles,
            name => Assert.Equal("keep", File.ReadAllText(Path.Combine(legacy, name)))
        );
        Assert.False(Directory.Exists(Path.Combine(legacy, "Cache/CompiledInGameScripts")));
        if (populateDestination)
            Assert.Equal(
                "new",
                File.ReadAllText(Path.Combine(destination, "Cache/CompiledMods", Key))
            );
        else
            Assert.Empty(Directory.EnumerateFileSystemEntries(destination));
    }

    [Fact]
    public void RemovesEmptyLegacyTree()
    {
        var legacy = Path.Combine(root, "ds", "Performance");
        Directory.CreateDirectory(Path.Combine(root, "ds/Storage/Performance"));
        Write(legacy, "Cache/CompiledMods/" + Key, "old");
        Prepare("ds");
        Assert.False(Directory.Exists(legacy));
    }

    [Fact]
    public void LegacyLinkIntoActiveStorageIsPreserved()
    {
        if (OperatingSystem.IsWindows())
            return;
        var active = Directory
            .CreateDirectory(Path.Combine(root, "ds/Storage/Performance"))
            .FullName;
        Write(active, "Cache/CompiledMods/" + Key, "active");
        var old = Path.Combine(root, "ds/Performance");
        Directory.CreateSymbolicLink(old, active);
        Prepare("ds");
        Assert.Equal("active", File.ReadAllText(Path.Combine(active, "Cache/CompiledMods", Key)));
        Assert.True(Directory.Exists(old));
    }

    [Fact]
    public void ColdNodeCanOpenAnAlreadyPopulatedSharedRoot()
    {
        string shared = Path.Combine(root, "shared");
        string cacheA = PerformanceStorage.CompilationCache(Prepare("nodeA"), shared);
        string cacheB = PerformanceStorage.CompilationCache(Prepare("nodeB"), shared);
        foreach (string name in new[] { "CompiledMods", "CompiledInGameScripts" })
        {
            var path = PerformanceStorage.OpenCache(cacheA, name);
            Write(path, Key, "published");
            var second = PerformanceStorage.OpenCache(cacheB, name);
            Assert.Equal("published", File.ReadAllText(Path.Combine(second, Key)));
        }
    }

    [Fact]
    public void LegacyCleanupLeavesDirectoryLinksAndTheirContentsAlone()
    {
        if (OperatingSystem.IsWindows())
            return; // Creating a symlink may require elevated rights on Windows.
        var legacyCache = Directory
            .CreateDirectory(Path.Combine(root, "ds/Performance/Cache"))
            .FullName;
        Directory.CreateDirectory(Path.Combine(root, "ds/Storage/Performance"));
        var external = Path.Combine(root, "external");
        Write(external, Key, "keep");
        var link = Path.Combine(legacyCache, "CompiledMods");
        Directory.CreateSymbolicLink(link, external);
        Prepare("ds");
        Assert.True(Directory.Exists(link));
        Assert.Equal("keep", File.ReadAllText(Path.Combine(external, Key)));
    }

    [Fact]
    public void FailedMoveWarnsAndStartsWithEmptyCache()
    {
        // Linux stand-in for a file held open on Windows: the instance folder refuses the move.
        if (OperatingSystem.IsWindows() || Environment.UserName == "root")
            return;
        var instance = Path.Combine(root, "locked");
        Write(Path.Combine(instance, "Performance", "Cache", "CompiledMods"), Key, "old");
        Directory.CreateDirectory(Path.Combine(instance, "Storage"));
        File.SetUnixFileMode(instance, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        try
        {
            string warning = null;
            var storage = PerformanceStorage.Prepare(instance, message => warning = message);
            Assert.Equal(Path.Combine(instance, "Storage"), storage);
            Assert.Contains("Could not move", warning);
            Assert.True(
                File.Exists(Path.Combine(instance, "Performance", "Cache", "CompiledMods", Key))
            );
        }
        finally
        {
            File.SetUnixFileMode(
                instance,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute
            );
        }
    }
}
