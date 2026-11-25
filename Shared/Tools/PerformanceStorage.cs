using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared.Tools;

internal static class PerformanceStorage
{
    internal const string DefaultRoot = "Performance";

    // Called before any cache or debug folder is created.
    internal static string Prepare(string userDataPath, Action<string> warn)
    {
        var storage = Path.Combine(userDataPath, "Storage");
        Directory.CreateDirectory(storage);
        var oldFolder = Path.Combine(userDataPath, DefaultRoot);
        var newFolder = Path.Combine(storage, DefaultRoot);
        if (!Directory.Exists(oldFolder) || IsLink(new DirectoryInfo(oldFolder)))
            return storage;

        if (!Directory.Exists(newFolder))
        {
            try
            {
                Directory.Move(oldFolder, newFolder);
                return storage;
            }
            catch (IOException) when (Directory.Exists(newFolder))
            {
                // Another startup created the destination first.
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // A running instance may hold an old file open. Start with an empty cache.
                warn(
                    $"Could not move {oldFolder} to {newFolder}, starting with an empty cache. Will clean up the old folder on a later startup. {ex.Message}"
                );
                return storage;
            }
        }

        try
        {
            // A linked destination may lead back into the legacy tree.
            if (IsLink(new DirectoryInfo(newFolder)) || IsLink(new DirectoryInfo(storage)))
                warn(
                    $"Leaving {oldFolder} in place because its destination uses a directory link."
                );
            else
                CleanLegacyDirectory(new DirectoryInfo(oldFolder), "");
        }
        catch (DirectoryNotFoundException)
        {
            // Another startup already moved or removed it.
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            warn($"Could not finish cleaning {oldFolder}. Will retry next startup. {ex.Message}");
        }

        return storage;
    }

    private static bool IsLink(FileSystemInfo entry) =>
        (entry.Attributes & FileAttributes.ReparsePoint) != 0;

    // Match the names produced by the old writers, only in their original directories.
    private static readonly Regex AssemblyCacheFile = new(
        @"\A[0-9a-f]{40}\.cache(?:\.broken|\.[0-9a-f]{32}\.tmp)?\z"
    );

    private static readonly Regex ImageSharpFile = new(
        @"\ASixLabors\.ImageSharp\.Performance\.[0-9]+(?:\.[0-9]+){1,3}\.dll(?:\.tmp)?\z"
    );

    internal static bool IsDebugDump(string file) =>
        file.EndsWith(".original.il", StringComparison.Ordinal)
        || file.EndsWith(".patched.il", StringComparison.Ordinal);

    private static bool IsLegacyFile(string folder, string file) =>
        folder switch
        {
            "Cache" => file == "GameVersion.txt",
            "Cache/CompiledMods" or "Cache/CompiledInGameScripts" => AssemblyCacheFile.IsMatch(
                file
            ),
            "Cache/ImageSharp" => ImageSharpFile.IsMatch(file)
                || file
                    is "System.Buffers.dll"
                        or "System.Memory.dll"
                        or "System.Numerics.Vectors.dll"
                        or "System.Runtime.CompilerServices.Unsafe.dll"
                        or "System.Text.Encoding.CodePages.dll",
            "Debug" => file == "PluginVersion.txt" || IsDebugDump(file),
            _ => false,
        };

    private static void CleanLegacyDirectory(DirectoryInfo directory, string relative)
    {
        if (IsLink(directory))
            return;

        foreach (var entry in directory.EnumerateFileSystemInfos())
        {
            if (IsLink(entry))
                continue;
            if (entry is DirectoryInfo child)
                CleanLegacyDirectory(
                    child,
                    relative.Length == 0 ? entry.Name : relative + "/" + entry.Name
                );
            else if (IsLegacyFile(relative, entry.Name))
                entry.Delete();
        }

        if (!directory.EnumerateFileSystemInfos().Any())
            directory.Delete();
    }

    internal static string CompilationCache(string storage, string configured)
    {
        var path = string.IsNullOrWhiteSpace(configured) ? DefaultRoot : configured;
        return Path.GetFullPath(Path.Combine(storage, path, "Cache"));
    }

    internal static string OpenCache(string cacheRoot, string name)
    {
        var folder = Path.Combine(cacheRoot, name);
        Directory.CreateDirectory(folder);
        // The game build is in the key. A new or upgraded node can join a warm cache
        // without clearing assemblies used by other nodes.
        var now = DateTime.UtcNow;
        RemoveExpired(folder, "*.cache", now.AddDays(-90));
        RemoveExpired(folder, "*.cache.*.tmp", now.AddHours(-1));
        return folder;
    }

    private static void RemoveExpired(string folder, string pattern, DateTime cutoff)
    {
        foreach (var path in Directory.EnumerateFiles(folder, pattern))
        {
            if (
                !AssemblyCacheFile.IsMatch(Path.GetFileName(path))
                || File.GetCreationTimeUtc(path) >= cutoff
            )
                continue;
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // Another node may hold the file open. Retry expiry on a later startup.
            }
        }
    }
}
