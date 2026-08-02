// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;
using EliteSharpLib.Types;

namespace EliteSharpLib.Missions;

/// <summary>
/// The game as a mission is allowed to see it. Everything a mission can read
/// goes through here, which is what keeps the commander, the ship, the market
/// and the universe out of a plugin's reach.
/// </summary>
internal sealed class MissionContext(GameState gameState, PlanetController planet) : IMissionContext
{
    private GalaxySeed _numberedPlanet = new();
    private int _numberedGalaxy = -1;
    private int _planetNumber = -1;

    /// <inheritdoc/>
    public int CombatScore => gameState.Cmdr.Score;

    /// <inheritdoc/>
    public int GalaxyNumber => gameState.Cmdr.GalaxyNumber;

    /// <summary>
    /// Gets the system the commander is at, by the number the galaxy gives it.
    /// <para>
    /// Missions are asked this on every encounter check, and finding a planet's
    /// number is a scan of all 256 of them, so the answer is remembered until
    /// the commander is somewhere else. Which system that is cannot be watched
    /// for - it is set by docking, by hyperspace and by loading a commander -
    /// so what is kept is the seed the number was worked out from, and a
    /// different seed is what asks the question again.
    /// </para>
    /// </summary>
    public int CurrentPlanetNumber
    {
        get
        {
            if (_planetNumber >= 0
                && _numberedGalaxy == gameState.Cmdr.GalaxyNumber
                && IsSameSeed(_numberedPlanet, gameState.DockedPlanet))
            {
                return _planetNumber;
            }

            _numberedPlanet = new(gameState.DockedPlanet);
            _numberedGalaxy = gameState.Cmdr.GalaxyNumber;
            _planetNumber = planet.FindPlanetNumber(gameState.Cmdr.Galaxy, gameState.DockedPlanet);

            return _planetNumber;
        }
    }

    /// <inheritdoc/>
    public bool IsDocked => gameState.IsDocked;

    /// <inheritdoc/>
    public string? StageOf(string missionName) => gameState.Cmdr.Missions.StageOf(missionName);

    private static bool IsSameSeed(GalaxySeed left, GalaxySeed right)
        => left.A == right.A
            && left.B == right.B
            && left.C == right.C
            && left.D == right.D
            && left.E == right.E
            && left.F == right.F;
}
