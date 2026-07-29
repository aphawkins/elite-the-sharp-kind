// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Abstraction.Config;

/// <summary>
/// Facts about the config file format itself, shared by every game.
/// </summary>
public static class ConfigSchema
{
    /// <summary>
    /// Gets the schema version this build writes. Bump it whenever the shape of the
    /// file changes in a way a migration would need to recognise, and handle the
    /// older versions in <see cref="ConfigSettings.Repair"/>.
    /// </summary>
    public static int CurrentVersion => 1;
}
