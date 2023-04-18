namespace Elite.Engine.Lasers
{
    using Elite.Engine.Enums;

    public class BeamLaser : ILaser
    {
        public string Name => "Beam";

        public int Strength => 143;

        public LaserType Type => LaserType.Beam;

        public int Temperature { get; set; } = 0;
    }
}
