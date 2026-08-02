// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// A saved commander, as the .cmdr file holds them. Everything here is named
/// rather than positional - cargo and station stock are keyed by the goods'
/// own names, lasers by the mount they are on - so a file can be read, and
/// hand-edited, without a copy of this class to count array indices against.
/// </summary>
public sealed class SaveState
{
    public SaveState()
    {
    }

    public SaveState(SaveState other)
    {
        ArgumentNullException.ThrowIfNull(other);

        FileType = other.FileType;
        Version = other.Version;
        SavedAtUtc = other.SavedAtUtc;
        CommanderName = other.CommanderName;
        Missions = other.Missions;
        Score = other.Score;
        LegalStatus = other.LegalStatus;
        Credits = other.Credits;
        Fuel = other.Fuel;
        Missiles = other.Missiles;
        CargoCapacity = other.CargoCapacity;
        EnergyUnit = other.EnergyUnit;
        Lasers = other.Lasers;
        HasECM = other.HasECM;
        HasFuelScoop = other.HasFuelScoop;
        HasEnergyBomb = other.HasEnergyBomb;
        HasDockingComputer = other.HasDockingComputer;
        HasGalacticHyperdrive = other.HasGalacticHyperdrive;
        HasEscapeCapsule = other.HasEscapeCapsule;
        GalaxyNumber = other.GalaxyNumber;
        GalaxySeed = other.GalaxySeed;
        ShipLocation = other.ShipLocation;
        MarketRandomiser = other.MarketRandomiser;
        Cargo = other.Cargo;
        StationStock = other.StationStock;
    }

    /// <summary>
    /// Gets what marks the file as ours, so a .cmdr from somewhere else is
    /// turned away with the version rather than half-parsed.
    /// </summary>
    public static string CurrentFileType { get; } = "EliteSharp commander";

    /// <summary>
    /// Gets the format written today. A file carrying anything else -
    /// including the unversioned files written before this existed - is not
    /// loaded.
    /// </summary>
    public static int CurrentVersion { get; } = 1;

    public string FileType { get; set; } = CurrentFileType;

    public int Version { get; set; } = CurrentVersion;

    public DateTimeOffset SavedAtUtc { get; set; }

    public string CommanderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets where the commander has got to in each mission, keyed by the
    /// mission's name. A mission added later is another key, and leaves the
    /// ones already here alone.
    /// </summary>
    public IDictionary<string, MissionState> Missions { get; init; }
        = new Dictionary<string, MissionState>(StringComparer.Ordinal);

    public int Score { get; set; }

    public LegalStatusState LegalStatus { get; set; } = new();

    public float Credits { get; set; }

    public float Fuel { get; set; }

    public int Missiles { get; set; }

    public int CargoCapacity { get; set; }

    public string EnergyUnit { get; set; } = string.Empty;

    public LaserMountState Lasers { get; set; } = new();

    public bool HasECM { get; set; }

    public bool HasFuelScoop { get; set; }

    public bool HasEnergyBomb { get; set; }

    public bool HasDockingComputer { get; set; }

    public bool HasGalacticHyperdrive { get; set; }

    public bool HasEscapeCapsule { get; set; }

    public int GalaxyNumber { get; set; }

    public GalaxySeedState GalaxySeed { get; set; } = new();

    public ShipLocationState ShipLocation { get; set; } = new();

    public int MarketRandomiser { get; set; }

    /// <summary>
    /// Gets what the commander is carrying, keyed by the goods' names.
    /// </summary>
    public IDictionary<string, int> Cargo { get; init; } = new Dictionary<string, int>(StringComparer.Ordinal);

    /// <summary>
    /// Gets what the station the commander is docked at has for sale, keyed
    /// by the goods' names.
    /// </summary>
    public IDictionary<string, int> StationStock { get; init; } = new Dictionary<string, int>(StringComparer.Ordinal);
}
