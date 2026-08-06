// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.UI.Tests;

// A setting held in memory, so a control test can assert on what the control
// wrote back without an application behind it.
internal sealed class FakeSetting(string name, params string[] values) : ISetting
{
    public string Name => name;

    public IReadOnlyList<string> Values { get; set; } = values;

    public int SelectedIndex { get; set; }
}
