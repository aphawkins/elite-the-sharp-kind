using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class MiningLaser : ILaser
    {
        public string Name => "Mining";

        public int Strength => 50;

        public LaserType Type => LaserType.Mining;

        public int Temperature { get; set; } = 0;
    }
}
