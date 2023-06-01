﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Lasers
{
    internal static class LaserFactory
    {
        internal static ILaser GetLaser(LaserType type) => type switch
        {
            LaserType.Military => new MilitaryLaser(),
            LaserType.Mining => new MiningLaser(),
            LaserType.Pulse => new PulseLaser(),
            LaserType.Beam => new BeamLaser(),
            LaserType.None => new LaserNone(),
            _ => new LaserNone(),
        };
    }
}
