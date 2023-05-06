/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine
{
    internal static class RNG
    {
        internal static RandomSeed Seed = new();

        internal static int Random(int maxValue) => Random(0, maxValue);

        internal static int Random(int minValue, int maxValue) => new Random().Next(minValue, maxValue + 1);

        internal static bool TrueOrFalse() => Random(0, 1) == 1;

        /// <summary>
        /// Guassian random number generator.
        /// </summary>
        /// <param name="min">The lower bound of the distribution (inclusive).</param>
        /// <param name="max">The upper bound of the distribution (inclusive).</param>
        /// <returns>A number between min and max with Gaussian distribution.</returns>
        internal static int GaussianRandom(int min, int max)
        {
            int iterations = 12;
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
        /// This is the version used in the MSX and 16bit Elites.
        /// </summary>
        /// <returns>A random number between 0 and 255.</returns>
        internal static int GenMSXRandomNumber()
        {
            int a = Seed.a;
            int b = Seed.b;

            Seed.a = Seed.c;
            Seed.b = Seed.d;

            a += Seed.c;
            b = (b + Seed.d) & 255;
            if (a > 255)
            {
                a &= 255;
                b++;
            }

            Seed.c = a;
            Seed.d = b;

            return Seed.c / 0x34;
        }

        /// <summary>
        /// Generate a random number between 0 and 255.
        /// This is the version used in the 6502 Elites.
        /// </summary>
        /// <returns>A random number between 0 and 255.</returns>
        internal static int GenerateRandomNumber()
        {
            int x = (Seed.a * 2) & 0xFF;
            int a = x + Seed.c;
            if (Seed.a > 127)
            {
                a++;
            }

            Seed.a = a & 0xFF;
            Seed.c = x;

            a /= 256;    /* a = any carry left from above */
            x = Seed.b;
            a = (a + x + Seed.d) & 0xFF;
            Seed.b = a;
            Seed.d = x;
            return a;
        }
    }
}