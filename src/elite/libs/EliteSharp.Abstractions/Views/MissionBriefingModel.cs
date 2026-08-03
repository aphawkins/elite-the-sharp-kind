// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One screen of a mission's message sequence, whichever mission it came from.
/// One screen draws all of them, so it lays itself out from what is here -
/// whether there is a headline, how many paragraphs there are, whether somebody
/// is pictured - and never from which mission or which stage it came. That is
/// what lets a mission the game was never built against put a message up.
/// </summary>
/// <param name="Headline">
/// The line set above the message in the larger font, or empty when the message
/// stands on its own. Having one is what tells a congratulation from a plain
/// message.
/// </param>
/// <param name="Paragraphs">The message, split where it should be blocked on screen.</param>
/// <param name="ShowPortrait">Whether the speaker is pictured beside the message.</param>
public sealed record MissionBriefingModel(
    string Headline,
    IReadOnlyList<string> Paragraphs,
    bool ShowPortrait)
{
    /// <summary>
    /// Gets the screen with nothing on it, which the commander never sees: the
    /// screen is left as soon as no mission has anything to say.
    /// </summary>
    public static MissionBriefingModel Nothing { get; } = new(string.Empty, [], false);

    /// <summary>
    /// Gets a value indicating whether a headline is set above the message.
    /// </summary>
    public bool HasHeadline => !string.IsNullOrEmpty(Headline);
}
