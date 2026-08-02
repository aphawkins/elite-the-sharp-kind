// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Types;

namespace EliteSharpLib.Views;

/// <summary>
/// One screen of the Constrictor mission's message sequence. The briefing
/// text is content and lives here; <paramref name="Stage"/> (the brief or the
/// debrief) is what the view keys its layout off, since the two are laid out
/// differently.
/// </summary>
internal sealed record ConstrictorMissionModel(
    ConstrictorStage Stage,
    string Headline,
    IReadOnlyList<string> Paragraphs);
