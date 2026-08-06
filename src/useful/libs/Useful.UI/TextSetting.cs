// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.UI;

/// <summary>
/// A setting whose value is free text rather than a choice between values.
/// What a <see cref="TextBox"/> is bound to, and what a caption that never
/// changes is: a label needs a binding, and this is the smallest one there
/// is.
/// <para>
/// <see cref="Values"/> is empty because there is no list to choose from,
/// which is also what stops a <see cref="ComboBox"/> bound to one of these
/// from offering arrows to cycle with.
/// </para>
/// </summary>
/// <param name="text">The text it starts on.</param>
public sealed class TextSetting(string text = "") : ISetting
{
    /// <summary>
    /// Gets the text, which for a setting that is only text is also what it
    /// is called: there is no caption separate from the value.
    /// </summary>
    public string Name => Value;

    /// <summary>
    /// Gets the values this setting can take, of which there are none.
    /// </summary>
    public IReadOnlyList<string> Values => [];

    /// <summary>
    /// Gets or sets which value is current, of which there are none.
    /// </summary>
    public int SelectedIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets the text. Real storage rather than the choice the
    /// interface derives by default, since there is no choice to derive it
    /// from.
    /// </summary>
    public string Value { get; set; } = text;
}
