// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Globalization;

namespace Useful.UI.Gallery;

/// <summary>
/// The time of day, as a setting. Nothing writes to it and nothing tells the
/// label about it: the label asks its binding what to draw at every frame, and
/// this one answers with a different time each time it is asked.
/// <para>
/// Which is the whole point of it being here. Every other row in the gallery
/// would look the same if a control kept its own copy of its text; this one
/// would freeze.
/// </para>
/// </summary>
internal sealed class ClockSetting : ISetting
{
    public string Name => DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

    public IReadOnlyList<string> Values => [];

    public int SelectedIndex { get; set; } = -1;
}
