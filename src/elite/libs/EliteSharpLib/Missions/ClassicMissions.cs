// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;
using Microsoft.Extensions.Logging.Abstractions;

namespace EliteSharpLib.Missions;

/// <summary>
/// The two missions the game has always had. They are built in rather than
/// found on disk, and they move out to a plugin of their own once they carry
/// their own behaviour.
/// </summary>
internal static class ClassicMissions
{
    /// <summary>
    /// Gets the built-in missions, freshly built. Missions hold no state, but
    /// no two registries should share instances either.
    /// </summary>
    internal static IEnumerable<IMission> All => [new ConstrictorMission(), new ThargoidMission()];

    /// <summary>
    /// A registry of the built-in missions alone, for everything that has no
    /// plugin folder to look in - which is every test that is not about
    /// loading plugins.
    /// </summary>
    /// <returns>The registry.</returns>
    internal static MissionRegistry Registry() => new(All, NullLogger<MissionRegistry>.Instance);
}
