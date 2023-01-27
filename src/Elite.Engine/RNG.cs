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
		internal static int Random(int maxValue)
		{
			return Random(0, maxValue);
        }

        internal static int Random(int minValue, int maxValue)
        {
            return new Random().Next(minValue, maxValue);
        }

        internal static bool TrueOrFalse()
        {
            return Random(0, 1) == 1;
        }
    }
}