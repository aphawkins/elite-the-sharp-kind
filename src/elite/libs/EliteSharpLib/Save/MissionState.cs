// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// Where the commander has got to in one mission. An object rather than a
/// bare stage, so a mission that later needs state of its own - a deadline,
/// a system to reach - can carry it without the file changing shape again.
/// </summary>
public sealed class MissionState
{
    public string Stage { get; set; } = string.Empty;
}
