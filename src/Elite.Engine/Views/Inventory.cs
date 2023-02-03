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

    internal static class Inventory
    {
        internal static void display_inventory()
        {
            elite.SetView(SCR.SCR_INVENTORY);
            elite.draw.DrawInventory(elite.cmdr.fuel, elite.cmdr.credits, trade.stock_market, elite.cmdr.current_cargo);
        }
    }
}