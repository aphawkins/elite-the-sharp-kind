// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The docked planet's market: its stock rows and the commander's cash.
/// <paramref name="Cash"/> is left as a raw number since the view right-aligns
/// it to a fixed column width, a layout concern rather than content.
/// </summary>
internal sealed record MarketModel(string Title, IReadOnlyList<MarketRow> Rows, float Cash);
