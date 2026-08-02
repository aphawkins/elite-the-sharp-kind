// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Text.Json;
using System.Text.Json.Serialization;
using EliteSharpLib.Equipment;
using EliteSharpLib.Lasers;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EliteSharpLib.Save;

internal sealed class SaveFile
{
    /// <summary>
    /// Set (to any value) to start with <see cref="CommanderFactory.Max"/>
    /// instead of the default <see cref="CommanderFactory.Jameson"/> -
    /// convenient for manually exercising late-game equipment/cargo without
    /// a save file. Unset in normal play.
    /// </summary>
    internal const string DebugCommanderEnvVar = "ELITE_DEBUG_COMMANDER";

    private const string FileExtension = ".cmdr";

    /// <summary>
    /// The most missiles the equipment screen will sell.
    /// </summary>
    private const int MissilesMax = 4;

    /// <summary>
    /// The hold without, and with, the large cargo bay fitted.
    /// </summary>
    private const int CargoCapacityStandard = 20;

    /// <inheritdoc cref="CargoCapacityStandard"/>
    private const int CargoCapacityLarge = 35;

    /// <summary>
    /// The eight galaxies the hyperdrive cycles through.
    /// </summary>
    private const int GalaxyNumberMax = 7;

    /// <summary>
    /// Seeds and the market randomiser are single bytes.
    /// </summary>
    private const int SeedByteMax = 255;

    /// <summary>
    /// The market clamps every quantity to this, so no save may hold more.
    /// </summary>
    private const int QuantityMax = 63;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly string _baseDirectory;
    private readonly ILogger<SaveFile> _logger;
    private readonly PlanetController _planet;
    private readonly PlayerShip _ship;
    private readonly GameState _state;
    private readonly Trade _trade;
    private SaveState _lastSaved;

    internal SaveFile(
        GameState state,
        PlayerShip ship,
        Trade trade,
        PlanetController planet,
        string baseDirectory,
        ILogger<SaveFile>? logger = null)
    {
        _state = state;
        _ship = ship;
        _trade = trade;
        _planet = planet;
        _baseDirectory = baseDirectory;
        _logger = logger ?? NullLogger<SaveFile>.Instance;
        Directory.CreateDirectory(_baseDirectory);

        bool debugCommanderSet = Environment.GetEnvironmentVariable(DebugCommanderEnvVar) is not null;
        _lastSaved = debugCommanderSet ? CommanderFactory.Max() : CommanderFactory.Jameson();
        LogMessages.DebugCommanderEnvVar(
            _logger,
            DebugCommanderEnvVar,
            debugCommanderSet ? "set" : "not set",
            _lastSaved.CommanderName);
    }

    internal void GetLastSave()
    {
        SaveStateToGameState();
        RestoreSavedCommander();
    }

    internal bool LoadCommander(string name)
    {
        string path = PathFor(name);
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            SaveState? save = JsonSerializer.Deserialize<SaveState>(stream, _options);
            if (save != null)
            {
                if (IsValidSave(save))
                {
                    _lastSaved = save;
                    SaveStateToGameState();
                    return true;
                }

                LogMessages.CommanderValidationFailed(_logger, path);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            LogMessages.FailedToLoadCommander(_logger, path, ex);
        }

        _lastSaved = CommanderFactory.Jameson();
        return false;
    }

