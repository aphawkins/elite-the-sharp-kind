namespace Elite.Engine.Types
{
    public class Commander
    {
        public string name { get; set; }
        public int mission { get; set; }
        public galaxy_seed galaxy { get; set; } = new();
        public int galaxy_number { get; set; }
        public int legal_status { get; set; }
        public int score { get; set; }
        public int saved { get; set; }
    }
}