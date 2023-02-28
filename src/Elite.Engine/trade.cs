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
	using Elite.Engine.Enums;
	using Elite.Engine.Types;

	internal class trade
	{
        private readonly GameState _gameState;

        internal const int ALIEN_ITEMS_IDX = 16;
        private const int SLAVES = 3;
        private const int NARCOTICS = 6;
        private const int FIREARMS = 10;

        private readonly swat _swat;

        internal static string TONNES = "t";
		internal static string KILOGRAMS = "Kg";
		internal static string GRAMS = "g";

		/*
		* The following holds the Elite Planet Stock Market.
		*/
		internal static stock_item[] stock_market = new stock_item[]
		{
			new("Food",         0, 0,  1.9f, -2,   6, 0x01, TONNES),
			new("Textiles",     0, 0,  2.0f, -1,  10, 0x03, TONNES),
			new("Radioactives", 0, 0,  6.5f, -3,   2, 0x07, TONNES),
			new("Slaves",       0, 0,  4.0f, -5, 226, 0x1F, TONNES),
			new("Liquor/Wines", 0, 0,  8.3f, -5, 251, 0x0F, TONNES),
			new("Luxuries",     0, 0, 19.6f,  8,  54, 0x03, TONNES),
			new("Narcotics",    0, 0, 23.5f, 29,   8, 0x78, TONNES),
			new("Computers",    0, 0, 15.4f, 14,  56, 0x03, TONNES),
			new("Machinery",    0, 0, 11.7f,  6,  40, 0x07, TONNES),
			new("Alloys",       0, 0,  7.8f,  1,  17, 0x1F, TONNES),
			new("Firearms",     0, 0, 12.4f, 13,  29, 0x07, TONNES),
			new("Furs",         0, 0, 17.6f, -9, 220, 0x3F, TONNES),
			new("Minerals",     0, 0,  3.2f, -1,  53, 0x03, TONNES),
			new("Gold",         0, 0,  9.7f, -1,  66, 0x07, KILOGRAMS),
			new("Platinum",     0, 0, 17.1f, -2,  55, 0x1F, KILOGRAMS),
			new("Gem-Stones",   0, 0,  4.5f, -1, 250, 0x0F, GRAMS),
			new("Alien Items",  0, 0,  5.3f, 15, 192, 0x07, TONNES),
		};

		internal trade(GameState gameState, swat swat)
		{
            _gameState = gameState;
            _swat = swat;
        }

		/*
		 * Generate the Elite stock market.
		 * The prices and quantities are affected by the planet's economy.
		 * There is also a slight amount of randomness added in.
		 * The random value is changed each time we hyperspace.
		 */
		internal static void generate_stock_market()
		{
			int quant;
			float price;
			int i;

			for (i = 0; i < stock_market.Length; i++)
			{
				price = stock_market[i].base_price;                             /* Start with the base price	*/
				price += (elite.cmdr.market_rnd & stock_market[i].mask) / 10;                  /* Add in a random amount		*/
				price += elite.current_planet_data.economy * stock_market[i].eco_adjust / 10;    /* Adjust for planet economy	*/

				quant = stock_market[i].base_quantity;                              /* Start with the base quantity */
				quant += elite.cmdr.market_rnd & stock_market[i].mask;                  /* Add in a random amount		*/
				quant -= elite.current_planet_data.economy * stock_market[i].eco_adjust;    /* Adjust for planet economy	*/
				quant = Math.Clamp(quant, 0, 63); // Quantities range from 0..63

				stock_market[i].current_price = price * 4;
				stock_market[i].current_quantity = quant;
			}

			/* Alien Items are never available for purchase... */
			stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
		}

		internal static void set_stock_quantities(int[] quant)
		{
			int i;

			for (i = 0; i < stock_market.Length; i++)
            {
                stock_market[i].current_quantity = quant[i];
            }

            stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
		}

		internal static int carrying_contraband()
		{
			return ((elite.cmdr.current_cargo[SLAVES] + elite.cmdr.current_cargo[NARCOTICS]) * 2) + elite.cmdr.current_cargo[FIREARMS];
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

		internal void scoop_item(int un)
		{
			SHIP type;
			int trade;

			if (space.universe[un].flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			type = space.universe[un].type;

			if (type == SHIP.SHIP_MISSILE)
            {
                return;
            }

            if ((!elite.cmdr.fuel_scoop) || (space.universe[un].location.Y >= 0) ||
				(total_cargo() == elite.cmdr.cargo_capacity))
			{
				_swat.explode_object(un);
                _gameState.damage_ship(128 + (space.universe[un].energy / 2), space.universe[un].location.Z > 0);
				return;
			}

			if (type == SHIP.SHIP_CARGO)
			{
				trade = RNG.Random(7);
				elite.cmdr.current_cargo[trade]++;
                elite.info_message(stock_market[trade].name);
				swat.remove_ship(un);
				return;
			}

			if (elite.ship_list[(int)type].scoop_type != 0)
			{
				trade = elite.ship_list[(int)type].scoop_type + 1;
				elite.cmdr.current_cargo[trade]++;
                elite.info_message(stock_market[trade].name);
				swat.remove_ship(un);
				return;
			}

			_swat.explode_object(un);
            _gameState.damage_ship(space.universe[un].energy / 2, space.universe[un].location.Z > 0);
		}
	}
}