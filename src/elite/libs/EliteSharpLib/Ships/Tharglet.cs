// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Ships;

internal sealed class Tharglet : ShipBase
{
    internal Tharglet(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Tharglet;
        Flags = ShipProperties.Tharglet | ShipProperties.Angry;
        Bounty = 5;
        EnergyMax = 20;
        LaserStrength = 8;
        MinDistance = 384;
        Name = "Tharglet";
        ScoopedType = StockType.AlienItems;
        Size = 1600;
        VanishPoint = 20;
        VelocityMax = 30;
    }
}
