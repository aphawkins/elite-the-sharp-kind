// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// An Enigma Rotor.
/// </summary>
public interface IEnigmaRotor
{
    /// <summary>
    /// Gets or sets the current letter the rotor is set to.
    /// </summary>
    public char CurrentSetting { get; set; }

    /// <summary>
    /// Gets the notches.
    /// </summary>
    public string Notches { get; }

    /// <summary>
    /// Gets or sets the current letter the rotor's ring is set to.
    /// </summary>
    public int RingPosition { get; set; }

    /// <summary>
    /// Gets or sets the designation of this rotor.
    /// </summary>
    public EnigmaRotorNumber RotorNumber { get; set; }

    /// <summary>
    /// The letter this rotor encodes to going backwards through it.
    /// </summary>
    /// <param name="letter">The letter to transform.</param>
    /// <returns>The transformed letter.</returns>
    public char Backward(char letter);

    /// <summary>
    /// The letter this rotor encodes to going forward through it.
    /// </summary>
    /// <param name="letter">The letter to transform.</param>
    /// <returns>The transformed letter.</returns>
    public char Forward(char letter);
}
