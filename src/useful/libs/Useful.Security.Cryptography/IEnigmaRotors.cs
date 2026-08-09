// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Enigma rotor settings.
/// </summary>
public interface IEnigmaRotors
{
    /// <summary>
    /// Gets the rotors.
    /// </summary>
    public IReadOnlyDictionary<EnigmaRotorPosition, IEnigmaRotor> Rotors { get; init; }

    /// <summary>
    /// Gets the rotor settings.
    /// </summary>
    /// <param name="position">The rotor position.</param>
    /// <returns>The rotor in this position.</returns>
    public IEnigmaRotor this[EnigmaRotorPosition position] { get; }

    /// <summary>
    /// Advances the rotor one setting.
    /// </summary>
    public void AdvanceRotors();
}
