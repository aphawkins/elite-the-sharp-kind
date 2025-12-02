// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Anaconda : ShipBase
{
    internal Anaconda(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Anaconda;
        Flags = ShipProperties.Trader | ShipProperties.Slow;
        EnergyMax = 252;
        LaserFront = 12;
        LaserStrength = 31;
        LootMax = 7;
        MinDistance = 800;
        MissilesMax = 7;
        Name = "Anaconda";
        Size = 10000;
        VanishPoint = 36;
        VelocityMax = 14;
    }
}
