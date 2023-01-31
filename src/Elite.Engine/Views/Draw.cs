namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Assets;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;
    using Elite.Engine.Types;
    using static Elite.Engine.Settings;

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
            DrawTextPretty(16, 298, 400, description);
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
        internal void DrawTextPretty(int x, int y, int width, string text)
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

        internal void DrawSaveCommander(string name, bool? isSuccess = null)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "SAVE COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0, 36), new(511, 36));

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, name, 140, GFX_COL.GFX_COL_WHITE);

            if (isSuccess.HasValue)
            {
                if (isSuccess.Value)
                {
                    _gfx.DrawTextCentre(175, "Commander Saved.", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextCentre(175, "Error Saving Commander!", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
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

        internal void DrawInventory(float fuel, float credits, stock_item[] stocks, int[] cargo)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "INVENTORY", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            _gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 50, $"{fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 66, $"{credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            int y = 98;
            for (int i = 0; i < cargo.Length; i++)
            {
                if (cargo[i] > 0)
                {
                    _gfx.DrawTextLeft(16, y, stocks[i].name, GFX_COL.GFX_COL_WHITE);
                    _gfx.DrawTextLeft(180, y, $"{cargo[i]}{stocks[i].units}", GFX_COL.GFX_COL_WHITE);
                    y += 16;
                }
            }
        }

        internal void DrawCommanderStatus(string name, bool isWitchspace, string dockedPlanetName, string hyperspacePlanetName, int condition, Commander cmdr)
        {
            int EQUIP_START_Y = 202;
            int x = 50;
            int y = EQUIP_START_Y;
            int Y_INC = 16;
            int EQUIP_MAX_Y = 290;
            int EQUIP_WIDTH = 200;
            string[] laser_name = new string[5] { "Pulse", "Beam", "Military", "Mining", "Custom" };
            string laser_type(int strength)
            {
                return strength switch
                {
                    elite.PULSE_LASER => laser_name[0],
                    elite.BEAM_LASER => laser_name[1],
                    elite.MILITARY_LASER => laser_name[2],
                    elite.MINING_LASER => laser_name[3],
                    _ => laser_name[4],
                };
            }

            void IncrementPosition()
            {
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            };

            string[] condition_txt = new string[]
            {
                "Docked",
                "Green",
                "Yellow",
                "Red"
            };

            (int score, string title)[] ratings = new (int score, string title)[]
            {
                new(0x0000, "Harmless"),
                new(0x0008, "Mostly Harmless"),
                new(0x0010, "Poor"),
                new(0x0020, "Average"),
                new(0x0040, "Above Average"),
                new(0x0080, "Competent"),
                new(0x0200, "Dangerous"),
                new(0x0A00, "Deadly"),
                new(0x1900, "- - - E L I T E - - -")
            };

            string rating = string.Empty;
            foreach ((int score, string title) in ratings)
            {
                if (cmdr.score >= score)
                {
                    rating = title;
                }
            }

            elite.draw.ClearDisplay();

            _gfx.DrawTextCentre(20, $"COMMANDER {name}", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));
            _gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

            if (!isWitchspace)
            {
                _gfx.DrawTextLeft(190, 58, dockedPlanetName, GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(190, 74, hyperspacePlanetName, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(190, 90, condition_txt[condition], GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 106, $"{cmdr.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(70, 122, $"{cmdr.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(128, 138, cmdr.legal_status == 0 ? "Clean" : elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(80, 154, rating, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

            if (cmdr.cargo_capacity > 20)
            {
                _gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.escape_pod)
            {
                _gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.fuel_scoop)
            {
                _gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.ecm)
            {
                _gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.energy_bomb)
            {
                _gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.energy_unit != EnergyUnit.None)
            {
                _gfx.DrawTextLeft(x, y, cmdr.energy_unit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.docking_computer)
            {
                _gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.galactic_hyperdrive)
            {
                _gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.front_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Front {laser_type(cmdr.front_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.rear_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Rear {laser_type(cmdr.rear_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.left_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Left {laser_type(cmdr.left_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (cmdr.right_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Right {laser_type(cmdr.right_laser)} Laser", GFX_COL.GFX_COL_WHITE);
            }
        }

        internal void DrawOptions(option[] option_list, int highlightedItem)
        {
            int OPTION_BAR_WIDTH = 400;
            int OPTION_BAR_HEIGHT = 15;

            ClearDisplay();
            _gfx.DrawTextCentre(20, "GAME OPTIONS", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            for (int i = 0; i < option_list.Length; i++)
            {
                int y = (384 - (30 * option_list.Length)) / 2;
                y += i * 30;

                if (i == highlightedItem)
                {
                    int x = gfx.GFX_X_CENTRE - (OPTION_BAR_WIDTH / 2);
                    _gfx.DrawRectangleFilled(x, y - 7, OPTION_BAR_WIDTH, OPTION_BAR_HEIGHT, GFX_COL.GFX_COL_DARK_RED);
                }

                GFX_COL col = ((!elite.docked) && option_list[i].docked_only) ? GFX_COL.GFX_COL_GREY_1 : GFX_COL.GFX_COL_WHITE;

                _gfx.DrawTextCentre(y, option_list[i].text, 120, col);
            }

            //TODO: get proper version number from assembly
            _gfx.DrawTextCentre(300, "Version: Beta 0.1", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(320, "The Sharp Kind by Andy Hawkins 2023", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(340, "The New Kind by Christian Pinder 1999-2001", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(360, "Based on original code by Ian Bell & David Braben", 120, GFX_COL.GFX_COL_WHITE);
        }

        internal void DrawSettings(Setting[] setting_list, int highlighted)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "GAME SETTINGS", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            for (int i = 0; i < setting_list.Length; i++)
            {
                int x;
                int y;
                if (i == (setting_list.Length - 1))
                {
                    y = ((setting_list.Length + 1) / 2 * 30) + 96 + 32;
                    if (i == highlighted)
                    {
                        x = gfx.GFX_X_CENTRE - 200;
                        _gfx.DrawRectangleFilled(x, y - 7, 400, 15, GFX_COL.GFX_COL_DARK_RED);
                    }

                    _gfx.DrawTextCentre(y, setting_list[i].name, 120, GFX_COL.GFX_COL_WHITE);
                    return;
                }

                int v = i switch
                {
                    0 => elite.config.UseWireframe ? 1 : 0,
                    1 => elite.config.AntiAliasWireframe ? 1 : 0,
                    2 => (int)elite.config.PlanetRenderStyle,
                    3 => elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
                    4 => elite.config.InstantDock ? 1 : 0,
                    _ => 0,
                };
                x = ((i & 1) * 250) + 32;
                y = (i / 2 * 30) + 96;

                if (i == highlighted)
                {
                    _gfx.DrawRectangleFilled(x, y, 100, 15, GFX_COL.GFX_COL_DARK_RED);
                }
                _gfx.DrawTextLeft(x, y, setting_list[i].name, GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(x + 120, y, setting_list[i].value[v], GFX_COL.GFX_COL_WHITE);
            }
        }

        internal void DrawQuit()
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "GAME OPTIONS", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0, 36), new(511, 36));

            _gfx.DrawTextCentre(175, "QUIT GAME (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);

            _gfx.ScreenUpdate();
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
            _gfx.DrawImage(Image.Scanner, new(gfx.GFX_X_OFFSET, 385 + gfx.GFX_Y_OFFSET));
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

            foreach (Image img in Enum.GetValues<Image>())
            {
                Stream? stream = loader.Load(img);
                if (stream != null)
                {
                    _gfx.LoadBitmap(img, stream);
                }
            }
        }

        internal void DrawLaserLines()
        {
            Vector2 point = new()
            {
                X = RNG.Random(126, 129) * gfx.GFX_SCALE,
                Y = RNG.Random(94, 97) * gfx.GFX_SCALE,
            };

            if (elite.config.UseWireframe)
            {
                // Left laser
                _gfx.DrawTriangle(new(32 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(48 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangle(new(208 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(224 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
            else
            {
                // Left laser
                _gfx.DrawTriangleFilled(new(32 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(48 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangleFilled(new(208 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(224 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
        }

        internal void draw_sun(univ_object planet)
        {
            Vector2 centre = new()
            {
                X = ((planet.location.X * 256 / planet.location.Z) + 128) * gfx.GFX_SCALE,
                Y = ((-planet.location.Y * 256 / planet.location.Z) + 96) * gfx.GFX_SCALE,
            };

            float radius = 6291456 / planet.location.Length() * gfx.GFX_SCALE;

            if ((centre.X + radius < 0) ||
                (centre.X - radius > 511) ||
                (centre.Y + radius < 0) ||
                (centre.Y - radius > 383))
            {
                return;
            }

            centre.X += gfx.GFX_X_OFFSET;
            centre.Y += gfx.GFX_Y_OFFSET;

            float s = -radius;
            float x = radius;
            float y = 0;

            // s -= x + x;
            while (y <= x)
            {
                // Top of top half
                render_sun_line(centre, y, -MathF.Floor(x), radius);
                // Top of top half
                render_sun_line(centre, x, -y, radius);
                // Top of bottom half
                render_sun_line(centre, x, y, radius);
                // Bottom of bottom half
                render_sun_line(centre, y, MathF.Floor(x), radius);

                s += y + y + 1;
                y++;
                if (s >= 0)
                {
                    s -= x + x + 2;
                    x--;
                }
            }
        }

        private void render_sun_line(Vector2 centre, float x, float y, float radius)
        {
            Vector2 s = new()
            {
                Y = centre.Y + y
            };

            if (s.Y is < (gfx.GFX_VIEW_TY + gfx.GFX_Y_OFFSET) or
                > (gfx.GFX_VIEW_BY + gfx.GFX_Y_OFFSET))
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            s.X -= radius * RNG.Random(2, 9) / 256f;
            ex += radius * RNG.Random(2, 9) / 256f;

            if ((s.X > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET) ||
                (ex < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET))
            {
                return;
            }

            if (s.X < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET)
            {
                s.X = gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET;
            }

            if (ex > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET)
            {
                ex = gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET;
            }

            float inner = radius * (200 + RNG.Random(7)) / 256;
            inner *= inner;

            float inner2 = radius * (220 + RNG.Random(7)) / 256;
            inner2 *= inner2;

            float outer = radius * (239 + RNG.Random(7)) / 256;
            outer *= outer;

            float dy = y * y;
            float dx = s.X - centre.X;

            for (; s.X <= ex; s.X++, dx++)
            {
                float distance = (dx * dx) + dy;

                GFX_COL colour = distance < inner
                    ? GFX_COL.GFX_COL_WHITE
                    : distance < inner2
                        ? GFX_COL.GFX_COL_YELLOW_4
                        : distance < outer
                            ? GFX_COL.GFX_COL_ORANGE_3
                            : ((int)s.X ^ (int)y).IsOdd() ? GFX_COL.GFX_COL_ORANGE_1 : GFX_COL.GFX_COL_ORANGE_2;

                _gfx.DrawPixelFast(s, colour);
            }
        }

    }
}