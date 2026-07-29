// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The inventory screen's contents: fuel, cash, and only the cargo actually
/// held, each already formatted with its units.
/// </summary>
internal sealed record InventoryModel(
    string Title,
    string Fuel,
    string Cash,
    IReadOnlyList<(string Name, string Quantity)> Cargo);
