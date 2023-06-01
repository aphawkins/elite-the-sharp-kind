﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;

namespace EliteSharp
{
    public interface ILaser
    {
        string Name { get; }

        int Strength { get; }

        int Temperature { get; set; }

        LaserType Type { get; }
    }
}
