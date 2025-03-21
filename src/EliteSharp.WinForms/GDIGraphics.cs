// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.Versioning;
using EliteSharp.Assets.Fonts;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms;

[SupportedOSPlatform("windows")]
public sealed class GDIGraphics : IGraphics
{
    private readonly FastBitmap _fastScreen;
    private readonly Dictionary<FontType, Font> _fonts;
    private readonly Dictionary<ImageType, Bitmap> _images;
    private readonly Dictionary<FastColor, Pen> _pens = [];
    private readonly Bitmap _screen;
    private readonly System.Drawing.Graphics _screenGraphics;
    private readonly Action<Bitmap> _screenUpdate;
    private RectangleF _clipRegion;
    private bool _isDisposed;

    public GDIGraphics(
        float screenWidth,
        float screenHeight,
        GDIAssetLoader assetLoader,
        Action<Bitmap> screenUpdate)
    {
        Guard.ArgumentNull(assetLoader);

        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        _images = assetLoader.LoadImages();
        _fonts = assetLoader.LoadFonts();
        _screenUpdate = screenUpdate;
        _fastScreen = new((int)screenWidth, (int)screenHeight);
        _screen = new((int)screenWidth, (int)screenHeight, (int)screenWidth * 4, PixelFormat.Format32bppArgb, _fastScreen.BitmapHandle);
        _screenGraphics = System.Drawing.Graphics.FromImage(_screen);

        foreach (FastColor color in EliteColors.AllColors())
        {
            Pen pen = new(Color.FromArgb((int)color.Argb));
            _pens.Add(color, pen);
        }
    }

    public float Scale { get; } = 2;

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    public void Clear()
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.Clear(Color.Black);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void DrawCircle(Vector2 centre, float radius, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.DrawEllipse(_pens[color], centre.X - radius, centre.Y - radius, 2 * radius, 2 * radius);
    }

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.FillEllipse(_pens[color].Brush, centre.X - radius, centre.Y - radius, 2 * radius, 2 * radius);
    }

    public void DrawImage(ImageType image, Vector2 position)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.DrawImage(
            _images[image],
            position.X,
            position.Y,
            _images[image].Width,
            _images[image].Height);
    }

    public void DrawImageCentre(ImageType image, float y)
    {
        float x = (ScreenWidth - _images[image].Width) / 2;
        DrawImage(image, new(x, y));
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.DrawLine(_pens[color], lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
    }

    public void DrawPixel(Vector2 position, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        // Prevent SetPixel from drawing outside of the clip region
        if (position.X < _clipRegion.Left ||
            position.X > _clipRegion.Right ||
            position.Y < _clipRegion.Top ||
            position.Y > _clipRegion.Bottom)
        {
            return;
        }

        _fastScreen.SetPixel((int)position.X, (int)position.Y, (uint)_pens[color].Color.ToArgb());
    }

    public void DrawPolygon(Vector2[] points, FastColor lineColor)
    {
        if (_isDisposed || points == null)
        {
            return;
        }

        PointF[] drawPoints = new PointF[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            drawPoints[i] = new PointF(points[i].X, points[i].Y);
        }

        _screenGraphics.DrawPolygon(_pens[lineColor], drawPoints);
    }

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor)
    {
        if (_isDisposed || points == null)
        {
            return;
        }

        PointF[] drawPoints = new PointF[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            drawPoints[i] = new PointF(points[i].X, points[i].Y);
        }

        _screenGraphics.FillPolygon(_pens[faceColor].Brush, drawPoints);
    }

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.DrawRectangle(_pens[color], position.X, position.Y, width, height);
    }

    public void DrawRectangleCentre(float y, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.DrawRectangle(
            _pens[color],
            (ScreenWidth - width) / 2,
            y / (2 / Scale),
            width,
            height);
    }

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        _screenGraphics.FillRectangle(_pens[color].Brush, position.X, position.Y, width, height);
    }

    public void DrawTextCentre(float y, string text, FontType fontType, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        using StringFormat stringFormat = new()
        {
            Alignment = StringAlignment.Center,
        };

        _screenGraphics.DrawString(
            text,
            _fonts[fontType],
            _pens[color].Brush,
            ScreenWidth / 2,
            y / (2 / Scale),
            stringFormat);
    }

    public void DrawTextLeft(Vector2 position, string text, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        PointF point = new(position.X / (2 / Scale), position.Y / (2 / Scale));
        _screenGraphics.DrawString(text, _fonts[FontType.Small], _pens[color].Brush, point);
    }

    public void DrawTextRight(Vector2 position, string text, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        using StringFormat stringFormat = new()
        {
            Alignment = StringAlignment.Far,
        };

        _screenGraphics.DrawString(
            text,
            _fonts[FontType.Small],
            _pens[color].Brush,
            position.X / (2 / Scale),
            position.Y / (2 / Scale),
            stringFormat);
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        PointF[] points =
        [
            new(a.X, a.Y),
            new(b.X, b.Y),
            new(c.X, c.Y),
        ];

        _screenGraphics.DrawLines(_pens[color], points);
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        PointF[] points =
        [
            new(a.X, a.Y),
            new(b.X, b.Y),
            new(c.X, c.Y),
        ];

        _screenGraphics.FillPolygon(_pens[color].Brush, points);
    }

    public void ScreenUpdate()
    {
        if (_isDisposed)
        {
            return;
        }

        _screenUpdate(_screen);
    }

    public void SetClipRegion(Vector2 position, float width, float height)
    {
        if (_isDisposed)
        {
            return;
        }

        _clipRegion = new RectangleF(position.X, position.Y, width, height);
        _screenGraphics.Clip = new Region(_clipRegion);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _screenGraphics?.Dispose();
                _screen?.Dispose();
                _fastScreen?.Dispose();

                // Images
                foreach (KeyValuePair<FontType, Font> font in _fonts)
                {
                    font.Value.Dispose();
                }

                // Images
                foreach (KeyValuePair<ImageType, Bitmap> image in _images)
                {
                    image.Value.Dispose();
                }
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }
}
