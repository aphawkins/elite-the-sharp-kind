// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Text.Json;
using System.Text.Json.Serialization;
using EliteSharp.Abstractions.Missions;
using EliteSharp.Abstractions.Ships;
using EliteSharpLib.Equipment;
using EliteSharpLib.Lasers;
using EliteSharpLib.Missions;
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
    private readonly MissionRegistry _missions;
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
        MissionRegistry missions,
        string baseDirectory,
        ILogger<SaveFile>? logger = null)
    {
        _state = state;
        _ship = ship;
        _trade = trade;
        _planet = planet;
        _missions = missions;
        _baseDirectory = baseDirectory;
        _logger = logger ?? NullLogger<SaveFile>.Instance;
        Directory.CreateDirectory(_baseDirectory);

        bool debugCommanderSet = Environment.GetEnvironmentVariable(DebugCommanderEnvVar) is not null;
        _lastSaved = debugCommanderSet ? CommanderFactory.Max(_trade.Goods) : CommanderFactory.Jameson(_trade.Goods);
        LogMessages.DebugCommanderEnvVar(
            _logger,
            DebugCommanderEnvVar,
            debugCommanderSet ? "set" : "not set",
            _lastSaved.CommanderName);
    }

    /// <summary>
    /// Gets what went wrong with the last <see cref="LoadCommander"/> that
    /// failed, short enough for the screen to show. Empty when the last load
    /// worked. A commander whose file is one field wrong could otherwise not
    /// tell that from a file that is not a save at all.
    /// </summary>
    internal string LastLoadError { get; private set; } = string.Empty;

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
            LastLoadError = "No Such Commander";
            return false;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            SaveState? save = JsonSerializer.Deserialize<SaveState>(stream, _options);
            if (save != null)
            {
                SaveProblem? problem = FindProblem(save);

                if (problem is null)
                {
                    _lastSaved = save;
                    SaveStateToGameState();
                    LastLoadError = string.Empty;
                    return true;
                }

                LogMessages.CommanderValidationFailed(_logger, path, problem.Field, problem.Detail);
                LastLoadError = $"Bad {problem.Field}";
            }
            else
            {
                LastLoadError = "Empty Commander File";
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            LogMessages.FailedToLoadCommander(_logger, path, ex);
            LastLoadError = "Unreadable Commander File";
        }

        _lastSaved = CommanderFactory.Jameson(_trade.Goods);
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
    /// The file a commander of this name is kept in. The name is upper-cased
    /// because that is the only case the load and save screens can produce: a
    /// keyboard letter arrives as <see cref="ConsoleKey.A"/> upwards, so a
    /// commander cannot type a lower-case name to match a file that has one.
    /// Windows would find the file anyway, Linux would not, and the game would
    /// say "No Such Commander" about a save sitting right there.
    /// </summary>
    /// <param name="name">The commander name, in any case.</param>
    /// <returns>The full path of that commander's save file.</returns>
    internal string PathFor(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string sanitized = string.Concat(name.Select(c => invalidChars.Contains(c) ? '_' : c));
        return Path.Combine(_baseDirectory, sanitized.ToUpperInvariant() + FileExtension);
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

    private static bool IsSeedByte(int value) => value is >= 0 and <= SeedByteMax;

    // Is this one of ours at all, and one this build can read?
    private static SaveProblem? FindFileProblem(SaveState save)
        => save.FileType != SaveState.CurrentFileType
            ? new("File Type", $"'{save.FileType}' is not '{SaveState.CurrentFileType}'")
            : save.Version != SaveState.CurrentVersion
                ? new("Version", $"{save.Version}, but this build writes {SaveState.CurrentVersion}")
                : null;

    private static SaveProblem? FindNameProblem(SaveState save)
        => string.IsNullOrWhiteSpace(save.CommanderName) ? new("Name", "the commander has no name") : null;

    private static SaveProblem? FindScoreProblem(SaveState save)
        => save.Score < 0 ? new("Score", $"{save.Score} is negative") : null;

    private static SaveProblem? FindLegalStatusProblem(SaveState save)
    {
        if (save.LegalStatus is not { Bounty: >= 0 and <= LegalStatusBand.BountyMax } legal)
        {
            return new("Legal Status", $"bounty {save.LegalStatus?.Bounty} is outside 0..{LegalStatusBand.BountyMax}");
        }

        string band = LegalStatusBand.For(legal.Bounty);

        return string.Equals(legal.Status, band, StringComparison.Ordinal)
            ? null
            : new("Legal Status", $"'{legal.Status}' does not match bounty {legal.Bounty} ('{band}')");
    }

    private static SaveProblem? FindCreditsProblem(SaveState save)
        => !float.IsFinite(save.Credits) || save.Credits < 0
            ? new("Credits", $"{save.Credits} is not a cash balance")
            : null;

    private static SaveProblem? FindFuelProblem(SaveState save, float maxFuel)
        => !float.IsFinite(save.Fuel) || save.Fuel < 0 || save.Fuel > maxFuel
            ? new("Fuel", $"{save.Fuel} is outside 0..{maxFuel}")
            : null;

    private static SaveProblem? FindMissileProblem(SaveState save)
        => save.Missiles is < 0 or > MissilesMax
            ? new("Missiles", $"{save.Missiles} is outside 0..{MissilesMax}")
            : null;

    private static SaveProblem? FindCargoBayProblem(SaveState save)
        => save.CargoCapacity is not (CargoCapacityStandard or CargoCapacityLarge)
            ? new("Cargo Bay", $"{save.CargoCapacity} is neither {CargoCapacityStandard} nor {CargoCapacityLarge}")
            : null;

    private static SaveProblem? FindEnergyUnitProblem(SaveState save)
        => IsNamed<EnergyUnit>(save.EnergyUnit)
            ? null
            : new("Energy Unit", $"'{save.EnergyUnit}' is not one this game has");

    private static SaveProblem? FindLaserProblem(LaserMountState? lasers)
    {
        if (lasers is not { } mounts)
        {
            return new("Lasers", "the file names no laser mounts");
        }

        (string Mount, string? Type)[] fitted =
        [
            ("front", mounts.Front),
            ("rear", mounts.Rear),
            ("left", mounts.Left),
            ("right", mounts.Right),
        ];

        foreach ((string mount, string? type) in fitted)
        {
            if (!IsNamed<LaserType>(type))
            {
                return new("Lasers", $"the {mount} mount says '{type}', which is not a laser this game has");
            }
        }

        return null;
    }

    private static SaveProblem? FindGalaxyProblem(SaveState save)
        => save.GalaxyNumber is < 0 or > GalaxyNumberMax
            ? new("Galaxy", $"{save.GalaxyNumber} is outside 0..{GalaxyNumberMax}")
            : null;

    private static SaveProblem? FindSeedProblem(GalaxySeedState? seed)
    {
        if (seed is not { } bytes)
        {
            return new("Galaxy Seed", "the file carries no seed");
        }

        int[] values = [bytes.A, bytes.B, bytes.C, bytes.D, bytes.E, bytes.F];

        return Array.Exists(values, value => !IsSeedByte(value))
            ? new("Galaxy Seed", $"[{string.Join(", ", values)}] is not six bytes of 0..{SeedByteMax}")
            : null;
    }

    private static SaveProblem? FindShipLocationProblem(SaveState save)
        => save.ShipLocation is not { } location || !IsSeedByte(location.D) || !IsSeedByte(location.B)
            ? new("Ship Location", $"({save.ShipLocation?.D}, {save.ShipLocation?.B}) is not inside 0..{SeedByteMax}")
            : null;

    private static SaveProblem? FindMarketProblem(SaveState save)
        => save.MarketRandomiser is < 0 or > SeedByteMax
            ? new("Market", $"randomiser {save.MarketRandomiser} is outside 0..{SeedByteMax}")
            : null;

    /// <summary>
    /// Whether every mission the save names is installed and is at one of the stages that
    /// mission declares. The vocabulary comes from the missions themselves rather than from
    /// an enum, which is the only way a mission that arrived in a plugin could be checked at
    /// all. A save may name fewer missions than are installed - the rest have not been
    /// started - but a mission it names that nothing provides is a save that cannot be
    /// applied, so it is turned away rather than half-loaded.
    /// </summary>
    private static SaveProblem? FindMissionProblem(IDictionary<string, MissionState>? missions, MissionRegistry installed)
    {
        if (missions is null)
        {
            return new("Missions", "the file lists none");
        }

        foreach ((string name, MissionState state) in missions)
        {
            IMission? mission = installed.Find(name);

            if (mission is null)
            {
                return new("Missions", $"'{name}' is a mission nothing installed provides");
            }

            if (state?.Stage is null || mission.Stages.IndexOf(state.Stage) < 0)
            {
                return new("Missions", $"'{name}' is at stage '{state?.Stage ?? "(none)"}', which it does not have");
            }
        }

        return null;
    }

    /// <summary>
    /// Whether the goods are named exactly once each, with a quantity the market could have
    /// produced. Counting as well as looking each name up leaves no room for an unknown one -
    /// and naming the good that is missing is what tells a commander their save was written
    /// against a different goods set rather than simply corrupted.
    /// </summary>
    private static SaveProblem? FindStockProblem(
        string field,
        IDictionary<string, int>? stock,
        IReadOnlyList<StockItem> market)
    {
        if (stock is not { } goods)
        {
            return new(field, "the file lists none");
        }

        foreach (StockItem item in market)
        {
            if (!goods.TryGetValue(item.Good.Id, out int quantity))
            {
                return new(field, $"nothing is listed for '{item.Good.Id}'");
            }

            if (quantity is < 0 or > QuantityMax)
            {
                return new(field, $"'{item.Good.Id}' is {quantity}, outside 0..{QuantityMax}");
            }
        }

        return goods.Count == market.Count
            ? null
            : new(field, $"{goods.Count} goods listed, but this game trades {market.Count}");
    }

    /// <summary>
    /// Rejects anything <see cref="SaveStateToGameState"/> would otherwise take on trust: a
    /// file that is not ours or not this version, a name the enums do not know, a missing or
    /// unknown item of cargo, and any value outside the range the game itself keeps it in.
    /// A save that fails here is discarded rather than half-applied.
    /// <para>
    /// It reports the first thing it finds wrong rather than a bare yes or no. A file is
    /// usually wrong in one place - a hand edit, or a save written against another goods set
    /// - and "fuel is 99, and the tank holds 7" is something a commander can act on where
    /// "error loading commander" is not. Each check is its own named method returning the
    /// same nullable problem, so the whole thing is one chain of first-wins alternatives.
    /// </para>
    /// </summary>
    /// <param name="save">The file just read.</param>
    /// <returns>The first problem found, or null if there is none.</returns>
    private SaveProblem? FindProblem(SaveState save)
        => FindFileProblem(save)
            ?? FindNameProblem(save)
            ?? FindScoreProblem(save)
            ?? FindLegalStatusProblem(save)
            ?? FindMissionProblem(save.Missions, _missions)
            ?? FindCreditsProblem(save)
            ?? FindFuelProblem(save, _ship.MaxFuel)
            ?? FindMissileProblem(save)
            ?? FindCargoBayProblem(save)
            ?? FindEnergyUnitProblem(save)
            ?? FindLaserProblem(save.Lasers)
            ?? FindGalaxyProblem(save)
            ?? FindSeedProblem(save.GalaxySeed)
            ?? FindShipLocationProblem(save)
            ?? FindMarketProblem(save)
            ?? FindStockProblem("Cargo", save.Cargo, _trade.StockMarket)
            ?? FindStockProblem("Station Stock", save.StationStock, _trade.StockMarket)
            ?? FindHoldProblem(save);

    /// <summary>
    /// Whether the cargo would fit. Checked after the stock itself, so a file whose goods do
    /// not match the set at all is reported as that rather than as an overloaded hold.
    /// </summary>
    private SaveProblem? FindHoldProblem(SaveState save)
    {
        int tonnage = TonnageOf(save.Cargo);

        return tonnage > save.CargoCapacity
            ? new("Cargo", $"{tonnage}t is more than the {save.CargoCapacity}t bay holds")
            : null;
    }

    /// <summary>
    /// The hold the cargo would take up. Gold, platinum and gem stones are weighed in
    /// kilograms and grams, so they do not count against the cargo bay.
    /// </summary>
    private int TonnageOf(IDictionary<string, int> cargo) => _trade.StockMarket
        .Where(item => item.Good.FillsHold)
        .Sum(item => cargo[item.Good.Id]);

    private SaveState GameStateToSaveState(string newName) => new()
    {
        FileType = SaveState.CurrentFileType,
        Version = SaveState.CurrentVersion,
        SavedAtUtc = DateTimeOffset.UtcNow,
        CargoCapacity = _ship.CargoCapacity,
        CommanderName = newName,
        Credits = _trade.Credits,
        Cargo = _trade.StockMarket.ToDictionary(x => x.Good.Id, x => x.CurrentCargo, StringComparer.Ordinal),
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
        Missions = _state.Cmdr.Missions.Recorded.ToDictionary(
            x => x.Key,
            x => new MissionState { Stage = x.Value },
            StringComparer.Ordinal),
        Score = _state.Cmdr.Score,
        ShipLocation = new()
        {
            D = _state.DockedPlanet.D,
            B = _state.DockedPlanet.B,
        },
        StationStock = _trade.StockMarket.ToDictionary(x => x.Good.Id, x => x.StationStock, StringComparer.Ordinal),
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
        foreach (StockItem stock in _trade.StockMarket)
        {
            stock.CurrentCargo = _lastSaved.Cargo[stock.Good.Id];
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
        _state.Cmdr.Missions.Clear();
        foreach ((string name, MissionState mission) in _lastSaved.Missions)
        {
            _state.Cmdr.Missions.MoveTo(name, mission.Stage);
        }

        _state.Cmdr.Score = _lastSaved.Score;
        _state.DockedPlanet.D = _lastSaved.ShipLocation.D;
        _state.DockedPlanet.B = _lastSaved.ShipLocation.B;
        foreach (StockItem stock in _trade.StockMarket)
        {
            stock.StationStock = _lastSaved.StationStock[stock.Good.Id];
        }
    }

    /// <summary>
    /// What is wrong with a save, if anything: the field a commander would
    /// recognise and the detail worth writing to the log. Null means the file
    /// is good.
    /// </summary>
    /// <param name="Field">
    /// The part of the file at fault, short enough for the load screen.
    /// </param>
    /// <param name="Detail">What was wrong with it, and what was expected.</param>
    private sealed record SaveProblem(string Field, string Detail);
}
