// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// Implemented by a mission that has something of its own to say about a system
/// on the planet data screen - the rumours about where the stolen ship was last
/// seen. Kept out of <see cref="IMission"/> so that the data screen asks only
/// the missions that ever answer.
/// </summary>
public interface IMissionPlanetDescriptions
{
    /// <summary>
    /// The line to print in place of the system's usual description, or null to
    /// leave the description alone - which is the answer for all but a handful
    /// of systems, and for every stage but the one that is looking.
    /// <para>
    /// The data screen shows any system the commander picks off the chart, not
    /// only the one under their feet. A rumour is somebody in this station
    /// talking, so a mission that means "what they are saying here" answers
    /// only when <see cref="IMissionContext.IsDocked"/> and
    /// <paramref name="planetNumber"/> is
    /// <see cref="IMissionContext.CurrentPlanetNumber"/>.
    /// </para>
    /// </summary>
    /// <param name="context">The game, seen through the mission facade.</param>
    /// <param name="stage">
    /// The stage the commander has reached in this mission, which is one of
    /// <see cref="IMission.Stages"/>.
    /// </param>
    /// <param name="planetNumber">
    /// The system being described, numbered as the galaxy numbers its systems.
    /// Which galaxy that is comes from
    /// <see cref="IMissionContext.GalaxyNumber"/>.
    /// </param>
    /// <returns>The mission's description, or null for none.</returns>
    public string? DescribePlanet(IMissionContext context, string stage, int planetNumber);
}
