// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// The whole of the game a mission may see, and all it may do is look. The
/// commander, the ship, the market and the universe are the game's own and stay
/// internal; a mission reads the few things below and says what it wants to
/// happen by handing back a <see cref="MissionStep"/>, which the game applies
/// itself. A mission that needs more than this is a reason to widen this
/// interface on purpose, not a reason to hand out the game's types.
/// </summary>
public interface IMissionContext
{
    /// <summary>
    /// Gets the commander's combat score, the running kill count the combat
    /// ratings are read off. Missions are usually offered at a rating, so they
    /// gate on the score that rating starts at.
    /// </summary>
    public int CombatScore { get; }

    /// <summary>
    /// Gets the galaxy the commander is in, counted from 0. A mission that
    /// belongs to one galaxy - or that must not be offered once the commander
    /// has jumped past it - tests this.
    /// </summary>
    public int GalaxyNumber { get; }

    /// <summary>
    /// Gets the system the commander is at, numbered as the galaxy numbers its
    /// systems: the one docked at, or, in flight, the one launched from, which
    /// is how the game tracks where the commander is. A mission that must
    /// happen somewhere in particular compares against this, and the same
    /// number is what the data screen asks about in
    /// <see cref="IMissionPlanetDescriptions.DescribePlanet(IMissionContext, string, int)"/>.
    /// </summary>
    public int CurrentPlanetNumber { get; }

    /// <summary>
    /// Gets a value indicating whether the commander is docked. Briefings are
    /// handed over at a station, and what is said about a system is only said
    /// to somebody standing on it.
    /// </summary>
    public bool IsDocked { get; }

    /// <summary>
    /// The stage the named mission has reached, so that one mission can require
    /// another to be finished first. Names are compared with
    /// <see cref="StringComparison.Ordinal"/>.
    /// </summary>
    /// <param name="missionName">The <see cref="IMission.Name"/> to look up.</param>
    /// <returns>
    /// One of that mission's <see cref="MissionStages.Names"/>, or null when no
    /// mission of that name is installed - which a mission should read as "the
    /// requirement cannot be met", not as "not started".
    /// </returns>
    public string? StageOf(string missionName);
}
