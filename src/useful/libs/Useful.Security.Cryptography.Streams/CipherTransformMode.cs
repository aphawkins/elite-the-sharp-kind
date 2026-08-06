// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Define the direction of encryption.
/// </summary>
public enum CipherTransformMode
{
    /// <summary>
    /// Encryption transformer.
    /// </summary>
    Encrypt = 0,

    /// <summary>
    /// Decryption transformer.
    /// </summary>
    Decrypt = 1,
}
