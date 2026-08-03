// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// A settings screen: its rows (the Back row is always last), which one the
/// cursor is on, and an optional footer note shown under the Back row.
/// </summary>
public sealed record SettingsListModel(
    string Header,
    IReadOnlyList<SettingsRow> Rows,
    int HighlightedIndex,
    string Footer);
