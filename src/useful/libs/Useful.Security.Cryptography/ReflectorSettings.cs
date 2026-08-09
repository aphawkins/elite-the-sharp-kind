// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The Reflector algorithm settings.
/// </summary>
public sealed record ReflectorSettings : IReflectorSettings
{
    private IList<char> _substitutions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <inheritdoc />
    public IList<char> CharacterSet
    {
        get => field ??= "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        init
        {
            field = ParseCharacterSet(value);
            _substitutions = field;
        }
    }

    /// <inheritdoc />
    public IList<char> Substitutions
    {
        get => _substitutions;

        init
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                _substitutions = ParseSubstitutions(CharacterSet, value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error parsing subsititutions", nameof(Substitutions), ex);
            }

            for (int i = 0; i < CharacterSet.Count; i++)
            {
                SetSubstitution(CharacterSet[i], value[i]);
            }

            if (!value.SequenceEqual(_substitutions))
            {
                throw new ArgumentException("Not valid to substitute these letters.", nameof(Substitutions));
            }
        }
    }

    /// <inheritdoc />
    public int SubstitutionCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < CharacterSet.Count; i++)
            {
                if (CharacterSet[i] != _substitutions[i])
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <inheritdoc />
    public char GetSubstitution(char letter)
    {
        int subsIndex = CharacterSet.IndexOf(letter);
        return subsIndex < 0 ? letter : _substitutions[subsIndex];
    }

    /// <inheritdoc />
    public void SetSubstitution(char substitution, char newSubstitution)
    {
        char from = substitution;
        int fromIndex = CharacterSet.IndexOf(from);

        if (fromIndex < 0)
        {
            throw new ArgumentException("Substitution must be a valid character.", nameof(substitution));
        }

        char to = newSubstitution;
        int toIndex = CharacterSet.IndexOf(to);

        if (toIndex < 0)
        {
            throw new ArgumentException("Substitution must be a valid character.", nameof(newSubstitution));
        }

        if (_substitutions[fromIndex] == to)
        {
            // Trying to set the same as already set
            return;
        }

        char fromSubs = _substitutions[fromIndex];
        int fromSubsIndex = CharacterSet.IndexOf(fromSubs);

        char toSubs = _substitutions[toIndex];
        int toSubsIndex = CharacterSet.IndexOf(toSubs);

        char[] temp = [.. _substitutions];
        temp[fromIndex] = to;
        temp[toIndex] = from;
        _substitutions = temp;

        if (fromSubs != from)
        {
            temp = [.. _substitutions];
            temp[fromSubsIndex] = fromSubs;
            _substitutions = temp;
        }

        if (toSubs != to)
        {
            temp = [.. _substitutions];
            temp[toSubsIndex] = toSubs;
            _substitutions = temp;
        }
    }

    /// <summary>
    /// Gets the reverse substitution for a letter.
    /// </summary>
    /// <param name="letter">The letter to match.</param>
    /// <returns>The letter that substiutes to this letter.</returns>
    public char Reflect(char letter) => GetSubstitution(letter);

    private static IList<char> ParseCharacterSet(IList<char> characterSet)
    {
        if (characterSet == null || characterSet.Count == 0)
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

        return characterSet.Count != characterSet.Distinct().Count()
            ? throw new ArgumentException("Characters must not be duplicated.", nameof(characterSet))
            : characterSet;
    }

    private static IList<char> ParseSubstitutions(IList<char> characterSet, IList<char> substitutions)
        => substitutions.Count > characterSet.Count
            ? throw new ArgumentException("Too many substitutions.", nameof(substitutions))
            : !substitutions.All(characterSet.Contains)
            ? throw new ArgumentException("Substitutions must be in the character set.", nameof(substitutions))
            : characterSet;
}
