// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.UI;

namespace EliteSharpLib.Config;

/// <summary>
/// A setting backed by a bool. Two values, the off one first, so that the
/// index is the flag.
/// </summary>
/// <param name="name">The label shown against the value.</param>
/// <param name="offLabel">What false is called on screen.</param>
/// <param name="onLabel">What true is called on screen.</param>
/// <param name="get">Reads the current value.</param>
/// <param name="set">Stores a new value and applies whatever follows from it.</param>
internal sealed class ToggleSetting(
    string name,
    string offLabel,
    string onLabel,
    Func<bool> get,
    Action<bool> set) : ISetting
{
    private readonly string[] _values = [offLabel, onLabel];

    public string Name => name;

    public IReadOnlyList<string> Values => _values;

    public int SelectedIndex
    {
        get => get() ? 1 : 0;
        set => set(value != 0);
    }
}
