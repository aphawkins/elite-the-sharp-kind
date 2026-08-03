// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One row of a settings list: a name and its current value, already
/// resolved to display text. The Back row (always last) carries an empty
/// value, matching the original's own special case for it.
/// </summary>
public readonly record struct SettingsRow(string Name, string Value);
