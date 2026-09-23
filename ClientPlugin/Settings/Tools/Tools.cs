using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRageMath;

namespace ClientPlugin.Settings.Tools;

public static class Tools
{
    private static readonly Regex UpperCaseWordRegex = new Regex(@"[A-Z][a-z]*", RegexOptions.Compiled);

    public static string GetLabelOrDefault(string name, string label = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(name) && name.Trim().Length != 0);

        if (label != null)
            return label;

        var words = UpperCaseWordRegex.Matches(name).Cast<Match>().Select(m => m.Value).ToArray();
        Debug.Assert(words.Length != 0);

        for (var i = 1; i < words.Length; i++)
        {
            words[i] = words[i].ToLower();
        }

        return string.Join(" ", words);
    }

    // Breaks text into lines no wider than the given width, so a long description fits the
    // settings dialog instead of running off the screen. Neither MyGuiControlLabel nor the
    // tooltips wrap on their own: both measure and draw whatever they are given, and only
    // honour the line breaks already in the string.
    //
    // The game's own StringBuilder.Autowrap is not used because it measures the whole buffer
    // rather than the line being filled, so once a break has been made the limit it enforces
    // is the width of the first line rather than the width asked for.
    //
    // Line breaks already in the text are kept, so a description can group its own lines (one
    // per enum value, say) and have each of them wrapped in turn.
    public static string Wrap(string text, float width)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // A label of its own, to measure with the font, text scale and language scale the
        // real label will be drawn with instead of constants repeated here.
        var probe = new MyGuiControlLabel();
        var font = probe.Font;
        var scale = probe.TextScaleWithLanguage;

        var wrapped = new StringBuilder(text.Length);
        var line = new StringBuilder();

        foreach (var paragraph in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
        {
            foreach (var word in paragraph.Split(' '))
            {
                if (word.Length == 0)
                    continue;

                if (line.Length == 0)
                {
                    // The first word of a line is kept whatever its width: breaking inside a
                    // word would be worse than one line sticking out.
                    line.Append(word);
                    continue;
                }

                var candidate = line + " " + word;
                if (MyGuiManager.MeasureString(font, candidate, scale).X > width)
                {
                    wrapped.Append(line).Append('\n');
                    line.Clear().Append(word);
                }
                else
                {
                    line.Clear().Append(candidate);
                }
            }

            wrapped.Append(line).Append('\n');
            line.Clear();
        }

        // Drop the newline the last paragraph added
        wrapped.Length--;
        return wrapped.ToString();
    }

    private static readonly Regex RxHexColorRgbRegex = new Regex("([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})", RegexOptions.IgnoreCase);
    private static readonly Regex RxHexColorRgbaRegex = new Regex("([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})", RegexOptions.IgnoreCase);

    public static string ToHexStringRgb(this Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    public static string ToHexStringRgba(this Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }

    public static bool TryParseColorFromHexRgb(this string hex, out Color color)
    {
        var match = RxHexColorRgbRegex.Match(hex);
        if (!match.Success)
        {
            color = Color.Black;
            return false;
        }

        color = new Color(
            Convert.ToInt16(match.Groups[1].Value, 16),
            Convert.ToInt16(match.Groups[2].Value, 16),
            Convert.ToInt16(match.Groups[3].Value, 16),
            255);
        return true;
    }

    public static bool TryParseColorFromHexRgba(this string hex, out Color color)
    {
        var match = RxHexColorRgbaRegex.Match(hex);
        if (!match.Success)
        {
            color = Color.Transparent;
            return false;
        }

        color = new Color(
            Convert.ToInt16(match.Groups[1].Value, 16),
            Convert.ToInt16(match.Groups[2].Value, 16),
            Convert.ToInt16(match.Groups[3].Value, 16),
            Convert.ToInt16(match.Groups[4].Value, 16));
        return true;
    }
}