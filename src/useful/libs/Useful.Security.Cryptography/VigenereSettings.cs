// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Settings for the Vigenere cipher.
/// </summary>
public sealed record VigenereSettings : IVigenereSettings
{
    /// <summary>
    /// Gets or sets the keyword of the cipher.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Length must be between 0 and 26 letters.</exception>
    public string Keyword
    {
        get => field ?? string.Empty;

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length > 25)
            {
                throw new ArgumentOutOfRangeException(nameof(Keyword), "Length must be between 0 and 26 letters.");
            }

            field = value.ToUpperInvariant();
        }
    }
}
