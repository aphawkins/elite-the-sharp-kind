// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// Implemented by a mission that changes what the commander meets in open space
/// - the Thargoids that hunt the stolen plans, or the ship a mission sent the
/// commander after. Kept out of <see cref="IMission"/> so that a mission which
/// is only a message sequence carries none of it, and the game asks only the
/// missions with something to add.
/// <para>
/// The game makes traffic in two places and asks separately at each, so an
/// answer is always an answer to the question that was put. A mission with
/// nothing to say at one of them returns null there.
/// </para>
/// </summary>
public interface IMissionEncounters
{
    /// <summary>
    /// Ships this mission sends after the commander in its own right, on top of
    /// whatever traffic the system would have had. Asked on each encounter
    /// check while the commander is flying; the game rolls the odds.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">
    /// The stage the commander has reached in this mission, which is one of
    /// <see cref="IMission.Stages"/>.
    /// </param>
    /// <returns>The ambush to roll for, or null for none - the usual answer.</returns>
    public AmbushEncounter? Ambush(IMissionContext context, string stage);

    /// <summary>
    /// The ship this mission wants sent in place of the lone pirate the game is
    /// about to make. Asked only at that moment, so answering costs the
    /// commander no extra traffic: it changes who turns up, not how often.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">
    /// The stage the commander has reached in this mission, which is one of
    /// <see cref="IMission.Stages"/>.
    /// </param>
    /// <returns>The ship to send instead, or null to let the pirate come - the usual answer.</returns>
    public LoneWolfEncounter? LoneWolfSubstitute(IMissionContext context, string stage);
}
