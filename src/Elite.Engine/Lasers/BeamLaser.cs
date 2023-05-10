// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class BeamLaser : ILaser
    {
        public string Name => "Beam";

        public int Strength => 143;

        public LaserType Type => LaserType.Beam;

        public int Temperature { get; set; }
    }
}
