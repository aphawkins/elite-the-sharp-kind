namespace Elite.Engine.Lasers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Elite.Engine.Enums;

    public class LaserNone : ILaser
    {
        public string Name => "None";

        public int Strength => 0;

        public LaserType Type => LaserType.None;
    }
}
