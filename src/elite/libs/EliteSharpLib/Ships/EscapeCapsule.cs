// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Ships;

internal sealed class EscapeCapsule : ShipBase
{
    internal EscapeCapsule(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.EscapeCapsule;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Slow | ShipProperties.FlyToPlanet;
        EnergyMax = 17;
        MinDistance = 200;
        Name = "Escape Capsule";
        ScoopedType = StockType.Slaves;
        Size = 256;
        VanishPoint = 8;
        VelocityMax = 8;
    }
}
