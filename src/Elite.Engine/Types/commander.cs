namespace Elite.Engine.Types
{
    public class Commander
    {
        public string name { get; set; }
        public int mission { get; set; }
        public galaxy_seed galaxy { get; set; } = new();
        public float credits { get; set; }
        public int galaxy_number { get; set; }
        public int[] current_cargo { get; set; } = new int[17];
        public int legal_status { get; set; }
        public int[] station_stock { get; set; } = new int[17];
        public int market_rnd { get; set; }
        public int score { get; set; }
        public int saved { get; set; }
    }
}