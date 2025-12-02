// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Sidewinder : ShipBase
{
    internal Sidewinder(IEliteDraw draw)
    : base(draw)
    {
        Type = ShipType.Sidewinder;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 5;
        EnergyMax = 70;
        LaserStrength = 8;
        MinDistance = 384;
        Name = "Sidewinder";
        Size = 4225;
        VanishPoint = 20;
        VelocityMax = 37;
    }
}
