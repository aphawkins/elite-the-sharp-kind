// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;

namespace EliteSharpLib.Missions;

/// <summary>
/// Running the Thargoid defence plans to Birera. The Navy only calls once the
/// Constrictor has been paid for, which <see cref="ConstrictorMission.Rewarded"/>
/// is what says. As with <see cref="ConstrictorMission"/>, this declares the
/// stages and nothing else yet;
/// <see cref="Views.ThargoidMissionController"/> still runs the sequence.
/// </summary>
internal sealed class ThargoidMission : IMission
{
    /// <inheritdoc cref="ConstrictorMission.Id"/>
    internal const string Id = "Thargoid";

    /// <summary>
    /// The mission has not been offered.
    /// </summary>
    internal const string None = "None";

    /// <summary>
    /// The Navy has asked the commander to report to Ceerdi.
    /// </summary>
    internal const string Summoned = "Summoned";

    /// <summary>
    /// The plans were handed over at Ceerdi and must reach Birera.
    /// </summary>
    internal const string CarryingPlans = "CarryingPlans";

    /// <summary>
    /// The plans reached Birera and the Navy energy unit has been fitted.
    /// </summary>
    internal const string Rewarded = "Rewarded";

    /// <inheritdoc/>
    public string Name => Id;

    /// <inheritdoc/>
    public MissionStages Stages => Declared;

    /// <inheritdoc cref="ConstrictorMission.Declared"/>
    internal static MissionStages Declared { get; } = new([None, Summoned, CarryingPlans, Rewarded]);

    /// <inheritdoc/>
    public MissionStep? Advance(IMissionContext context, string stage) => null;
}
