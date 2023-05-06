namespace Elite.Engine.Types
{
    public class SaveState : ICloneable
    {
        public string CommanderName { get; set; } = string.Empty;
        public int Mission { get; set; }
        public int[] ShipLocation { get; set; } = Array.Empty<int>();
        public int[] GalaxySeed { get; set; } = Array.Empty<int>();
        public float Credits { get; set; }
        public float Fuel { get; set; }
        public int GalaxyNumber { get; set; }
        public string[] Lasers { get; set; } = Array.Empty<string>();
        public int CargoCapacity { get; set; }
        public int[] CurrentCargo { get; set; } = Array.Empty<int>();
        public bool HasECM { get; set; }
        public bool HasFuelScoop { get; set; }
        public bool HasEnergyBomb { get; set; }
        public string EnergyUnit { get; set; } = string.Empty;
        public bool HasDockingComputer { get; set; }
        public bool HasGalacticHyperdrive { get; set; }
        public bool HasEscapePod { get; set; }
        public int Missiles { get; set; }
        public int LegalStatus { get; set; }
        public int[] StationStock { get; set; } = Array.Empty<int>();
        public int MarketRandomiser { get; set; }
        public int Score { get; set; }
        public int Saved { get; set; }

        public SaveState()
        {
        }

        public SaveState(SaveState other)
        {
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
            HasEscapePod = other.HasEscapePod;
            Missiles = other.Missiles;
            LegalStatus = other.LegalStatus;
            StationStock = other.StationStock;
            MarketRandomiser = other.MarketRandomiser;
            Score = other.Score;
            Saved = other.Saved;
        }

        public object Clone() => new SaveState(this);
    }
}
