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

    internal class Inventory : IView
    {
        private readonly IGfx _gfx;

        internal Inventory(IGfx gfx)
        {
            _gfx = gfx;
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader("INVENTORY");

            _gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 50, $"{elite.cmdr.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 66, $"{elite.cmdr.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            int y = 98;
            for (int i = 0; i < elite.cmdr.current_cargo.Length; i++)
            {
                if (elite.cmdr.current_cargo[i] > 0)
                {
                    _gfx.DrawTextLeft(16, y, trade.stock_market[i].name, GFX_COL.GFX_COL_WHITE);
                    _gfx.DrawTextLeft(180, y, $"{elite.cmdr.current_cargo[i]}{trade.stock_market[i].units}", GFX_COL.GFX_COL_WHITE);
                    y += 16;
                }
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
        }
    }
}