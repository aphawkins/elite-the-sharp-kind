// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;

namespace EliteSharpLib.Conflict;

/// <summary>
/// What combat does on the missions' behalf. Kept out of the main file because
/// that one is already at the size the analyzers allow, and because this is a
/// coherent slice: a mission says what it wants to happen in open space, and
/// these are the only places the game acts on it.
/// </summary>
internal sealed partial class Combat
{
    /// <summary>
    /// Spawns the ship a mission's ambush asked for. A Thargoid goes through
    /// <see cref="CreateThargoid"/> because its Tharglet belongs to the ship
    /// rather than to whoever sent it - a random encounter's Thargoid brings
    /// one too.
    /// </summary>
    /// <param name="shipName">The ship the mission named.</param>
    private void CreateMissionShip(string shipName)
    {
        if (string.Equals(shipName, "Thargoid", StringComparison.Ordinal))
        {
            CreateThargoid();
            return;
        }

        IShip ship = _shipFactory.CreateShip(shipName);
        if (_universe.AddNewShip(ship))
        {
            ship.Flags = ShipProperties.Angry;
        }
        else
        {
            LogMessages.FailedToCreateShip(_logger, shipName);
        }
    }
}
