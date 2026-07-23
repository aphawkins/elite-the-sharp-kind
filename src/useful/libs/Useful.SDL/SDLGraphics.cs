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

    // CPU-rasterised, per-pixel depth-tested layer for DrawPolygonFilledDepth
    // / DrawPolygonTexturedDepth: SDL's accelerated 2D renderer has no depth
    // buffer of its own, so without this, depth-tested polygons (a whole
    // track, a rotating ship) draw in submission order and pop/flicker
    // wherever that order doesn't match actual camera distance. Allocated on
    // first ClearDepth() call; composited onto the renderer as one texture
    // blit the next time anything else is drawn (FlushDepthLayer), so it
    // lands after whatever was drawn before it (e.g. a backdrop) and before
    // whatever is drawn after (e.g. the HUD), and inherits whatever clip
    // region is active at that point - the same clip that was active while
    // the depth-tested content was rasterised, since nothing else runs in
    // between.
    //
    // ZBufferRenderer interleaves depth-tested faces with plain 2-point
    // line submissions in the same z-sorted chain (undecorated hull edges
    // draw as lines, not faces) and calls DrawLine directly for those - if
    // DrawLine drew straight to the renderer, a line landing between two
    // faces would need the layer flushed around it, and *every* later face
    // in the same pass would need re-flushing too, repainting that line's
    // pixels with content submitted after it regardless of who's actually
    // nearer. Routing DrawLine into this same CPU layer while a pass is
    // open keeps every draw in one pass in the one buffer, submission order
    // intact, with a single flush at the end - exactly how SoftwareGraphics
    // gets this right, by writing every draw straight into one shared
    // buffer with no batching to reorder.
    private FastBitmap? _depthLayer;
    private float[]? _depthBuffer;
    private bool _depthLayerDirty;
    private bool _depthPassOpen;

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

        FlushDepthLayer();
        EvictStaleTextTextures();

        SetRenderDrawColor(BaseColors.Black.Argb);

        SDLGuard.Execute(() => SDL_RenderClear(NativeRenderer));
    }

    public void ClearDepth()
    {
        if (_isDisposed)
        {
            return;
        }

        _depthLayer ??= new FastBitmap((int)ScreenWidth, (int)ScreenHeight);
        _depthBuffer ??= new float[(int)ScreenWidth * (int)ScreenHeight];

        _depthLayer.Clear(BaseColors.TransparentBlack);
        Array.Clear(_depthBuffer);
        _depthLayerDirty = false;
        _depthPassOpen = true;
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

        FlushDepthLayer();

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

        FlushDepthLayer();

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

        if (_depthPassOpen && _depthLayer != null)
        {
            DrawLineToDepthLayer(lineStart, lineEnd, color);
            _depthLayerDirty = true;
            return;
        }

        FlushDepthLayer();
        SetRenderDrawColor(color);

        SDLGuard.Execute(() => SDL_RenderLine(NativeRenderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y));
    }

    public void DrawPixel(Vector2 position, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        FlushDepthLayer();
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
    {
        if (_isDisposed || points == null || depths == null || depths.Length < points.Length || _depthLayer == null)
        {
            return;
        }

        for (int i = 1; i < points.Length - 1; i++)
        {
            DrawTriangleFilledDepthToLayer(points[0], points[i], points[i + 1], depths[0], depths[i], depths[i + 1], faceColor);
        }

        _depthLayerDirty = true;
    }

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
    {
        if (_isDisposed ||
            points == null ||
            depths == null ||
            textureCoords == null ||
            texture == null ||
            depths.Length < points.Length ||
            textureCoords.Length < points.Length ||
            _depthLayer == null)
        {
            return;
        }

        for (int i = 1; i < points.Length - 1; i++)
        {
            DrawTriangleTexturedDepthToLayer(
                points[0],
                points[i],
                points[i + 1],
                depths[0],
                depths[i],
                depths[i + 1],
                textureCoords[0],
                textureCoords[i],
                textureCoords[i + 1],
                texture);
        }

        _depthLayerDirty = true;
    }

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        FlushDepthLayer();
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

        FlushDepthLayer();
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

        FlushDepthLayer();
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

        FlushDepthLayer();
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

        FlushDepthLayer();
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

        FlushDepthLayer();

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

        FlushDepthLayer();

        SDLGuard.Execute(() => SDL_RenderPresent(NativeRenderer));
    }

    public void SaveScreen(string path)
    {
        Guard.ArgumentNull(path);

        if (_isDisposed)
        {
            return;
        }

        FlushDepthLayer();

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

    // The interpolation parameter of the edge p0-p1 at scanline y, clamped
    // to the edge's endpoints (p0.Y must not be greater than p1.Y). A
    // horizontal or degenerate edge yields 0. Mirrors SoftwareGraphics's
    // EdgeT: the depth layer is a second, independent CPU rasterizer, since
    // this one only ever runs for the small slice of draws SDL's
    // accelerated renderer cannot depth-test itself.
    private static float EdgeT(Vector2 p0, Vector2 p1, float y)
    {
        float dy = p1.Y - p0.Y;
        return dy <= 0 ? 0f : Math.Clamp((y - p0.Y) / dy, 0f, 1f);
    }

    // Sample the texture at a [0,1] coordinate, clamping at the edges.
    private static uint SampleDepthLayerTexture(FastBitmap texture, Vector2 uv)
    {
        int x = Math.Clamp((int)(uv.X * texture.Width), 0, texture.Width - 1);
        int y = Math.Clamp((int)(uv.Y * texture.Height), 0, texture.Height - 1);
        return texture.GetPixel(x, y);
    }

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

    // Composites the accumulated depth layer onto the renderer as one
    // texture blit and clears the pending flag, so the next call is a
    // no-op. Called at the top of every other draw method (and
    // ScreenUpdate/SaveScreen): the first non-depth call after a run of
    // DrawPolygonFilledDepth/DrawPolygonTexturedDepth calls is exactly the
    // point the composited depth content needs to land on the renderer, in
    // between whatever was drawn before it and whatever draws after.
    private void FlushDepthLayer()
    {
        if (!_depthLayerDirty || _depthLayer == null)
        {
            return;
        }

        _depthLayerDirty = false;
        _depthPassOpen = false;

        nint surfacePtr = SDLGuard.Execute(() => (nint)SDL_CreateSurfaceFrom(
            _depthLayer.Width,
            _depthLayer.Height,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            _depthLayer.BitmapHandle,
            _depthLayer.Width * 4));

        nint texturePtr = SDLGuard.Execute(() => (nint)SDL_CreateTextureFromSurface(NativeRenderer, (SDL_Surface*)surfacePtr));
        SDL_DestroySurface((SDL_Surface*)surfacePtr);

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = 0, y = 0, w = ScreenWidth, h = ScreenHeight };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)texturePtr, null, &dest);
        });

        SDL_DestroyTexture((SDL_Texture*)texturePtr);

        // Closing the pass (above) means DrawLine stops routing here once
        // this flush runs, so in the normal case there is nothing left to
        // draw into this layer until the next ClearDepth(). Clearing it
        // anyway is cheap insurance: if something unexpected flushes twice
        // within one pass, the second flush won't repaint the first flush's
        // (already on-screen) pixels over whatever was drawn in between.
        _depthLayer.Clear(BaseColors.TransparentBlack);
    }

    // Bresenham line into the CPU depth layer, matching SoftwareGraphics's
    // DrawLineInt. Writes pixels directly with no depth test: ZBufferRenderer's
    // 2-point line submissions were never depth-tested even in
    // SoftwareGraphics (a plain "draw this over whatever came before" edge),
    // so this preserves that behaviour rather than adding a test that never
    // existed for lines.
    private void DrawLineToDepthLayer(Vector2 lineStart, Vector2 lineEnd, in FastColor color)
    {
        int x0 = (int)MathF.Floor(lineStart.X);
        int y0 = (int)MathF.Floor(lineStart.Y);
        int x1 = (int)MathF.Floor(lineEnd.X);
        int y1 = (int)MathF.Floor(lineEnd.Y);

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        int width = (int)ScreenWidth;
        int height = (int)ScreenHeight;

        while (true)
        {
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
            {
                _depthLayer!.SetPixel(x0, y0, color);
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

    // Depth-tested triangle fill into the CPU depth layer: inverse depth
    // (1/z) is interpolated linearly in screen space (perspective-correct
    // for depth) and each pixel only draws when it passes the depth test.
    // Mirrors SoftwareGraphics.DrawTriangleFilledDepth.
    private void DrawTriangleFilledDepthToLayer(Vector2 a, Vector2 b, Vector2 c, float za, float zb, float zc, in FastColor color)
    {
        if (za <= 0 || zb <= 0 || zc <= 0)
        {
            return;
        }

        if (b.Y < a.Y)
        {
            (a, b, za, zb) = (b, a, zb, za);
        }

        if (c.Y < a.Y)
        {
            (a, c, za, zc) = (c, a, zc, za);
        }

        if (c.Y < b.Y)
        {
            (b, c, zb, zc) = (c, b, zc, zb);
        }

        float ia = 1f / za;
        float ib = 1f / zb;
        float ic = 1f / zc;

        int firstY = Math.Max((int)MathF.Ceiling(a.Y), 0);
        int lastY = Math.Min((int)MathF.Floor(c.Y), (int)ScreenHeight - 1);

        for (int y = firstY; y <= lastY; y++)
        {
            float t0 = EdgeT(a, c, y);
            float x0 = a.X + ((c.X - a.X) * t0);
            float i0 = ia + ((ic - ia) * t0);

            float x1;
            float i1;
            if (y < b.Y)
            {
                float t1 = EdgeT(a, b, y);
                x1 = a.X + ((b.X - a.X) * t1);
                i1 = ia + ((ib - ia) * t1);
            }
            else
            {
                float t1 = EdgeT(b, c, y);
                x1 = b.X + ((c.X - b.X) * t1);
                i1 = ib + ((ic - ib) * t1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (i0, i1) = (i1, i0);
            }

            int start = Math.Max((int)MathF.Floor(x0), 0);
            int end = Math.Min((int)MathF.Floor(x1), (int)ScreenWidth - 1);
            float span = x1 - x0;

            for (int x = start; x <= end; x++)
            {
                float t = span <= 0 ? 0f : Math.Clamp((x - x0) / span, 0f, 1f);
                if (DepthTestLayer(x, y, i0 + ((i1 - i0) * t)))
                {
                    _depthLayer!.SetPixel(x, y, color);
                }
            }
        }
    }

    // Depth-tested, perspective-correct textured triangle fill into the CPU
    // depth layer. Mirrors SoftwareGraphics.DrawTriangleTexturedDepth.
    private void DrawTriangleTexturedDepthToLayer(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        float za,
        float zb,
        float zc,
        Vector2 ta,
        Vector2 tb,
        Vector2 tc,
        FastBitmap texture)
    {
        if (za <= 0 || zb <= 0 || zc <= 0)
        {
            return;
        }

        if (b.Y < a.Y)
        {
            (a, b, za, zb, ta, tb) = (b, a, zb, za, tb, ta);
        }

        if (c.Y < a.Y)
        {
            (a, c, za, zc, ta, tc) = (c, a, zc, za, tc, ta);
        }

        if (c.Y < b.Y)
        {
            (b, c, zb, zc, tb, tc) = (c, b, zc, zb, tc, tb);
        }

        float ia = 1f / za;
        float ib = 1f / zb;
        float ic = 1f / zc;
        Vector2 ua = ta * ia;
        Vector2 ub = tb * ib;
        Vector2 uc = tc * ic;

        int firstY = Math.Max((int)MathF.Ceiling(a.Y), 0);
        int lastY = Math.Min((int)MathF.Floor(c.Y), (int)ScreenHeight - 1);

        for (int y = firstY; y <= lastY; y++)
        {
            float t0 = EdgeT(a, c, y);
            float x0 = a.X + ((c.X - a.X) * t0);
            float i0 = ia + ((ic - ia) * t0);
            Vector2 uv0 = Vector2.Lerp(ua, uc, t0);

            float x1;
            float i1;
            Vector2 uv1;
            if (y < b.Y)
            {
                float t1 = EdgeT(a, b, y);
                x1 = a.X + ((b.X - a.X) * t1);
                i1 = ia + ((ib - ia) * t1);
                uv1 = Vector2.Lerp(ua, ub, t1);
            }
            else
            {
                float t1 = EdgeT(b, c, y);
                x1 = b.X + ((c.X - b.X) * t1);
                i1 = ib + ((ic - ib) * t1);
                uv1 = Vector2.Lerp(ub, uc, t1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (i0, i1) = (i1, i0);
                (uv0, uv1) = (uv1, uv0);
            }

            int start = Math.Max((int)MathF.Floor(x0), 0);
            int end = Math.Min((int)MathF.Floor(x1), (int)ScreenWidth - 1);
            float span = x1 - x0;

            for (int x = start; x <= end; x++)
            {
                float t = span <= 0 ? 0f : Math.Clamp((x - x0) / span, 0f, 1f);
                float inverseDepth = i0 + ((i1 - i0) * t);
                if (DepthTestLayer(x, y, inverseDepth))
                {
                    Vector2 uv = Vector2.Lerp(uv0, uv1, t) / inverseDepth;
                    _depthLayer!.SetPixel(x, y, SampleDepthLayerTexture(texture, uv));
                }
            }
        }
    }

    // Test-and-set a depth-layer pixel's inverse depth: the draw passes when
    // at least as near as what is already there, so later draws win ties
    // (matching SoftwareGraphics.DepthTest and the original Direct3D
    // LESSEQUAL depth test).
    private bool DepthTestLayer(int x, int y, float inverseDepth)
    {
        if (x < 0 || y < 0 || x >= (int)ScreenWidth || y >= (int)ScreenHeight)
        {
            return false;
        }

        int index = (y * (int)ScreenWidth) + x;
        if (inverseDepth < _depthBuffer![index])
        {
            return false;
        }

        _depthBuffer[index] = inverseDepth;
        return true;
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (disposing)
            {
                // dispose managed state (managed objects)
                _depthLayer?.Dispose();
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
