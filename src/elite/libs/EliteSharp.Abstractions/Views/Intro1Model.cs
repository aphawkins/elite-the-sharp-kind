// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The title screen's wording: the credit lines in display order, and the
/// load-commander prompt. Content, not layout, so both tiers share it.
/// </summary>
public sealed record Intro1Model(IReadOnlyList<string> Credits, string Prompt);
