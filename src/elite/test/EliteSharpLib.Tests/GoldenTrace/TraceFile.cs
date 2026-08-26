// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Globalization;
using System.Text;
using EliteSharpLib.Views;

namespace EliteSharpLib.Tests.GoldenTrace;

// Reads and writes the committed baseline files.
//
// The format is line-based text rather than JSON so a baseline diff is
// readable in a pull request: a rate change that moves one counter shows up
// as one column changing down a run of lines. Every number is written
// invariant and to four decimals; numbers are compared numerically, so the
// four decimals bound how tight a tolerance can usefully be, not how exact
// the comparison is.
internal static class TraceFile
{
    internal const string Extension = ".trace";

    // Bumped whenever the record shape changes, so a stale baseline fails
    // with a clear message instead of a confusing field mismatch.
    private const int SchemaVersion = 1;

    private const string NumberFormat = "0.0000";

    private const string Banner = "# elite golden trace v";

    internal static string Write(TraceScenario scenario, IReadOnlyList<TraceSample> samples)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ArgumentNullException.ThrowIfNull(samples);

        // Each record is joined from its fields rather than appended piece by
        // piece, so the field order is one readable list per record type.
        StringBuilder text = new();
        AppendLine(text, [Banner + SchemaVersion.ToString(CultureInfo.InvariantCulture)]);
        AppendLine(text, ["# scenario", scenario.Name]);
        AppendLine(text, ["# seed", Int(scenario.RandomSeed)]);
        AppendLine(text, ["# ticks", Int(scenario.Ticks)]);

        foreach (TraceSample sample in samples)
        {
            AppendLine(
                text,
                [
                    "T",
                    Int(sample.Tick),
                    sample.Screen.ToString(),
                    Bit(sample.IsDocked),
                    Bit(sample.IsGameOver),
                    Int(sample.MCount),
                    Int(sample.MessageCount),
                    Num(sample.LaserTemp),
                    Num(sample.Roll),
                    Num(sample.Climb),
                    Num(sample.Speed),
                    Num(sample.Energy),
                    Num(sample.ShieldFront),
                    Num(sample.ShieldRear),
                    Num(sample.Fuel),
                    Num(sample.CabinTemperature),
                    Num(sample.Altitude),
                ]);

            foreach (TraceObject obj in sample.Objects)
            {
                AppendLine(
                    text,
                    [
                        "O",
                        Int(sample.Tick),
                        Int(obj.Slot),
                        obj.Type,
                        Num(obj.X),
                        Num(obj.Y),
                        Num(obj.Z),
                        Num(obj.RotX),
                        Num(obj.RotZ),
                        Flags(obj.Flags),
                    ]);
            }
        }

        return text.ToString();
    }

    internal static IReadOnlyList<TraceSample> Read(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<TraceSample> samples = [];
        List<TraceObject>? objects = null;
        TraceSample? pending = null;

        foreach (string rawLine in text.Split('\n'))
        {
            string line = rawLine.Trim('\r', ' ');
            if (line.Length == 0)
            {
                continue;
            }

            if (line[0] == '#')
            {
                CheckBanner(line);
                continue;
            }

            string[] f = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (f[0] == "T")
            {
                Flush(samples, ref pending);
                objects = [];
                pending = ReadSample(f, objects);
            }
            else
            {
                objects!.Add(ReadObject(f));
            }
        }

        Flush(samples, ref pending);
        return samples;
    }

    private static TraceObject ReadObject(string[] f) => new(
        ReadInt(f[2]),
        f[3],
        ReadNum(f[4]),
        ReadNum(f[5]),
        ReadNum(f[6]),
        ReadNum(f[7]),
        ReadNum(f[8]),
        f[9]);

    private static TraceSample ReadSample(string[] f, IReadOnlyList<TraceObject> objects) => new(
        ReadInt(f[1]),
        Enum.Parse<Screen>(f[2]),
        f[3] == "1",
        f[4] == "1",
        ReadInt(f[5]),
        ReadInt(f[6]),
        ReadNum(f[7]),
        ReadNum(f[8]),
        ReadNum(f[9]),
        ReadNum(f[10]),
        ReadNum(f[11]),
        ReadNum(f[12]),
        ReadNum(f[13]),
        ReadNum(f[14]),
        ReadNum(f[15]),
        ReadNum(f[16]),
        objects);

    private static void CheckBanner(string line)
    {
        if (line.StartsWith(Banner, StringComparison.Ordinal)
            && line != Banner + SchemaVersion.ToString(CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException(
                $"Baseline is '{line}' but this build writes v{SchemaVersion}. Regenerate the baselines.");
        }
    }

    private static void Flush(List<TraceSample> samples, ref TraceSample? pending)
    {
        if (pending is not null)
        {
            samples.Add(pending);
        }

        pending = null;
    }

    // ShipProperties is a flags enum, so its ToString separates with ", " -
    // and a space would split the field. Joined with '+' instead.
    private static string Flags(string flags) => flags.Replace(", ", "+", StringComparison.Ordinal);

    // A literal newline, never Environment.NewLine: a baseline committed on
    // one platform has to read identically on another.
    private static void AppendLine(StringBuilder text, string[] fields)
    {
        _ = text.AppendJoin(' ', fields);
        _ = text.Append('\n');
    }

    private static string Bit(bool value) => value ? "1" : "0";

    private static string Int(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Num(float value)

        // Negative zero and positive zero are the same number and must not
        // read as a difference in a diff.
        => (value == 0 ? 0f : value).ToString(NumberFormat, CultureInfo.InvariantCulture);

    private static float ReadNum(string value) => float.Parse(value, CultureInfo.InvariantCulture);

    private static int ReadInt(string value) => int.Parse(value, CultureInfo.InvariantCulture);
}
