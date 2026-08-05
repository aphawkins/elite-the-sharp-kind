// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets.Tests;

// A setting held in memory, so a widget test can assert on what the widget
// wrote back without an application behind it.
internal sealed class FakeSetting(string name, params string[] values) : ISetting
{
    public string Name => name;

    public IReadOnlyList<string> Values { get; set; } = values;

    public int SelectedIndex { get; set; }
}
