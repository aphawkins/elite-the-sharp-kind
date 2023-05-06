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

            return Seed.C / 0x34;
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

            a /= 256;    /* a = any carry left from above */
            x = Seed.B;
            a = (a + x + Seed.D) & 0xFF;
            Seed.B = a;
            Seed.D = x;
            return a;
        }
    }
}
