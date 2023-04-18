namespace Elite.Engine.Lasers
{
    using Elite.Engine.Enums;

    public class PulseLaser : ILaser
    {
        public string Name => "Pulse";

        public int Strength => 15;

        public LaserType Type => LaserType.Pulse;

        public int Temperature { get; set; } = 0;
    }
}
