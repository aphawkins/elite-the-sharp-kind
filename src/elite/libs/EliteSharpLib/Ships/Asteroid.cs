// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Asteroid : ShipBase
{
    internal Asteroid(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Asteroid;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        Bounty = 0.5f;
        EnergyMax = 60;
        MinDistance = 384;
        Name = "Asteroid";
        Size = 6400;
        VanishPoint = 50;
        VelocityMax = 30;
    }
}
