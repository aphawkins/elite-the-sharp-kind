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

namespace Elite.Views
{
    using Elite.Enums;

    internal static class Market
    {
        static int hilite_item;

        internal static void select_previous_stock()
        {
            if (!elite.docked || hilite_item <= 0)
            {
                return;
            }

            hilite_item--;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void select_next_stock()
        {
            if (!elite.docked || hilite_item >= trade.stock_market.Length - 1)
            {
                return;
            }

            hilite_item++;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
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

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
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

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void display_market_prices()
        {
            elite.current_screen = SCR.SCR_MARKET_PRICES;

            if (!elite.docked)
            {
                hilite_item = -1;
            }

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

        internal static void display_inventory()
        {
            int i;
            int y;
            string str;

            elite.current_screen = SCR.SCR_INVENTORY;

            elite.alg_gfx.ClearDisplay();
            elite.alg_gfx.DrawTextCentre(20, "INVENTORY", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.DrawLine(0, 36, 511, 36);

            str = $"{elite.cmdr.fuel / 10:D}.{elite.cmdr.fuel % 10:D} Light Years";
            elite.alg_gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 50, str, GFX_COL.GFX_COL_WHITE);

            str = $"{elite.cmdr.credits / 10:D}.{elite.cmdr.credits % 10:D} Cr";
            elite.alg_gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 66, str, GFX_COL.GFX_COL_WHITE);

            y = 98;
            for (i = 0; i < 17; i++)
            {
                if (elite.cmdr.current_cargo[i] > 0)
                {
                    elite.alg_gfx.DrawTextLeft(16, y, trade.stock_market[i].name, GFX_COL.GFX_COL_WHITE);

                    str = $"{elite.cmdr.current_cargo[i]}{trade.stock_market[i].units}";

                    elite.alg_gfx.DrawTextLeft(180, y, str, GFX_COL.GFX_COL_WHITE);
                    y += 16;
                }
            }
        }
    }
}