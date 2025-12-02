// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Numerics;
using Useful.Assets;

namespace Useful.Graphics;

public sealed class SoftwareGraphics : IGraphics, IDisposable
{
    private readonly FastBitmap _screen;
    private readonly Action<FastBitmap> _screenUpdate;
    private readonly Dictionary<string, FastBitmap> _textCache = [];
    private bool _isDisposed;

    private SoftwareGraphics(float screenWidth, float screenHeight, Action<FastBitmap> screenUpdate)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        _screen = new((int)screenWidth, (int)screenHeight);
        _screenUpdate = screenUpdate;
        Clear();
    }

    public float Scale { get; } = 2;

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    internal Dictionary<int, BitmapFont> Fonts { get; set; } = [];

    internal Dictionary<int, FastBitmap> Images { get; set; } = [];

    public static SoftwareGraphics Create(
        float screenWidth,
        float screenHeight,
        Action<FastBitmap> screenUpdate,
        IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(screenUpdate);
        Guard.ArgumentNull(assetLocator);

        return new(screenWidth, screenHeight, screenUpdate)
        {
            Images = assetLocator.ImagePaths.ToDictionary(
                x => x.Key,
                x => BitmapFile.Read(x.Value)),

            Fonts = assetLocator.FontBitmapPaths.ToDictionary(
                x => x.Key,
                x => new BitmapFont(BitmapFile.Read(x.Value))),
        };
    }

    public void Clear() => _screen.Clear();

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void DrawCircle(Vector2 centre, float radius, uint color)
    {
        float diameter = radius * 2;
        float x = MathF.Floor(radius);
        float y = 0;
        float tx = 1;
        float ty = 1;
        float error = tx - diameter;

        while (x >= y)
        {
            DrawPixel(new(centre.X + x, centre.Y + y), color);
            DrawPixel(new(centre.X + x, centre.Y - y), color);
            DrawPixel(new(centre.X - x, centre.Y + y), color);
            DrawPixel(new(centre.X - x, centre.Y - y), color);
            DrawPixel(new(centre.X + y, centre.Y + x), color);
            DrawPixel(new(centre.X + y, centre.Y - x), color);
            DrawPixel(new(centre.X - y, centre.Y + x), color);
            DrawPixel(new(centre.X - y, centre.Y - x), color);

            if (error <= 0)
            {
                y++;
                error += ty;
                ty += 2;
            }

            if (error > 0)
            {
                x--;
                tx += 2;
                error += tx - diameter;
            }
        }
    }

    public void DrawCircleFilled(Vector2 centre, float radius, uint color)
    {
        float diameter = MathF.Floor(radius) * 2;
        float x = MathF.Floor(radius);
        float y = 0;
        float tx = 1;
        float ty = 1;
        float error = tx - diameter;

        while (x >= y)
        {
            Debug.WriteLine($"{x},{y}");

            // Top of top half
            DrawLine(new(centre.X - y, centre.Y - x), new(centre.X + y, centre.Y - x), color);

            // Bottom of top half
            DrawLine(new(centre.X - x, centre.Y - y), new(centre.X + x, centre.Y - y), color);

            // Top of bottom half
            DrawLine(new(centre.X - x, centre.Y + y), new(centre.X + x, centre.Y + y), color);

            // Bottom of bottom half
            DrawLine(new(centre.X - y, centre.Y + x), new(centre.X + y, centre.Y + x), color);

            if (error <= 0)
            {
                y++;
                error += ty;
                ty += 2;
            }

            if (error > 0)
            {
                x--;
                tx += 2;
                error += tx - diameter;
            }
        }
    }

    public void DrawImage(int imageType, Vector2 position)
    {
        Debug.Assert(Images.ContainsKey(imageType), "Image has not been loaded");

        FastBitmap bitmap = Images[imageType];
        DrawImage(bitmap, position);
    }

    public void DrawImageCentre(int imageType, float y)
    {
        float x = (ScreenWidth - Images[imageType].Width) / 2;
        DrawImage(imageType, new(x, y));
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, uint color)
        => DrawLineInt(
            (int)MathF.Floor(lineStart.X),
            (int)MathF.Floor(lineStart.Y),
            (int)MathF.Floor(lineEnd.X),
            (int)MathF.Floor(lineEnd.Y),
            color);

    public void DrawPixel(Vector2 position, uint color)
    {
        // TODO: Optimize bounds checking
        if (position.X < 0 || position.Y < 0 || position.X >= ScreenWidth || position.Y >= ScreenHeight)
        {
            return;
        }

        _screen.SetPixel((int)position.X, (int)position.Y, color);
    }

    public void DrawPolygon(Vector2[] points, uint lineColor)
    {
        if (points == null)
        {
            return;
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            DrawLine(points[i], points[i + 1], lineColor);
        }

        DrawLine(points[0], points[^1], lineColor);
    }

    public void DrawPolygonFilled(Vector2[] points, uint faceColor)
    {
        if (points == null)
        {
            return;
        }

        // Create triangles of which each share the first vertex
        for (int i = 1; i < points.Length - 1; i++)
        {
            DrawTriangleFilled(points[0], points[i], points[i + 1], faceColor);
        }
    }

    public void DrawRectangle(Vector2 position, float width, float height, uint color)
        => DrawRectangleInt(
            (int)MathF.Floor(position.X),
            (int)MathF.Floor(position.Y),
            (int)MathF.Floor(width),
            (int)MathF.Floor(height),
            color);

    public void DrawRectangleCentre(float y, float width, float height, uint color)
        => DrawRectangle(new((ScreenWidth - width) / Scale, y), width, height, color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, uint color)
        => DrawRectangleFilledInt(
            (int)MathF.Floor(position.X),
            (int)MathF.Floor(position.Y),
            (int)MathF.Floor(width),
            (int)MathF.Floor(height),
            color);

    public void DrawTextCentre(float y, string text, int fontType, uint color)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        using FastBitmap bitmapText = GenerateTextBitmap(text, fontType, color);
        int x = (int)((ScreenWidth / 2) - (bitmapText.Width / 2));
        DrawImage(bitmapText, new(x, y));
    }

    public void DrawTextLeft(Vector2 position, string text, int fontType, uint color)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        using FastBitmap bitmapText = GenerateTextBitmap(text, fontType, color);
        DrawImage(bitmapText, position);
    }

    public void DrawTextRight(Vector2 position, string text, int fontType, uint color)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        using FastBitmap bitmapText = GenerateTextBitmap(text, fontType, color);
        DrawImage(bitmapText, position - new Vector2(bitmapText.Width, 0));
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
        DrawLine(a, b, color);
        DrawLine(b, c, color);
        DrawLine(c, a, color);
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
        // Sort the points so that a.Y <= b.Y <= c.Y
        (a, b, c) = SortPointsByY(a, b, c);

        // Clamp Y range to screen bounds
        int ay = Math.Max((int)MathF.Ceiling(a.Y), 0);
        ////int by = Math.Max((int)MathF.Ceiling(b.Y), 0);
        int cy = Math.Min((int)MathF.Floor(c.Y), (int)ScreenHeight - 1);

        // Interpolate edges
        int[] ab = Interpolate(a.Y, a.X, b.Y, b.X);
        int[] bc = Interpolate(b.Y, b.X, c.Y, c.X);
        int[] ac = Interpolate(a.Y, a.X, c.Y, c.X);

        // Combine ab and bc (excluding duplicate at join)
        int[] abc = new int[ab.Length + bc.Length - 1];
        Array.Copy(ab, 0, abc, 0, ab.Length - 1);
        Array.Copy(bc, 0, abc, ab.Length - 1, bc.Length);

        // Determine left and right edges
        int m = abc.Length / 2;
        int[] leftX = abc;
        int[] rightX = ac;

        if (ac.Length > m && abc.Length > m && ac[m] < abc[m])
        {
            (leftX, rightX) = (rightX, leftX);
        }

        // Draw scanlines
        for (int y = ay; y <= cy; y++)
        {
            int i = y - (int)MathF.Floor(a.Y);
            if (i < 0 || i >= leftX.Length || i >= rightX.Length)
            {
                continue;
            }

            int start = Math.Max(leftX[i], 0);
            int end = Math.Min(rightX[i], (int)ScreenWidth - 1);

            for (int x = start; x <= end; x++)
            {
                DrawPixel(x, y, color);
            }
        }
    }

    public void ScreenUpdate() => _screenUpdate(_screen);

    public void SetClipRegion(Vector2 position, float width, float height)
    {
    }

    private static int[] Interpolate(float i0, float d0, float i1, float d1)
    {
        if ((int)MathF.Floor(i0) == (int)MathF.Floor(i1))
        {
            return [(int)MathF.Floor(d0)];
        }

        List<int> values = [];

        float a = (d1 - d0) / (i1 - i0);
        float d = d0;
        for (int i = (int)MathF.Floor(i0); i <= (int)MathF.Floor(i1); i++)
        {
            values.Add((int)MathF.Floor(d));

            d += a;
        }

        return [.. values];
    }

    private static (Vector2 A, Vector2 B, Vector2 C) SortPointsByY(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2[] sorted = [a, b, c];
        Array.Sort(sorted, (i, j) => i.Y.CompareTo(j.Y));
        return (sorted[0], sorted[1], sorted[2]);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _screen?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }

    private void DrawImage(FastBitmap bitmap, Vector2 position)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                uint color = bitmap.GetPixel(x, y);
                if ((color & 0xFF000000) != 0)
                {
                    // TODO: should mix the transparent colors correctly here
                    // but the only transparency being used is transparent or opaque
                    DrawPixel((int)(position.X + x), (int)(position.Y + y), color);
                }
            }
        }
    }

    private void DrawLineInt(int x0, int y0, int x1, int y1, in uint color)
    {
        int screenWidth = (int)ScreenWidth;   // Replace with actual screen width
        int screenHeight = (int)ScreenHeight; // Replace with actual screen height

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x0 >= 0 && x0 < screenWidth && y0 >= 0 && y0 < screenHeight)
            {
                DrawPixel(x0, y0, color);
            }

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private void DrawPixel(int x, int y, uint color) => _screen.SetPixel(x, y, color);

    private void DrawRectangleFilledInt(int startX, int startY, int width, int height, in uint color)
    {
        startX = Math.Min(Math.Max(startX, 0), (int)ScreenWidth);
        startY = Math.Min(Math.Max(startY, 0), (int)ScreenWidth);
        int endX = Math.Min(Math.Max(startX + width - 1, 0), (int)ScreenWidth);
        int endY = Math.Min(Math.Max(startY + height - 1, 0), (int)ScreenWidth);

        // Draw horizontal lined
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                _screen.SetPixel(x, y, color);
            }
        }
    }

    private void DrawRectangleInt(int startX, int startY, int width, int height, in uint color)
    {
        startX = Math.Min(Math.Max(startX, 0), (int)ScreenWidth);
        startY = Math.Min(Math.Max(startY, 0), (int)ScreenWidth);
        int endX = Math.Min(Math.Max(startX + width - 1, 0), (int)ScreenWidth);
        int endY = Math.Min(Math.Max(startY + height - 1, 0), (int)ScreenWidth);

        // Draw horizontal lines
        for (int x = startX; x <= endX; x++)
        {
            _screen.SetPixel(x, startY, color);
            _screen.SetPixel(x, endY, color);
        }

        for (int y = startY + 1; y <= endY - 1; y++)
        {
            _screen.SetPixel(startX, y, color);
            _screen.SetPixel(endX, y, color);
        }
    }

    private FastBitmap GenerateTextBitmap(string text, int fontType, uint color)
    {
        string key = $"{fontType}_{color}_{text}";

        if (_textCache.TryGetValue(key, out FastBitmap? cacheBitmap))
        {
            return cacheBitmap;
        }

        BitmapFont font = Fonts[fontType];

        using FastBitmap temp = new(text.Length * BitmapFont.CharSize, BitmapFont.CharSize);
        int totalWidth = 0;

        foreach (char letter in text)
        {
            int charRow = (letter >> 4) - 2;
            int charColumn = letter & 0xF;
            int charX = 0;
            int charY = 0;
            int maxCharWidth = 0;

            uint GetPixel()
            {
                uint pixelColor = font.Image.GetPixel(
                    charX + (BitmapFont.CharSize * charColumn) + 1,
                    charY + (BitmapFont.CharSize * charRow) + 1);

                return pixelColor == BaseColors.Cyan.Argb ? color : pixelColor;
            }

            uint pixelColor = GetPixel();

            do
            {
                maxCharWidth = 0;

                do
                {
                    temp.SetPixel(totalWidth + charX, charY, pixelColor);
                    charX++;
                    pixelColor = GetPixel();
                }
                while (pixelColor != BaseColors.Magenta.Argb);

                maxCharWidth = Math.Max(maxCharWidth, charX);
                charX = 0;
                charY++;

                pixelColor = GetPixel();
            }
            while (pixelColor != BaseColors.Magenta.Argb);

            totalWidth += maxCharWidth;
        }

        FastBitmap bitmap = temp.Resize(totalWidth, BitmapFont.CharSize);
        _textCache.Add(key, bitmap);
        return bitmap;
    }
}
