// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Coriolis : ShipBase
{
    internal Coriolis(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Coriolis;
        Flags = ShipProperties.Station;
        EnergyMax = 240;
        LaserStrength = 3;
        MinDistance = 800;
        MissilesMax = 6;
        Name = "Coriolis Space Station";
        Size = 25600;
        VanishPoint = 120;
    }
}
