// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.UI;

/// <summary>
/// Which of a control's looks to draw. The cursor being on a row and a row
/// being unavailable are the two things a menu screen varies, so they are
/// states rather than flags: a control is in exactly one of them.
/// </summary>
public enum ControlState
{
    /// <summary>
    /// Available, and the cursor is elsewhere.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The cursor is on this control.
    /// </summary>
    Selected = 1,

    /// <summary>
    /// Shown, but not available to the player.
    /// </summary>
    Disabled = 2,

    /// <summary>
    /// The cursor is on this control, but it is not available. A real state
    /// rather than a combination of the two above: the options menu leaves
    /// the cursor free to rest on a docked-only row while flying, and that
    /// row keeps the cursor's block while its text stays greyed out.
    /// </summary>
    SelectedDisabled = 3,
}
