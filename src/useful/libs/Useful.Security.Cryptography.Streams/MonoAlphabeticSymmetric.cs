// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// The MonoAlphabetic cipher.
/// </summary>
public class MonoAlphabeticSymmetric : SymmetricAlgorithm
{
    /// <summary>
    /// States how many parts there are in the key.
    /// </summary>
    private const int KeyParts = 2;

    /// <summary>
    /// The char that separates part of the key.
    /// </summary>
    private const char KeySeperator = '|';

    /// <summary>
    /// The encoding used by this cipher.
    /// </summary>
    private static readonly Encoding s_encoding = new UnicodeEncoding(false, false);

    private readonly MonoAlphabetic _algorithm;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonoAlphabeticSymmetric"/> class.
    /// </summary>
    public MonoAlphabeticSymmetric()
    {
        Reset();
        _algorithm = new MonoAlphabetic(new MonoAlphabeticSettings());
    }

    /// <inheritdoc />
    public override byte[] IV
    {
        get => [];
        set => _ = value;
    }

    /// <inheritdoc />
    public override byte[] Key
    {
        get
        {
            // CharacterSet|Substitutions
            StringBuilder key = new(_algorithm.Settings.CharacterSet);
            key.Append(KeySeperator)
                .Append(_algorithm.Settings.Substitutions);

            return s_encoding.GetBytes(key.ToString());
        }

        set
        {
            (string CharacterSet, string Substitutions) key;
            try
            {
                key = ParseKey(value);

                _algorithm.Settings = new MonoAlphabeticSettings()
                {
                    CharacterSet = key.CharacterSet,
                    Substitutions = key.Substitutions,
                };
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Argument exception.", nameof(value), ex);
            }

            base.Key = value;
        }
    }

    /// <inheritdoc />
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        Key = rgbKey;
        IV = rgbIV ?? [];
        return new ClassicalSymmetricTransform(_algorithm, CipherTransformMode.Decrypt);
    }

    /// <inheritdoc />
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        Key = rgbKey;
        IV = rgbIV ?? [];
        return new ClassicalSymmetricTransform(_algorithm, CipherTransformMode.Encrypt);
    }

    /// <inheritdoc />
    public override void GenerateIV()
    {
    }

    /// <inheritdoc />
    public override void GenerateKey()
    {
        _algorithm.GenerateSettings();
        KeyValue = Key;
    }

    /// <inheritdoc />
    public override string ToString() => _algorithm.CipherName;

    private static (string CharacterSet, string Substitutions) ParseKey(byte[] key)
    {
        // Example:
        // characterSet|substitutions
        ArgumentNullException.ThrowIfNull(key);

        if (key.SequenceEqual([]))
        {
            throw new ArgumentException("Invalid format.", nameof(key));
        }

        string keyString = s_encoding.GetString(key);

        string[] parts = keyString.Split([KeySeperator], StringSplitOptions.None);

        return parts.Length != KeyParts
            ? throw new ArgumentException("Incorrect number of key parts.", nameof(key))
            : ((string CharacterSet, string Substitutions))(parts[0], parts[1]);
    }

    private void Reset()
    {
#pragma warning disable CA5358 // Do Not Use Unsafe Cipher Modes - this cipher is inherently unsafe
        ModeValue = CipherMode.ECB;
        PaddingValue = PaddingMode.None;
        KeySizeValue = 16;
        BlockSizeValue = 16;
        FeedbackSizeValue = 16;
        LegalBlockSizesValue = new KeySizes[1];
        LegalBlockSizesValue[0] = new KeySizes(16, 16, 16);
        LegalKeySizesValue = new KeySizes[1];
        LegalKeySizesValue[0] = new KeySizes(0, int.MaxValue, 16);

        KeyValue = [];
        IVValue = [];
    }
}
