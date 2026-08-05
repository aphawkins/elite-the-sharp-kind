// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets;

/// <summary>
/// A widget's colours, one pair per <see cref="WidgetState"/>, and the font
/// they are drawn in. A rendition builds one of these from its own palette
/// and shares it across a screen's widgets, which is why the widgets need to
/// know nothing about palettes or tiers.
/// <para>
/// A transparent background is how a widget draws no block at all, which is
/// what every unselected row wants: there is no separate "has a background"
/// flag to get out of step with the colour.
/// </para>
/// </summary>
/// <param name="FontType">The font name to pass to the graphics surface.</param>
/// <param name="Normal">Available, cursor elsewhere.</param>
/// <param name="Selected">The cursor is on the widget.</param>
/// <param name="Disabled">Shown, but not available.</param>
/// <param name="SelectedDisabled">
/// The cursor is on it, but it is not available. Defaults to the selected
/// block with the disabled text colour, which is what a menu wants.
/// </param>
public sealed record WidgetStyle(
    string FontType,
    WidgetColors Normal,
    WidgetColors Selected,
    WidgetColors Disabled,
    WidgetColors? SelectedDisabled = null)
{
    /// <summary>
    /// The colours for a given state.
    /// </summary>
    /// <param name="state">Which of the widget's looks is being drawn.</param>
    /// <returns>That state's background and text colours.</returns>
    public WidgetColors Colors(WidgetState state) => state switch
    {
        WidgetState.Selected => Selected,
        WidgetState.Disabled => Disabled,
        WidgetState.SelectedDisabled => SelectedDisabled ?? new(Selected.Background, Disabled.Text),
        _ => Normal,
    };
}
