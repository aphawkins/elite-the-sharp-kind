// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// One screen of the Thargoid mission's message sequence. The briefing text
/// is content and lives here; <paramref name="Stage"/> (the commander's
/// mission number, 4-6) is what the view keys its layout off, since the
/// three messages are laid out differently.
/// </summary>
internal sealed record ThargoidMissionModel(
    int Stage,
    string Headline,
    IReadOnlyList<string> Paragraphs,
    bool ShowPortrait);
