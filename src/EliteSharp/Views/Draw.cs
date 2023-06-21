// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal sealed class Draw
    {
        private readonly IGraphics _graphics;

        internal Draw(IGraphics graphics)
        {
            _graphics = graphics;
            BorderWidth = 1;
            ScannerWidth = 512;
            ScannerHeight = 129;
            ScannerLeft = _graphics.Centre.X - (ScannerWidth / 2);
            ScannerTop = _graphics.Height - ScannerHeight;
        }

        internal float BorderWidth { get; }

        internal float Left { get; }

        internal float ScannerHeight { get; }

        internal float ScannerLeft { get; }

        internal float ScannerTop { get; }

        internal float ScannerWidth { get; }

        internal float Top { get; }

        internal void ClearDisplay() => _graphics.ClearArea(new(Left + BorderWidth, Top + BorderWidth), _graphics.Width - (2 * BorderWidth), ScannerTop - BorderWidth);

        internal void SetDisplayClipRegion() => _graphics.SetClipRegion(new(Left + BorderWidth, Top + BorderWidth), _graphics.Width - (2 * BorderWidth), ScannerTop - BorderWidth);

        internal void SetScannerClipRegion() => _graphics.SetClipRegion(new(ScannerLeft, ScannerTop), ScannerWidth, ScannerHeight);

        internal void DrawBorder()
        {
            for (int i = 0; i < BorderWidth; i++)
            {
                _graphics.DrawRectangle(new(Left + i, Top + i), _graphics.Width - 1 - (2 * i), ScannerTop + BorderWidth - (2 * i), Colour.White);
            }
        }

        internal void DrawSun(IShip planet)
        {
            Vector2 centre = new()
            {
                X = ((planet.Location.X * 256 / planet.Location.Z) + 128) * _graphics.Scale,
                Y = ((-planet.Location.Y * 256 / planet.Location.Z) + 96) * _graphics.Scale,
            };

            float radius = 6291456 / planet.Location.Length() * _graphics.Scale;

            if ((centre.X + radius < 0) ||
                (centre.X - radius > 511) ||
                (centre.Y + radius < 0) ||
                (centre.Y - radius > 383))
            {
                return;
            }

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
                _graphics.DrawTextLeft(x, y, text[previous..i], Colour.White);
                previous = i;
                y += 8 * _graphics.Scale;
            }
        }

        internal void DrawViewHeader(string title)
        {
            _graphics.DrawTextCentre(20, title, FontSize.Large, Colour.Gold);
            _graphics.DrawLine(new(0, 36), new(511, 36));
        }

        internal async Task LoadImagesAsync(CancellationToken token)
        {
            AssetFileLoader loader = new();
            ParallelOptions options = new()
            {
                CancellationToken = token,
            };

            await Parallel.ForEachAsync(
                Enum.GetValues<Image>(),
                options,
                async (img, token) => _graphics.LoadBitmap(img, await loader.LoadAsync(img, token).ConfigureAwait(false)))
                .ConfigureAwait(false);
        }

        private void RenderSunLine(Vector2 centre, float x, float y, float radius)
        {
            Vector2 s = new()
            {
                Y = centre.Y + y,
            };

            if (s.Y < _graphics.ViewT.Y || s.Y > _graphics.ViewB.Y)
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            s.X -= radius * RNG.Random(2, 9) / 256f;
            ex += radius * RNG.Random(2, 9) / 256f;

            if ((s.X > _graphics.ViewB.X) || (ex < _graphics.ViewT.X))
            {
                return;
            }

            if (s.X < _graphics.ViewT.X)
            {
                s.X = _graphics.ViewT.X;
            }

            if (ex > _graphics.ViewB.X)
            {
                ex = _graphics.ViewB.X;
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

                Colour colour = distance < inner
                    ? Colour.White
                    : distance < inner2
                        ? Colour.LightYellow
                        : distance < outer
                            ? Colour.LightOrange
                            : ((int)s.X ^ (int)y).IsOdd() ? Colour.Orange : Colour.DarkOrange;

                _graphics.DrawPixelFast(s, colour);
            }
        }
    }
}
