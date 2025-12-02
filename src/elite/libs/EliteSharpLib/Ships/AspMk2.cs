// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class AspMk2 : ShipBase
{
    internal AspMk2(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.AspMk2;
        Flags = ShipProperties.LoneWolf | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 20;
        EnergyMax = 150;
        LaserFront = 8;
        LaserStrength = 20;
        MinDistance = 384;
        MissilesMax = 1;
        Name = "Asp MkII";
        Size = 3600;
        VanishPoint = 40;
        VelocityMax = 40;
    }
}
