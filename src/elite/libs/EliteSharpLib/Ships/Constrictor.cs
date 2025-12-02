// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Constrictor : ShipBase
{
    internal Constrictor(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Constrictor;
        Flags = ShipProperties.LoneWolf | ShipProperties.Angry;
        EnergyMax = 252;
        LaserStrength = 26;
        LootMax = 3;
        MissilesMax = 4;
        Name = "Constrictor";
        Size = 4225;
        VanishPoint = 45;
        VelocityMax = 36;
    }
}
