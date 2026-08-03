// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// A sun as it appears this frame. Unlike a planet it has no orientation - it
/// is the same all the way round - so where it is and how big is the whole of
/// it.
/// </summary>
/// <param name="Centre">Where the sun's centre is on screen.</param>
/// <param name="Radius">How big it is on screen, in pixels.</param>
public readonly record struct SunView(Vector2 Centre, float Radius);
