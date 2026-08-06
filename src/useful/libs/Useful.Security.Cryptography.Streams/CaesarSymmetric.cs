// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// The Caesar cipher.
/// </summary>
public sealed class CaesarSymmetric : SymmetricAlgorithm
{
    private Caesar _algorithm;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaesarSymmetric"/> class.
    /// </summary>
    public CaesarSymmetric()
    {
        Reset();
        _algorithm = new(new CaesarSettings());
    }

    /// <inheritdoc />
    public override byte[] Key
    {
        get => Encoding.Unicode.GetBytes($"{_algorithm.Settings.RightShift}");

        set
        {
            _algorithm = new(GetSettings(value));
            base.Key = value;
        }
    }

    /// <inheritdoc />
    public override byte[] IV
    {
        get => [];
        set => _ = value;
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

    private static CaesarSettings GetSettings(byte[] key) => key == null
            ? throw new ArgumentNullException(nameof(key))
            : !int.TryParse(Encoding.Unicode.GetString(key), out int rightShift)
            ? throw new ArgumentException("Value must be a number.", nameof(key))
            : rightShift is < 0 or > 25
            ? throw new ArgumentOutOfRangeException(nameof(key), "Value must be between 0 and 25.")
            : new CaesarSettings() { RightShift = rightShift };

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
