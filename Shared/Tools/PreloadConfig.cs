using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Shared.Tools;

// Reads a single bool option of the plugin config from disk before the game initializes, for
// the options that change how the Preloader patches game assemblies. The file is found the way
// the game finds its user data folder: -appdata on the client (config in Storage), -path on the
// dedicated server, else the default folder under the user's application data. A Windows
// service instance with no -path is not found, and then the option reads as false.
public static class PreloadConfig
{
    public static bool ReadBool(bool dedicated, string option)
    {
        var path = ConfigPath(dedicated);
        if (path == null || !File.Exists(path))
            return false;

        try
        {
            using var reader = XmlReader.Create(path);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == option)
                    return string.Equals(
                        reader.ReadElementContentAsString().Trim(),
                        "true",
                        StringComparison.OrdinalIgnoreCase
                    );
            }
        }
        catch (Exception)
        {
            // A broken file leaves the option off; the plugin reports the config problem itself
        }

        return false;
    }

    private static string ConfigPath(bool dedicated)
    {
        var args = Environment.GetCommandLineArgs();
        if (dedicated)
        {
            var custom = Argument(args, "-path")?.Trim('"');
            if (custom != null && !Path.IsPathRooted(custom))
                custom = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? "") ?? "",
                    custom
                );
            var folder =
                custom
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SpaceEngineersDedicated"
                );
            return Path.Combine(folder, "Performance.cfg");
        }

        var appData = Argument(args, "-appdata") ?? Argument(args, "--appdata");
        var userData =
            appData != null && !appData.StartsWith("-")
                ? Path.GetFullPath(Environment.ExpandEnvironmentVariables(appData))
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SpaceEngineers"
                );
        return Path.Combine(userData, "Storage", "Performance.cfg");
    }

    private static string Argument(string[] args, string name)
    {
        var i = Array.IndexOf(args, name);
        return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
    }
}
