// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// The six bytes the galaxy's systems are generated from, named as
/// <see cref="Types.GalaxySeed"/> names them.
/// </summary>
public sealed class GalaxySeedState
{
    public int A { get; set; }

    public int B { get; set; }

    public int C { get; set; }

    public int D { get; set; }

    public int E { get; set; }

    public int F { get; set; }
}
