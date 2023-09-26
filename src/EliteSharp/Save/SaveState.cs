// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics.CodeAnalysis;

namespace EliteSharp.Save
{
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
    public sealed class SaveState
    {
        public SaveState()
        {
        }

        public SaveState(SaveState other)
        {
            ArgumentNullException.ThrowIfNull(other);

            CommanderName = other.CommanderName;
            Mission = other.Mission;
            ShipLocation = other.ShipLocation;
            GalaxySeed = other.GalaxySeed;
            Credits = other.Credits;
            Fuel = other.Fuel;
            GalaxyNumber = other.GalaxyNumber;
            Lasers = other.Lasers;
            CargoCapacity = other.CargoCapacity;
            CurrentCargo = other.CurrentCargo;
            HasECM = other.HasECM;
            HasFuelScoop = other.HasFuelScoop;
            HasEnergyBomb = other.HasEnergyBomb;
            EnergyUnit = other.EnergyUnit;
            HasDockingComputer = other.HasDockingComputer;
            HasGalacticHyperdrive = other.HasGalacticHyperdrive;
            HasEscapeCapsule = other.HasEscapeCapsule;
            Missiles = other.Missiles;
            LegalStatus = other.LegalStatus;
            StationStock = other.StationStock;
            MarketRandomiser = other.MarketRandomiser;
            Score = other.Score;
            Saved = other.Saved;
        }

        public int CargoCapacity { get; set; }

        public string CommanderName { get; set; } = string.Empty;

        public float Credits { get; set; }

        public IList<int> CurrentCargo { get; set; } = new List<int>();

        public string EnergyUnit { get; set; } = string.Empty;

        public float Fuel { get; set; }

        public int GalaxyNumber { get; set; }

        public IList<int> GalaxySeed { get; set; } = new List<int>();

        public bool HasDockingComputer { get; set; }

        public bool HasECM { get; set; }

        public bool HasEnergyBomb { get; set; }

        public bool HasEscapeCapsule { get; set; }

        public bool HasFuelScoop { get; set; }

        public bool HasGalacticHyperdrive { get; set; }

        public IList<string> Lasers { get; set; } = new List<string>();

        public int LegalStatus { get; set; }

        public int MarketRandomiser { get; set; }

        public int Missiles { get; set; }

        public int Mission { get; set; }

        public int Saved { get; set; }

        public int Score { get; set; }

        public IList<int> ShipLocation { get; set; } = new List<int>();

        public IList<int> StationStock { get; set; } = new List<int>();
    }
}
