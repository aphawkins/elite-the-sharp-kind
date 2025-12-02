// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Ships;

internal sealed class RockSplinter : ShipBase
{
    internal RockSplinter(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Rock;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        EnergyMax = 20;
        MinDistance = 200;
        Name = "Rock Splinter";
        ScoopedType = StockType.Minerals;
        Size = 256;
        VanishPoint = 8;
        VelocityMax = 10;
    }
}
