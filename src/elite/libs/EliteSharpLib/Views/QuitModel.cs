// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The quit confirmation's wording. Fixed, but still content rather than
/// layout - keeping it here is what stops each tier's view carrying its own
/// copy of the strings.
/// </summary>
internal sealed record QuitModel(string Header, string Prompt);
