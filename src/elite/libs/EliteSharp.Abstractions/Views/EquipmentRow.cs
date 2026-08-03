// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One visible row of the equip-ship list - hidden stock (laser sub-items
/// collapsed behind their category) never reaches the view at all.
/// <paramref name="Price"/> is empty when nothing should be shown, matching
/// the original's own truncate-to-int check on a computed price of zero.
/// </summary>
public readonly record struct EquipmentRow(
    string Name,
    bool IsIndented,
    bool IsAffordable,
    bool IsHighlighted,
    string Price);
