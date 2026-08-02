// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// Where on the galactic chart the commander is docked. The two seed bytes
/// the system is searched for by, named as
/// <see cref="Types.GalaxySeed"/> names them.
/// </summary>
public sealed class ShipLocationState
{
    public int D { get; set; }

    public int B { get; set; }
}
