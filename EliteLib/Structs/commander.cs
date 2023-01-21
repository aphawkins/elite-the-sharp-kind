namespace Elite.Structs
{
    using System.Numerics;
    using Elite;

    public class Commander
    {
        public string name { get; set; }
        public int mission { get; set; }
        public Vector2 shiplocation { get; set; }
        public galaxy_seed galaxy { get; set; }
        public int credits { get; set; }
        public int fuel { get; set; }
        public int galaxy_number { get; set; }
        public int front_laser { get; set; }
        public int rear_laser { get; set; }
        public int left_laser { get; set; }
        public int right_laser { get; set; }
        public int cargo_capacity { get; set; }
        public int[] current_cargo { get; set; } = new int[trade.stock_market.Length];
        public bool ecm { get; set; }
        public bool fuel_scoop { get; set; }
        public bool energy_bomb { get; set; }
        public int energy_unit { get; set; }
        public bool docking_computer { get; set; }
        public bool galactic_hyperdrive { get; set; }
        public bool escape_pod { get; set; }

        public int missiles { get; set; }
        public int legal_status { get; set; }
        public int[] station_stock { get; set; } = new int[trade.stock_market.Length];
        public int market_rnd { get; set; }
        public int score { get; set; }
        public int saved { get; set; }

        public Commander(string name, int mission, Vector2 shipLocation, galaxy_seed galaxy, int credits, int fuel, 
            int galaxy_number, int front_laser, int rear_laser, int left_laser, int right_laser, int cargo_capacity,
            int[] current_cargo, bool ecm, bool fuel_scoop, bool energy_bomb, int energy_unit, bool docking_computer, bool galactic_hyperdrive, bool escape_pod,
            int missiles, int legal_status, int[] station_stock, int market_rnd, int score, int saved)
        {
            this.name = name;
            this.mission = mission;
            this.shiplocation = shipLocation;
            this.galaxy = galaxy;
            this.credits = credits;
            this.fuel = fuel;
            this.galaxy_number = galaxy_number;
            this.front_laser = front_laser;
            this.rear_laser = rear_laser;
            this.left_laser = left_laser;
            this.right_laser = right_laser;
            this.cargo_capacity = cargo_capacity;
            this.current_cargo = current_cargo;
            this.ecm = ecm;
            this.fuel_scoop = fuel_scoop;
            this.energy_bomb = energy_bomb;
            this.energy_unit = energy_unit;
            this.docking_computer = docking_computer;
            this.galactic_hyperdrive = galactic_hyperdrive;
            this.escape_pod = escape_pod;
            this.missiles = missiles;
            this.legal_status = legal_status;
            this.station_stock = station_stock;
            this.market_rnd = market_rnd;
            this.score = score;
            this.saved = saved;
        }
    };
}