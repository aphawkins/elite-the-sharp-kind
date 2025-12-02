// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Viper : ShipBase
{
    internal Viper(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Viper;
        Flags = ShipProperties.Police | ShipProperties.Bold;
        EnergyMax = 140;
        LaserStrength = 8;
        MinDistance = 384;
        MissilesMax = 1;
        Name = "Viper";
        Size = 5625;
        VanishPoint = 23;
        VelocityMax = 32;
    }
}
