// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Abstraction.Config;

/// <summary>
/// The root of a game's config file: the schema version, shared engine settings
/// under <c>engine</c>, and the game's own settings under <c>game</c>. Each game
/// derives a concrete type from this so it can be referenced without repeating
/// the type argument.
/// </summary>
/// <typeparam name="TGameSettings">The game-specific settings type.</typeparam>
public abstract class ConfigSettings<TGameSettings>
    where TGameSettings : new()
{
    /// <summary>
    /// Gets or sets the schema version of the file. It exists so a later rename or restructure
    /// can be migrated rather than silently reset; a file with no version reads as
    /// the current one, which is what every file written before versioning is.
    /// </summary>
    public int Version { get; set; } = ConfigSchema.CurrentVersion;

    public EngineConfigSettings Engine { get; set; } = new();

    public TGameSettings Game { get; set; } = new();

    /// <summary>
    /// Replaces any value that cannot be honoured with its default, in place,
    /// so one bad entry costs the user that entry rather than the whole file.
    /// Games override this to add their own settings, calling the base first.
    /// </summary>
    /// <returns><see langword="true"/> if anything had to be replaced.</returns>
    public virtual bool Repair()
    {
        bool repaired = false;

        // A version from the future means the file was written by a later
        // build whose shape this one doesn't know. There's nothing to migrate
        // to, so it's stamped back and the settings are taken as read - the
        // caller keeps a copy of the original either way.
        if (Version < 1 || Version > ConfigSchema.CurrentVersion)
        {
            Version = ConfigSchema.CurrentVersion;
            repaired = true;
        }

        return Engine.Repair() || repaired;
    }
}
