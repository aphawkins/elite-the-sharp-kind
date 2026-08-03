// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Missions;

/// <summary>
/// What moving to a stage is worth to the commander. It travels with the step
/// rather than being paid out by the mission, so that the payment and the stage
/// that earned it are applied together or not at all - a mission cannot pay for
/// a move the game then refuses, and a screen shown twice cannot pay twice.
/// </summary>
public sealed record MissionAward
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissionAward"/> class,
    /// paying in rank and cash. It has to be worth something: a step that pays
    /// nothing carries no award at all.
    /// </summary>
    /// <param name="combatScore">Kills' worth to add to the commander's combat score.</param>
    /// <param name="credits">Whole credits to add to the commander's cash.</param>
    public MissionAward(int combatScore, int credits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(combatScore);
        ArgumentOutOfRangeException.ThrowIfNegative(credits);

        if (combatScore == 0 && credits == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(combatScore), "An award worth nothing is no award: leave it out instead.");
        }

        CombatScore = combatScore;
        Credits = credits;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissionAward"/> class,
    /// paying in kit as well - which is worth something whatever the numbers
    /// say, so both of those may be zero.
    /// </summary>
    /// <param name="combatScore">Kills' worth to add to the commander's combat score.</param>
    /// <param name="credits">Whole credits to add to the commander's cash.</param>
    /// <param name="equipment">Kit to fit to the commander's ship.</param>
    public MissionAward(int combatScore, int credits, MissionEquipment equipment)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(combatScore);
        ArgumentOutOfRangeException.ThrowIfNegative(credits);

        CombatScore = combatScore;
        Credits = credits;
        Equipment = equipment;
    }

    /// <summary>
    /// Gets the kills' worth added to the commander's combat score. The game
    /// decides what that does to the rating shown.
    /// </summary>
    public int CombatScore { get; }

    /// <summary>
    /// Gets the whole credits added to the commander's cash.
    /// </summary>
    public int Credits { get; }

    /// <summary>
    /// Gets the kit fitted to the commander's ship, or null when the mission
    /// pays in rank or cash alone.
    /// </summary>
    public MissionEquipment? Equipment { get; }
}
