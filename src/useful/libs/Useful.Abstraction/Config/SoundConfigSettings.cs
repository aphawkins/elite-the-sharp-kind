// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Abstraction.Config;

/// <summary>
/// The engine's sound settings, stored under the config file's
/// <c>engine.sound</c> element.
/// </summary>
public sealed class SoundConfigSettings
{
    public bool Effects { get; set; } = true;

    public bool Music { get; set; } = true;
}
