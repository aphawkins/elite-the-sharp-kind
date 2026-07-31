// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// Breaks text into rows of at most a given number of characters. Splitting
/// only, no drawing: the tiers' <see cref="IBaseView.DrawTextPretty"/> draw
/// the rows left-aligned, and the 8-bit options screen centres the same rows
/// and stacks them upwards from the bottom of the viewport, so a helper that
/// drew as it wrapped could serve neither of them.
/// </summary>
internal static class TextWrap
{
    /// <summary>
    /// Splits <paramref name="text"/> into rows of at most
    /// <paramref name="maxChars"/> characters, breaking on a space, comma or
    /// period where there is one and mid-word where there is not.
    /// </summary>
    internal static List<string> Split(string text, int maxChars)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxChars, 1);

        List<string> lines = [];
        int start = 0;

        while (start < text.Length)
        {
            // What is left already fits, so there is nothing to break.
            if (text.Length - start <= maxChars)
            {
                lines.Add(text[start..]);
                break;
            }

            // The last character that fits on the row, scanned backwards for
            // somewhere to break. Starting a character further on - which is
            // what this used to do - lets a row ending in a comma or period
            // run one character past its width.
            int i = start + maxChars - 1;

            while (i > start && text[i] is not ' ' and not ',' and not '.')
            {
                i--;
            }

            if (i == start)
            {
                // Nowhere to break in the whole row: the word is longer than
                // the row is wide, so break it mid-word at the row width.
                lines.Add(text.Substring(start, maxChars));
                start += maxChars;
                continue;
            }

            // A comma or period stays on the row it ends; a space is dropped,
            // since it draws nothing left-aligned and would push a centred row
            // half a character off centre.
            lines.Add(text[start..(text[i] == ' ' ? i : i + 1)]);
            start = i + 1;
        }

        return lines;
    }
}
