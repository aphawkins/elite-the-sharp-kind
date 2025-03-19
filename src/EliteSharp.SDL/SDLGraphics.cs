// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using System.Runtime.InteropServices;
using EliteSharp.Assets.Fonts;
using EliteSharp.Graphics;
using static SDL2.SDL;
using static SDL2.SDL_gfx;
using static SDL2.SDL_ttf;

namespace EliteSharp.SDL;

internal sealed class SDLGraphics : IGraphics
{
    private readonly Dictionary<FontType, nint> _fonts;
    private readonly Dictionary<ImageType, nint> _images;
    private readonly Dictionary<FastColor, SDL_Color> _sdlColors = [];
    private readonly SDLRenderer _renderer;
    private bool _isDisposed;

    public SDLGraphics(SDLRenderer renderer, float screenWidth, float screenHeight, SDLAssetLoader assetLoader)
    {
        Guard.ArgumentNull(renderer);
        Guard.ArgumentNull(assetLoader);

        _renderer = renderer;
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;

        foreach (FastColor color in EliteColors.AllColors())
        {
            SDL_Color sdlColor = new()
            {
                a = color.A,
                r = color.R,
                b = color.B,
                g = color.G,
            };
            _sdlColors.Add(color, sdlColor);
        }

        _images = assetLoader.LoadImages();
        _fonts = assetLoader.LoadFonts();
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

    public void Clear()
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(EliteColors.Black);

        SDLGuard.Execute(() => SDL_RenderClear(_renderer));
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void DrawCircle(Vector2 centre, float radius, FastColor color)
        => SDLGuard.Execute(() => circleColor(_renderer, (short)centre.X, (short)centre.Y, (short)radius, color.Argb));

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color)
        => SDLGuard.Execute(() => filledCircleColor(_renderer, (short)centre.X, (short)centre.Y, (short)radius, color.Argb));

    public void DrawImage(ImageType image, Vector2 position)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[image]);
        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, _images[image]));

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

    public void DrawImageCentre(ImageType image, float y)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[image]);
        float x = (ScreenWidth - surface.w) / 2;
        DrawImage(image, new(x, y));
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderDrawLineF(_renderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y));
    }

    public void DrawPixel(Vector2 position, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderDrawPointF(_renderer, position.X, position.Y));
    }

    public void DrawPolygon(Vector2[] points, FastColor lineColor)
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

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor)
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

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
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

    public void DrawRectangleCentre(float y, float width, float height, FastColor color)
        => DrawRectangle(new((ScreenWidth - width) / Scale, y), width, height, color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
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

    public void DrawTextCentre(float y, string text, FontType fontType, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[fontType],
            text,
            _sdlColors[color]));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)((ScreenWidth / 2) - (dest.w / 2));
        dest.y = (int)(y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTextLeft(Vector2 position, string text, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[FontType.Small],
            text,
            _sdlColors[color]));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)(position.X / (2 / Scale));
        dest.y = (int)(position.Y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTextRight(Vector2 position, string text, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        nint surfacePtr = SDLGuard.Execute(() => TTF_RenderText_Solid(
            _fonts[FontType.Small],
            text,
            _sdlColors[color]));

        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
        SDL_Rect dest = surface.clip_rect;
        dest.x = (int)((position.X - dest.w) / (2 / Scale));
        dest.y = (int)(position.Y / (2 / Scale));

        nint texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surfacePtr));
        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_FreeSurface(surfacePtr);
        SDL_DestroyTexture(texture);
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
        DrawLine(a, b, color);
        DrawLine(b, c, color);
        DrawLine(c, a, color);
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color)
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

    private SDL_Vertex ConvertVertex(Vector2 point, in FastColor color) => new()
    {
        position = new() { x = point.X, y = point.Y },
        tex_coord = new() { x = 0.0f, y = 0.0f },
        color = _sdlColors[color],
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
            foreach (KeyValuePair<FontType, nint> font in _fonts)
            {
                TTF_CloseFont(font.Value);
            }

            // Images
            foreach (KeyValuePair<ImageType, nint> image in _images)
            {
                SDL_FreeSurface(image.Value);
            }
        }
    }

    private void SetRenderDrawColor(FastColor color)
        => SDLGuard.Execute(() => SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A));
}
