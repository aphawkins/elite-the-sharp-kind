namespace Elite.Engine.Lasers
{
    using Elite.Engine.Enums;

    public class LaserNone : ILaser
    {
        public string Name => "None";

        public int Strength => 0;

        public LaserType Type => LaserType.None;

        public int Temperature { get; set; } = 0;
    }
}
