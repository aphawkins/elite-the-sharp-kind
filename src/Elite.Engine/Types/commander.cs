using Elite.Engine;

namespace Elite.Engine.Types
{
    using System.Numerics;
    using Elite.Engine.Enums;

    public class Commander : ICloneable
    {
        public string name { get; set; }
        public int mission { get; set; }
        public Vector2 shiplocation { get; set; }
        public galaxy_seed galaxy { get; set; }
        public float credits { get; set; }
        public float fuel { get; set; }
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
        public EnergyUnit energy_unit { get; set; }
        public bool docking_computer { get; set; }
        public bool galactic_hyperdrive { get; set; }
        public bool escape_pod { get; set; }

        public int missiles { get; set; }
        public int legal_status { get; set; }
        public int[] station_stock { get; set; } = new int[trade.stock_market.Length];
        public int market_rnd { get; set; }
        public int score { get; set; }
        public int saved { get; set; }

        public Commander()
        {
        }

        private Commander(Commander other)
        {
            this.name = other.name;
            this.mission = other.mission;
            this.shiplocation = other.shiplocation;
            this.galaxy = other.galaxy;
            this.credits = other.credits;
            this.fuel = other.fuel;
            this.galaxy_number = other.galaxy_number;
            this.front_laser = other.front_laser;
            this.rear_laser = other.rear_laser;
            this.left_laser = other.left_laser;
            this.right_laser = other.right_laser;
            this.cargo_capacity = other.cargo_capacity;
            this.current_cargo = other.current_cargo;
            this.ecm = other.ecm;
            this.fuel_scoop = other.fuel_scoop;
            this.energy_bomb = other.energy_bomb;
            this.energy_unit = other.energy_unit;
            this.docking_computer = other.docking_computer;
            this.galactic_hyperdrive = other.galactic_hyperdrive;
            this.escape_pod = other.escape_pod;
            this.missiles = other.missiles;
            this.legal_status = other.legal_status;
            this.station_stock = other.station_stock;
            this.market_rnd = other.market_rnd;
            this.score = other.score;
            this.saved = other.saved;
        }

        public object Clone()
        {
            return new Commander(this);
        }
    }
}