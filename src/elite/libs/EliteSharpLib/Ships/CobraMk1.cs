// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class CobraMk1 : ShipBase
{
    internal CobraMk1(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.CobraMk1;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 7.5f;
        EnergyMax = 90;
        LaserFront = 10;
        LaserStrength = 9;
        LootMax = 3;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Cobra MkI";
        Size = 9801;
        VanishPoint = 19;
        VelocityMax = 26;
    }
}
