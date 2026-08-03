// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Ships;

namespace EliteSharpLib.Lasers;

internal sealed class MiningLaser : ILaser
{
    public string Name => "Mining";

    public int Strength => 50;

    public int Temperature { get; set; }

    public LaserType Type => LaserType.Mining;
}
