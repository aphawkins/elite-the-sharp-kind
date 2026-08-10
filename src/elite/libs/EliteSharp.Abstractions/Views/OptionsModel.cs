// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The options menu: its rows, and which one the cursor is on. The version and
/// the credits are their own screen - see <see cref="CreditsModel"/>.
/// </summary>
public sealed record OptionsModel(IReadOnlyList<OptionRow> Options, int HighlightedIndex);
