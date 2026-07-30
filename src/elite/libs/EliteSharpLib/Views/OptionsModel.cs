// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The options menu: its rows, which one the cursor is on, and the credits
/// footer. <paramref name="Version"/> is read from the assembly, so it is
/// content the controller supplies rather than something the view can derive.
/// </summary>
internal sealed record OptionsModel(
    IReadOnlyList<OptionRow> Options,
    int HighlightedIndex,
    string Version,
    IReadOnlyList<string> Credits);
