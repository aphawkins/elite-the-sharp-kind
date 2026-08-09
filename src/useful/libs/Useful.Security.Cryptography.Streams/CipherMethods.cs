// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;
using System.Text;

namespace Useful.Security.Cryptography;

/// <summary>
/// Methods to aid cryptography.
/// </summary>
public static class CipherMethods
{
    /// <summary>
    /// Encrypts or decrypts a stream.
    /// </summary>
    /// <param name="cipher">The cipher to use.</param>
    /// <param name="transformMode">Defines the way in which the cipher will work.</param>
    /// <param name="input">The input stream.</param>
    /// <param name="output">The output stream.</param>
    public static void SymmetricTransform(SymmetricAlgorithm cipher, CipherTransformMode transformMode, Stream input, Stream output)
    {
        ArgumentNullException.ThrowIfNull(cipher);

        using ICryptoTransform transformer = GetTransformer(cipher, transformMode);
        using StreamReader reader = new(input, new UnicodeEncoding());
        reader.Peek();

        // Create a CryptoStream using the FileStream and the passed key and initialization vector (IV).
        using CryptoStream crypto = new(output, transformer, CryptoStreamMode.Write);
        byte[] bytes;

        // The cipher is expecting Unicode
        while (!reader.EndOfStream)
        {
            bytes = reader.CurrentEncoding.GetBytes([(char)reader.Read()]);
            crypto.Write(bytes, 0, bytes.Length);
        }
    }

    /// <summary>
    /// Enciphers or deciphers a string.
    /// </summary>
    /// <param name="cipher">The cipher to use.</param>
    /// <param name="transformMode">Defines the way in which the cipher will work.</param>
    /// <param name="input">The input text.</param>
    /// <returns>The output text.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cipher" /> is null.</exception>
    /// <exception cref="NotImplementedException"> Thrown sometimes.</exception>
    public static string SymmetricTransform(SymmetricAlgorithm cipher, CipherTransformMode transformMode, string input)
    {
        ArgumentNullException.ThrowIfNull(cipher);

        Encoding encoding = new UnicodeEncoding();

        using ICryptoTransform cryptoTransform = GetTransformer(cipher, transformMode);

        // Encrypt the data.
        using MemoryStream inputStream = new(encoding.GetBytes(input));
        using MemoryStream outputStream = new();
        SymmetricTransform(cipher, transformMode, inputStream, outputStream);

        // Remove padding on odd length bytes
        byte[] encrypted = cryptoTransform.OutputBlockSize % 2 != 0 ? TrimArray(outputStream.ToArray()) : outputStream.ToArray();
        char[] encryptedChars = encoding.GetChars(encrypted);

        return new string(encryptedChars);
    }

    /// <summary>
    /// Gets a SymmetricAlgorithm's transformer.
    /// </summary>
    /// <param name="cipher">The SymmetricAlgorithm.</param>
    /// <param name="transformMode">The get the encrypt/decrypt transformer.</param>
    /// <returns>The SymmetricAlgorithm's transformer.</returns>
    private static ICryptoTransform GetTransformer(SymmetricAlgorithm cipher, CipherTransformMode transformMode)
    {
        if (transformMode == CipherTransformMode.Encrypt)
        {
            // Get an encryptor.
            return cipher.CreateEncryptor();
        }

        // Get an decryptor.
        return cipher.CreateDecryptor();
    }

    // Resize the dimensions of the array to a size that contains only valid bytes.
    private static byte[] TrimArray(byte[] targetArray)
    {
        int i = 0;

        foreach (byte b in targetArray)
        {
            if ($"{b}".Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            i++;
        }

        // Create a new array with the number of valid bytes.
        byte[] returnedArray = new byte[i];
        Array.Copy(targetArray, returnedArray, i);
        return returnedArray;
    }
}
