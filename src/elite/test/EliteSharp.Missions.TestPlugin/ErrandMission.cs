// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Missions;

namespace EliteSharp.Missions.TestPlugin;

/// <summary>
/// A mission with nothing to it beyond being found: three stages, a briefing
/// handed over at a station, and no encounters, kills or rumours. It exists to
/// prove that a class implementing <see cref="IMission"/> in an assembly that
/// knows nothing of the game or of MEF is discovered and can be asked to
/// advance.
/// </summary>
public sealed class ErrandMission : IMission
{
    /// <inheritdoc/>
    public string Name => "Errand";

    /// <inheritdoc/>
    public MissionStages Stages { get; } = new(["NotStarted", "Briefed", "Done"]);

    /// <inheritdoc/>
    public MissionStep? Advance(IMissionContext context, string stage)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.IsDocked && string.Equals(stage, Stages.NotStarted, StringComparison.Ordinal)
            ? Stages.Step(
                stage,
                "Briefed",
                new MissionBriefing { Paragraphs = ["Run this errand, Commander."] })
            : null;
    }
}
