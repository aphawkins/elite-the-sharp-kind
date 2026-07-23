// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Numerics;
using SDL;
using Useful.Assets;
using Useful.Graphics;
using static SDL.SDL3;
using static SDL.SDL3_ttf;

namespace Useful.SDL;

#pragma warning disable S6640 // Avoid using this unsafe code block - required by ppy.SDL3-CS/ppy.SDL3_ttf-CS's raw pointer API
public sealed unsafe class SDLGraphics : IGraphics, IDisposable
#pragma warning restore S6640
{
    private const int CircleSegments = 32;

    private readonly SDLRenderer _renderer;
    private readonly Dictionary<(string FontType, string Text, uint Color), TextTextureEntry> _textTextures = [];
    private Dictionary<string, nint> _fonts = [];
    private Dictionary<string, nint> _images = [];
    private Dictionary<string, nint> _imageTextures = [];
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

    private SDL_Renderer* NativeRenderer => (SDL_Renderer*)(nint)_renderer;

    public static SDLGraphics Create(SDLRenderer renderer, float screenWidth, float screenHeight, IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        SDLGraphics graphics = new(renderer, screenWidth, screenHeight)
        {
            _images = assetLocator.ImagePaths.ToDictionary(
                x => x.Key,
                x => SDLGuard.Execute(() => (nint)SDL_LoadBMP(x.Value))),

            _fonts = assetLocator.FontTrueTypePaths.ToDictionary(
                x => x.Key,
                x => LoadFont(x.Key, x.Value)),
        };

        // Textures are created once here rather than per-draw: creating one
        // is a synchronous GPU upload, and images are static assets that
        // never change after load.
        graphics._imageTextures = graphics._images.ToDictionary(
            x => x.Key,
            x => SDLGuard.Execute(() => (nint)SDL_CreateTextureFromSurface(graphics.NativeRenderer, (SDL_Surface*)x.Value)));

        return graphics;
    }

    public void Clear()
    {
        if (_isDisposed)
        {
            return;
        }

        EvictStaleTextTextures();

        SetRenderDrawColor(BaseColors.Black.Argb);

        SDLGuard.Execute(() => SDL_RenderClear(NativeRenderer));
    }

    public void ClearDepth()
    {
        // The SDL renderer has no depth buffer; the software rasterizer is
        // the primary rendering path for depth-tested polygons.
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

        Vector2 previous = centre + new Vector2(radius, 0);

        for (int i = 1; i <= CircleSegments; i++)
        {
            float angle = i * MathF.Tau / CircleSegments;
            Vector2 next = centre + new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
            DrawLine(previous, next, color);
            previous = next;
        }
    }

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        // There is no equivalent filled-circle primitive without the removed
        // gfx dependency - build the same shape as DrawPolygonFilled does:
        // a triangle fan sharing the centre.
        Vector2 previous = centre + new Vector2(radius, 0);

        for (int i = 1; i <= CircleSegments; i++)
        {
            float angle = i * MathF.Tau / CircleSegments;
            Vector2 next = centre + new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
            DrawTriangleFilled(centre, previous, next, color);
            previous = next;
        }
    }

    public void DrawImage(string imageType, Vector2 position)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface* imageSurface = (SDL_Surface*)_images[imageType];
        nint texturePtr = _imageTextures[imageType];

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new()
            {
                x = position.X,
                y = position.Y,
                w = imageSurface->w,
                h = imageSurface->h,
            };

            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)texturePtr, null, &dest);
        });
    }

    public void DrawImageCentre(string imageType, float y)
    {
        if (_isDisposed)
        {
            return;
        }

        SDL_Surface* imageSurface = (SDL_Surface*)_images[imageType];
        float x = (ScreenWidth - imageSurface->w) / 2;
        DrawImage(imageType, new(x, y));
    }

    public void DrawImagePart(string imageType, Vector2 position, Vector2 size, Vector2 sourcePosition, Vector2 sourceSize)
    {
        if (_isDisposed)
        {
            return;
        }

        nint texturePtr = _imageTextures[imageType];

        SDL_FlipMode flip = sourceSize.X < 0 ? SDL_FlipMode.SDL_FLIP_HORIZONTAL : SDL_FlipMode.SDL_FLIP_NONE;

        SDLGuard.Execute(() =>
        {
            SDL_FRect source = new()
            {
                x = sourcePosition.X,
                y = sourcePosition.Y,
                w = MathF.Abs(sourceSize.X),
                h = sourceSize.Y,
            };

            SDL_FRect dest = new()
            {
                x = position.X,
                y = position.Y,
                w = size.X,
                h = size.Y,
            };

            return SDL_RenderTextureRotated(NativeRenderer, (SDL_Texture*)texturePtr, &source, &dest, 0, null, flip);
        });
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderLine(NativeRenderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y));
    }

    public void DrawPixel(Vector2 position, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderPoint(NativeRenderer, position.X, position.Y));
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

    public void DrawPolygonFilledDepth(Vector2[] points, float[] depths, FastColor faceColor)
        => DrawPolygonFilled(points, faceColor); // no depth buffer - drawn unsorted

    public void DrawPolygonTextured(Vector2[] points, Vector2[] textureCoords, FastBitmap texture)
    {
        if (points == null || textureCoords == null || texture == null || textureCoords.Length < points.Length)
        {
            return;
        }

        // The SDL renderer cannot sample a FastBitmap directly, so
        // approximate with a flat fill of the texture colour at the
        // polygon's average texture coordinate. The software rasterizer is
        // the primary rendering path for textured polygons.
        Vector2 averageUv = Vector2.Zero;
        for (int i = 0; i < points.Length; i++)
        {
            averageUv += textureCoords[i];
        }

        averageUv /= points.Length;

        int x = Math.Clamp((int)(averageUv.X * texture.Width), 0, texture.Width - 1);
        int y = Math.Clamp((int)(averageUv.Y * texture.Height), 0, texture.Height - 1);
        DrawPolygonFilled(points, texture.GetPixel(x, y));
    }

    public void DrawPolygonTexturedDepth(Vector2[] points, float[] depths, Vector2[] textureCoords, FastBitmap texture)
        => DrawPolygonTextured(points, textureCoords, texture); // no depth buffer - drawn unsorted

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        SetRenderDrawColor(color);

        float x = position.X / (2 / Scale);
        float y = position.Y / (2 / Scale);

        SDLGuard.Execute(() =>
        {
            SDL_FRect rectangle = new()
            {
                x = x,
                y = y,
                w = width + 1,
                h = height + 1,
            };

            return SDL_RenderRect(NativeRenderer, &rectangle);
        });
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

        float x = position.X / (2 / Scale);
        float y = position.Y / (2 / Scale);

        SDLGuard.Execute(() =>
        {
            SDL_FRect rectangle = new()
            {
                x = x,
                y = y,
                w = width + 1,
                h = height + 1,
            };

            return SDL_RenderFillRect(NativeRenderer, &rectangle);
        });
    }

    public void DrawTextCentre(float y, string text, string fontType, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        TextTextureEntry entry = GetOrCreateTextTexture(fontType, text, color);
        float destX = (ScreenWidth / 2) - (entry.Width / 2);
        float destY = y / (2 / Scale);

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = destX, y = destY, w = entry.Width, h = entry.Height };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)entry.Texture, null, &dest);
        });
    }

    public void DrawTextLeft(Vector2 position, string text, string fontType, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        TextTextureEntry entry = GetOrCreateTextTexture(fontType, text, color);
        float destX = position.X / (2 / Scale);
        float destY = position.Y / (2 / Scale);

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = destX, y = destY, w = entry.Width, h = entry.Height };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)entry.Texture, null, &dest);
        });
    }

    public void DrawTextRight(Vector2 position, string text, string fontType, FastColor color)
    {
        if (_isDisposed || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        TextTextureEntry entry = GetOrCreateTextTexture(fontType, text, color);
        float destX = (position.X - entry.Width) / (2 / Scale);
        float destY = position.Y / (2 / Scale);

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = destX, y = destY, w = entry.Width, h = entry.Height };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)entry.Texture, null, &dest);
        });
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

        SDLGuard.Execute(() =>
        {
            SDL_Vertex* vertices = stackalloc SDL_Vertex[3];
            vertices[0] = ConvertVertex(a, color);
            vertices[1] = ConvertVertex(b, color);
            vertices[2] = ConvertVertex(c, color);

            return SDL_RenderGeometry(NativeRenderer, null, vertices, 3, null, 0);
        });
    }

    public void ScreenUpdate()
    {
        if (_isDisposed)
        {
            return;
        }

        SDLGuard.Execute(() => SDL_RenderPresent(NativeRenderer));
    }

    public void SaveScreen(string path)
    {
        Guard.ArgumentNull(path);

        if (_isDisposed)
        {
            return;
        }

        int width = (int)ScreenWidth;
        int height = (int)ScreenHeight;

        nint rawSurfacePtr = SDLGuard.Execute(() =>
        {
            SDL_Rect rect = new() { x = 0, y = 0, w = width, h = height };
            return (nint)SDL_RenderReadPixels(NativeRenderer, &rect);
        });

        // SDL_RenderReadPixels does not guarantee a particular pixel format,
        // so convert explicitly to match FastBitmap's tightly packed ARGB8888 layout.
        nint convertedSurfacePtr = SDLGuard.Execute(
            () => (nint)SDL_ConvertSurface((SDL_Surface*)rawSurfacePtr, SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888));
        SDL_DestroySurface((SDL_Surface*)rawSurfacePtr);

        SDL_Surface* converted = (SDL_Surface*)convertedSurfacePtr;
        Debug.Assert(converted->pitch == width * 4, "Converted surface is not tightly packed.");

        using FastBitmap bitmap = new(width, height);
        long byteCount = (long)width * height * 4;
        Buffer.MemoryCopy((void*)converted->pixels, (void*)bitmap.BitmapHandle, byteCount, byteCount);
        SDL_DestroySurface(converted);

        BitmapWriter.Write(bitmap, path);
    }

    public void SetClipRegion(Vector2 position, float width, float height)
    {
        if (_isDisposed)
        {
            return;
        }

        int x = (int)(position.X / (2 / Scale));
        int y = (int)(position.Y / (2 / Scale));
        int w = (int)width;
        int h = (int)height;

        SDLGuard.Execute(() =>
        {
            SDL_Rect rectangle = new() { x = x, y = y, w = w, h = h };
            return SDL_SetRenderClipRect(NativeRenderer, &rectangle);
        });
    }

    private static nint LoadFont(string fontType, string fontPath)
    {
        Debug.Assert(File.Exists(fontPath), $"Font file '{fontPath}' does not exist.");
        Debug.Assert(
            string.Equals(Path.GetExtension(fontPath), ".ttf", StringComparison.OrdinalIgnoreCase),
            $"Font file '{fontPath}' must be a TTF file.");

        return fontType switch
        {
            "Small" => SDLGuard.Execute(() => (nint)TTF_OpenFont(fontPath, 12)),
            "Large" => SDLGuard.Execute(() => (nint)TTF_OpenFont(fontPath, 18)),
            _ => throw new ArgumentOutOfRangeException(nameof(fontType), fontType, null),
        };
    }

    private static SDL_Vertex ConvertVertex(Vector2 point, uint color) => new()
    {
        position = new() { x = point.X, y = point.Y },
        tex_coord = new() { x = 0.0f, y = 0.0f },
        color = ToSDLFColor(color),
    };

    // ARGB, matching FastColor's decoding - not RGBA.
    private static SDL_Color ToSDLColor(in FastColor color) => new()
    {
        r = color.R,
        g = color.G,
        b = color.B,
        a = color.A,
    };

    // SDL_Vertex colours are normalised floats (0..1), not bytes.
    private static SDL_FColor ToSDLFColor(in FastColor color) => new()
    {
        r = color.R / 255f,
        g = color.G / 255f,
        b = color.B / 255f,
        a = color.A / 255f,
    };

    // Rendering a glyph texture is a CPU render plus a synchronous GPU
    // upload, so cache by (font, text, colour) instead of doing it on every
    // draw call. Entries not reused since the last frame's Clear() are
    // evicted there, keeping the cache bounded to what's currently on screen.
    private TextTextureEntry GetOrCreateTextTexture(string fontType, string text, in FastColor color)
    {
        (string FontType, string Text, uint Color) key = (fontType, text, color.Argb);

        if (_textTextures.TryGetValue(key, out TextTextureEntry? cached))
        {
            cached.UsedThisFrame = true;
            return cached;
        }

        SDL_Color colour = ToSDLColor(color);
        nint surfacePtr = SDLGuard.Execute(() => (nint)TTF_RenderText_Solid((TTF_Font*)_fonts[fontType], text, 0, colour));
        SDL_Surface* surface = (SDL_Surface*)surfacePtr;

        TextTextureEntry entry = new()
        {
            Texture = SDLGuard.Execute(() => (nint)SDL_CreateTextureFromSurface(NativeRenderer, surface)),
            Width = surface->w,
            Height = surface->h,
            UsedThisFrame = true,
        };

        SDL_DestroySurface(surface);

        _textTextures[key] = entry;
        return entry;
    }

    private void EvictStaleTextTextures()
    {
        List<(string, string, uint)> stale = [];

        foreach (KeyValuePair<(string, string, uint), TextTextureEntry> pair in _textTextures)
        {
            if (!pair.Value.UsedThisFrame)
            {
                SDL_DestroyTexture((SDL_Texture*)pair.Value.Texture);
                stale.Add(pair.Key);
                continue;
            }

            pair.Value.UsedThisFrame = false;
        }

        foreach ((string, string, uint) key in stale)
        {
            _textTextures.Remove(key);
        }
    }

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
            foreach (KeyValuePair<string, nint> font in _fonts)
            {
                TTF_CloseFont((TTF_Font*)font.Value);
            }

            // Images
            foreach (KeyValuePair<string, nint> image in _images)
            {
                SDL_DestroySurface((SDL_Surface*)image.Value);
            }

            foreach (KeyValuePair<string, nint> texture in _imageTextures)
            {
                SDL_DestroyTexture((SDL_Texture*)texture.Value);
            }

            foreach (KeyValuePair<(string, string, uint), TextTextureEntry> entry in _textTextures)
            {
                SDL_DestroyTexture((SDL_Texture*)entry.Value.Texture);
            }
        }
    }

    private void SetRenderDrawColor(uint color)
    {
        FastColor fastColor = new(color);
        SDLGuard.Execute(() => SDL_SetRenderDrawColor(NativeRenderer, fastColor.R, fastColor.G, fastColor.B, fastColor.A));
    }

    private sealed class TextTextureEntry
    {
        public required nint Texture { get; init; }

        public required float Width { get; init; }

        public required float Height { get; init; }

        public bool UsedThisFrame { get; set; }
    }
}
