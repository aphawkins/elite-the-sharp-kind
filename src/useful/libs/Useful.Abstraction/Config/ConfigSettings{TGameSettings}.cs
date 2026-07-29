// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Abstraction.Config;

/// <summary>
/// The root of a game's config file: the shared half in <see cref="ConfigSettings"/>,
/// plus the game's own settings under <c>game</c>. Each game derives a concrete
/// type from this so it can be referenced without repeating the type argument.
/// </summary>
/// <typeparam name="TGameSettings">The game-specific settings type.</typeparam>
public abstract class ConfigSettings<TGameSettings> : ConfigSettings
    where TGameSettings : new()
{
    public TGameSettings Game { get; set; } = new();
}
