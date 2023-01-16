namespace Elite
{
    using System.Numerics;
    using Elite.Enums;
    using Elite.Structs;

    internal class Draw
    {
        private readonly IGfx _gfx;

        internal Draw(IGfx gfx)
        {
            _gfx = gfx;
        }

        internal void DrawGalacticChart(int galaxyNumber, IList<Vector2> planetPixels, string planetName, int distanceToPlanet)
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
            DrawDistanceToPlanet(planetName, distanceToPlanet);
        }

        internal void DrawShortRangeChart(IList<(Vector2 position, string name)> planetNames, 
            IList<(Vector2 position, int size)> planetSizes,
            string planetName, 
            int lightYears)
        {
            _gfx.ClearDisplay();
            _gfx.DrawTextCentre(20, "SHORT RANGE CHART", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);
            DrawFuelLimitCircle(new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE));
            foreach ((Vector2 position, string name) in planetNames)
            {
                _gfx.DrawTextLeft((int)position.X, (int)position.Y, name, GFX_COL.GFX_COL_WHITE);
            }
            foreach ((Vector2 position, int size) in planetSizes)
            {
                _gfx.DrawCircleFilled(position, size, GFX_COL.GFX_COL_GOLD);
            }
            DrawDistanceToPlanet(planetName, lightYears);
        }

        private void DrawDistanceToPlanet(string planetName, int lightYears)
        {
            _gfx.ClearTextArea();
            _gfx.DrawTextLeft(16, 340, $"{planetName:-18s}", GFX_COL.GFX_COL_WHITE);
            string str = lightYears > 0
                ? $"Distance: {lightYears / 10}.{lightYears % 10} Light Years "
                : "                                                     ";
            _gfx.DrawTextLeft(16, 356, str, GFX_COL.GFX_COL_WHITE);
        }

        internal void DrawDataOnPlanet(string planetName, int lightYears, string economy, 
            string government, int techLevel, int population, string inhabitants,
            int productivity, int radius, string description)
        {
            _gfx.ClearDisplay();
            _gfx.DrawTextCentre(20, $"DATA ON {planetName}", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);
            string str = lightYears > 0
                ? $"Distance: {lightYears / 10}.{lightYears % 10} Light Years "
                : "                                                     ";
            _gfx.DrawTextLeft(16, 42, str, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 74, $"Economy: {economy}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 106, $"Government: {government}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 138, $"Tech Level: {techLevel}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 170, $"Population: {population / 10}.{population % 10} Billion", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 202, inhabitants, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 234, $"Gross Productivity: {productivity} M CR", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 266, $"Average Radius: {radius} km", GFX_COL.GFX_COL_WHITE);
            DrawTextPretty(16, 298, 400, 384, description);
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

        // TOOD: This should be private
        internal void DrawTextPretty(int x, int y, int width, int height, string text)
        {
            int i = 0;
            int maxlen = (width - x) / 8;
            int previous = i;

            while (i < text.Length)
            {
                i += maxlen;
                i = Math.Clamp(i, 0, text.Length - 1);

                while (text[i] is not ' ' and not ',' and not '.')
                {
                    i--;
                }

                i++;
                _gfx.DrawTextLeft(x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, text[previous..i], GFX_COL.GFX_COL_WHITE);
                previous = i;
                y += 8 * gfx.GFX_SCALE;
            }
        }

        internal void DrawLoadCommander(bool isError, string name)
        {
            _gfx.ClearDisplay();
            _gfx.DrawTextCentre(20, "LOAD COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, name, 140, GFX_COL.GFX_COL_WHITE);

            if (isError)
            {
                _gfx.DrawTextCentre(175, "Error Loading Commander!", 140, GFX_COL.GFX_COL_GOLD);
                _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
            }

            _gfx.ScreenUpdate();
        }

        internal void DrawMarketPrices(string planetName, stock_item[] stocks, int highlightedStock, int[] currentCargo, int credits)
        {
            _gfx.ClearDisplay();

            _gfx.DrawTextCentre(20, $"{planetName} MARKET PRICES", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(0, 36, 511, 36);
            _gfx.DrawTextLeft(16, 40, "PRODUCT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(166, 40, "UNIT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(246, 40, "PRICE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(314, 40, "FOR SALE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(420, 40, "IN HOLD", GFX_COL.GFX_COL_GREEN_1);

            for (int i = 0; i < stocks.Length; i++)
            {
                int y = (i * 15) + 55;
                display_stock_price(stocks[i], i == highlightedStock, currentCargo[i], y);
            }

            elite.alg_gfx.ClearTextArea();
            elite.alg_gfx.DrawTextLeft(16, 340, $"Cash: {credits / 10, 10:R}.{credits % 10} credits", GFX_COL.GFX_COL_WHITE);
        }

        private void display_stock_price(stock_item stockPrice, bool isHighlighted, int currentCargo, int y)
        {
            if (isHighlighted)
            {
                elite.alg_gfx.DrawRectangleFilled(2, y, 508, 15, GFX_COL.GFX_COL_DARK_RED);
            }

            _gfx.DrawTextLeft(16, y, stockPrice.name, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(180, y, stockPrice.units, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextRight(285, y, $"{stockPrice.current_price / 10}.{stockPrice.current_price % 10}", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextRight(365, y, stockPrice.current_quantity > 0 ? $"{stockPrice.current_quantity}" : "-", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(365, y, stockPrice.current_quantity > 0 ? stockPrice.units : "", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextRight(455, y, currentCargo > 0 ? $"{currentCargo, 2}" : "-", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(455, y, currentCargo > 0 ? stockPrice.units : "", GFX_COL.GFX_COL_WHITE);
        }
    }
}