// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets;

/// <summary>
/// One state's look: the block behind the widget and the text on top of it.
/// </summary>
/// <param name="Background">
/// Filled across the widget's bounds before the text. Fully transparent means
/// no block is drawn, which is what an unselected row wants.
/// </param>
/// <param name="Text">The text colour.</param>
public readonly record struct WidgetColors(FastColor Background, FastColor Text)
{
    /// <summary>
    /// Gets a value indicating whether the background is worth drawing.
    /// </summary>
    public bool HasBackground => Background.A != 0;

    /// <summary>
    /// Text on no block at all - the common case for a row that is not the
    /// selected one.
    /// </summary>
    /// <param name="text">The text colour.</param>
    /// <returns>Colours with a transparent background.</returns>
    public static WidgetColors TextOnly(in FastColor text) => new(BaseColors.TransparentBlack, text);
}
