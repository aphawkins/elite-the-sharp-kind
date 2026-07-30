// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The game over screen's wording. Fixed, but content rather than layout, so
/// both tiers share it instead of each carrying its own copy.
/// </summary>
internal sealed record GameOverModel(string Message);
