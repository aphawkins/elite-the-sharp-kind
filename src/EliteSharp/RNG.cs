// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Security.Cryptography;

namespace EliteSharp;

internal static class RNG
{
    internal static RandomSeed Seed { get; set; } = new();

    /// <summary>
    /// Guassian random number generator.
    /// </summary>
    /// <param name="min">The lower bound of the distribution (inclusive).</param>
    /// <param name="max">The upper bound of the distribution (exclusive).</param>
    /// <returns>A number between min and max with Gaussian distribution.</returns>
    internal static int GaussianRandom(int min, int max)
    {
        const int iterations = 12;
        int r = 0;
        for (int i = 0; i < iterations; i++)
        {
            r += Random(min, max);
        }

        r /= iterations;

        return r;
    }

    /// <summary>
    /// Generate a random number between 0 and 255.
    /// This is the version used in the 6502 Elites.
    /// </summary>
    /// <returns>A random number between 0 and 255.</returns>
    internal static int GenerateRandomNumber()
    {
        int x = (Seed.A * 2) & 0xFF;
        int a = x + Seed.C;
        if (Seed.A > 127)
        {
            a++;
        }

        Seed.A = a & 0xFF;
        Seed.C = x;

        a /= 256;    // a = any carry left from above
        x = Seed.B;
        a = (a + x + Seed.D) & 0xFF;
        Seed.B = a;
        Seed.D = x;
        return a;
    }

    /// <summary>
    /// Generate a random number between 0 and 255.
    /// This is the version used in the MSX and 16bit Elites.
    /// </summary>
    /// <returns>A random number between 0 and 255.</returns>
    internal static int GenMSXRandomNumber()
    {
        int a = Seed.A;
        int b = Seed.B;

        Seed.A = Seed.C;
        Seed.B = Seed.D;

        a += Seed.C;
        b = (b + Seed.D) & 255;
        if (a > 255)
        {
            a &= 255;
            b++;
        }

        Seed.C = a;
        Seed.D = b;

        return Seed.C / 52;
    }

    /// <summary>
    /// Generates a random number from zero to the exclusive upper bound.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random number.</returns>
    internal static int Random(int toExclusive) => Random(0, toExclusive);

    /// <summary>
    /// Generates a random number.
    /// </summary>
    /// <param name="fromInclusive">The exclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random number.</returns>
    internal static int Random(int fromInclusive, int toExclusive) => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);

    internal static bool TrueOrFalse() => Random(0, 2) != 0;
}
