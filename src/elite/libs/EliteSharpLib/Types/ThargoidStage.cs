// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Types;

/// <summary>
/// How far the commander has got through the Thargoid plans run. The Navy
/// only calls once the Constrictor has been paid for, which
/// <see cref="ConstrictorStage.Rewarded"/> is what says.
/// </summary>
internal enum ThargoidStage
{
    /// <summary>
    /// The mission has not been offered.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Navy has asked the commander to report to Ceerdi.
    /// </summary>
    Summoned = 1,

    /// <summary>
    /// The plans were handed over at Ceerdi and must reach Birera.
    /// </summary>
    CarryingPlans = 2,

    /// <summary>
    /// The plans reached Birera and the Navy energy unit has been fitted.
    /// </summary>
    Rewarded = 3,
}
