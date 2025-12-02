// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Useful.Assets;
using Useful.Graphics;
using static SDL2.SDL;
using static SDL2.SDL_gfx;
using static SDL2.SDL_ttf;

namespace Useful.SDL;

public sealed class SDLGraphics : IGraphics, IDisposable
{
    private readonly SDLRenderer _renderer;
    private Dictionary<int, nint> _fonts = [];
    private Dictionary<int, nint> _images = [];
    private bool _isDisposed;

    private SDLGraphics(SDLRenderer renderer, float screenWidth, float screenHeight)
    {
        Guard.ArgumentNull(renderer);

        _renderer = renderer;
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
    }

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~SDLGraphics()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public float Scale { get; } = 2;

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    public static SDLGraphics Create(SDLRenderer renderer, float screenWidth, float screenHeight, IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        return new(renderer, screenWidth, screenHeight)
        {
            _images = assetLocator.ImagePaths.ToDictionary(
                x => x.Key,
                x => SDLGuard.Execute(() => SDL_LoadBMP(x.Value))),

            _fonts = assetLocator.FontTrueTypePaths.ToDictionary(
                x => x.Key,
                x => LoadFont(x.Key, x.Value)),
        };
    }

    public void Clear()
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(BaseColors.Black.Argb);

        SDLGuard.Execute(() => SDL_RenderClear(_renderer));
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void DrawCircle(Vector2 centre, float radius, uint color)
        => SDLGuard.Execute(() => circleColor(_renderer, (short)centre.X, (short)centre.Y, (short)radius, color));

    public void DrawCircleFilled(Vector2 centre, float radius, uint color)
        => SDLGuard.Execute(() => filledCircleColor(_renderer, (short)centre.X, (short)centre.Y, (short)radius, color));

    public void DrawImage(int imageType, Vector2 position)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[imageType]);
        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, _images[imageType]));

        SDL_Rect dest = new()
        {
            x = (int)position.X,
            y = (int)position.Y,
            w = surface.w,
            h = surface.h,
        };

        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_DestroyTexture(texture);
    }

    public void DrawImageCentre(int imageType, float y)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[imageType]);
        float x = (ScreenWidth - surface.w) / 2;
        DrawImage(imageType, new(x, y));
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, uint color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderDrawLineF(_renderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y));
    }

    public void DrawPixel(Vector2 position, uint color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderDrawPointF(_renderer, position.X, position.Y));
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

        // SDL_RenderGeometry only renders triangles and quads?
        // Create triangles of which each share the first vertex
        for (int i = 1; i < points.Length - 1; i++)
        {
            DrawTriangleFilled(points[0], points[i], points[i + 1], faceColor);
        }
    }

    public void DrawRectangle(Vector2 position, float width, float height, uint color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDL_FRect rectangle = new()
        {
            x = position.X / (2 / Scale),
            y = position.Y / (2 / Scale),
            w = width + 1,
            h = height + 1,
        };

        SDLGuard.Execute(() => SDL_RenderDrawRectF(_renderer, ref rectangle));
    }

    public void DrawRectangleCentre(float y, float width, float height, uint color)
        => DrawRectangle(new((ScreenWidth - width) / Scale, y), width, height, color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, uint color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDL_FRect rectangle = new()
        {
            x = position.X / (2 / Scale),
            y = position.Y / (2 / Scale),
            w = width + 1,
            h = height + 1,
        };

        SDLGuard.Execute(() => SDL_RenderFillRectF(_renderer, ref rectangle));
    }

    public void DrawTextCentre(float y, string text, int fontType, uint color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[fontType],
            text,
            ToSDLColor(color)));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)((ScreenWidth / 2) - (dest.w / 2));
        dest.y = (int)(y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTextLeft(Vector2 position, string text, int fontType, uint color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[fontType],
            text,
            ToSDLColor(color)));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)(position.X / (2 / Scale));
        dest.y = (int)(position.Y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTextRight(Vector2 position, string text, int fontType, uint color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[fontType],
            text,
            ToSDLColor(color)));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)((position.X - dest.w) / (2 / Scale));
        dest.y = (int)(position.Y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
        DrawLine(a, b, color);
        DrawLine(b, c, color);
        DrawLine(c, a, color);
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Vertex[] vertices =
        [
            ConvertVertex(a, color),
            ConvertVertex(b, color),
            ConvertVertex(c, color),
        ];

        SDLGuard.Execute(() => SDL_RenderGeometry(_renderer, nint.Zero, vertices, vertices.Length, null, 0));
    }

    public void ScreenUpdate()
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_RenderPresent(_renderer);
    }

    public void SetClipRegion(Vector2 position, float width, float height)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Rect rectangle = new()
        {
            x = (int)(position.X / (2 / Scale)),
            y = (int)(position.Y / (2 / Scale)),
            w = (int)width,
            h = (int)height,
        };

        SDLGuard.Execute(() => SDL_RenderSetClipRect(_renderer, ref rectangle));
    }

    private static nint LoadFont(int fontType, string fontPath)
    {
        Debug.Assert(File.Exists(fontPath), $"Font file '{fontPath}' does not exist.");
        Debug.Assert(
            string.Equals(Path.GetExtension(fontPath), ".ttf", StringComparison.OrdinalIgnoreCase),
            $"Font file '{fontPath}' must be a TTF file.");

        return fontType switch
        {
            0 => SDLGuard.Execute(() => TTF_OpenFont(fontPath, 12)),
            1 => SDLGuard.Execute(() => TTF_OpenFont(fontPath, 18)),
            _ => throw new ArgumentOutOfRangeException(nameof(fontType), fontType, null),
        };
    }

    private static SDL_Vertex ConvertVertex(Vector2 point, uint color) => new()
    {
        position = new() { x = point.X, y = point.Y },
        tex_coord = new() { x = 0.0f, y = 0.0f },
        color = ToSDLColor(color),
    };

    private static SDL_Color ToSDLColor(uint color) => new()
    {
        r = (byte)((color >> 24) & 0xFF),
        g = (byte)((color >> 16) & 0xFF),
        b = (byte)((color >> 8) & 0xFF),
        a = (byte)(color & 0xFF),
    };

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (disposing)
            {
                // dispose managed state (managed objects)
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null

            // Fonts
            foreach (KeyValuePair<int, nint> font in _fonts)
            {
                TTF_CloseFont(font.Value);
            }

            // Images
            foreach (KeyValuePair<int, nint> image in _images)
            {
                SDL_FreeSurface(image.Value);
            }
        }
    }

    private void SetRenderDrawColor(uint color)
    {
        FastColor fastColor = new(color);
        SDLGuard.Execute(() => SDL_SetRenderDrawColor(_renderer, fastColor.R, fastColor.G, fastColor.B, fastColor.A));
    }
}
