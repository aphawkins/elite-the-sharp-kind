// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;

namespace EliteSharpLib.Missions;

/// <summary>
/// How far the commander has got in each mission. Stages are strings now,
/// because a compile-time enum cannot cover missions that arrive in an
/// assembly the game was never built against; this is what stands in for the
/// enum's guarantee. Every name that goes in is checked against what the
/// mission itself declared, so a stage nobody declared cannot be reached, and
/// everything reading a stage back can take it on trust.
/// <para>
/// Only stages that have been moved to are held. A mission nobody has started
/// reads as its own first stage rather than as nothing, so a fresh commander
/// needs no entries at all.
/// </para>
/// </summary>
internal sealed class MissionProgress(MissionRegistry registry)
{
    private readonly Dictionary<string, string> _stages = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the stages that have actually been moved to, for the save file to
    /// write. A mission that is not here has not been started, so a fresh
    /// commander writes nothing and a mission added later needs no entry.
    /// </summary>
    public IReadOnlyDictionary<string, string> Recorded => _stages;

    /// <summary>
    /// The stage the commander has reached in the named mission, or null when
    /// no mission of that name is installed - which is what a save file naming
    /// a removed plugin comes back as.
    /// </summary>
    /// <param name="missionName">The <see cref="IMission.Name"/> to look up.</param>
    /// <returns>One of that mission's stages, or null.</returns>
    public string? StageOf(string missionName)
        => registry.Find(missionName) is not { } mission
            ? null
            : _stages.TryGetValue(missionName, out string? stage) ? stage : mission.Stages.NotStarted;

    /// <summary>
    /// Whether the commander has reached exactly this stage of this mission.
    /// The question nearly every caller is really asking, and asking it this
    /// way keeps stage names from being compared with the wrong comparison.
    /// </summary>
    /// <param name="missionName">The <see cref="IMission.Name"/> to look up.</param>
    /// <param name="stage">The stage to test for.</param>
    /// <returns>Whether that is where the commander is.</returns>
    public bool IsAt(string missionName, string stage)
        => string.Equals(StageOf(missionName), stage, StringComparison.Ordinal);

    /// <summary>
    /// Records that the commander has reached a stage of a mission.
    /// </summary>
    /// <param name="missionName">The <see cref="IMission.Name"/> to record against.</param>
    /// <param name="stage">One of that mission's declared stages.</param>
    /// <exception cref="ArgumentException">
    /// No mission of that name is installed, or it never declared that stage.
    /// Either is a bug in the caller rather than anything a commander did.
    /// </exception>
    public void MoveTo(string missionName, string stage)
    {
        IMission mission = registry.Find(missionName)
            ?? throw new ArgumentException($"No mission called '{missionName}' is installed.", nameof(missionName));

        if (mission.Stages.IndexOf(stage) < 0)
        {
            throw new ArgumentException($"Mission '{missionName}' has no stage called '{stage}'.", nameof(stage));
        }

        _stages[missionName] = stage;
    }

    /// <summary>
    /// Forgets every stage, putting every mission back to not started - what a
    /// commander who has just started a new game has.
    /// </summary>
    public void Clear() => _stages.Clear();
}
