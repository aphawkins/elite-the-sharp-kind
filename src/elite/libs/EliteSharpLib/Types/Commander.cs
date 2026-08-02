// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Missions;

namespace EliteSharpLib.Types;

internal sealed class Commander(MissionProgress missions)
{
    internal GalaxySeed Galaxy { get; set; } = new();

    internal int GalaxyNumber { get; set; }

    internal int LegalStatus { get; set; }

    /// <summary>
    /// Gets how far the commander has got in each mission. Which missions there
    /// are is settled at startup, so this is handed in rather than made here.
    /// </summary>
    internal MissionProgress Missions { get; } = missions;

    internal string Name { get; set; } = string.Empty;

    internal int Score { get; set; }
}
