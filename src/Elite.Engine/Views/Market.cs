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
            elite.current_screen = SCR.SCR_INVENTORY;

            elite.draw.ClearDisplay();
            elite.alg_gfx.DrawTextCentre(20, "INVENTORY", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            elite.alg_gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 50, $"{elite.cmdr.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            elite.alg_gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 66, $"{elite.cmdr.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            int y = 98;
            for (int i = 0; i < 17; i++)
            {
                if (elite.cmdr.current_cargo[i] > 0)
                {
                    elite.alg_gfx.DrawTextLeft(16, y, trade.stock_market[i].name, GFX_COL.GFX_COL_WHITE);
                    elite.alg_gfx.DrawTextLeft(180, y, $"{elite.cmdr.current_cargo[i]}{trade.stock_market[i].units}", GFX_COL.GFX_COL_WHITE);
                    y += 16;
                }
            }
        }
    }
}