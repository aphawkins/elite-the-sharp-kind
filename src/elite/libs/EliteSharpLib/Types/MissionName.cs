// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Types;

/// <summary>
/// The missions the commander can have a stage in. Each has its own stage
/// type, so the save file names both the mission and where in it the
/// commander is, and a mission added later needs no renumbering of these.
/// </summary>
internal enum MissionName
{
    /// <summary>
    /// The Navy's hunt for the stolen Constrictor.
    /// </summary>
    Constrictor = 0,

    /// <summary>
    /// Running the Thargoid defence plans to Birera.
    /// </summary>
    Thargoid = 1,
}
