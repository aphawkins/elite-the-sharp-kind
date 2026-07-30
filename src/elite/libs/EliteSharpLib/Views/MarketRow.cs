// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// One row of the market prices list. <paramref name="ForSaleQuantity"/> and
/// <paramref name="InHoldQuantity"/> are left as raw counts rather than
/// formatted text: the view draws each as a right-aligned number followed by
/// a separately positioned unit suffix, which is a layout choice, not content.
/// </summary>
internal readonly record struct MarketRow(
    string Name,
    string Units,
    float Price,
    int ForSaleQuantity,
    int InHoldQuantity,
    bool IsHighlighted);
