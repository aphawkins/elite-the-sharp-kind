// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets.Gallery;

/// <summary>
/// A setting that is only ever itself: no config file, no side effects, just
/// somewhere for a <see cref="ComboBox"/> to read and write. The gallery is
/// showing the widget, not an application.
/// </summary>
/// <param name="name">The label shown against the value.</param>
/// <param name="values">The values it can take, in cycling order.</param>
internal sealed class MemorySetting(string name, params string[] values) : ISetting
{
    public string Name => name;

    public IReadOnlyList<string> Values => values;

    public int SelectedIndex { get; set; }
}
