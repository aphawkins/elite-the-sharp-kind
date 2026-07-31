// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// One planet blob on the short range chart, in screen space.
/// <paramref name="Size"/> is the original's radius, already scaled.
/// </summary>
internal readonly record struct ShortRangeChartPlanet(Vector2 Position, float Size);
