namespace Elite.Engine.Types
{
    using Elite.Engine.Enums;

    public class Commander
    {
        public string name { get; set; }
        public int mission { get; set; }
        public galaxy_seed galaxy { get; set; } = new();
        public float credits { get; set; }
        public float fuel { get; set; }
        public int galaxy_number { get; set; }
        public ILaser front_laser { get; set; }
        public ILaser rear_laser { get; set; }
        public ILaser left_laser { get; set; }
        public ILaser right_laser { get; set; }
        public int cargo_capacity { get; set; }
        public int[] current_cargo { get; set; } = new int[17];
        public bool ecm { get; set; }
        public bool fuel_scoop { get; set; }
        public bool energy_bomb { get; set; }
        public EnergyUnit energy_unit { get; set; }
        public bool docking_computer { get; set; }
        public bool galactic_hyperdrive { get; set; }
        public bool escape_pod { get; set; }

        public int missiles { get; set; }
        public int legal_status { get; set; }
        public int[] station_stock { get; set; } = new int[17];
        public int market_rnd { get; set; }
        public int score { get; set; }
        public int saved { get; set; }
    }
}