// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets;

/// <summary>
/// One thing a <see cref="ComboBox"/> is bound to: a named choice between
/// values, and which of them is current. The widget reads through this and
/// writes back through it, keeping no copy of its own, so what the screen
/// shows and what the application holds cannot drift apart.
/// <para>
/// Setting <see cref="SelectedIndex"/> is what applying a choice means: the
/// implementation is where the value is stored, where it is persisted, and
/// where anything that has to happen as a result happens. The widget knows
/// none of that.
/// </para>
/// </summary>
public interface ISetting
{
    /// <summary>
    /// Gets the label shown against the value.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the values this setting can take, as they are to be shown, in
    /// cycling order.
    /// </summary>
    public IReadOnlyList<string> Values { get; }

    /// <summary>
    /// Gets or sets which of <see cref="Values"/> is current. Setting it
    /// applies the choice.
    /// </summary>
    public int SelectedIndex { get; set; }
}
