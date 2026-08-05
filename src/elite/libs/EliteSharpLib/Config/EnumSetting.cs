// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Widgets;

namespace EliteSharpLib.Config;

/// <summary>
/// A setting backed by an enum, offering named values in the order they are
/// to be cycled.
/// <para>
/// Each value is paired with its own label rather than the labels being
/// indexed by the enum's ordinal. A screen does not always offer every member
/// - the game's Planet Style offers three of PlanetStyle's four, wireframe
/// being the engine's business rather than the game's - and it does not
/// always call them what the enum does. Pairing them means neither can be
/// assumed.
/// </para>
/// </summary>
/// <typeparam name="TEnum">The enum the setting stores.</typeparam>
/// <param name="name">The label shown against the value.</param>
/// <param name="choices">The values offered and what each is called, in cycling order.</param>
/// <param name="get">Reads the current value.</param>
/// <param name="set">Stores a new value and applies whatever follows from it.</param>
internal sealed class EnumSetting<TEnum>(
    string name,
    IReadOnlyList<(TEnum Value, string Label)> choices,
    Func<TEnum> get,
    Action<TEnum> set) : ISetting
    where TEnum : struct, Enum
{
    private readonly string[] _values = [.. choices.Select(choice => choice.Label)];

    public string Name => name;

    public IReadOnlyList<string> Values => _values;

    /// <summary>
    /// Gets or sets the current value's place in the offered list. A stored
    /// value the screen does not offer - which is reachable, since the config
    /// file can hold one - reads as the first rather than throwing on a row
    /// the commander only wanted to look at.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            TEnum current = get();
            for (int i = 0; i < choices.Count; i++)
            {
                if (EqualityComparer<TEnum>.Default.Equals(choices[i].Value, current))
                {
                    return i;
                }
            }

            return 0;
        }

        set
        {
            if (value >= 0 && value < choices.Count)
            {
                set(choices[value].Value);
            }
        }
    }
}
