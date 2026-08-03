// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Missions;
using EliteSharpLib.Equipment;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Missions;

/// <summary>
/// The one place the game talks to its missions. Everywhere a mission can act -
/// the briefing screen, a kill, an encounter check, the planet data screen -
/// asks through here, so what a mission is allowed to change is exactly what a
/// <see cref="MissionStep"/> can carry, and applying one is written once.
/// </summary>
internal sealed class MissionRunner
{
    private readonly MissionContext _context;
    private readonly GameState _gameState;
    private readonly MissionRegistry _registry;
    private readonly PlayerShip _ship;
    private readonly Trade _trade;

    internal MissionRunner(
        GameState gameState,
        PlayerShip ship,
        Trade trade,
        MissionRegistry registry,
        PlanetController planet)
    {
        _gameState = gameState;
        _ship = ship;
        _trade = trade;
        _registry = registry;
        _context = new(gameState, planet);
    }

    /// <summary>
    /// Gets every mission, in the order they are asked.
    /// </summary>
    internal IEnumerable<IMission> All => _registry.All;

    /// <summary>
    /// Asks a mission whether it wants to move, without applying anything.
    /// </summary>
    /// <param name="mission">The mission to ask.</param>
    /// <returns>The step it wants, or null.</returns>
    internal MissionStep? Advance(IMission mission)
    {
        ArgumentNullException.ThrowIfNull(mission);

        return mission.Advance(_context, StageOf(mission));
    }

    /// <summary>
    /// Records a mission's move and pays what it is worth, together, so a
    /// reward can never be collected for a stage that was not taken.
    /// </summary>
    /// <param name="mission">The mission moving.</param>
    /// <param name="step">What it asked for.</param>
    internal void Apply(IMission mission, MissionStep step)
    {
        ArgumentNullException.ThrowIfNull(mission);
        ArgumentNullException.ThrowIfNull(step);

        _gameState.Cmdr.Missions.MoveTo(mission.Name, step.Stage);

        if (step.Award is not { } award)
        {
            return;
        }

        _gameState.Cmdr.Score += award.CombatScore;
        _trade.Credits += award.Credits;

        if (award.Equipment == MissionEquipment.NavalEnergyUnit)
        {
            _ship.EnergyUnit = EnergyUnit.Naval;
        }
    }

    /// <summary>
    /// Tells the missions a ship has been destroyed, and applies whatever the
    /// one that was waiting for it asks for. The move usually carries no
    /// message: the commander is in the middle of a fight.
    /// </summary>
    /// <param name="shipName">The ship destroyed.</param>
    internal void ShipDestroyed(string shipName)
    {
        foreach (IMission mission in All)
        {
            if (mission is IMissionKills kills
                && kills.ShipDestroyed(_context, StageOf(mission), shipName) is { } step)
            {
                Apply(mission, step);
                return;
            }
        }
    }

    /// <summary>
    /// The ambush a mission wants rolled for on this encounter check, or null
    /// when none does - which is nearly always.
    /// </summary>
    /// <returns>The ambush, or null.</returns>
    internal AmbushEncounter? Ambush()
    {
        foreach (IMission mission in All)
        {
            if (mission is IMissionEncounters encounters
                && encounters.Ambush(_context, StageOf(mission)) is { } ambush)
            {
                return ambush;
            }
        }

        return null;
    }

    /// <summary>
    /// The ship a mission wants sent in place of the lone pirate the game is
    /// about to make, or null to let the pirate come.
    /// </summary>
    /// <returns>The substitute, or null.</returns>
    internal LoneWolfEncounter? LoneWolfSubstitute()
    {
        foreach (IMission mission in All)
        {
            if (mission is IMissionEncounters encounters
                && encounters.LoneWolfSubstitute(_context, StageOf(mission)) is { } loneWolf)
            {
                return loneWolf;
            }
        }

        return null;
    }

    /// <summary>
    /// What a mission has to say about a system on the data screen, in place of
    /// its usual description, or null to leave the description alone.
    /// </summary>
    /// <param name="planetNumber">The system being described.</param>
    /// <returns>The mission's description, or null.</returns>
    internal string? DescribePlanet(int planetNumber)
    {
        foreach (IMission mission in All)
        {
            if (mission is IMissionPlanetDescriptions descriptions
                && descriptions.DescribePlanet(_context, StageOf(mission), planetNumber) is { } description)
            {
                return description;
            }
        }

        return null;
    }

    private string StageOf(IMission mission)
        => _gameState.Cmdr.Missions.StageOf(mission.Name) ?? mission.Stages.NotStarted;
}
