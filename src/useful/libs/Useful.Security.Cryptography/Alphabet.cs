// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The Latin alphabet that the shift ciphers operate over.
/// </summary>
internal static class Alphabet
{
    /// <summary>
    /// The number of letters in the alphabet.
    /// </summary>
    internal const int Length = 26;

    /// <summary>
    /// Gets the position of a letter in the alphabet, ignoring case.
    /// </summary>
    /// <param name="letter">The letter to look up.</param>
    /// <returns>The zero based position, or -1 if <paramref name="letter"/> is not a letter of the alphabet.</returns>
    internal static int IndexOf(char letter)
        => letter switch
        {
            >= 'A' and <= 'Z' => letter - 'A',
            >= 'a' and <= 'z' => letter - 'a',
            _ => -1,
        };

    /// <summary>
    /// Maps every letter of <paramref name="text"/> through <paramref name="map"/>, leaving
    /// everything else untouched. Letters are returned in upper case, as described on
    /// <see cref="ICipher"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to <paramref name="map"/>.</typeparam>
    /// <param name="text">The text to map.</param>
    /// <param name="state">State handed to <paramref name="map"/>, so that it can stay a static lambda.</param>
    /// <param name="map">Maps a zero based alphabet position onto another zero based position.</param>
    /// <returns>The mapped text.</returns>
    internal static string Map<TState>(string text, TState state, Func<TState, int, int> map)
        => string.Create(
            text.Length,
            (Text: text, State: state, Map: map),
            static (chars, args) =>
            {
                for (int i = 0; i < args.Text.Length; i++)
                {
                    char letter = args.Text[i];
                    int index = IndexOf(letter);
                    chars[i] = index < 0 ? letter : (char)('A' + args.Map(args.State, index));
                }
            });
}
