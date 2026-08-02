// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Types;

/// <summary>
/// How far the commander has got through the Constrictor hunt.
/// </summary>
internal enum ConstrictorStage
{
    /// <summary>
    /// The mission has not been offered.
    /// </summary>
    None = 0,

    /// <summary>
    /// The brief has been shown; the ship is out there to be found.
    /// </summary>
    Briefed = 1,

    /// <summary>
    /// The Constrictor has been destroyed, but the reward not collected.
    /// </summary>
    Destroyed = 2,

    /// <summary>
    /// The debrief has been shown and the bounty paid.
    /// </summary>
    Rewarded = 3,
}
