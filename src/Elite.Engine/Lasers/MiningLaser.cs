namespace Elite.Engine.Lasers
{
    using Elite.Engine.Enums;

    public class MiningLaser : ILaser
    {
        public string Name => "Mining";

        public int Strength => 50;

        public LaserType Type => LaserType.Mining;
    }
}
