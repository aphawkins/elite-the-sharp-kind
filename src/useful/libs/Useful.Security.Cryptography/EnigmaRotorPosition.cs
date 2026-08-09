// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Enigma rotor positions.
/// </summary>
public enum EnigmaRotorPosition
{
    /// <summary>
    /// Right rotor - The fastest rotor.
    /// </summary>
    Fastest = 0,

    /// <summary>
    /// Second rotor - The middle speed rotor.
    /// </summary>
    Second = 1,

    /// <summary>
    /// Third rotor - The slowest rotor.
    /// </summary>
    Third = 2,
}
