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

namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal static class Market
    {
        private static int hilite_item;

        internal static void select_previous_stock()
        {
            if (!elite.docked || hilite_item <= 0)
            {
                return;
            }

            hilite_item--;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet, false), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void select_next_stock()
        {
            if (!elite.docked || hilite_item >= trade.stock_market.Length - 1)
            {
                return;
            }

            hilite_item++;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet, false), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void buy_stock()
        {
            if (!elite.docked)
            {
                return;
            }

            if (trade.stock_market[hilite_item].current_quantity == 0 || elite.cmdr.credits < trade.stock_market[hilite_item].current_price)
            {
                return;
            }

            int cargo_held = trade.total_cargo();

            if (trade.stock_market[hilite_item].units == trade.TONNES && cargo_held == elite.cmdr.cargo_capacity)
            {
                return;
            }

            elite.cmdr.current_cargo[hilite_item]++;
            trade.stock_market[hilite_item].current_quantity--;
            elite.cmdr.credits -= trade.stock_market[hilite_item].current_price;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet, false), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void sell_stock()
        {
            if (!elite.docked || elite.cmdr.current_cargo[hilite_item] == 0)
            {
                return;
            }

            elite.cmdr.current_cargo[hilite_item]--;
            trade.stock_market[hilite_item].current_quantity++;
            elite.cmdr.credits += trade.stock_market[hilite_item].current_price;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet, false), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void display_market_prices()
        {
            elite.SetView(SCR.SCR_MARKET_PRICES);

            if (!elite.docked)
            {
                hilite_item = -1;
            }

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet, false), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }
    }
}