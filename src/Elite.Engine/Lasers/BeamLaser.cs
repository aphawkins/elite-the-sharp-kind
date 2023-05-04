using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class BeamLaser : ILaser
    {
        public string Name => "Beam";

        public int Strength => 143;

        public LaserType Type => LaserType.Beam;

        public int Temperature { get; set; } = 0;
    }
}
