// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// The mission's ship in place of the lone pirate the game was about to send -
/// how the stolen Constrictor comes at the commander hunting it, as often as a
/// pirate would have and no more. There are no odds to set: the game has
/// already decided somebody is coming, and the mission only says who.
/// </summary>
public sealed record LoneWolfEncounter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoneWolfEncounter"/> class.
    /// </summary>
    /// <param name="shipName">The ship to spawn, by the name the game's ship list knows it by.</param>
    /// <param name="unique">
    /// Whether the game should send its usual pirate instead while one of these
    /// is already flying.
    /// </param>
    public LoneWolfEncounter(string shipName, bool unique)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(shipName);

        ShipName = shipName;
        Unique = unique;
    }

    /// <summary>
    /// Gets the ship to spawn, by the name the game's ship list knows it by.
    /// </summary>
    public string ShipName { get; }

    /// <summary>
    /// Gets a value indicating whether the game will send its usual pirate
    /// instead while one of these is already flying. The mission cannot see the
    /// universe, so it says what it wants and the game keeps the count - which
    /// is how the stolen Constrictor stays one ship rather than a squadron.
    /// </summary>
    public bool Unique { get; }
}
