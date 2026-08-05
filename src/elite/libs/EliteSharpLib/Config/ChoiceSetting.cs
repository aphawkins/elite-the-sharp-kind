// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Widgets;

namespace EliteSharpLib.Config;

/// <summary>
/// A setting whose values are a list discovered at runtime rather than an
/// enum known at compile time - the installed renditions being the one that
/// is. The stored value is the label itself, so nothing has to map an index
/// back to a name.
/// </summary>
/// <param name="name">The label shown against the value.</param>
/// <param name="values">The labels, in cycling order.</param>
/// <param name="get">Reads the current value.</param>
/// <param name="set">Stores a new value and applies whatever follows from it.</param>
internal sealed class ChoiceSetting(
    string name,
    IReadOnlyList<string> values,
    Func<string> get,
    Action<string> set) : ISetting
{
    public string Name => name;

    public IReadOnlyList<string> Values => values;

    /// <summary>
    /// Gets or sets the selected value's place in the list. The stored value
    /// is always one of them - the game would not have started otherwise -
    /// but an unknown one falls back to the first rather than leaving this
    /// screen with an index it cannot draw.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (string.Equals(values[i], get(), StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return 0;
        }

        set
        {
            if (value >= 0 && value < values.Count)
            {
                set(values[value]);
            }
        }
    }
}
