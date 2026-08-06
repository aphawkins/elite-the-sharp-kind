// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;

namespace Useful.Security.Cryptography;

/// <summary>
/// The Atbash cipher.
/// </summary>
public sealed class AtbashSymmetric : SymmetricAlgorithm
{
    private readonly Atbash _algorithm;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtbashSymmetric"/> class.
    /// </summary>
    public AtbashSymmetric()
    {
        Reset();
        _algorithm = new Atbash();
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
        IVValue = [];
        IV = IVValue;
    }

    /// <inheritdoc />
    public override void GenerateKey()
    {
        KeyValue = [];
        Key = KeyValue;
    }

    /// <inheritdoc />
    public override string ToString() => _algorithm.CipherName;

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
