// 'Useful Libraries' - Andy Hawkins 2025.

using System.Globalization;

namespace Useful.Controls;

/// <summary>
/// Parses the text format read by <see cref="KeyScriptPlayer"/>: one event
/// per line, "&lt;tick&gt; &lt;Tap|Hold|Release&gt; &lt;ConsoleKey&gt;
/// [modifier[,modifier...]]" or "&lt;tick&gt; SaveFrame". Blank lines and
/// lines starting with '#' are ignored.
/// </summary>
public static class KeyScriptParser
{
    public static IReadOnlyList<KeyScriptEvent> Parse(string script)
    {
        Guard.ArgumentNull(script);

        List<KeyScriptEvent> events = [];
        foreach (string rawLine in script.Split('\n'))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            events.Add(ParseLine(line));
        }

        return events;
    }

    private static KeyScriptEvent ParseLine(string line)
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            throw new FormatException($"Key script line has too few fields: '{line}'.");
        }

        int tick = int.Parse(parts[0], CultureInfo.InvariantCulture);
        KeyScriptAction action = Enum.Parse<KeyScriptAction>(parts[1], ignoreCase: true);

        if (action == KeyScriptAction.SaveFrame)
        {
            return new KeyScriptEvent(tick, ConsoleKey.None, action);
        }

        if (parts.Length < 3)
        {
            throw new FormatException($"Key script line is missing a key: '{line}'.");
        }

        ConsoleKey key = Enum.Parse<ConsoleKey>(parts[2], ignoreCase: true);
        ConsoleModifiers modifiers = ConsoleModifiers.None;
        if (parts.Length > 3)
        {
            foreach (string modifier in parts[3].Split(','))
            {
                modifiers |= Enum.Parse<ConsoleModifiers>(modifier, ignoreCase: true);
            }
        }

        return new KeyScriptEvent(tick, key, action, modifiers);
    }
}
