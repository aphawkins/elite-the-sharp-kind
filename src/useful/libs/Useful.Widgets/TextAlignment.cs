// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Widgets;

/// <summary>
/// Where a widget's text sits within the widget's own bounds. Note that this
/// is relative to the widget, never to the screen.
/// </summary>
public enum TextAlignment
{
    /// <summary>
    /// Against the widget's left edge.
    /// </summary>
    Left = 0,

    /// <summary>
    /// Centred in the widget's width.
    /// </summary>
    Centre = 1,

    /// <summary>
    /// Against the widget's right edge.
    /// </summary>
    Right = 2,
}
