// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal class CobraMk3 : ShipBase
{
    internal CobraMk3(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.CobraMk3;
        Flags = ShipProperties.Trader;
        EnergyMax = 150;
        LaserFront = 21;
        LaserStrength = 9;
        LootMax = 3;
        MinDistance = 420;
        MissilesMax = 3;
        Name = "Cobra MkIII";
        Size = 9025;
        VanishPoint = 50;
        VelocityMax = 28;
    }
}
