// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// A mission the game can offer, written in its own assembly and found at
/// startup. Adding a mission is then a new assembly rather than edits spread
/// across the game.
/// <para>
/// A mission holds no state of its own: the stage it has reached belongs to the
/// commander and lives in the save file, and is handed back to the mission on
/// every call. What it may read of the game is <see cref="IMissionContext"/>,
/// and what it may change is whatever it puts in a <see cref="MissionStep"/>.
/// </para>
/// </summary>
public interface IMission
{
    /// <summary>
    /// Gets the name this mission is known by - in the save file, and when one
    /// mission gates on another. It has to stay put across releases: a renamed
    /// mission is a mission the save file no longer recognises.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the stages this mission passes through, which is also the only
    /// thing that can build a <see cref="MissionStep"/>. The game reads this
    /// once when it finds the mission and keeps it, so a mission builds it once
    /// and hands back the same one every time.
    /// </summary>
    public MissionStages Stages { get; }

    /// <summary>
    /// Decides whether the mission moves on. The game calls this wherever a
    /// mission can progress - on docking, and on the briefing screen - and the
    /// mission tests <paramref name="context"/> for the conditions it cares
    /// about.
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">
    /// The stage the commander has reached in this mission, which is one of
    /// <see cref="Stages"/>.
    /// </param>
    /// <returns>
    /// The step to take, built with <see cref="Stages"/>, or null when nothing
    /// applies - which is the usual answer.
    /// </returns>
    public MissionStep? Advance(IMissionContext context, string stage);
}
