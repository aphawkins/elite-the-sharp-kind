// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Interface that all ciphers should implement.
/// </summary>
public interface ICipher
{
    /// <summary>
    /// Gets the name of this cipher.
    /// </summary>
    public string CipherName { get; }

    /// <summary>
    /// Encrypts a plaintext string.
    /// </summary>
    /// <param name="plaintext">The text to encrypt.</param>
    /// <returns>The encrypted text.</returns>
    public string Encrypt(string plaintext);

    /// <summary>
    /// Decrypts a ciphertext string.
    /// </summary>
    /// <param name="ciphertext">The text to decrypt.</param>
    /// <returns>The decrypted text.</returns>
    public string Decrypt(string ciphertext);
}
