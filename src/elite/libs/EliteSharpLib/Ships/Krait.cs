// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Krait : ShipBase
{
    internal Krait(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Krait;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 10;
        EnergyMax = 80;
        LaserStrength = 8;
        LootMax = 1;
        MinDistance = 384;
        Name = "Krait";
        Size = 3600;
        VanishPoint = 20;
        VelocityMax = 30;
    }
}
