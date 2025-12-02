// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Ships;

internal sealed class Alloy : ShipBase
{
    internal Alloy(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Alloy;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        EnergyMax = 16;
        MinDistance = 200;
        Name = "Alloy";
        ScoopedType = StockType.Alloys;
        Size = 100;
        VanishPoint = 5;
        VelocityMax = 16;
    }
}
