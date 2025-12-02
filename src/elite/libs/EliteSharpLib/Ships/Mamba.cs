// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Mamba : ShipBase
{
    internal Mamba(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Mamba;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 15;
        EnergyMax = 90;
        LaserStrength = 9;
        LootMax = 1;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Mamba";
        Size = 4900;
        VanishPoint = 25;
        VelocityMax = 30;
    }
}
