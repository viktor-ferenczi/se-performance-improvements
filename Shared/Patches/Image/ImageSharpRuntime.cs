using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Shared.Logging;
using Shared.Plugin;
using VRage.Render.Image;
using VRageMath;

namespace Shared.Patches;

// Loads a newer ImageSharp than the game ships and decodes images with it, without
// ever referencing that ImageSharp at compile time.
//
// Why not a plain assembly reference: the game's own copy (1.0.0-beta0006, simple name
// SixLabors.ImageSharp) is loaded early by dotnet-compat and linux-compat, and on .NET
// (Core) a process holds one assembly per simple name. The bundled copy is therefore
// renamed with Cecil before loading (SixLabors.ImageSharp.Performance), which makes it a
// different assembly on every runtime, and this class talks to it through reflection.
// The handful of reflective calls happen once per image; the decoding itself runs at
// full speed inside the library.
//
// Load mirrors the pixel format selection of the game's MyImage.Load exactly, so the
// decoded arrays are byte-identical to what the game's own decoder produces (verified
// against an independent PNG reader, see the ledger note). Height maps feed voxel
// generation; a one bit difference would change planet terrain.
public static class ImageSharpRuntime
{
    private static IPluginLogger Log => Common.Logger;

    // File name of the bundled library, next to the plugin assembly (NuGet runtime file)
    private const string SourceFileName = "SixLabors.ImageSharp.dll";

    // Simple name given to the loaded copy, must differ from the game's
    private const string RenamedName = "SixLabors.ImageSharp.Performance";

    private delegate void CopyPixelDataTo(Span<byte> destination);

    private static Assembly assembly;
    private static MethodInfo identify; // Image.Identify(Stream)
    private static MethodInfo loadGeneric; // Image.Load<TPixel>(Stream)
    private static MethodInfo loadL8, loadL16, loadRgba32;
    private static PropertyInfo infoWidth, infoHeight, infoPixelType, infoMetadata;
    private static PropertyInfo pixelTypeBitsPerPixel;
    private static MethodInfo getPngMetadata; // ImageMetadata.GetFormatMetadata<PngMetadata>(PngFormat)
    private static object pngFormatInstance;
    private static PropertyInfo pngColorType;
    private static object pngColorTypeGrayscale;
    private static PropertyInfo imageWidth, imageHeight;
    private static MethodInfo copyPixelDataTo; // Image<TPixel>.CopyPixelDataTo(Span<byte>), per pixel type

    public static Version Version { get; private set; }

    public static bool IsLoaded => assembly != null;

    // Locates, renames, loads and binds the library. Returns false (after logging why)
    // when it is not usable, in which case the caller keeps the game's own decoder.
    public static bool TryLoad()
    {
        if (assembly != null)
            return true;

        try
        {
            var source = FindSource();
            if (source == null)
            {
                Log.Warning($"ImageSharp: {SourceFileName} not found next to the plugin, keeping the game's decoder");
                return false;
            }

            var renamed = RenameIntoCache(source);
            var loaded = Assembly.LoadFrom(renamed);
            Bind(loaded);

            assembly = loaded;
            Version = loaded.GetName().Version;
            Log.Info($"ImageSharp: loaded {Version} from {renamed}");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ImageSharp: failed to load the bundled ImageSharp, keeping the game's decoder");
            return false;
        }
    }

    private static string FindSource()
    {
        var location = typeof(ImageSharpRuntime).Assembly.Location;
        if (string.IsNullOrEmpty(location))
            return null;

        var dir = Path.GetDirectoryName(location);
        if (dir == null)
            return null;

        // Pulsar and Magnetar copy NuGet runtime files flat into the plugin's folder;
        // a search below it covers a runtimes/ layout as well
        var direct = Path.Combine(dir, SourceFileName);
        if (File.Exists(direct))
            return direct;

        return Directory.EnumerateFiles(dir, SourceFileName, SearchOption.AllDirectories).FirstOrDefault();
    }

    // The renamed copy is cached per library version; the rename runs only once per version.
    // The library's own dependencies (System.Memory and friends on .NET Framework) are
    // copied beside it, so the LoadFrom context finds them.
    private static string RenameIntoCache(string source)
    {
        var sourceVersion = AssemblyName.GetAssemblyName(source).Version;
        var cacheDir = Path.Combine(Common.CacheDir, "ImageSharp");
        Directory.CreateDirectory(cacheDir);

        var target = Path.Combine(cacheDir, $"{RenamedName}.{sourceVersion}.dll");
        if (!File.Exists(target))
        {
            var temp = target + ".tmp";
            using (var definition = AssemblyDefinition.ReadAssembly(source))
            {
                definition.Name.Name = RenamedName;
                definition.Name.PublicKey = Array.Empty<byte>();
                definition.Name.HasPublicKey = false;
                definition.MainModule.Name = RenamedName + ".dll";
                definition.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;
                definition.Write(temp);
            }

            File.Delete(target);
            File.Move(temp, target);
            Log.Info($"ImageSharp: renamed {source} to {target}");
        }

        var sourceDir = Path.GetDirectoryName(source);
        if (sourceDir == null)
            return target;

        foreach (var dependency in Directory.EnumerateFiles(sourceDir, "System.*.dll"))
        {
            var copy = Path.Combine(cacheDir, Path.GetFileName(dependency));
            if (!File.Exists(copy) || File.GetLastWriteTimeUtc(copy) < File.GetLastWriteTimeUtc(dependency))
                File.Copy(dependency, copy, true);
        }

        return target;
    }

