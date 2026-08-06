// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.UI;

/// <summary>
/// Where a control's text sits within the control's own bounds. Note that this
/// is relative to the control, never to the screen.
/// </summary>
public enum TextAlignment
{
    /// <summary>
    /// Against the control's left edge.
    /// </summary>
    Left = 0,

    /// <summary>
    /// Centred in the control's width.
    /// </summary>
    Centre = 1,

    /// <summary>
    /// Against the control's right edge.
    /// </summary>
    Right = 2,
}
