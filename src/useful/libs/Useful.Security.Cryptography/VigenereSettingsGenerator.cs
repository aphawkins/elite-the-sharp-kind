// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;

namespace Useful.Security.Cryptography;

/// <summary>
/// Vigenere key generator.
/// </summary>
internal static class VigenereSettingsGenerator
{
    public static VigenereSettings Generate()
    {
        int length = RandomNumberGenerator.GetInt32(1, VigenereSettings.MaxKeywordLength + 1);
        char[] keyword = new char[length];

        for (int i = 0; i < length; i++)
        {
            keyword[i] = (char)('A' + RandomNumberGenerator.GetInt32(0, Alphabet.Length));
        }

        return new VigenereSettings() { Keyword = new string(keyword) };
    }
}
