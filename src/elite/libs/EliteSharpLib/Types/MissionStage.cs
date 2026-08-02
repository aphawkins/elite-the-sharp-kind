// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Types;

/// <summary>
/// How far the commander has got through the two missions, which play out in
/// this order. The numbers are the mission numbers the original game stored,
/// and the save file writes the names.
/// </summary>
internal enum MissionStage
{
    /// <summary>
    /// No mission is running.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Constrictor brief has been shown; the ship is out there to be found.
    /// </summary>
    ConstrictorBriefed = 1,

    /// <summary>
    /// The Constrictor has been destroyed, but the reward has not been collected.
    /// </summary>
    ConstrictorDestroyed = 2,

    /// <summary>
    /// The Constrictor debrief has been shown and the bounty paid.
    /// </summary>
    ConstrictorRewarded = 3,

    /// <summary>
    /// The Navy has asked the commander to report to Ceerdi.
    /// </summary>
    ThargoidSummoned = 4,

    /// <summary>
    /// The plans have been handed over at Ceerdi and must reach Birera.
    /// </summary>
    ThargoidCarryingPlans = 5,

    /// <summary>
    /// The plans reached Birera and the Navy energy unit has been fitted.
    /// </summary>
    ThargoidRewarded = 6,
}
