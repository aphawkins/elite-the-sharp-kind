// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Save;

namespace EliteSharpLib;

internal static class CommanderFactory
{
    /// <summary>
    /// The default commander. Do not modify.
    /// </summary>
    /// <returns>Commander Jameson.</returns>
    internal static SaveState Jameson() => new()
    {
        CommanderName = "JAMESON",
        Mission = 0,
        ShipLocation = [20, 173],
        GalaxySeed = [0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7],
        Credits = 100,
        Fuel = 7,
        GalaxyNumber = 0,
        Lasers = ["Pulse", "None", "None", "None"],
        CargoCapacity = 20,
        CurrentCargo = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        HasECM = false,
        HasFuelScoop = false,
        HasEnergyBomb = false,
        EnergyUnit = "None",
        HasDockingComputer = false,
        HasGalacticHyperdrive = false,
        HasEscapeCapsule = false,
        Missiles = 3,
        LegalStatus = 0,
        StationStock =
        [
            0x10,
            0x0F,
            0x11,
            0x00,
            0x03,
            0x1C,
            0x0E,
            0x00,
            0x00,
            0x0A,
            0x00,
            0x11,
            0x3A,
            0x07,
            0x09,
            0x08,
            0x00,
        ],
        MarketRandomiser = 0,
        Score = 0,
        Saved = 0x80,
    };

    /// <summary>
    /// The maximum equipment level, for testing purposes.
    /// </summary>
    /// <returns>Commander Max.</returns>
    internal static SaveState Max() => new()
    {
        CommanderName = "MAX",
        Mission = 0,
        ShipLocation = [20, 173],
        GalaxySeed = [0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7],
        Credits = 10000,
        Fuel = 7,
        GalaxyNumber = 0,
        Lasers = ["Military", "Pulse", "Beam", "Mining"],
        CargoCapacity = 35,
        CurrentCargo = [1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0],
        HasECM = true,
        HasFuelScoop = true,
        HasEnergyBomb = true,
        EnergyUnit = "Naval",
        HasDockingComputer = true,
        HasGalacticHyperdrive = true,
        HasEscapeCapsule = true,
        Missiles = 4,
        LegalStatus = 0,
        StationStock =
        [
            0x10,
            0x0F,
            0x11,
            0x00,
            0x03,
            0x1C,
            0x0E,
            0x00,
            0x00,
            0x0A,
            0x00,
            0x11,
            0x3A,
            0x07,
            0x09,
            0x08,
            0x00,
        ],
        MarketRandomiser = 0,
        Score = 0x1900,
        Saved = 0x80,
    };
}
