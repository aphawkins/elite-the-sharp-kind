// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One missile indicator. Which sprite it draws is the game's decision, since
/// arming and targeting are game state, not layout.
/// </summary>
public enum MissileIndicator
{
    /// <summary>
    /// A missile aboard and not armed.
    /// </summary>
    Stowed = 0,

    /// <summary>
    /// Armed with nothing locked.
    /// </summary>
    Armed = 1,

    /// <summary>
    /// Armed and locked on.
    /// </summary>
    Locked = 2,
}
