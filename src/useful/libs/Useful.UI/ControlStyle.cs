// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.UI;

/// <summary>
/// A control's colours, one pair per <see cref="ControlState"/>, and the font
/// they are drawn in. A rendition builds one of these from its own palette
/// and shares it across a screen's controls, which is why the controls need to
/// know nothing about palettes or tiers.
/// <para>
/// A transparent background is how a control draws no block at all, which is
/// what every unselected row wants: there is no separate "has a background"
/// flag to get out of step with the colour.
/// </para>
/// </summary>
/// <param name="FontType">The font name to pass to the graphics surface.</param>
/// <param name="Normal">Available, cursor elsewhere.</param>
/// <param name="Selected">The cursor is on the control.</param>
/// <param name="Disabled">Shown, but not available.</param>
/// <param name="SelectedDisabled">
/// The cursor is on it, but it is not available. Defaults to the selected
/// block with the disabled text colour, which is what a menu wants.
/// </param>
public sealed record ControlStyle(
    string FontType,
    ControlColors Normal,
    ControlColors Selected,
    ControlColors Disabled,
    ControlColors? SelectedDisabled = null)
{
    /// <summary>
    /// The same look with every block taken away, leaving the text colours as
    /// they are. What a control draws its own parts in once it has filled its
    /// bounds: a second fill over the first would paint out the block the
    /// selected state just drew.
    /// </summary>
    /// <returns>This style with transparent backgrounds throughout.</returns>
    public ControlStyle WithoutBackground() => new(
        FontType,
        ControlColors.TextOnly(Colors(ControlState.Normal).Text),
        ControlColors.TextOnly(Colors(ControlState.Selected).Text),
        ControlColors.TextOnly(Colors(ControlState.Disabled).Text),
        ControlColors.TextOnly(Colors(ControlState.SelectedDisabled).Text));

    /// <summary>
    /// The colours for a given state.
    /// </summary>
    /// <param name="state">Which of the control's looks is being drawn.</param>
    /// <returns>That state's background and text colours.</returns>
    public ControlColors Colors(ControlState state) => state switch
    {
        ControlState.Selected => Selected,
        ControlState.Disabled => Disabled,
        ControlState.SelectedDisabled => SelectedDisabled ?? new(Selected.Background, Disabled.Text),
        _ => Normal,
    };
}
