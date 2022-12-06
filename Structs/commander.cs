namespace EliteLib.Structs
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
    };
}