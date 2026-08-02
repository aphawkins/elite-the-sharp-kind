// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// Implemented by a mission that is watching for a kill - the one ship it sent
/// the commander after. Kept out of <see cref="IMission"/> so that the combat
/// code tells only the missions that are listening, and a mission which is only
/// a message sequence carries none of it.
/// </summary>
public interface IMissionKills
{
    /// <summary>
    /// The commander has destroyed a ship. A mission that was waiting for this
    /// one moves on; the rest answer null, which is nearly always the answer.
    /// The move usually carries no briefing, since the commander is in the
    /// middle of a fight - the reward comes with the debrief that follows.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">
    /// The stage the commander has reached in this mission, which is one of
    /// <see cref="IMission.Stages"/>.
    /// </param>
    /// <param name="shipName">
    /// The ship destroyed, by the name the game's ship list knows it by.
    /// </param>
    /// <returns>The step to take, or null to stay put.</returns>
    public MissionStep? ShipDestroyed(IMissionContext context, string stage, string shipName);
}
