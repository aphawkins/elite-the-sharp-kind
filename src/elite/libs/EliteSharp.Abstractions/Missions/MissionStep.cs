// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Missions;

/// <summary>
/// What a mission wants to happen now: the stage it moves to, the message the
/// commander is shown on the way, and what the move is worth. The game applies
/// all three together, so a mission never pays for a move that does not
/// happen.
/// <para>
/// Only <see cref="MissionStages"/> can build one, so a step always names a
/// stage its mission declared and always moves forwards.
/// </para>
/// </summary>
public sealed record MissionStep
{
    internal MissionStep(string stage, MissionBriefing? briefing, MissionAward? award)
    {
        Stage = stage;
        Briefing = briefing;
        Award = award;
    }

    /// <summary>
    /// Gets the stage to move to, which is one of the mission's
    /// <see cref="MissionStages.Names"/> and comes later than the stage the
    /// commander was in.
    /// </summary>
    public string Stage { get; }

    /// <summary>
    /// Gets the message to put on screen, or null for a move the commander is
    /// not told about - a kill claimed mid-fight, say, where the reward waits
    /// for the next debrief.
    /// </summary>
    public MissionBriefing? Briefing { get; }

    /// <summary>
    /// Gets what the move is worth, or null when it is worth nothing. The game
    /// pays this as it records the stage.
    /// </summary>
    public MissionAward? Award { get; }
}
