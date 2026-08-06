// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Globalization;

namespace Useful.UI.Gallery;

/// <summary>
/// What a button in the gallery is bound to: one value, and a count of how
/// often it has been applied. Applying it is the press, and the counter going
/// up is the whole of what this button does - a real one would save a
/// commander or leave a screen from the same place.
/// </summary>
internal sealed class PressSetting : ISetting
{
    private int _presses;

    public string Name => _presses == 0
        ? "PRESS ENTER"
        : string.Create(CultureInfo.InvariantCulture, $"PRESSED {_presses}");

    public IReadOnlyList<string> Values => ["Press"];

    /// <summary>
    /// Gets or sets the chosen value, of which there is only ever the one.
    /// Setting it is the press, so the count rises whatever it is set to.
    /// </summary>
    public int SelectedIndex
    {
        get;

        // Being set at all is the press: there is one value, so which one was
        // chosen was never in question.
        set
        {
            field = value;
            _presses++;
        }
    }
}
