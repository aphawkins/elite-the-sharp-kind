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

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;

	internal static class trade
	{
		internal const int NO_OF_STOCK_ITEMS = 17;
		internal const int ALIEN_ITEMS_IDX = 16;

		const int SLAVES = 3;
		const int NARCOTICS = 6;
		const int FIREARMS = 10;

		internal static int TONNES = 0;
		internal static int KILOGRAMS = 1;
		internal static int GRAMS = 2;

		/*
		* The following holds the Elite Planet Stock Market.
		*/
		internal static stock_item[] stock_market = new stock_item[NO_OF_STOCK_ITEMS]
		{
			new("Food",         0, 0,  19, -2,   6, 0x01, TONNES),
			new("Textiles",     0, 0,  20, -1,  10, 0x03, TONNES),
			new("Radioactives", 0, 0,  65, -3,   2, 0x07, TONNES),
			new("Slaves",       0, 0,  40, -5, 226, 0x1F, TONNES),
			new("Liquor/Wines", 0, 0,  83, -5, 251, 0x0F, TONNES),
			new("Luxuries",     0, 0, 196,  8,  54, 0x03, TONNES),
			new("Narcotics",    0, 0, 235, 29,   8, 0x78, TONNES),
			new("Computers",    0, 0, 154, 14,  56, 0x03, TONNES),
			new("Machinery",    0, 0, 117,  6,  40, 0x07, TONNES),
			new("Alloys",       0, 0,  78,  1,  17, 0x1F, TONNES),
			new("Firearms",     0, 0, 124, 13,  29, 0x07, TONNES),
			new("Furs",         0, 0, 176, -9, 220, 0x3F, TONNES),
			new("Minerals",     0, 0,  32, -1,  53, 0x03, TONNES),
			new("Gold",         0, 0,  97, -1,  66, 0x07, KILOGRAMS),
			new("Platinum",     0, 0, 171, -2,  55, 0x1F, KILOGRAMS),
			new("Gem-Stones",   0, 0,  45, -1, 250, 0x0F, GRAMS),
			new("Alien Items",  0, 0,  53, 15, 192, 0x07, TONNES),
		};

		/*
		 * Generate the Elite stock market.
		 * The prices and quantities are affected by the planet's economy.
		 * There is also a slight amount of randomness added in.
		 * The random value is changed each time we hyperspace.
		 */
		static void generate_stock_market()
		{
			int quant;
			int price;
			int i;

			for (i = 0; i < NO_OF_STOCK_ITEMS; i++)
			{
				price = stock_market[i].base_price;                             /* Start with the base price	*/
				price += elite.cmdr.market_rnd & stock_market[i].mask;                  /* Add in a random amount		*/
				price += elite.current_planet_data.economy * stock_market[i].eco_adjust;    /* Adjust for planet economy	*/
				price &= 255;                                                       /* Only need bottom 8 bits		*/

				quant = stock_market[i].base_quantity;                              /* Start with the base quantity */
				quant += elite.cmdr.market_rnd & stock_market[i].mask;                  /* Add in a random amount		*/
				quant -= elite.current_planet_data.economy * stock_market[i].eco_adjust;    /* Adjust for planet economy	*/
				quant &= 255;                                                       /* Only need bottom 8 bits		*/

				if (quant > 127)    /* In an 8-bit environment '>127' would be negative */
					quant = 0;      /* So we set it to a minimum of zero. */

				quant &= 63;        /* Quantities range from 0..63 */

				stock_market[i].current_price = price * 4;
				stock_market[i].current_quantity = quant;
			}


			/* Alien Items are never available for purchase... */

			stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
		}

		internal static void set_stock_quantities(int[] quant)
		{
			int i;

			for (i = 0; i < NO_OF_STOCK_ITEMS; i++)
				stock_market[i].current_quantity = quant[i];

			stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
		}

		internal static int carrying_contraband()
		{
			return (elite.cmdr.current_cargo[SLAVES] + elite.cmdr.current_cargo[NARCOTICS]) * 2 + elite.cmdr.current_cargo[FIREARMS];
		}

		internal static int total_cargo()
		{
			int i;
			int cargo_held;

			cargo_held = 0;
			for (i = 0; i < 17; i++)
			{
				if ((elite.cmdr.current_cargo[i] > 0) &&
					(stock_market[i].units == TONNES))
				{
					cargo_held += elite.cmdr.current_cargo[i];
				}
			}

			return cargo_held;
		}

		internal static void scoop_item(int un)
		{
			int type;
			int trade;

			if (universe[un].flags & FLG.FLG_DEAD)
				return;

			type = universe[un].type;

			if (type == shipdata.SHIP_MISSILE)
				return;

			if ((elite.cmdr.fuel_scoop == 0) || (universe[un].location.y >= 0) ||
				(total_cargo() == elite.cmdr.cargo_capacity))
			{
				explode_object(un);
				damage_ship(128 + (universe[un].energy / 2), universe[un].location.z > 0);
				return;
			}

			if (type == shipdata.SHIP_CARGO)
			{
				trade = rand255() & 7;
				elite.cmdr.current_cargo[trade]++;
				info_message(stock_market[trade].name);
				remove_ship(un);
				return;
			}

			if (ship_list[type].scoop_type != 0)
			{
				trade = ship_list[type].scoop_type + 1;
				elite.cmdr.current_cargo[trade]++;
				info_message(stock_market[trade].name);
				remove_ship(un);
				return;
			}

			explode_object(un);
			damage_ship(universe[un].energy / 2, universe[un].location.z > 0);
		}
	}
}