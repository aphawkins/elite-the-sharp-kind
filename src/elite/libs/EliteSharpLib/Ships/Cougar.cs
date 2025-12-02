// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Cougar : ShipBase
{
    internal Cougar(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Cougar;
        Flags = ShipProperties.LoneWolf | ShipProperties.Police | ShipProperties.Cloaked;
        EnergyMax = 252;
        LaserStrength = 26;
        LootMax = 3;
        MissilesMax = 4;
        Name = "Cougar";
        Size = 4900;
        VanishPoint = 34;
        VelocityMax = 40;
    }
}
