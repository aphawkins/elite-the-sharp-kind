// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// Which laser is fitted to each of the ship's four mounts, named rather than
/// left to the order of an array.
/// </summary>
public sealed class LaserMountState
{
    public string Front { get; set; } = string.Empty;

    public string Rear { get; set; } = string.Empty;

    public string Left { get; set; } = string.Empty;

    public string Right { get; set; } = string.Empty;
}
