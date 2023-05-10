// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    public class LaserNone : ILaser
    {
        public string Name => "None";

        public int Strength => 0;

        public int Temperature { get; set; }
        public LaserType Type => LaserType.None;
    }
}
