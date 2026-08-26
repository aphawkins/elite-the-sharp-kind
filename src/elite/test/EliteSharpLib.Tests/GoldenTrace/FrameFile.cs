// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using System.Text;

namespace EliteSharpLib.Tests.GoldenTrace;

// Reads and writes the committed frame-signature baselines, in the same
// line-based text as the traces and for the same reason: a change should be
// reviewable in a pull request without special tooling.
internal static class FrameFile
{
    internal const string Extension = ".frames";

    private const int SchemaVersion = 1;

    private const string Banner = "# elite frame signatures v";

    internal static string Write(TraceScenario scenario, IReadOnlyList<FrameSignature> frames)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ArgumentNullException.ThrowIfNull(frames);

        StringBuilder text = new();
        Line(text, Banner + SchemaVersion.ToString(CultureInfo.InvariantCulture));
        Line(text, "# scenario " + scenario.Name);

        foreach (FrameSignature frame in frames)
        {
            Line(text, string.Create(CultureInfo.InvariantCulture, $"F {frame.Tick} {frame.Hash}"));
            foreach (string row in frame.Thumbnail)
            {
                Line(text, "| " + row);
            }
        }

        return text.ToString();
    }

    internal static IReadOnlyList<FrameSignature> Read(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<FrameSignature> frames = [];
        int tick = 0;
        string hash = string.Empty;
        List<string> thumbnail = [];

        foreach (string rawLine in text.Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            if (line[0] == 'F')
            {
                Flush(frames, tick, hash, thumbnail);
                string[] f = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                tick = int.Parse(f[1], CultureInfo.InvariantCulture);
                hash = f[2];
                thumbnail = [];
            }
            else
            {
                // "| " prefix, so a row of leading dots cannot be mistaken
                // for blank and trimmed away by an editor.
                thumbnail.Add(line[2..]);
            }
        }

        Flush(frames, tick, hash, thumbnail);
        return frames;
    }

    private static void Flush(List<FrameSignature> frames, int tick, string hash, List<string> thumbnail)
    {
        if (hash.Length != 0)
        {
            frames.Add(new(tick, hash, thumbnail));
        }
    }

    private static void Line(StringBuilder text, string content)
    {
        _ = text.Append(content);
        _ = text.Append('\n');
    }
}
