// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Assets;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine.Views
{
    internal sealed class Draw
    {
        private readonly IGfx _gfx;

        internal Draw(IGfx gfx) => _gfx = gfx;

        internal void ClearDisplay() => _gfx.ClearArea(Graphics.GFX_X_OFFSET + 1, Graphics.GFX_Y_OFFSET + 1, 510 + Graphics.GFX_X_OFFSET, 383 + Graphics.GFX_Y_OFFSET);

        internal void DrawBorder()
        {
            _gfx.DrawLine(new(0, 0), new(0, 384));
            _gfx.DrawLine(new(0, 0), new(511, 0));
            _gfx.DrawLine(new(511, 0), new(511, 384));
        }

        internal void DrawScanner() => _gfx.DrawImage(Image.Scanner, new(Graphics.GFX_X_OFFSET, 385 + Graphics.GFX_Y_OFFSET));

        internal void DrawSun(UniverseObject planet)
        {
            Vector2 centre = new()
            {
                X = ((planet.Location.X * 256 / planet.Location.Z) + 128) * Graphics.GFX_SCALE,
                Y = ((-planet.Location.Y * 256 / planet.Location.Z) + 96) * Graphics.GFX_SCALE,
            };

            float radius = 6291456 / planet.Location.Length() * Graphics.GFX_SCALE;

            if ((centre.X + radius < 0) ||
                (centre.X - radius > 511) ||
                (centre.Y + radius < 0) ||
                (centre.Y - radius > 383))
            {
                return;
            }

            centre.X += Graphics.GFX_X_OFFSET;
            centre.Y += Graphics.GFX_Y_OFFSET;

            float s = -radius;
            float x = radius;
            float y = 0;

            // s -= x + x;
            while (y <= x)
            {
                // Top of top half
                RenderSunLine(centre, y, -MathF.Floor(x), radius);
                // Top of top half
                RenderSunLine(centre, x, -y, radius);
                // Top of bottom half
                RenderSunLine(centre, x, y, radius);
                // Bottom of bottom half
                RenderSunLine(centre, y, MathF.Floor(x), radius);

                s += y + y + 1;
                y++;
                if (s >= 0)
                {
                    s -= x + x + 2;
                    x--;
                }
            }
        }

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
                _gfx.DrawTextLeft(x + Graphics.GFX_X_OFFSET, y + Graphics.GFX_Y_OFFSET, text[previous..i], GFX_COL.GFX_COL_WHITE);
                previous = i;
                y += 8 * Graphics.GFX_SCALE;
            }
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

        private void RenderSunLine(Vector2 centre, float x, float y, float radius)
        {
            Vector2 s = new()
            {
                Y = centre.Y + y
            };

            if (s.Y is < (Graphics.GFX_VIEW_TY + Graphics.GFX_Y_OFFSET) or
                > (Graphics.GFX_VIEW_BY + Graphics.GFX_Y_OFFSET))
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            s.X -= radius * RNG.Random(2, 9) / 256f;
            ex += radius * RNG.Random(2, 9) / 256f;

            if ((s.X > Graphics.GFX_VIEW_BX + Graphics.GFX_X_OFFSET) ||
                (ex < Graphics.GFX_VIEW_TX + Graphics.GFX_X_OFFSET))
            {
                return;
            }

            if (s.X < Graphics.GFX_VIEW_TX + Graphics.GFX_X_OFFSET)
            {
                s.X = Graphics.GFX_VIEW_TX + Graphics.GFX_X_OFFSET;
            }

            if (ex > Graphics.GFX_VIEW_BX + Graphics.GFX_X_OFFSET)
            {
                ex = Graphics.GFX_VIEW_BX + Graphics.GFX_X_OFFSET;
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
