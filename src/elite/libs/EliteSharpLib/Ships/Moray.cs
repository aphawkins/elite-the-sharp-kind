// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Moray : ShipBase
{
    internal Moray(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Moray;
        Flags = ShipProperties.LoneWolf | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 5;
        EnergyMax = 100;
        LaserStrength = 8;
        LootMax = 1;
        MinDistance = 384;
        Name = "Moray Star Boat";
        Size = 900;
        VanishPoint = 40;
        VelocityMax = 25;
    }
}
