using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Shared.Logging;
using Shared.Plugin;
using VRage.Render.Image;

namespace Shared.Patches;

// Decodes with the bundled ImageSharp and logs one line per image in debug builds: size,
// format, time and a hash of the decoded pixels. The offline comparison in the ledger note
// uses the same hash, which is how the byte-exact requirement is checked against the game's
// own decoder and an independent PNG reader.
public static class ImageLoader
{
    private static IPluginLogger Log => Common.Logger;

    public static IMyImage Load(Stream stream, bool oneChannel, bool headerOnly, string debugName)
    {
        var timer = Stopwatch.StartNew();
        var image = ImageSharpRuntime.Load(stream, oneChannel, headerOnly);
        timer.Stop();

        if (Log.IsDebugEnabled)
            LogLoad(image, headerOnly, debugName, timer.Elapsed.TotalMilliseconds);

        return image;
    }

    private static void LogLoad(IMyImage image, bool headerOnly, string debugName, double milliseconds)
    {
        if (image == null)
        {
            Log.Debug($"ImageSharp: unsupported image {debugName ?? "<unnamed>"} ({milliseconds:0.0} ms)");
            return;
        }

        Log.Debug($"ImageSharp: loaded {Describe(image, headerOnly, debugName)} in {milliseconds:0.0} ms");
    }

    // Same line for an image the game's own decoder produced (debug builds, fix off)
    public static void LogGameDecoder(IMyImage image, bool headerOnly, string debugName, double milliseconds)
    {
        if (image == null)
            Log.Debug($"ImageSharp: game decoder returned nothing for {debugName ?? "<unnamed>"} ({milliseconds:0.0} ms)");
        else
            Log.Debug($"ImageSharp: game decoder loaded {Describe(image, headerOnly, debugName)} in {milliseconds:0.0} ms");
    }

    private static string Describe(IMyImage image, bool headerOnly, string debugName)
    {
        var hash = headerOnly || image.Data == null ? "-" : Hash(image).ToString("x16");
        return $"{debugName ?? "<unnamed>"} {image.Size.X}x{image.Size.Y} {image.BitsPerPixel}bpp{(headerOnly ? " header" : "")} hash {hash}";
    }

    // FNV-1a over the raw pixel bytes, also implemented by the offline comparison tool
    public static ulong Hash(IMyImage image)
    {
        switch (image.Data)
        {
            case byte[] b:
                return Fnv1A(MemoryMarshal.AsBytes(b.AsSpan()));
            case ushort[] s:
                return Fnv1A(MemoryMarshal.AsBytes(s.AsSpan()));
            case uint[] u:
                return Fnv1A(MemoryMarshal.AsBytes(u.AsSpan()));
            default:
                return 0;
        }
    }

    private static ulong Fnv1A(ReadOnlySpan<byte> bytes)
    {
        var hash = 14695981039346656037UL;
        foreach (var b in bytes)
        {
            hash ^= b;
            hash *= 1099511628211UL;
        }

        return hash;
    }
}
