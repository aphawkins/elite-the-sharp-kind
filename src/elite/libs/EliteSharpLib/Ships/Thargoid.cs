// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Thargoid : ShipBase
{
    internal Thargoid(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Thargoid;
        Flags = ShipProperties.LoneWolf | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 50;
        EnergyMax = 240;
        LaserFront = 15;
        LaserStrength = 11;
        MinDistance = 700;
        MissilesMax = 6;
        Name = "Thargoid";
        Size = 9801;
        VanishPoint = 55;
        VelocityMax = 39;
    }
}
