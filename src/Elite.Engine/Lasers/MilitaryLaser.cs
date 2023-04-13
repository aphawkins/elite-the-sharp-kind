namespace Elite.Engine.Lasers
{
    using Elite.Engine.Enums;

    public class MilitaryLaser : ILaser
    {
        public string Name => "Military";

        public int Strength => 151;

        public LaserType Type => LaserType.Military;
    }
}
