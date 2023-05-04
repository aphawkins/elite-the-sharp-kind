using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class PulseLaser : ILaser
    {
        public string Name => "Pulse";

        public int Strength => 15;

        public LaserType Type => LaserType.Pulse;

        public int Temperature { get; set; } = 0;
    }
}
