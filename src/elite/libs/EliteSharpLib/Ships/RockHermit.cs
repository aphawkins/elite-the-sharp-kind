// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class RockHermit : ShipBase
{
    internal RockHermit(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Hermit;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Slow;
        EnergyMax = 180;
        LaserStrength = 1;
        LootMax = 7;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Rock Hermit";
        Size = 6400;
        VanishPoint = 50;
        VelocityMax = 30;
    }
}
