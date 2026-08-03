// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Missions;

/// <summary>
/// Who a briefing may show alongside its message. A mission is an assembly and
/// cannot bring artwork with it, so the choice is from the pictures the game
/// already ships - which is one - rather than from a name that might match
/// nothing.
/// </summary>
public enum MissionPortrait
{
    /// <summary>
    /// Agent Blake of Naval Intelligence, drawn beside the message he hands
    /// over.
    /// </summary>
    Blake = 0,
}
