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