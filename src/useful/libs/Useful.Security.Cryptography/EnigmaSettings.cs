// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The Enigma algorithm settings.
/// </summary>
public sealed record EnigmaSettings : IEnigmaSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaSettings"/> class.
    /// </summary>
    public EnigmaSettings()
    {
        Reflector = new EnigmaReflector() { ReflectorNumber = EnigmaReflectorNumber.B };
        Rotors = new EnigmaRotors();
        Plugboard = new EnigmaPlugboard();
    }

    /// <inheritdoc />
    public IEnigmaPlugboard Plugboard { get; init; }

    /// <inheritdoc />
    public IEnigmaReflector Reflector { get; init; }

    /// <inheritdoc />
    public IEnigmaRotors Rotors { get; init; }
}
