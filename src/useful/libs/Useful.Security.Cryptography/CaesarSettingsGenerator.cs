// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;

namespace Useful.Security.Cryptography;

/// <summary>
/// Caesar key generator.
/// </summary>
internal static class CaesarSettingsGenerator
{
    public static CaesarSettings Generate()
    {
        byte[] randomNumber = new byte[1];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        int rightShift = randomNumber[0] % 26;
        return new CaesarSettings() { RightShift = rightShift };
    }
}
