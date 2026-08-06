// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The Atbash cipher.
/// </summary>
public sealed class Atbash : ICipher
{
    /// <inheritdoc />
    public string CipherName => "Atbash";

    /// <inheritdoc />
    public string Decrypt(string ciphertext)
        => Encrypt(ciphertext); // To decipher just need to use the encryption method as the cipher is reversible

    /// <inheritdoc />
    public string Encrypt(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        // A=Z, B=Y, C=X, etc
        return Alphabet.Map(plaintext, 0, static (_, index) => Alphabet.Length - 1 - index);
    }

    /// <inheritdoc />
    public override string ToString() => CipherName;
}
