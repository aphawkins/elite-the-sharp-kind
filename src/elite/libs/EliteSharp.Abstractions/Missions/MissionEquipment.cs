// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Missions;

/// <summary>
/// The kit a mission may fit to the commander's ship. The game has no way to
/// look equipment up by name, so this lists what a mission can actually be
/// given rather than letting one ask for something that does not exist. It is
/// short because the game's own missions only ever hand over one thing; it
/// grows when a mission needs it to.
/// </summary>
public enum MissionEquipment
{
    /// <summary>
    /// The Navy's energy unit, which recharges faster than the one sold at
    /// tech level 8 and cannot be bought at all.
    /// </summary>
    NavalEnergyUnit = 0,
}
