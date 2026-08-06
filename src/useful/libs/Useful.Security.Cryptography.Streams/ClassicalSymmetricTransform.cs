// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;

namespace Useful.Security.Cryptography;

internal sealed class ClassicalSymmetricTransform : ICryptoTransform
{
    private const int BlockSize = 2;  // 2 for Unicode, 1 for UTF8
    private readonly CipherTransformMode _transformMode;
    private readonly Encoding _encoding = new UnicodeEncoding();

    internal ClassicalSymmetricTransform(ICipher cipher, CipherTransformMode transformMode)
    {
        _transformMode = transformMode;
        Cipher = cipher;
    }

    public bool CanReuseTransform => true;

    public bool CanTransformMultipleBlocks => false;

    public int InputBlockSize => BlockSize;

    public int OutputBlockSize => BlockSize;

    public ICipher Cipher { get; }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    public static void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
        }

        // free native resources if there are any.
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        if (inputCount <= 0)
        {
            // Nothing to do
            return 0;
        }

        ArgumentNullException.ThrowIfNull(inputBuffer);

        ArgumentNullException.ThrowIfNull(outputBuffer);

        if (inputBuffer.Length < (inputOffset + inputCount))
        {
            throw new ArgumentException("Input buffer not long enough.", nameof(inputBuffer));
        }

        string inputString = new(_encoding.GetChars(inputBuffer));

        string outputString = _transformMode switch
        {
            CipherTransformMode.Encrypt => Cipher.Encrypt(inputString),
            CipherTransformMode.Decrypt => Cipher.Decrypt(inputString),
            _ => throw new CryptographicException($"Unsupported transform mode {_transformMode}."),
        };

        if (string.IsNullOrEmpty(outputString) || outputString == "\0")
        {
            return 0;
        }

        byte[] outputBytes = _encoding.GetBytes(outputString);
        Array.Copy(outputBytes, 0, outputBuffer, 0, OutputBlockSize);
        return inputCount;
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        if (inputBuffer[0] == 0)
        {
            return [];
        }

        byte[] outputBuffer = new byte[inputBuffer.Length];
        return TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0) > 0 ? outputBuffer : [];
    }
}
