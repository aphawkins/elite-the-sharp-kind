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

        // TOOD: This should be private
        internal void DrawTextPretty(float x, float y, float width, string text)
        {
            int i = 0;
            float maxlen = (width - x) / 8;
            int previous = i;

            while (i < text.Length)
            {
                i += (int)maxlen;
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

        internal void DrawSettings(Setting[] setting_list, int highlighted)
        {
            ClearDisplay();
            _gfx.DrawTextCentre(20, "GAME SETTINGS", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            for (int i = 0; i < setting_list.Length; i++)
            {
                float x;
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
            _gfx.DrawLine(new(0, 0), new(0, 384));
            _gfx.DrawLine(new(0, 0), new(511, 0));
            _gfx.DrawLine(new(511, 0), new(511, 384));
        }

        internal void DrawViewHeader(string title)
        {
            _gfx.DrawTextCentre(20, title, 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0, 36), new(511, 36));
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