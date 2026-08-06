// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Security.Cryptography;

namespace Useful.Security.Cryptography;

/// <summary>
/// MonoAlphabetic settings generator.
/// </summary>
internal static class MonoAlphabeticSettingsGenerator
{
    public static MonoAlphabeticSettings Generate()
    {
        MonoAlphabeticSettings settings = new();
        List<char> allowedLettersCloneFrom = [.. settings.CharacterSet];
        List<char> allowedLettersCloneTo = [.. settings.CharacterSet];

        int indexFrom;
        int indexTo;

        char from;
        char to;

        while (allowedLettersCloneFrom.Count > 0)
        {
            indexFrom = RandomNumberGenerator.GetInt32(0, allowedLettersCloneFrom.Count);
            from = allowedLettersCloneFrom[indexFrom];
            allowedLettersCloneFrom.RemoveAt(indexFrom);

            indexTo = RandomNumberGenerator.GetInt32(0, allowedLettersCloneTo.Count);
            to = allowedLettersCloneTo[indexTo];
            allowedLettersCloneTo.RemoveAt(indexTo);

            settings.SetSubstitution(from, to);
        }

        return settings;
    }
}
