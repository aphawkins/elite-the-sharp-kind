namespace Elite.Structs
{
    using Elite;

    internal struct commander
    {
        internal string name;
        internal int mission;
        internal int ship_x;
        internal int ship_y;
        internal galaxy_seed galaxy;
	    internal int credits;
        internal int fuel;
        internal int unused1;
        internal int galaxy_number;
        internal int front_laser;
        internal int rear_laser;
        internal int left_laser;
        internal int right_laser;
        internal int unused2;
        internal int unused3;
        internal int cargo_capacity;
        internal int[] current_cargo = new int[trade.NO_OF_STOCK_ITEMS];
        internal int ecm;
        internal int fuel_scoop;
        internal int energy_bomb;
        internal int energy_unit;
        internal int docking_computer;
        internal int galactic_hyperdrive;
        internal int escape_pod;
        internal int unused4;
        internal int unused5;
        internal int unused6;
        internal int unused7;
        internal int missiles;
        internal int legal_status;
        internal int[] station_stock = new int[trade.NO_OF_STOCK_ITEMS];
        internal int market_rnd;
        internal int score;
        internal int saved;

        public commander(string name, int mission, int ship_x, int ship_y, galaxy_seed galaxy, int credits, int fuel, int unused1, 
            int galaxy_number, int front_laser, int rear_laser, int left_laser, int right_laser, int unused2, int unused3, int cargo_capacity,
            int[] current_cargo, int ecm, int fuel_scoop, int energy_bomb, int energy_unit, int docking_computer, int galactic_hyperdrive, int escape_pod,
            int unused4, int unused5, int unused6, int unused7, int missiles, int legal_status, int[] station_stock, int market_rnd, int score, int saved)
        {
            this.name = name;
            this.mission = mission;
            this.ship_x = ship_x;
            this.ship_y = ship_y;
            this.galaxy = galaxy;
            this.credits = credits;
            this.fuel = fuel;
            this.unused1 = unused1;
            this.galaxy_number = galaxy_number;
            this.front_laser = front_laser;
            this.rear_laser = rear_laser;
            this.left_laser = left_laser;
            this.right_laser = right_laser;
            this.unused2 = unused2;
            this.unused3 = unused3;
            this.cargo_capacity = cargo_capacity;
            this.current_cargo = current_cargo;
            this.ecm = ecm;
            this.fuel_scoop = fuel_scoop;
            this.energy_bomb = energy_bomb;
            this.energy_unit = energy_unit;
            this.docking_computer = docking_computer;
            this.galactic_hyperdrive = galactic_hyperdrive;
            this.escape_pod = escape_pod;
            this.unused4 = unused4;
            this.unused5 = unused5;
            this.unused6 = unused6;
            this.unused7 = unused7;
            this.missiles = missiles;
            this.legal_status = legal_status;
            this.station_stock = station_stock;
            this.market_rnd = market_rnd;
            this.score = score;
            this.saved = saved;
        }
    };
}