// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal class Python : ShipBase
{
    internal Python(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Python;
        Flags = ShipProperties.Trader;
        EnergyMax = 250;
        LaserStrength = 13;
        LootMax = 5;
        MinDistance = 900;
        MissilesMax = 3;
        Name = "Python";
        Size = 6400;
        VanishPoint = 40;
        VelocityMax = 20;
    }
}