    internal bool SaveCommander(string newName)
    {
        string path = PathFor(newName);

        try
        {
            SaveState save = GameStateToSaveState(newName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using FileStream stream = File.OpenWrite(path);
            JsonSerializer.Serialize(stream, save, _options);

            _lastSaved = save;

            return true;
        }
        catch (IOException ex)
        {
            LogMessages.FailedToSaveCommander(_logger, path, ex);
            return false;
        }
        catch (Exception ex)
        {
            LogMessages.FailedToSaveCommander(_logger, path, ex);
            throw;
        }
    }

    /// <summary>
    /// Whether the text names a member of the enum. The numeric strings
    /// <see cref="Enum.TryParse{T}(string, out T)"/> also accepts are turned away, so a save
    /// has to spell its lasers, missions and energy units out.
    /// </summary>
    /// <typeparam name="T">The enum the text has to name a member of.</typeparam>
    /// <param name="value">The text from the save file.</param>
    /// <returns>Whether the text is one of the enum's names.</returns>
    private static bool IsNamed<T>(string? value)
        where T : struct, Enum
        => value != null
            && !int.TryParse(value, out _)
            && Enum.TryParse(value, out T parsed)
            && Enum.IsDefined(parsed);

    /// <summary>
    /// Whether every mission is named exactly once, each with a stage of its own mission's.
    /// Counting as well as looking each name up leaves no room for an unknown mission, and
    /// giving each mission its own stage type keeps one mission's stages out of another's.
    /// </summary>
    private static bool IsValidMissions(IDictionary<string, MissionState>? missions)
        => missions is { } stages
            && stages.Count == Enum.GetValues<MissionName>().Length
            && IsValidStage<ConstrictorStage>(stages, MissionName.Constrictor)
            && IsValidStage<ThargoidStage>(stages, MissionName.Thargoid);

    /// <summary>
    /// Whether the named mission is present with a stage this mission knows.
    /// </summary>
    /// <typeparam name="T">The named mission's own stage type.</typeparam>
    /// <param name="missions">The missions the save file holds.</param>
    /// <param name="name">The mission to look for.</param>
    /// <returns>Whether the mission is there and names one of its own stages.</returns>
    private static bool IsValidStage<T>(IDictionary<string, MissionState> missions, MissionName name)
        where T : struct, Enum
        => missions.TryGetValue(name.ToString(), out MissionState? mission)
            && mission is { } stage
            && IsNamed<T>(stage.Stage);

    private static bool IsValidLegalStatus(LegalStatusState? legal)
        => legal is { Bounty: >= 0 and <= LegalStatusBand.BountyMax }
            && string.Equals(legal.Status, LegalStatusBand.For(legal.Bounty), StringComparison.Ordinal);

    private static bool IsValidLasers(LaserMountState? lasers) => lasers is { } mounts
        && IsNamed<LaserType>(mounts.Front)
        && IsNamed<LaserType>(mounts.Rear)
        && IsNamed<LaserType>(mounts.Left)
        && IsNamed<LaserType>(mounts.Right);

    private static bool IsValidGalaxySeed(GalaxySeedState? seed) => seed is { } bytes
        && IsSeedByte(bytes.A)
        && IsSeedByte(bytes.B)
        && IsSeedByte(bytes.C)
        && IsSeedByte(bytes.D)
        && IsSeedByte(bytes.E)
        && IsSeedByte(bytes.F);

    private static bool IsValidShipLocation(ShipLocationState? location) => location is { } position
        && IsSeedByte(position.D)
        && IsSeedByte(position.B);

    private static bool IsSeedByte(int value) => value is >= 0 and <= SeedByteMax;

    private string PathFor(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string sanitized = string.Concat(name.Select(c => invalidChars.Contains(c) ? '_' : c));
        return Path.Combine(_baseDirectory, sanitized + FileExtension);
    }

    /// <summary>
    /// Rejects anything <see cref="SaveStateToGameState"/> would otherwise take on trust: a
    /// file that is not ours or not this version, a name the enums do not know, a missing or
    /// unknown item of cargo, and any value outside the range the game itself keeps it in.
    /// A save that fails here is discarded rather than half-applied.
    /// </summary>
    private bool IsValidSave(SaveState save) => save.FileType == SaveState.CurrentFileType
        && save.Version == SaveState.CurrentVersion
        && !string.IsNullOrWhiteSpace(save.CommanderName)
        && IsValidMissions(save.Missions)
        && save.Score >= 0
        && IsValidLegalStatus(save.LegalStatus)
        && float.IsFinite(save.Credits)
        && save.Credits >= 0
        && float.IsFinite(save.Fuel)
        && save.Fuel >= 0
        && save.Fuel <= _ship.MaxFuel
        && save.Missiles is >= 0 and <= MissilesMax
        && save.CargoCapacity is CargoCapacityStandard or CargoCapacityLarge
        && IsNamed<EnergyUnit>(save.EnergyUnit)
        && IsValidLasers(save.Lasers)
        && save.GalaxyNumber is >= 0 and <= GalaxyNumberMax
        && IsValidGalaxySeed(save.GalaxySeed)
        && IsValidShipLocation(save.ShipLocation)
        && save.MarketRandomiser is >= 0 and <= SeedByteMax
        && IsValidStock(save.Cargo)
        && IsValidStock(save.StationStock)
        && TonnageOf(save.Cargo) <= save.CargoCapacity;

    /// <summary>
    /// Whether the goods are named exactly once each, with a quantity the market could have
    /// produced. Counting as well as looking each name up leaves no room for an unknown one.
    /// </summary>
    private bool IsValidStock(IDictionary<string, int>? stock) => stock is { } goods
        && goods.Count == _trade.StockMarket.Count
        && _trade.StockMarket.Keys.All(type
            => goods.TryGetValue(type.ToString(), out int quantity) && quantity is >= 0 and <= QuantityMax);

    /// <summary>
    /// The hold the cargo would take up. Gold, platinum and gem stones are weighed in
    /// kilograms and grams, so they do not count against the cargo bay.
    /// </summary>
    private int TonnageOf(IDictionary<string, int> cargo) => _trade.StockMarket
        .Where(x => x.Value.Units == Trade.TONNES)
        .Sum(x => cargo[x.Key.ToString()]);

    private SaveState GameStateToSaveState(string newName) => new()
    {
        FileType = SaveState.CurrentFileType,
        Version = SaveState.CurrentVersion,
        SavedAtUtc = DateTimeOffset.UtcNow,
        CargoCapacity = _ship.CargoCapacity,
        CommanderName = newName,
        Credits = _trade.Credits,
        Cargo = _trade.StockMarket.ToDictionary(x => x.Key.ToString(), x => x.Value.CurrentCargo, StringComparer.Ordinal),
        EnergyUnit = _ship.EnergyUnit.ToString(),
        Fuel = _ship.Fuel,
        GalaxyNumber = _state.Cmdr.GalaxyNumber,
        GalaxySeed = new()
        {
            A = _state.Cmdr.Galaxy.A,
            B = _state.Cmdr.Galaxy.B,
            C = _state.Cmdr.Galaxy.C,
            D = _state.Cmdr.Galaxy.D,
            E = _state.Cmdr.Galaxy.E,
            F = _state.Cmdr.Galaxy.F,
        },
        HasDockingComputer = _ship.HasDockingComputer,
        HasECM = _ship.HasECM,
        HasEnergyBomb = _ship.HasEnergyBomb,
        HasEscapeCapsule = _ship.HasEscapeCapsule,
        HasFuelScoop = _ship.HasFuelScoop,
        HasGalacticHyperdrive = _ship.HasGalacticHyperdrive,
        Lasers = new()
        {
            Front = _ship.LaserFront.Type.ToString(),
            Rear = _ship.LaserRear.Type.ToString(),
            Left = _ship.LaserLeft.Type.ToString(),
            Right = _ship.LaserRight.Type.ToString(),
        },
        LegalStatus = new()
        {
            Status = LegalStatusBand.For(_state.Cmdr.LegalStatus),
            Bounty = _state.Cmdr.LegalStatus,
        },
        MarketRandomiser = _trade.MarketRandomiser,
        Missiles = _ship.MissileCount,
        Missions = new Dictionary<string, MissionState>(StringComparer.Ordinal)
        {
            [nameof(MissionName.Constrictor)] = new() { Stage = _state.Cmdr.Constrictor.ToString() },
            [nameof(MissionName.Thargoid)] = new() { Stage = _state.Cmdr.Thargoid.ToString() },
        },
        Score = _state.Cmdr.Score,
        ShipLocation = new()
        {
            D = _state.DockedPlanet.D,
            B = _state.DockedPlanet.B,
        },
        StationStock = _trade.StockMarket.ToDictionary(x => x.Key.ToString(), x => x.Value.StationStock, StringComparer.Ordinal),
    };

    private void RestoreSavedCommander()
    {
        _state.DockedPlanet = _planet.FindPlanet(_state.Cmdr.Galaxy, new(_state.DockedPlanet.D, _state.DockedPlanet.B));
        _state.PlanetName = _planet.NamePlanet(_state.DockedPlanet);
        _state.HyperspacePlanet = new(_state.DockedPlanet);
        _state.CurrentPlanetData = PlanetController.GeneratePlanetData(_state.DockedPlanet);
        _trade.GenerateStockMarket();
        _trade.SetStockQuantities();
    }

    private void SaveStateToGameState()
    {
        _ship.CargoCapacity = _lastSaved.CargoCapacity;
        _state.Cmdr.Name = _lastSaved.CommanderName;
        _trade.Credits = _lastSaved.Credits;
        foreach (StockType type in _trade.StockMarket.Keys)
        {
            _trade.StockMarket[type].CurrentCargo = _lastSaved.Cargo[type.ToString()];
        }

        _ship.EnergyUnit = Enum.Parse<EnergyUnit>(_lastSaved.EnergyUnit);
        _ship.Fuel = _lastSaved.Fuel;
        _state.Cmdr.GalaxyNumber = _lastSaved.GalaxyNumber;
        _state.Cmdr.Galaxy.A = _lastSaved.GalaxySeed.A;
        _state.Cmdr.Galaxy.B = _lastSaved.GalaxySeed.B;
        _state.Cmdr.Galaxy.C = _lastSaved.GalaxySeed.C;
        _state.Cmdr.Galaxy.D = _lastSaved.GalaxySeed.D;
        _state.Cmdr.Galaxy.E = _lastSaved.GalaxySeed.E;
        _state.Cmdr.Galaxy.F = _lastSaved.GalaxySeed.F;
        _ship.HasDockingComputer = _lastSaved.HasDockingComputer;
        _ship.HasECM = _lastSaved.HasECM;
        _ship.HasEnergyBomb = _lastSaved.HasEnergyBomb;
        _ship.HasEscapeCapsule = _lastSaved.HasEscapeCapsule;
        _ship.HasFuelScoop = _lastSaved.HasFuelScoop;
        _ship.HasGalacticHyperdrive = _lastSaved.HasGalacticHyperdrive;
        _ship.LaserFront = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers.Front));
        _ship.LaserRear = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers.Rear));
        _ship.LaserRight = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers.Right));
        _ship.LaserLeft = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers.Left));
        _state.Cmdr.LegalStatus = _lastSaved.LegalStatus.Bounty;
        _trade.MarketRandomiser = _lastSaved.MarketRandomiser;
        _ship.MissileCount = _lastSaved.Missiles;
        _state.Cmdr.Constrictor =
            Enum.Parse<ConstrictorStage>(_lastSaved.Missions[nameof(MissionName.Constrictor)].Stage);
        _state.Cmdr.Thargoid =
            Enum.Parse<ThargoidStage>(_lastSaved.Missions[nameof(MissionName.Thargoid)].Stage);
        _state.Cmdr.Score = _lastSaved.Score;
        _state.DockedPlanet.D = _lastSaved.ShipLocation.D;
        _state.DockedPlanet.B = _lastSaved.ShipLocation.B;
        foreach (StockType type in _trade.StockMarket.Keys)
        {
            _trade.StockMarket[type].StationStock = _lastSaved.StationStock[type.ToString()];
        }
    }
}
