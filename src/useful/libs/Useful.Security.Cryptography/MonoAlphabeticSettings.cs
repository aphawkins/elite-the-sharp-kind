// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The monoalphabetic algorithm settings.
/// </summary>
public sealed record MonoAlphabeticSettings : IMonoAlphabeticSettings
{
    private string _substitutions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <inheritdoc />
    public string CharacterSet
    {
        get => field ??= "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        init
        {
            field = ParseCharacterSet(value);
            _substitutions = field;
        }
    }

    /// <inheritdoc />
    public string Substitutions
    {
        get => _substitutions;
        init => _substitutions = ParseSubstitutions(CharacterSet, value);
    }

    /// <inheritdoc />
    public int SubstitutionCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < CharacterSet.Length; i++)
            {
                if (CharacterSet[i] != Substitutions[i])
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <inheritdoc />
    public char GetSubstitution(char substitution)
    {
        int subsIndex = CharacterSet.IndexOf(substitution, StringComparison.InvariantCulture);
        return subsIndex < 0 ? substitution : Substitutions[subsIndex];
    }

    /// <inheritdoc />
    public void SetSubstitution(char substitution, char newSubstitution)
    {
        char from = substitution;
        int fromIndex = CharacterSet.IndexOf(from, StringComparison.InvariantCulture);

        if (fromIndex < 0)
        {
            throw new ArgumentException("Substitution must be an valid character.", nameof(newSubstitution));
        }

        char to = newSubstitution;
        int toIndex = CharacterSet.IndexOf(to, StringComparison.InvariantCulture);

        if (toIndex < 0)
        {
            throw new ArgumentException("Substitution must be an valid character.", nameof(newSubstitution));
        }

        if (Substitutions[fromIndex] == to)
        {
            // Trying to set the same as already set
            return;
        }

        char fromSubs = Substitutions[fromIndex];
        int toInvIndex = Substitutions.IndexOf(to, StringComparison.InvariantCulture);

        if (Substitutions[fromIndex] == to)
        {
            return;
        }

        char[] temp = [.. Substitutions];
        temp[fromIndex] = to;
        temp[toInvIndex] = fromSubs;
        _substitutions = new string(temp);
    }

    /// <inheritdoc />
    public char Reverse(char letter)
        => CharacterSet.IndexOf(letter, StringComparison.InvariantCulture) < 0
            ? letter
            : Substitutions.First(x => GetSubstitution(x) == letter);

    private static string ParseCharacterSet(string characterSet)
    {
        if (string.IsNullOrWhiteSpace(characterSet))
        {
            throw new ArgumentException("Invalid number of characters.", nameof(characterSet));
        }

        foreach (char character in characterSet)
        {
            if (!char.IsLetter(character))
            {
                throw new ArgumentException("All characters must be letters.", nameof(characterSet));
            }
        }

        return characterSet.Length != characterSet.Distinct().Count()
            ? throw new ArgumentException("Characters must not be duplicated.", nameof(characterSet))
            : characterSet;
    }

    private static string ParseSubstitutions(string characterSet, string substitutions)
    {
        if (string.IsNullOrWhiteSpace(substitutions)
            || substitutions.Length != characterSet.Length)
        {
            throw new ArgumentException("Incorrect number of substitutions.", nameof(substitutions));
        }

        foreach (char character in substitutions)
        {
            if (!char.IsLetter(character))
            {
                throw new ArgumentException("All substitutions must be letters.", nameof(substitutions));
            }
        }

        return substitutions.Length != substitutions.Distinct().Count()
            ? throw new ArgumentException("Substitutions must not be duplicated.", nameof(substitutions))
            : !substitutions.All(characterSet.Contains)
            ? throw new ArgumentException("Substitutions must be in the character set.", nameof(substitutions))
            : substitutions;
    }
}
