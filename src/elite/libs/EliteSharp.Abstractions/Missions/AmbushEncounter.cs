// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Missions;

/// <summary>
/// A ship the mission sends after the commander in its own right, on top of
/// whatever traffic the system would have had - the Thargoids that come for the
/// stolen plans. The game rolls the chance on each encounter check, so a
/// mission needs no random numbers of its own and cannot spawn ships whenever
/// it likes.
/// </summary>
public sealed record AmbushEncounter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbushEncounter"/> class,
    /// which always has odds worth rolling.
    /// </summary>
    /// <param name="shipName">The ship to spawn, by the name the game's ship list knows it by.</param>
    /// <param name="chanceInTwoFiftySix">
    /// How often the ambush happens, out of 256 encounter checks - Elite counts
    /// its odds in 256ths, and never better than 255 in 256, so a byte says
    /// everything the game can roll. A mission with nothing to send answers
    /// null rather than asking for odds of nothing.
    /// </param>
    public AmbushEncounter(string shipName, byte chanceInTwoFiftySix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(shipName);
        ArgumentOutOfRangeException.ThrowIfZero(chanceInTwoFiftySix);

        ShipName = shipName;
        ChanceInTwoFiftySix = chanceInTwoFiftySix;
    }

    /// <summary>
    /// Gets the ship to spawn, by the name the game's ship list knows it by.
    /// </summary>
    public string ShipName { get; }

    /// <summary>
    /// Gets how often the ambush happens, out of 256 encounter checks.
    /// </summary>
    public byte ChanceInTwoFiftySix { get; }
}
