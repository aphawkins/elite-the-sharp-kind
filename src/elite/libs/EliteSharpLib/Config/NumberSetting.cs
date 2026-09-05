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
/// <param name="values">
/// Reads the numbers on offer, in cycling order. A function rather than a
/// fixed list because what is on offer can depend on another setting not yet
/// saved - the window scales a rendition allows depend on which rendition is
/// selected, which this same screen also lets the commander change - so the
/// row has to re-read it on every draw rather than freeze it at construction.
/// </param>
/// <param name="format">Turns one number into what the row shows.</param>
/// <param name="get">Reads the current value.</param>
/// <param name="set">Stores a new value.</param>
internal sealed class NumberSetting(
    string name,
    Func<IReadOnlyList<int>> values,
    Func<int, string> format,
    Func<int> get,
    Action<int> set) : ISetting
{
    public string Name => name;

    public IReadOnlyList<string> Values => [.. values().Select(format)];

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
            IReadOnlyList<int> current = values();
            int selected = get();
            for (int i = 0; i < current.Count; i++)
            {
                if (current[i] == selected)
                {
                    return i;
                }
            }

            return 0;
        }

        set
        {
            IReadOnlyList<int> current = values();
            if (value >= 0 && value < current.Count)
            {
                set(current[value]);
            }
        }
    }
}
