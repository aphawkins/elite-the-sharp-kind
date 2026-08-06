// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// The Caesar cipher cryptanalysis.
/// </summary>
public static class CaesarCryptanalysis
{
    private static readonly double[] s_letterFrequencies
        = [8.2,
            1.5,
            2.8,
            4.3,
            13.0,
            2.2,
            2.0,
            6.1,
            7.0,
            0.15,
            0.77,
            4.0,
            2.4,
            6.7,
            7.5,
            1.9,
            0.095,
            6.0,
            9.3,
            9.1,
            2.8,
            0.98,
            2.4,
            0.15,
            2.0,
            0.074];

    /// <summary>
    /// Calculates the optimal settings.
    /// </summary>
    /// <param name="ciphertext">The text to crack.</param>
    /// <returns>The best guess crack.</returns>
    public static (int BestShift, IDictionary<int, string> AllDecryptions) Crack(string ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);

        Dictionary<int, string> shifts = [];
        CaesarSettings settings = new();
        Caesar cipher = new(settings);
        for (int i = 0; i < 26; i++)
        {
            cipher.Settings.RightShift = i;
            shifts.Add(i, cipher.Decrypt(ciphertext));
        }

        return (BestShift(ciphertext.ToUpperInvariant()), shifts);
    }

    private static int BestShift(string ciphertext)
    {
        double[] cipherFrequencies = new double[26];

        // Totals for each letter
        foreach (char c in ciphertext)
        {
            if (c is >= 'A' and <= 'Z')
            {
                cipherFrequencies[c % 'A']++;
            }
        }

        // Frequencies for each letter
        for (int i = 0; i < 26; i++)
        {
            cipherFrequencies[i] = 100.0 * cipherFrequencies[i] / ciphertext.Length;
        }

        double bestDifference = double.MaxValue;
        int bestShift = 0;

        // Test all the shifts to find the best difference
        for (int shift = 0; shift < 26; shift++)
        {
            double difference = 0.0;
            for (int i = 0; i < 26; i++)
            {
                difference += Math.Abs(s_letterFrequencies[i] - cipherFrequencies[(i + shift) % 26]);
            }

            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestShift = shift;
            }
        }

        return bestShift;
    }
}
