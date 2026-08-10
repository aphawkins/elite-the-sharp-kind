// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.UI;

namespace EliteSharpLib.Config;

/// <summary>
/// A setting whose values are numbers rather than an enum's members - the
/// window scales a rendition offers being the one that is. The numbers are the
/// values, so nothing has to keep a label list beside them.
/// </summary>
/// <param name="name">The label shown against the value.</param>
/// <param name="values">The numbers, in cycling order.</param>
/// <param name="format">Turns one number into what the row shows.</param>
/// <param name="get">Reads the current value.</param>
/// <param name="set">Stores a new value.</param>
internal sealed class NumberSetting(
    string name,
    IReadOnlyList<int> values,
    Func<int, string> format,
    Func<int> get,
    Action<int> set) : ISetting
{
    private readonly string[] _labels = [.. values.Select(format)];

    public string Name => name;

    public IReadOnlyList<string> Values => _labels;

    /// <summary>
    /// Gets or sets the selected number's place in the list. A stored number
    /// this setting does not offer falls back to the first rather than leaving
    /// the screen with an index it cannot draw - it cannot happen while the
    /// value is pegged on the way in, but the row does not rely on that.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            int current = get();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == current)
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
