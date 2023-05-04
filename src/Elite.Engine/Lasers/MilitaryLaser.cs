using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class MilitaryLaser : ILaser
    {
        public string Name => "Military";

        public int Strength => 151;

        public LaserType Type => LaserType.Military;

        public int Temperature { get; set; } = 0;
    }
}
