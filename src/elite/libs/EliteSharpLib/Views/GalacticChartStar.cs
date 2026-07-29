// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// One plotted star on the galactic chart. <paramref name="Position"/> is in
/// galaxy space; <paramref name="IsWide"/> is the original's second pixel for
/// the brighter stars.
/// </summary>
internal readonly record struct GalacticChartStar(Vector2 Position, bool IsWide);
