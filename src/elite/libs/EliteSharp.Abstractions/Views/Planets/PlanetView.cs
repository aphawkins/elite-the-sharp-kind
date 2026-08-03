// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Abstractions.Views.Planets;

/// <summary>
/// A planet as it appears this frame: the game has done the projection, so
/// what arrives is already in screen terms. A renderer never sees the
/// planet's position in space, which is what keeps the universe on the game's
/// side of the seam.
/// </summary>
/// <param name="Centre">Where the planet's centre is on screen.</param>
/// <param name="Radius">How big it is on screen, in pixels.</param>
/// <param name="Orientation">
/// Which way up it is. The rows are the original's sidev, roofv and nosev,
/// which the surface styles map their detail from.
/// </param>
/// <param name="UnitScale">
/// Pixels per unit of the original's 256-wide space. A renderer with a
/// threshold written in the original's terms - "no surface detail below a
/// radius of 6" - multiplies by this rather than assuming a resolution.
/// </param>
public readonly record struct PlanetView(
    Vector2 Centre,
    float Radius,
    Matrix4x4 Orientation,
    float UnitScale);
