// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Save;
using EliteSharpLib.Trader;
using EliteSharpLib.Types;

namespace EliteSharpLib;

internal static class CommanderFactory
{
    /// <summary>
    /// The default commander. Do not modify.
    /// </summary>
    /// <returns>Commander Jameson.</returns>
    internal static SaveState Jameson() => new()
    {
        SavedAtUtc = DateTimeOffset.UtcNow,
        CommanderName = "JAMESON",
        Missions = NoMissionsStarted(),
        ShipLocation = new() { D = 20, B = 173 },
        GalaxySeed = new() { A = 0x4a, B = 0x5a, C = 0x48, D = 0x02, E = 0x53, F = 0xb7 },
        Credits = 100,
        Fuel = 7,
        GalaxyNumber = 0,
        Lasers = new() { Front = "Pulse", Rear = "None", Left = "None", Right = "None" },
        CargoCapacity = 20,
        Cargo = Cargo(loaded: false),
        HasECM = false,
        HasFuelScoop = false,
        HasEnergyBomb = false,
        EnergyUnit = "None",
        HasDockingComputer = false,
        HasGalacticHyperdrive = false,
        HasEscapeCapsule = false,
        Missiles = 3,
        LegalStatus = new() { Status = LegalStatusBand.For(0), Bounty = 0 },
        StationStock = StartingStationStock(),
        MarketRandomiser = 0,
        Score = 0,
    };

    /// <summary>
    /// The maximum equipment level, for testing purposes.
    /// </summary>
    /// <returns>Commander Max.</returns>
    internal static SaveState Max() => new()
    {
        SavedAtUtc = DateTimeOffset.UtcNow,
        CommanderName = "MAX",
        Missions = NoMissionsStarted(),
        ShipLocation = new() { D = 20, B = 173 },
        GalaxySeed = new() { A = 0x4a, B = 0x5a, C = 0x48, D = 0x02, E = 0x53, F = 0xb7 },
        Credits = 10000,
        Fuel = 7,
        GalaxyNumber = 0,
        Lasers = new() { Front = "Military", Rear = "Pulse", Left = "Mining", Right = "Beam" },
        CargoCapacity = 35,
        Cargo = Cargo(loaded: true),
        HasECM = true,
        HasFuelScoop = true,
        HasEnergyBomb = true,
        EnergyUnit = "Naval",
        HasDockingComputer = true,
        HasGalacticHyperdrive = true,
        HasEscapeCapsule = true,
        Missiles = 4,
        LegalStatus = new() { Status = LegalStatusBand.For(0), Bounty = 0 },
        StationStock = StartingStationStock(),
        MarketRandomiser = 0,
        Score = 0x1900,
    };

    /// <summary>
    /// An empty hold, or one holding a unit of everything Commander Max is
    /// allowed to be carrying.
    /// </summary>
    /// <param name="loaded">Whether to fill it.</param>
    private static Dictionary<string, int> Cargo(bool loaded)
        => ClassicGoods.All.ToDictionary(
            good => good.Id,
            good => loaded && IsSafeForMax(good) ? 1 : 0,
            StringComparer.Ordinal);

    /// <summary>
    /// Whether the test commander may carry this. Contraband is left out so Max
    /// does not launch as an Offender, and what a station never sells could not
    /// have been bought to be aboard. Derived from the goods themselves rather
    /// than listed, so a set with different contraband still gets a clean Max.
    /// </summary>
    private static bool IsSafeForMax(GoodsDefinition good)
        => good.IsSoldByStations && good.ContrabandWeight == 0;

    /// <summary>
    /// No mission started, which both commanders begin at. The save file holds
    /// only the stages that have been reached, so a mission nobody has started
    /// is an entry that is not there - which is also what lets a commander
    /// saved before a mission was installed load afterwards.
    /// </summary>
    private static Dictionary<string, MissionState> NoMissionsStarted()
        => new(StringComparer.Ordinal);

    /// <summary>
    /// The station's opening stock, which both commanders start docked at. It
    /// is the goods' own declaration now rather than a second list beside them.
    /// </summary>
    private static Dictionary<string, int> StartingStationStock()
        => ClassicGoods.All.ToDictionary(good => good.Id, good => good.OpeningStationStock, StringComparer.Ordinal);
}
