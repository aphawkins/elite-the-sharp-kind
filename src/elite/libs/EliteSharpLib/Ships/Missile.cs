// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Missile : ShipBase
{
    internal Missile(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Missile;
        Flags = ShipProperties.Missile;
        EnergyMax = 2;
        MinDistance = 200;
        Name = "Missile";
        Size = 1600;
        VanishPoint = 14;
        VelocityMax = 44;
    }
}
