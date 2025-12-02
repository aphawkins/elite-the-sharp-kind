// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Boa : ShipBase
{
    internal Boa(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Boa;
        Flags = ShipProperties.Trader;
        EnergyMax = 250;
        LaserStrength = 14;
        LootMax = 5;
        MinDistance = 500;
        MissilesMax = 4;
        Name = "Boa";
        Size = 4900;
        VanishPoint = 40;
        VelocityMax = 24;
    }
}
