// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib;

/// <summary>
/// The switch that turns the yaw controls on.
/// </summary>
/// <remarks>
/// Elite flies on two axes: the ship rolls and it pitches, and it turns by
/// rolling first. A third axis is a departure from the original, not a
/// correction to it, so it is hidden until it earns its place. With the
/// variable unset nothing reads the yaw controls, <see
/// cref="Ships.PlayerShip.Yaw"/> stays zero, and the game flies exactly as
/// it did.
/// </remarks>
internal static class DebugYaw
{
    /// <summary>
    /// Set (to any value) to fly with yaw. A runtime opt-in rather than a
    /// debug build, following <see cref="MissionJump.EnvVar"/>, so a Release
    /// build can be flown with it too. Unset in normal play.
    /// </summary>
    internal const string EnvVar = "ELITE_DEBUG_YAW";

    /// <summary>
    /// Gets a value indicating whether <see cref="EnvVar"/> is set.
    /// </summary>
    internal static bool IsEnabled => Environment.GetEnvironmentVariable(EnvVar) is not null;
}
