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

        internal void DrawGalacticChart(int galaxyNumber, IList<Vector2> planetPixels)
        {
            _gfx.ClearDisplay();
            _gfx.DrawTextCentre(20, "GALACTIC CHART " + galaxyNumber, 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);
            _gfx.DrawLine(0, 36 + 258, 511, 36 + 258);
            DrawFuelLimitCircle(new(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1));
            foreach (Vector2 pixel in planetPixels)
            {
                _gfx.PlotPixel(pixel, GFX_COL.GFX_COL_WHITE);
            }
        }

        internal void DrawShortRangeChart(IList<(Vector2 position, string name)> planetNames, IList<(Vector2 position, int size)> planetSizes)
        {
            elite.alg_gfx.ClearDisplay();
            elite.alg_gfx.DrawTextCentre(20, "SHORT RANGE CHART", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.DrawLine(0, 36, 511, 36);
            elite.draw.DrawFuelLimitCircle(new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE));
            // Docked.show_distance_to_planet();

            foreach (var planetName in planetNames)
            {
                elite.alg_gfx.DrawText((int)planetName.position.X, (int)planetName.position.Y, planetName.name);
            }

            foreach (var planetSize in planetSizes)
            {
                elite.alg_gfx.DrawCircleFilled(planetSize.position, planetSize.size, GFX_COL.GFX_COL_GOLD);
            }
        }

        private void DrawFuelLimitCircle(Vector2 centre)
        {
            float radius;
            float cross_size;

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

            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawLine(centre.X, centre.Y - cross_size, centre.X, centre.Y + cross_size);
            _gfx.DrawLine(centre.X - cross_size, centre.Y, centre.X + cross_size, centre.Y);
        }
    }
}
