// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// Accesses the Vigenere algorithm.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Vigenere"/> class.
/// </remarks>
/// <param name="settings">Settings.</param>
public sealed class Vigenere(IVigenereSettings settings) : ICipher
{
    /// <inheritdoc />
    public string CipherName => "Vigenere";

    /// <summary>
    /// Gets or sets settings.
    /// </summary>
    public IVigenereSettings Settings { get; set; } = settings;

    /// <inheritdoc />
    public string Encrypt(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        if (string.IsNullOrEmpty(Settings.Keyword))
        {
            return plaintext.ToUpperInvariant();
        }

        CaesarSettings caesarSettings = new();
        Caesar caesar = new(caesarSettings);
        StringBuilder ciphertext = new();
        int i = 0;

        foreach (char letter in plaintext.ToUpperInvariant())
        {
            if (letter is >= 'A' and <= 'Z')
            {
                caesarSettings.RightShift = Settings.Keyword[i % Settings.Keyword.Length] % 'A';
                ciphertext.Append(caesar.Encrypt(letter));
                i++;
            }
            else
            {
                ciphertext.Append(letter);
            }
        }

        return ciphertext.ToString();
    }

    /// <inheritdoc />
    public string Decrypt(string ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);

        if (string.IsNullOrEmpty(Settings.Keyword))
        {
            return ciphertext.ToUpperInvariant();
        }

        CaesarSettings caesarSettings = new();
        Caesar caesar = new(caesarSettings);
        StringBuilder plaintext = new();
        int i = 0;

        foreach (char letter in ciphertext.ToUpperInvariant())
        {
            if (letter is >= 'A' and <= 'Z')
            {
                caesarSettings.RightShift = Settings.Keyword[i % Settings.Keyword.Length] % 'A';
                plaintext.Append(caesar.Decrypt(letter));
                i++;
            }
            else
            {
                plaintext.Append(letter);
            }
        }

        return plaintext.ToString();
    }

    /////// <summary>
    /////// Generates random settings.
    /////// </summary>
    ////public void GenerateSettings() => Settings = VigenereSettingsGenerator.Generate() with { };

    /// <inheritdoc />
    public override string ToString() => CipherName;
}
