// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class MilitaryLaser : ILaser
    {
        public string Name => "Military";

        public int Strength => 151;

        public LaserType Type => LaserType.Military;

        public int Temperature { get; set; }
    }
}
