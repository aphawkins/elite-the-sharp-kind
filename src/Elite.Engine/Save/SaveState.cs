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
            this.CommanderName = other.CommanderName;
            this.Mission = other.Mission;
            this.ShipLocation = other.ShipLocation;
            this.GalaxySeed = other.GalaxySeed;
            this.Credits = other.Credits;
            this.Fuel = other.Fuel;
            this.GalaxyNumber = other.GalaxyNumber;
            this.Lasers = other.Lasers;
            this.CargoCapacity = other.CargoCapacity;
            this.CurrentCargo = other.CurrentCargo;
            this.HasECM = other.HasECM;
            this.HasFuelScoop = other.HasFuelScoop;
            this.HasEnergyBomb = other.HasEnergyBomb;
            this.EnergyUnit = other.EnergyUnit;
            this.HasDockingComputer = other.HasDockingComputer;
            this.HasGalacticHyperdrive = other.HasGalacticHyperdrive;
            this.HasEscapePod = other.HasEscapePod;
            this.Missiles = other.Missiles;
            this.LegalStatus = other.LegalStatus;
            this.StationStock = other.StationStock;
            this.MarketRandomiser = other.MarketRandomiser;
            this.Score = other.Score;
            this.Saved = other.Saved;
        }

        public object Clone()
        {
            return new SaveState(this);
        }
    }
}