// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// The Caesar cipher.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Caesar"/> class.
/// </remarks>
/// <param name="settings">Settings.</param>
public sealed class Caesar(ICaesarSettings settings) : ICipher
{
    /// <inheritdoc />
    public string CipherName => "Caesar";

    /// <summary>
    /// Gets or sets settings.
    /// </summary>
    public ICaesarSettings Settings { get; set; } = settings;

    /// <inheritdoc />
    public string Decrypt(string ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);

        StringBuilder sb = new(ciphertext.Length);

        for (int i = 0; i < ciphertext.Length; i++)
        {
            sb.Append(Decrypt(ciphertext[i]));
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public string Encrypt(string plaintext)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        StringBuilder sb = new(plaintext.Length);

        for (int i = 0; i < plaintext.Length; i++)
        {
            sb.Append(Encrypt(plaintext[i]));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates random settings.
    /// </summary>
    public void GenerateSettings() => Settings = CaesarSettingsGenerator.Generate() with { };

    /// <inheritdoc />
    public override string ToString() => CipherName;

    internal char Encrypt(char letter)
        => letter is >= 'A' and <= 'Z'
            ? (char)(((letter - 'A' + Settings.RightShift) % 26) + 'A')
            : letter is >= 'a' and <= 'z' ? (char)(((letter - 'a' + Settings.RightShift) % 26) + 'A') : letter;

    internal char Decrypt(char letter)
        => letter is >= 'A' and <= 'Z'
            ? (char)(((letter - 'A' + 26 - Settings.RightShift) % 26) + 'A')
            : letter is >= 'a' and <= 'z' ? (char)(((letter - 'a' + 26 - Settings.RightShift) % 26) + 'A') : letter;
}
