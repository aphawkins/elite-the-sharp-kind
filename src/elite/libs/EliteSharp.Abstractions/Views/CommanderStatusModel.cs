// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The commander status screen's contents, already formatted. The labels
/// stay with the layout in the view; everything here is derived from the
/// commander and their ship.
/// </summary>
public sealed record CommanderStatusModel(
    string Title,
    string PresentSystem,
    string HyperspaceSystem,
    string Condition,
    string Fuel,
    string Cash,
    string LegalStatus,
    string Rating,
    IReadOnlyList<string> Equipment);
