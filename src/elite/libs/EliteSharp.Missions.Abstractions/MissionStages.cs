// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Immutable;

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// The stages a mission passes through, in the order it passes through them,
/// and the only place a <see cref="MissionStep"/> comes from. Checking a stage
/// name once, here, is what lets everything downstream take a stage name on
/// trust: a step naming a stage the mission never declared, or one that would
/// take the commander backwards, cannot be built at all.
/// <para>
/// Stage names are compared with <see cref="StringComparison.Ordinal"/>
/// throughout, here and in <see cref="IMissionContext.StageOf(string)"/>. They
/// are save-file keys, not prose: the same bytes, or a different stage.
/// </para>
/// </summary>
public sealed class MissionStages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissionStages"/> class,
    /// checking the names once so that nothing need check them again.
    /// </summary>
    /// <param name="names">
    /// The stage names in order. The first is the stage a commander who has
    /// never met the mission is in, so a fresh commander needs nothing
    /// recorded. Names must be distinct and none may be blank.
    /// </param>
    /// <exception cref="ArgumentException">
    /// There are no names, one is blank, or two are the same.
    /// </exception>
    public MissionStages(IEnumerable<string> names)
    {
        ArgumentNullException.ThrowIfNull(names);

        ImmutableArray<string> declared = [.. names];

        if (declared.IsEmpty)
        {
            throw new ArgumentException("A mission needs at least the stage it has not started in.", nameof(names));
        }

        if (declared.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("A stage name is a save-file key, so it cannot be blank.", nameof(names));
        }

        if (declared.Distinct(StringComparer.Ordinal).Count() != declared.Length)
        {
            throw new ArgumentException("Two stages with one name are one stage the save file cannot tell apart.", nameof(names));
        }

        Names = declared;
    }

    /// <summary>
    /// Gets the stage names in order, for the game to check a save file against
    /// - a commander part way through a mission that has since dropped the
    /// stage their save recorded.
    /// </summary>
    public ImmutableArray<string> Names { get; }

    /// <summary>
    /// Gets the stage a commander who has never met this mission is in, which
    /// is the first declared.
    /// </summary>
    public string NotStarted => Names[0];

    /// <summary>
    /// Where a stage sits in the order, so that the game can tell one stage
    /// from a later one without knowing what any of them mean.
    /// </summary>
    /// <param name="stage">The stage name to look up.</param>
    /// <returns>Its position, counted from 0, or -1 if this mission has no such stage.</returns>
    public int IndexOf(string stage) => Names.IndexOf(stage, 0, Names.Length, StringComparer.Ordinal);

    /// <summary>
    /// Builds a move that the commander is not told about - a kill claimed
    /// mid-fight, where the reward waits for the debrief that follows.
    /// </summary>
    /// <param name="currentStage">The stage the commander is in, as handed to the mission.</param>
    /// <param name="nextStage">The stage to move to, which must come later than <paramref name="currentStage"/>.</param>
    /// <returns>The step to hand back from the mission.</returns>
    /// <exception cref="ArgumentException">
    /// Either stage is not this mission's, or the move does not go forwards.
    /// </exception>
    public MissionStep Step(string currentStage, string nextStage)
        => Step(currentStage, nextStage, null, null);

    /// <summary>
    /// Builds a move that puts a message on screen and pays nothing.
    /// </summary>
    /// <param name="currentStage">The stage the commander is in, as handed to the mission.</param>
    /// <param name="nextStage">The stage to move to, which must come later than <paramref name="currentStage"/>.</param>
    /// <param name="briefing">The message to show.</param>
    /// <returns>The step to hand back from the mission.</returns>
    /// <exception cref="ArgumentException">
    /// Either stage is not this mission's, or the move does not go forwards.
    /// </exception>
    public MissionStep Step(string currentStage, string nextStage, MissionBriefing briefing)
        => Step(currentStage, nextStage, briefing, null);

    /// <summary>
    /// Builds the mission's answer to the game: move from the stage the
    /// commander is in to a later one, with the message and the reward that go
    /// with the move. Both stages must be this mission's, and the move must go
    /// forwards - a mission that could step to where it already is could
    /// collect the same reward twice.
    /// </summary>
    /// <param name="currentStage">The stage the commander is in, as handed to the mission.</param>
    /// <param name="nextStage">The stage to move to, which must come later than <paramref name="currentStage"/>.</param>
    /// <param name="briefing">The message to show, or null for a move the commander is not told about.</param>
    /// <param name="award">What the move is worth, or null for a move that pays nothing.</param>
    /// <returns>The step to hand back from the mission.</returns>
    /// <exception cref="ArgumentException">
    /// Either stage is not this mission's, or the move does not go forwards.
    /// </exception>
    public MissionStep Step(string currentStage, string nextStage, MissionBriefing? briefing, MissionAward? award)
    {
        int current = Position(currentStage, nameof(currentStage));
        int next = Position(nextStage, nameof(nextStage));

        return next > current
            ? new MissionStep(nextStage, briefing, award)
            : throw new ArgumentException("A mission only ever moves forwards.", nameof(nextStage));
    }

    private int Position(string stage, string parameterName)
    {
        int index = IndexOf(stage);

        return index >= 0
            ? index
            : throw new ArgumentException("This mission has no such stage.", parameterName);
    }
}
