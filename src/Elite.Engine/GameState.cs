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

    internal class GameState
    {
        private const int SLAVES = 3;
        private const int NARCOTICS = 6;
        private const int FIREARMS = 10;
        private const int ALIEN_ITEMS_IDX = 16;

        internal const string TONNES = "t";
        internal const string KILOGRAMS = "Kg";
        internal const string GRAMS = "g";

        private readonly IKeyboard _keyboard;
        private readonly Dictionary<SCR, IView> _views;
        internal bool IsGameOver { get; private set; } = false;
        internal bool IsInitialised { get; set; } = false;
        internal SCR currentScreen = SCR.SCR_NONE;
        internal IView currentView;
        internal float energy { get; set; } = 255;
        internal float fore_shield { get; private set; } = 255;
        internal float aft_shield { get; private set; } = 255;
        internal float flight_roll;
        internal float flight_climb;
        internal bool witchspace;
        internal Commander cmdr;
        internal Commander saved_cmdr;
        internal galaxy_seed docked_planet;
        internal string planetName;
        internal galaxy_seed hyperspace_planet;
        internal planet_data current_planet_data = new();

        /// <summary>
        /// The following holds the Elite Planet Stock Market.
        /// </summary>
        internal stock_item[] stock_market = new stock_item[]
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

        internal GameState(IKeyboard keyboard, Dictionary<SCR, IView> views) 
        {
            _views = views;
            _keyboard = keyboard;
        }

        internal void Reset()
        {
            IsInitialised = true;
            IsGameOver = false;

            fore_shield = 255;
            aft_shield = 255;
            energy = 255;
            flight_roll = 0;
            flight_climb = 0;
            witchspace = false;
        }

        internal void SetView(SCR screen)
        {
            //lock (_state)
            //{
                currentScreen = screen;
                currentView = _views[screen];
                _keyboard.ClearKeyPressed();
                currentView.Reset();
            //}
        }

        /// <summary>
        /// Game Over...
        /// </summary>
        internal void GameOver()
        {
            if (!IsGameOver)
            {
                SetView(SCR.SCR_GAME_OVER);
            }

            IsGameOver = true;
        }

        /// <summary>
        /// Deplete the shields.  Drain the energy banks if the shields fail.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="front"></param>
        internal void damage_ship(int damage, bool front)
        {
            if (damage <= 0)    /* sanity check */
            {
                return;
            }

            float shield = front ? fore_shield : aft_shield;

            shield -= damage;
            if (shield < 0)
            {
                decrease_energy(shield);
                shield = 0;
            }

            if (front)
            {
                fore_shield = shield;
            }
            else
            {
                aft_shield = shield;
            }
        }

        internal void decrease_energy(float amount)
        {
            energy += amount;

            if (energy <= 0)
            {
                GameOver();
            }
        }

        internal bool IsEnergyLow()
        {
            return energy < 50;
        }

        /*
         * Regenerate the shields and the energy banks.
         */
        internal void regenerate_shields()
        {
            if (energy > 127)
            {
                if (fore_shield < 255)
                {
                    fore_shield++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }

                if (aft_shield < 255)
                {
                    aft_shield++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }
            }

            energy = Math.Clamp(energy + 1 + (int)cmdr.energy_unit, 0, 255);
        }

        internal void increase_flight_roll()
        {
            flight_roll = Math.Clamp(flight_roll + 1, -elite.myship.max_roll, elite.myship.max_roll);
        }

        internal void decrease_flight_roll()
        {
            flight_roll = Math.Clamp(flight_roll - 1, -elite.myship.max_roll, elite.myship.max_roll);
        }

        internal void increase_flight_climb()
        {
            flight_climb = Math.Clamp(flight_climb + 1, -elite.myship.max_climb, elite.myship.max_climb);
        }

        internal void decrease_flight_climb()
        {
            flight_climb = Math.Clamp(flight_climb - 1, -elite.myship.max_climb, elite.myship.max_climb);
        }

        internal int carrying_contraband()
        {
            return ((cmdr.current_cargo[SLAVES] + cmdr.current_cargo[NARCOTICS]) * 2) + cmdr.current_cargo[FIREARMS];
        }

        /// <summary>
        /// Generate the Elite stock market.
        /// The prices and quantities are affected by the planet's economy.
        /// There is also a slight amount of randomness added in.
        /// The random value is changed each time we hyperspace.
        /// </summary>
        internal void generate_stock_market()
        {
            for (int i = 0; i < stock_market.Length; i++)
            {
                float price = stock_market[i].base_price;                             /* Start with the base price	*/
                price += (cmdr.market_rnd & stock_market[i].mask) / 10;                  /* Add in a random amount		*/
                price += current_planet_data.economy * stock_market[i].eco_adjust / 10;    /* Adjust for planet economy	*/

                int quant = stock_market[i].base_quantity;                              /* Start with the base quantity */
                quant += cmdr.market_rnd & stock_market[i].mask;                  /* Add in a random amount		*/
                quant -= current_planet_data.economy * stock_market[i].eco_adjust;    /* Adjust for planet economy	*/
                quant = Math.Clamp(quant, 0, 63); // Quantities range from 0..63

                stock_market[i].current_price = price * 4;
                stock_market[i].current_quantity = quant;
            }

            /* Alien Items are never available for purchase... */
            stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
        }

        internal void set_stock_quantities(int[] quant)
        {
            for (int i = 0; i < stock_market.Length; i++)
            {
                stock_market[i].current_quantity = quant[i];
            }

            stock_market[ALIEN_ITEMS_IDX].current_quantity = 0;
        }

        internal void restore_saved_commander()
        {
            cmdr = (Commander)saved_cmdr.Clone();
            docked_planet = Planet.find_planet(cmdr.galaxy, new(cmdr.ShipLocationX, cmdr.ShipLocationY));
            planetName = Planet.name_planet(docked_planet, false);
            hyperspace_planet = (galaxy_seed)docked_planet.Clone();
            current_planet_data = Planet.generate_planet_data(docked_planet);
            generate_stock_market();
            set_stock_quantities(cmdr.station_stock);
        }
    }
}