    private static void Bind(Assembly loaded)
    {
        var image = Type(loaded, "SixLabors.ImageSharp.Image");
        identify = Method(image, "Identify", typeof(Stream));
        loadGeneric = image.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == "Load" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1 &&
                         m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Stream));
        loadL8 = loadGeneric.MakeGenericMethod(Type(loaded, "SixLabors.ImageSharp.PixelFormats.L8"));
        loadL16 = loadGeneric.MakeGenericMethod(Type(loaded, "SixLabors.ImageSharp.PixelFormats.L16"));
        loadRgba32 = loadGeneric.MakeGenericMethod(Type(loaded, "SixLabors.ImageSharp.PixelFormats.Rgba32"));
        imageWidth = Property(image, "Width");
        imageHeight = Property(image, "Height");

        var imageInfo = Type(loaded, "SixLabors.ImageSharp.IImageInfo");
        infoWidth = Property(imageInfo, "Width");
        infoHeight = Property(imageInfo, "Height");
        infoPixelType = Property(imageInfo, "PixelType");
        infoMetadata = Property(imageInfo, "Metadata");
        pixelTypeBitsPerPixel = Property(Type(loaded, "SixLabors.ImageSharp.Formats.PixelTypeInfo"), "BitsPerPixel");

        var pngFormat = Type(loaded, "SixLabors.ImageSharp.Formats.Png.PngFormat");
        pngFormatInstance = Property(pngFormat, "Instance").GetValue(null);
        var pngMetadata = Type(loaded, "SixLabors.ImageSharp.Formats.Png.PngMetadata");
        var imageMetadata = Type(loaded, "SixLabors.ImageSharp.Metadata.ImageMetadata");
        getPngMetadata = imageMetadata.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(m => m.Name == "GetFormatMetadata" && m.IsGenericMethodDefinition)
            .MakeGenericMethod(pngMetadata);
        pngColorType = Property(pngMetadata, "ColorType");
        pngColorTypeGrayscale = Enum.Parse(Type(loaded, "SixLabors.ImageSharp.Formats.Png.PngColorType"), "Grayscale");

        // Image<TPixel>.CopyPixelDataTo(Span<byte>) is bound per decoded image (see Decode),
        // the open generic type is enough here to fail early if it went missing
        var genericImage = Type(loaded, "SixLabors.ImageSharp.Image`1");
        copyPixelDataTo = genericImage.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(m => m.Name == "CopyPixelDataTo" && m.GetParameters()[0].ParameterType == typeof(Span<byte>));
    }

    private static Type Type(Assembly loaded, string name)
    {
        return loaded.GetType(name, true);
    }

    private static MethodInfo Method(Type type, string name, params Type[] parameters)
    {
        return type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, parameters, null)
               ?? throw new MissingMethodException(type.FullName, name);
    }

    private static PropertyInfo Property(Type type, string name)
    {
        return type.GetProperty(name) ?? throw new MissingMemberException(type.FullName, name);
    }

    // Same decision tree as MyImage.Load: a grayscale PNG is loaded as one channel even
    // when the caller did not ask for it; one channel images are 8 or 16 bit gray, anything
    // else is RGBA; unsupported combinations return null exactly like the original.
    public static IMyImage Load(Stream stream, bool oneChannel, bool headerOnly)
    {
        var info = identify.Invoke(null, new object[] { stream });
        stream.Position = 0;
        if (info == null)
            return null;

        var metadata = infoMetadata.GetValue(info);
        var png = getPngMetadata.Invoke(metadata, new[] { pngFormatInstance });
        if (!oneChannel)
            oneChannel = Equals(pngColorType.GetValue(png), pngColorTypeGrayscale);

        var width = (int)infoWidth.GetValue(info);
        var height = (int)infoHeight.GetValue(info);
        var bitsPerPixel = (int)pixelTypeBitsPerPixel.GetValue(infoPixelType.GetValue(info));

        if (headerOnly)
        {
            if (!oneChannel)
                return Header<uint>(width, height);

            switch (bitsPerPixel)
            {
                case 8:
                    return Header<byte>(width, height);
                case 16:
                    return Header<ushort>(width, height);
            }

            return null;
        }

        if (!oneChannel)
            return Decode<uint>(loadRgba32, stream);

        switch (bitsPerPixel)
        {
            case 8:
                return Decode<byte>(loadL8, stream);
            case 16:
                return Decode<ushort>(loadL16, stream);
        }

        return null;
    }

    private static MyImage<TData> Header<TData>(int width, int height) where TData : unmanaged
    {
        return new MyImage<TData>
        {
            Size = new Vector2I(width, height),
            Stride = width,
            Data = null,
        };
    }

    private static MyImage<TData> Decode<TData>(MethodInfo load, Stream stream) where TData : unmanaged
    {
        var image = load.Invoke(null, new object[] { stream });
        try
        {
            var width = (int)imageWidth.GetValue(image);
            var height = (int)imageHeight.GetValue(image);

            var data = new TData[width * height];
            var copy = (CopyPixelDataTo)Delegate.CreateDelegate(typeof(CopyPixelDataTo), image, CopyMethodFor(image.GetType()));
            copy(MemoryMarshal.AsBytes(data.AsSpan()));

            return new MyImage<TData>
            {
                Size = new Vector2I(width, height),
                Stride = width,
                Data = data,
            };
        }
        finally
        {
            (image as IDisposable)?.Dispose();
        }
    }

    private static readonly Dictionary<Type, MethodInfo> CopyMethods = new Dictionary<Type, MethodInfo>();

    private static MethodInfo CopyMethodFor(Type imageType)
    {
        lock (CopyMethods)
        {
            if (!CopyMethods.TryGetValue(imageType, out var method))
            {
                method = imageType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Single(m => m.Name == copyPixelDataTo.Name && m.GetParameters()[0].ParameterType == typeof(Span<byte>));
                CopyMethods[imageType] = method;
            }

            return method;
        }
    }
}
