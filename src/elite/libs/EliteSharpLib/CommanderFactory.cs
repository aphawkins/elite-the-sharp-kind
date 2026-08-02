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
    /// The station's opening stock, which both commanders start docked at.
    /// </summary>
    private static readonly (StockType Type, int Quantity)[] s_startingStationStock =
    [
        (StockType.Food, 0x10),
        (StockType.Textiles, 0x0F),
        (StockType.Radioactives, 0x11),
        (StockType.Slaves, 0x00),
        (StockType.LiquorWines, 0x03),
        (StockType.Luxuries, 0x1C),
        (StockType.Narcotics, 0x0E),
        (StockType.Computers, 0x00),
        (StockType.Machinery, 0x00),
        (StockType.Alloys, 0x0A),
        (StockType.Firearms, 0x00),
        (StockType.Furs, 0x11),
        (StockType.Minerals, 0x3A),
        (StockType.Gold, 0x07),
        (StockType.Platinum, 0x09),
        (StockType.GemStones, 0x08),
        (StockType.AlienItems, 0x00),
    ];

    /// <summary>
    /// The goods Commander Max starts with a unit of. The contraband is left
    /// out so the test commander does not launch as an Offender, and Alien
    /// Items cannot be bought at all.
    /// </summary>
    private static readonly StockType[] s_maxCargo =
    [
        StockType.Food,
        StockType.Textiles,
        StockType.Radioactives,
        StockType.LiquorWines,
        StockType.Luxuries,
        StockType.Computers,
        StockType.Machinery,
        StockType.Alloys,
        StockType.Furs,
        StockType.Minerals,
        StockType.Gold,
        StockType.Platinum,
        StockType.GemStones,
    ];

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
        Cargo = Cargo([]),
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
        Cargo = Cargo(s_maxCargo),
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
    /// An empty hold, holding one unit of each of the goods named.
    /// </summary>
    private static Dictionary<string, int> Cargo(IReadOnlyCollection<StockType> carrying)
        => Enum.GetValues<StockType>()
            .Where(type => type != StockType.None)
            .ToDictionary(type => type.ToString(), type => carrying.Contains(type) ? 1 : 0, StringComparer.Ordinal);

    /// <summary>
    /// No mission started, which both commanders begin at. The save file holds
    /// only the stages that have been reached, so a mission nobody has started
    /// is an entry that is not there - which is also what lets a commander
    /// saved before a mission was installed load afterwards.
    /// </summary>
    private static Dictionary<string, MissionState> NoMissionsStarted()
        => new(StringComparer.Ordinal);

    private static Dictionary<string, int> StartingStationStock()
        => s_startingStationStock.ToDictionary(x => x.Type.ToString(), x => x.Quantity, StringComparer.Ordinal);
}
