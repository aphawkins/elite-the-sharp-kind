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

/*
 * random.c
 */


//# include <stdlib.h>
//# include "allegro.h"

//# include "random.h"

namespace Elite
{
	internal static class random
	{
		/*
		 * Portable random number generator implementing the recursion:
		 *     IX = 16807 * IX MOD (2**(31) - 1)
		 * Using only 32 bits, including sign.
		 *
		 * Taken from "A Guide to Simulation" by Bratley, Fox and Schrage.
		 */
		internal static int randint()
		{
			int k1;
			int ix = rand_seed;

			k1 = ix / 127773;
			ix = (16807 * (ix - (k1 * 127773))) - (k1 * 2836);
			if (ix < 0)
			{
				ix += 2147483647;
			}

			rand_seed = ix;

			return ix;
		}

		internal static int rand_seed { get; set; }

		internal static int rand255()
		{
			return randint() & 255;
		}

		internal static int rand()
		{
			return new Random().Next();
		}
	}
}