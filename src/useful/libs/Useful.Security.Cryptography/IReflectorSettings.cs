// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The monoalphabetic algorithm settings.
/// </summary>
public interface IReflectorSettings
{
    /// <summary>
    /// Gets substitutions.
    /// </summary>
    public IList<char> Substitutions { get; }

    /// <summary>
    /// Gets the character set.
    /// </summary>
    /// <value>The character set.</value>
    public IList<char> CharacterSet { get; }

    /// <summary>
    /// Gets the number of substitutions made. One distinct pair swapped equals one substitution.
    /// </summary>
    /// <value>The number of distinct substitutions.</value>
    /// <returns>The number of distinct substitutions made.</returns>
    public int SubstitutionCount { get; }

    /// <summary>
    /// Gets the current substitutions.
    /// </summary>
    /// <param name="substitution">The substitution to get.</param>
    public char GetSubstitution(char substitution);

    /// <summary>
    /// Sets the current substitutions.
    /// </summary>
    /// <param name="substitution">The position to set.</param>
    /// <param name="newSubstitution">The substitution to set.</param>
    public void SetSubstitution(char substitution, char newSubstitution);

    /// <summary>
    /// Gets the reverse substitution for a letter.
    /// </summary>
    /// <param name="letter">The letter to match.</param>
    /// <returns>The letter that substiutes to this letter.</returns>
    public char Reflect(char letter);
}
