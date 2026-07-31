// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// One planet's name on the short range chart, in screen space. Fewer of
/// these than there are planets: a name is only placed when the row packing
/// finds it a free text row.
/// </summary>
internal readonly record struct ShortRangeChartLabel(Vector2 Position, string Name);
