// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;

namespace EliteSharpLib.Missions;

/// <summary>
/// The Navy's hunt for the stolen Constrictor. For now this declares the
/// stages and nothing else: the sequence still runs from
/// <see cref="Views.MissionBriefingController"/>, and moves in here when the
/// missions are ported. What it does today is give the save file and
/// <see cref="MissionProgress"/> a vocabulary of stages to check against, which
/// is what replaced the enum that used to be that vocabulary.
/// <para>
/// The names below are the old enum's names, so that save files written before
/// the missions became plugins still load.
/// </para>
/// </summary>
internal sealed class ConstrictorMission : IMission
{
    /// <summary>
    /// The name the save file records this mission under.
    /// </summary>
    internal const string Id = "Constrictor";

    /// <summary>
    /// The mission has not been offered.
    /// </summary>
    internal const string None = "None";

    /// <summary>
    /// The brief has been shown; the ship is out there to be found.
    /// </summary>
    internal const string Briefed = "Briefed";

    /// <summary>
    /// The Constrictor has been destroyed, but the reward not collected.
    /// </summary>
    internal const string Destroyed = "Destroyed";

    /// <summary>
    /// The debrief has been shown and the bounty paid.
    /// </summary>
    internal const string Rewarded = "Rewarded";

    /// <inheritdoc/>
    public string Name => Id;

    /// <inheritdoc/>
    public MissionStages Stages => Declared;

    /// <summary>
    /// Gets the stages, in the order the commander passes through them.
    /// </summary>
    internal static MissionStages Declared { get; } = new([None, Briefed, Destroyed, Rewarded]);

    /// <inheritdoc/>
    public MissionStep? Advance(IMissionContext context, string stage) => null;
}
