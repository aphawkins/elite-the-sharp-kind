// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One row of the options menu. <paramref name="IsEnabled"/> is false for a
/// docked-only option while undocked, which the view greys out rather than
/// hides.
/// </summary>
public readonly record struct OptionRow(string Label, bool IsEnabled);
