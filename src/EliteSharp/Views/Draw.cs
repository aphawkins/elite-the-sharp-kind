// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal sealed class Draw : IDraw
    {
        private readonly IGraphics _graphics;
        private readonly bool _isViewFullFrame;

        internal Draw(IGraphics graphics, bool isViewFullFrame)
        {
            _graphics = graphics;
            _isViewFullFrame = isViewFullFrame;
        }

        public float Bottom => _isViewFullFrame ? _graphics.ScreenHeight - BorderWidth : _graphics.ScreenHeight - ScannerHeight;

        public Vector2 Centre => new(_graphics.ScreenWidth / 2, (ScannerTop / 2) + BorderWidth);

        public bool IsWidescreen { get; }

        public float Left => BorderWidth;

        public float Right => _graphics.ScreenWidth - BorderWidth;

        public float ScannerLeft => Centre.X - (ScannerWidth / 2);

        public float ScannerTop => _graphics.ScreenHeight - ScannerHeight;

        public float Top => BorderWidth;

        internal float Height => Bottom - BorderWidth;

        internal float Width => _graphics.ScreenWidth - (2 * BorderWidth);

        private static float BorderWidth => 1;

        private static float ScannerHeight => 129;

        private static float ScannerWidth => 512;

        public void ClearDisplay() => _graphics.ClearArea(new(Left, Top), Width, Height);

        public void DrawBorder()
        {
            for (int i = 0; i < BorderWidth; i++)
            {
                _graphics.DrawRectangle(new(i, i), _graphics.ScreenWidth - 1 - (2 * i), Bottom - (2 * i), Colour.White);
            }
        }

        public void DrawHyperspaceCountdown(int countdown) => _graphics.DrawTextRight(Left + 21, Top + 4, $"{countdown}", Colour.White);

        public void DrawSun(IShip planet)
        {
            Vector2 centre = new(planet.Location.X, -planet.Location.Y);

            centre *= 256 / planet.Location.Z;
            centre += Centre / 2;
            centre *= _graphics.Scale;

            float radius = 6291456 / planet.Location.Length() * _graphics.Scale;

            if ((centre.X + radius < Left) ||
                (centre.X - radius > Right) ||
                (centre.Y + radius < Top) ||
                (centre.Y - radius > Bottom))
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

        public void DrawTextPretty(Vector2 position, float width, string text)
        {
            int i = 0;
            float maxlen = (width - position.X) / 8;
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
                _graphics.DrawTextLeft(position, text[previous..i], Colour.White);
                previous = i;
                position.Y += 8 * _graphics.Scale;
            }
        }

        public void DrawViewHeader(string title)
        {
            _graphics.DrawTextCentre(20, title, FontSize.Large, Colour.Gold);
            _graphics.DrawLine(new(0, 36), new(511, 36));
        }

        public async Task LoadImagesAsync(CancellationToken token)
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

        public void SetFullScreenClipRegion() => _graphics.SetClipRegion(new(0, 0), _graphics.ScreenWidth, _graphics.ScreenHeight);

        public void SetViewClipRegion() => _graphics.SetClipRegion(new(Left, Top), Width, Height);

        private void RenderSunLine(Vector2 centre, float x, float y, float radius)
        {
            Vector2 s = new()
            {
                Y = centre.Y + y,
            };

            if (s.Y < Top || s.Y > Bottom)
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            s.X -= radius * RNG.Random(2, 10) / 256f;
            ex += radius * RNG.Random(2, 10) / 256f;

            if (ex < Left || s.X > Right)
            {
                return;
            }

            if (s.X < Left)
            {
                s.X = Left;
            }

            if (ex > Right)
            {
                ex = Right;
            }

            float inner = radius * (200 + RNG.Random(8)) / 256;
            inner *= inner;

            float inner2 = radius * (220 + RNG.Random(8)) / 256;
            inner2 *= inner2;

            float outer = radius * (239 + RNG.Random(8)) / 256;
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
