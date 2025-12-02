// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Worm : ShipBase
{
    internal Worm(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Worm;
        Flags = ShipProperties.PackHunter | ShipProperties.Slow | ShipProperties.Angry;
        EnergyMax = 30;
        LaserStrength = 4;
        MinDistance = 384;
        Name = "Worm";
        Size = 9801;
        VanishPoint = 19;
        VelocityMax = 23;
    }
}
