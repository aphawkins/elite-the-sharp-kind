// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Types;

/// <summary>
/// The band the commander's bounty falls in. The bounty is the value the game
/// works in; the band is what the status screen shows and what the save file
/// writes alongside it, so both name it the same way.
/// </summary>
internal static class LegalStatusBand
{
    /// <summary>
    /// The largest bounty the game can hold, and so the largest a save may carry.
    /// </summary>
    internal const int BountyMax = 255;

    private const int FugitiveBounty = 50;

    internal static string For(int bounty) => bounty == 0
        ? "Clean"
        : bounty > FugitiveBounty ? "Fugitive" : "Offender";
}
