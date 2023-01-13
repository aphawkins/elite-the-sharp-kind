namespace Elite
{
    using System.Numerics;
    using Elite;
    using Elite.Enums;

    internal class Draw
    {
        private readonly IGfx _gfx;

        internal Draw(IGfx gfx)
        {
            _gfx = gfx;
        }

        internal void DrawGalacticChart(int galaxyNumber)
        {
            _gfx.ClearDisplay();
            _gfx.DrawTextCentre(20, "GALACTIC CHART " + galaxyNumber, 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);
            _gfx.DrawLine(0, 36 + 258, 511, 36 + 258);
            DrawFuelLimitCircle(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1);
            foreach (Vector2 pixel in Docked.planetPixels)
            {
                _gfx.PlotPixel(pixel, GFX_COL.GFX_COL_WHITE);
            }
        }

        internal void DrawFuelLimitCircle(int cx, int cy)
        {
            int radius;
            int cross_size;

            if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
            {
                radius = elite.cmdr.fuel / 4 * gfx.GFX_SCALE;
                cross_size = 7 * gfx.GFX_SCALE;
            }
            else
            {
                radius = elite.cmdr.fuel * gfx.GFX_SCALE;
                cross_size = 16 * gfx.GFX_SCALE;
            }

            _gfx.DrawCircle(cx, cy, radius, GFX_COL.GFX_COL_GREEN_1);

            _gfx.DrawLine(cx, cy - cross_size, cx, cy + cross_size);
            _gfx.DrawLine(cx - cross_size, cy, cx + cross_size, cy);
        }
    }
}
