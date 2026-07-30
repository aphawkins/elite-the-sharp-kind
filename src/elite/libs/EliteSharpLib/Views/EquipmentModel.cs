// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The equip-ship screen: its visible rows, in draw order, and the cash line.
/// </summary>
internal sealed record EquipmentModel(IReadOnlyList<EquipmentRow> Rows, string Cash);
