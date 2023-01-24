namespace Elite.Views
{
    using System.Numerics;
    using Elite.Assets;
    using Elite.Common.Enums;
    using Elite.Enums;
    using Elite.Structs;

    internal class Draw
    {
        private readonly IGfx _gfx;

        internal Draw(IGfx gfx)
        {
            _gfx = gfx;
        }

        internal void DrawGalacticChart(int galaxyNumber, IList<Vector2> planetPixels, string planetName, float distanceToPlanet)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "GALACTIC CHART " + galaxyNumber, 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            _gfx.DrawLine(new(0f, 36f + 258f), new(511f, 36f + 258f));
            DrawFuelLimitCircle(new(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1));
            foreach (Vector2 pixel in planetPixels)
            {
                _gfx.DrawPixel(pixel, GFX_COL.GFX_COL_WHITE);
            }
            DrawDistanceToPlanet(planetName, distanceToPlanet);
        }

        internal void DrawShortRangeChart(IList<(Vector2 position, string name)> planetNames,
            IList<(Vector2 position, int size)> planetSizes,
            string planetName,
            float lightYears)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "SHORT RANGE CHART", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            DrawFuelLimitCircle(new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE));
            foreach ((Vector2 position, string name) in planetNames)
            {
                _gfx.DrawTextLeft(position.X, position.Y, name, GFX_COL.GFX_COL_WHITE);
            }
            foreach ((Vector2 position, int size) in planetSizes)
            {
                _gfx.DrawCircleFilled(position, size, GFX_COL.GFX_COL_GOLD);
            }
            DrawDistanceToPlanet(planetName, lightYears);
        }

        private void DrawDistanceToPlanet(string planetName, float lightYears)
        {
            ClearTextArea();
            _gfx.DrawTextLeft(16, 340, $"{planetName:-18s}", GFX_COL.GFX_COL_WHITE);
            string str = lightYears > 0
                ? $"Distance: {lightYears:N1} Light Years "
                : "                                                     ";
            _gfx.DrawTextLeft(16, 356, str, GFX_COL.GFX_COL_WHITE);
        }

        internal void DrawDataOnPlanet(string planetName, float lightYears, string economy,
            string government, int techLevel, float population, string inhabitants,
            int productivity, int radius, string description)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, $"DATA ON {planetName}", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            string str = lightYears > 0
                ? $"Distance: {lightYears:N1} Light Years "
                : "                                                     ";
            _gfx.DrawTextLeft(16, 42, str, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 74, $"Economy: {economy}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 106, $"Government: {government}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 138, $"Tech Level: {techLevel}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 170, $"Population: {population:N1} Billion", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 202, inhabitants, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 234, $"Gross Productivity: {productivity} Million Credits", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 266, $"Average Radius: {radius} km", GFX_COL.GFX_COL_WHITE);
            DrawTextPretty(16, 298, 400, 384, description);
        }

        private void DrawFuelLimitCircle(Vector2 centre)
        {
            float radius;
            float cross_size;

            if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
            {
                radius = elite.cmdr.fuel * 2.5f * gfx.GFX_SCALE;
                cross_size = 7f * gfx.GFX_SCALE;
            }
            else
            {
                radius = elite.cmdr.fuel * 10 * gfx.GFX_SCALE;
                cross_size = 16f * gfx.GFX_SCALE;
            }

            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size));
            _gfx.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y));
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
            ClearDisplay();
            _gfx.DrawTextCentre(20, "LOAD COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

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

        internal void DrawMarketPrices(string planetName, stock_item[] stocks, int highlightedStock, int[] currentCargo, float credits)
        {
            ClearDisplay();

            _gfx.DrawTextCentre(20, $"{planetName} MARKET PRICES", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            _gfx.DrawTextLeft(16, 40, "PRODUCT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(166, 40, "UNIT", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(246, 40, "PRICE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(314, 40, "FOR SALE", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(420, 40, "IN HOLD", GFX_COL.GFX_COL_GREEN_1);

            for (int i = 0; i < stocks.Length; i++)
            {
                int y = (i * 15) + 55;

                if (i == highlightedStock)
                {
                    _gfx.DrawRectangleFilled(2, y, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                _gfx.DrawTextLeft(16, y, stocks[i].name, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextLeft(180, y, stocks[i].units, GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(285, y, $"{stocks[i].current_price:N1}", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(365, y, stocks[i].current_quantity > 0 ? $"{stocks[i].current_quantity}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(365, y, stocks[i].current_quantity > 0 ? stocks[i].units : "", GFX_COL.GFX_COL_WHITE);

                _gfx.DrawTextRight(455, y, currentCargo[i] > 0 ? $"{currentCargo[i],2}" : "-", GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(455, y, currentCargo[i] > 0 ? stocks[i].units : "", GFX_COL.GFX_COL_WHITE);
            }

            ClearTextArea();
            _gfx.DrawTextLeft(16, 340, $"Cash: {credits,10:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        internal void DrawEquipShip(EquipmentItem[] equip_stock, int highlightedItem, float credits)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "EQUIP SHIP", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            _gfx.ClearArea(2, 55, 508, 325);

            int y = 55;

            for (int i = 0; i < equip_stock.Length; i++)
            {
                if (!equip_stock[i].Show)
                {
                    continue;
                }

                if (i == highlightedItem)
                {
                    _gfx.DrawRectangleFilled(2, y + 1, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                GFX_COL col = equip_stock[i].CanBuy ? GFX_COL.GFX_COL_WHITE : GFX_COL.GFX_COL_GREY_1;
                int x = equip_stock[i].Name[0] == '>' ? 50 : 16;
                _gfx.DrawTextLeft(x, y, equip_stock[i].Name[1..], col);

                if (equip_stock[i].Price != 0)
                {
                    _gfx.DrawTextRight(450, y, $"{equip_stock[i].Price:N1}", col);
                }

                y += 15;
            }

            ClearTextArea();
            _gfx.DrawTextLeft(16, 340, $"Cash: {credits:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        internal void ClearDisplay()
        {
            _gfx.ClearArea(gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 1, 510 + gfx.GFX_X_OFFSET, 383 + gfx.GFX_Y_OFFSET);
        }

        internal void ClearTextArea()
        {
            _gfx.ClearArea(gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 340, 510 + gfx.GFX_X_OFFSET, 43 + gfx.GFX_Y_OFFSET);
        }

        internal void DrawScanner()
        {
            _gfx.DrawImage(IMG.IMG_SCANNER, new(gfx.GFX_X_OFFSET, 385 + gfx.GFX_Y_OFFSET));
        }

        internal void DrawBorder()
        {
            _gfx.DrawLine(new(0f, 0f), new(0f, 384f));
            _gfx.DrawLine(new(0f, 0f), new(511f, 0f));
            _gfx.DrawLine(new(511f, 0f), new(511f, 384f));
        }

        internal void LoadImages()
        {
            AssetLoader loader = new();

            foreach (IMG img in Enum.GetValues<IMG>())
            {
                Stream? stream = loader.Load(img);
                if (stream != null)
                {
                    _gfx.LoadBitmap(img, stream);
                }
            }
        }
    }
}