// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Enigma Plugboard settings generator.
/// </summary>
internal static class EnigmaPlugboardGenerator
{
    public static IEnigmaPlugboard Generate()
    {
        ReflectorSettings reflector = ReflectorSettingsGenerator.Generate();

        List<EnigmaPlugboardPair> pairs = [];
        List<char> usedLetters = [];

        for (int i = 0; i < reflector.CharacterSet.Count; i++)
        {
            char fromLetter = reflector.CharacterSet[i];
            char toLetter = reflector.Substitutions[i];

            if (!usedLetters.Contains(fromLetter)
                && !usedLetters.Contains(toLetter))
            {
                usedLetters.Add(fromLetter);
                usedLetters.Add(toLetter);
                pairs.Add(new EnigmaPlugboardPair() { From = fromLetter, To = toLetter });
            }
        }

        return new EnigmaPlugboard(pairs);
    }
}
