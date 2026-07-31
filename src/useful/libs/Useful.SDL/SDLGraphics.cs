// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Diagnostics;
using System.Numerics;
using Microsoft.Extensions.Logging;
using SDL;
using Useful.Assets;
using Useful.Graphics;
using static SDL.SDL3;
using static SDL.SDL3_ttf;

namespace Useful.SDL;

public sealed unsafe class SDLGraphics : IGraphics, IDisposable
{
    private const int CircleSegments = 32;

    private readonly SDLRenderer _renderer;
    private readonly Dictionary<(string FontType, string Text, uint Color), TextTextureEntry> _textTextures = [];
    private Dictionary<string, nint> _fonts = [];
    private Dictionary<string, FastBitmap> _images = [];
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

    // The GPU-side counterpart of _depthLayer, created alongside it and
    // reused for every flush: compositing used to build a surface and a
    // texture from the layer and destroy both again on each flush, i.e. a
    // GPU allocation and a synchronous upload for every frame that draws
    // any depth-tested geometry. Streaming + SDL_UpdateTexture re-uploads
    // into this one instead. Its blend mode is set explicitly because,
    // unlike SDL_CreateTextureFromSurface, SDL_CreateTexture does not infer
    // it from the pixels' alpha channel - and the layer is transparent
    // everywhere nothing was rasterised.
    private nint _depthTexture;

    private float[]? _depthBuffer;

    // The surface currently occupying each pixel, 0 = none; see
    // SoftwareGraphics._surfaceIds.
    private int[]? _surfaceIds;
    private bool _depthLayerDirty;
    private bool _depthPassOpen;

    // All drawing targets this persistent off-screen texture rather than
    // the window's swap-chain backbuffer directly. The game only composes
    // a new frame once per game tick but presents at the display's own
    // rate, which is usually higher and not a whole multiple of the tick
    // rate; a multi-buffered accelerated swap chain cycles through 2+
    // buffers on each present; presenting without redrawing means most of
    // those buffers still hold whatever was drawn into them ticks ago,
    // which reads as flicker (and, mid-window-drag, a frozen "ghost" of
    // one stale buffer). ScreenUpdate() blits this texture onto the
    // backbuffer fresh on every present instead, so every buffer gets
    // current content regardless of how many times it's presented between
    // ticks - the same trick SoftwareAbstraction already gets for free by
    // re-uploading its CPU bitmap every present.
    private nint _frameTarget;

    private SDLGraphics(SDLRenderer renderer, float screenWidth, float screenHeight)
    {
        ArgumentNullException.ThrowIfNull(renderer);

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

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    private SDL_Renderer* NativeRenderer => (SDL_Renderer*)(nint)_renderer;

    public static SDLGraphics Create(SDLRenderer renderer, float screenWidth, float screenHeight, IAssetLocator assetLocator)
        => Create(renderer, screenWidth, screenHeight, assetLocator, null);

    public static SDLGraphics Create(
        SDLRenderer renderer,
        float screenWidth,
        float screenHeight,
        IAssetLocator assetLocator,
        ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(assetLocator);

        // Images come from the shared managed decoder rather than SDL_LoadBMP,
        // so this backend sees the same pixels as the software one and the
        // tier's colour budget is checked whichever backend is running.
        AssetSet assets = AssetSet.Load(assetLocator, logger);

        SDLGraphics graphics = new(renderer, screenWidth, screenHeight)
        {
            _images = assets.Images,

            _fonts = assetLocator.FontTrueTypes.ToDictionary(
                x => x.Key,
                x => LoadFont(x.Value)),
        };

        // Textures are created once here rather than per-draw: creating one
        // is a synchronous GPU upload, and images are static assets that
        // never change after load.
        graphics._imageTextures = graphics._images.ToDictionary(
            x => x.Key,
            x => CreateImageTexture(graphics.NativeRenderer, x.Value));

        graphics._frameTarget = SDLGuard.Execute(() => (nint)SDL_CreateTexture(
            graphics.NativeRenderer,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
            (int)screenWidth,
            (int)screenHeight));

        // Magnifying must duplicate pixels rather than blend them: SDL's
        // default is linear, which would show a filtered image instead of
        // the tier's own whenever the window is larger than the frame.
        SDLGuard.Execute(() => SDL_SetTextureScaleMode(
            (SDL_Texture*)graphics._frameTarget,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST));

        // All drawing targets _frameTarget from here on (see its field
        // comment); ScreenUpdate() briefly switches back to the window to
        // blit it, then restores this.
        SDLGuard.Execute(() => SDL_SetRenderTarget(graphics.NativeRenderer, (SDL_Texture*)graphics._frameTarget));

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

        SetRenderDrawColor(BaseColors.Black);

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
        _surfaceIds ??= new int[(int)ScreenWidth * (int)ScreenHeight];

        if (_depthTexture == nint.Zero)
        {
            _depthTexture = SDLGuard.Execute(() => (nint)SDL_CreateTexture(
                NativeRenderer,
                SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
                SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                _depthLayer.Width,
                _depthLayer.Height));

            SDLGuard.Execute(() => SDL_SetTextureBlendMode((SDL_Texture*)_depthTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND));
        }

        _depthLayer.Clear(BaseColors.TransparentBlack);
        Array.Clear(_depthBuffer);
        Array.Clear(_surfaceIds);
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

        FastBitmap image = _images[imageType];
        nint texturePtr = _imageTextures[imageType];

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new()
            {
                x = position.X,
                y = position.Y,
                w = image.Width,
                h = image.Height,
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

        float x = (ScreenWidth - _images[imageType].Width) / 2;
        DrawImage(imageType, new(x, y));
    }

    public Vector2 ImageSize(string imageType)
    {
        FastBitmap image = _images[imageType];
        return new(image.Width, image.Height);
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

    public void DrawLineDepth(Vector2 lineStart, Vector2 lineEnd, float depthStart, float depthEnd, FastColor color, int surfaceId)
    {
        if (_isDisposed || _depthLayer == null)
        {
            return;
        }

        DrawLineDepthToLayer(lineStart, lineEnd, depthStart, depthEnd, color, surfaceId);
        _depthLayerDirty = true;
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
        => FillPolygonDepth(points, depths, faceColor, writeColor: true, surfaceId: 0);

    public void FillDepth(Vector2[] points, float[] depths, int surfaceId)
        => FillPolygonDepth(points, depths, BaseColors.Black, writeColor: false, surfaceId);

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

        float x = position.X;
        float y = position.Y;

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
        => DrawRectangle(new((ScreenWidth - width) / 2, y), width, height, color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
    {
        if (_isDisposed)
        {
            return;
        }

        FlushDepthLayer();
        SetRenderDrawColor(color);

        float x = position.X;
        float y = position.Y;

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
        float destY = y;

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
        float destX = position.X;
        float destY = position.Y;

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
        float destX = position.X - entry.Width;
        float destY = position.Y;

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

        // Blit the persistent frame texture onto whichever swap-chain
        // buffer is current, every time, rather than presenting whatever
        // was left in it - see _frameTarget's field comment.
        SDLGuard.Execute(() => SDL_SetRenderTarget(NativeRenderer, null));
        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = 0, y = 0, w = ScreenWidth, h = ScreenHeight };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)_frameTarget, null, &dest);
        });
        SDLGuard.Execute(() => SDL_RenderPresent(NativeRenderer));
        SDLGuard.Execute(() => SDL_SetRenderTarget(NativeRenderer, (SDL_Texture*)_frameTarget));
    }

    public void SaveScreen(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

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

        int x = (int)position.X;
        int y = (int)position.Y;
        int w = (int)width;
        int h = (int)height;

        SDLGuard.Execute(() =>
        {
            SDL_Rect rectangle = new() { x = x, y = y, w = w, h = h };
            return SDL_SetRenderClipRect(NativeRenderer, &rectangle);
        });
    }

    // Uploads a decoded bitmap as a static texture. The blend mode has to be
    // set explicitly: unlike SDL_CreateTextureFromSurface, SDL_CreateTexture
    // does not infer it from the pixels' alpha channel, and the HUD sprites
    // are transparent everywhere they aren't drawn.
    private static nint CreateImageTexture(SDL_Renderer* renderer, FastBitmap image)
    {
        nint texture = SDLGuard.Execute(() => (nint)SDL_CreateTexture(
            renderer,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC,
            image.Width,
            image.Height));

        SDLGuard.Execute(() => SDL_UpdateTexture((SDL_Texture*)texture, null, image.BitmapHandle, image.Width * 4));
        SDLGuard.Execute(() => SDL_SetTextureBlendMode((SDL_Texture*)texture, SDL_BlendMode.SDL_BLENDMODE_BLEND));

        return texture;
    }

    private static nint LoadFont(TrueTypeFontAsset font)
    {
        Debug.Assert(File.Exists(font.Path), $"Font file '{font.Path}' does not exist.");
        Debug.Assert(
            string.Equals(Path.GetExtension(font.Path), ".ttf", StringComparison.OrdinalIgnoreCase),
            $"Font file '{font.Path}' must be a TTF file.");
        Debug.Assert(font.PointSize > 0, $"Font '{font.Path}' must have a positive point size.");

        return SDLGuard.Execute(() => (nint)TTF_OpenFont(font.Path, font.PointSize));
    }

    private static SDL_Vertex ConvertVertex(Vector2 point, in FastColor color) => new()
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
    private static FastColor SampleDepthLayerTexture(FastBitmap texture, Vector2 uv)
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
        if (!_depthLayerDirty || _depthLayer == null || _depthTexture == nint.Zero)
        {
            return;
        }

        _depthLayerDirty = false;
        _depthPassOpen = false;

        SDLGuard.Execute(() => SDL_UpdateTexture(
            (SDL_Texture*)_depthTexture,
            null,
            _depthLayer.BitmapHandle,
            _depthLayer.Width * 4));

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new() { x = 0, y = 0, w = ScreenWidth, h = ScreenHeight };
            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)_depthTexture, null, &dest);
        });

        // Closing the pass (above) means DrawLine stops routing here once
        // this flush runs, so in the normal case there is nothing left to
        // draw into this layer until the next ClearDepth(). Clearing it
        // anyway is cheap insurance: if something unexpected flushes twice
        // within one pass, the second flush won't repaint the first flush's
        // (already on-screen) pixels over whatever was drawn in between.
        _depthLayer.Clear(BaseColors.TransparentBlack);
    }

    // Bresenham line into the CPU depth layer, matching SoftwareGraphics's
    // DrawLineInt. Writes pixels directly with no depth test - the untested
    // edge case, for callers that have no depth for the line. Callers that
    // do go through DrawLineDepthToLayer instead.
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

    // Depth-tested Bresenham line into the CPU depth layer: inverse depth
    // (1/z) is interpolated along the walk by its fraction of the major
    // axis. Mirrors SoftwareGraphics.DrawLineIntDepth.
    private void DrawLineDepthToLayer(
        Vector2 lineStart,
        Vector2 lineEnd,
        float depthStart,
        float depthEnd,
        in FastColor color,
        int surfaceId)
    {
        int x0 = (int)MathF.Floor(lineStart.X);
        int y0 = (int)MathF.Floor(lineStart.Y);
        int x1 = (int)MathF.Floor(lineEnd.X);
        int y1 = (int)MathF.Floor(lineEnd.Y);

        float inverseStart = 1f / depthStart;
        float inverseEnd = 1f / depthEnd;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        int steps = Math.Max(dx, dy);

        for (int step = 0; step <= steps; step++)
        {
            float t = steps == 0 ? 0f : (float)step / steps;
            PlotDepthTestedLayerPixel(x0, y0, inverseStart + ((inverseEnd - inverseStart) * t), color, surfaceId);

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

    private void PlotDepthTestedLayerPixel(int x, int y, float inverseDepth, in FastColor color, int surfaceId)
    {
        if (x < 0 || x >= (int)ScreenWidth || y < 0 || y >= (int)ScreenHeight)
        {
            return;
        }

        if (DepthTestLayer(x, y, inverseDepth, surfaceId))
        {
            _depthLayer!.SetPixel(x, y, color);
        }
    }

    private void FillPolygonDepth(Vector2[] points, float[] depths, in FastColor faceColor, bool writeColor, int surfaceId)
    {
        if (_isDisposed || points == null || depths == null || depths.Length < points.Length || _depthLayer == null)
        {
            return;
        }

        for (int i = 1; i < points.Length - 1; i++)
        {
            DrawTriangleFilledDepthToLayer(
                points[0],
                points[i],
                points[i + 1],
                depths[0],
                depths[i],
                depths[i + 1],
                faceColor,
                writeColor,
                surfaceId);
        }

        _depthLayerDirty = true;
    }

    // Depth-tested triangle fill into the CPU depth layer: inverse depth
    // (1/z) is interpolated linearly in screen space (perspective-correct
    // for depth) and each pixel only draws when it passes the depth test.
    // writeColor false runs the depth test and its writes but draws nothing,
    // which is how a hidden-line pass primes the buffer.
    // Mirrors SoftwareGraphics.DrawTriangleFilledDepth.
    private void DrawTriangleFilledDepthToLayer(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        float za,
        float zb,
        float zc,
        in FastColor color,
        bool writeColor,
        int surfaceId)
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

            DrawSpanFilledDepthToLayer(y, x0, x1, i0, i1, color, writeColor, surfaceId);
        }
    }

    // Draw one depth-tested scanline of a flat-shaded triangle into the depth
    // layer, interpolating inverse depth from i0 at x0 to i1 at x1.
    private void DrawSpanFilledDepthToLayer(
        int y,
        float x0,
        float x1,
        float i0,
        float i1,
        in FastColor color,
        bool writeColor,
        int surfaceId)
    {
        int start = Math.Max((int)MathF.Floor(x0), 0);
        int end = Math.Min((int)MathF.Floor(x1), (int)ScreenWidth - 1);
        float span = x1 - x0;

        for (int x = start; x <= end; x++)
        {
            float t = span <= 0 ? 0f : Math.Clamp((x - x0) / span, 0f, 1f);
            if (DepthTestLayer(x, y, i0 + ((i1 - i0) * t), surfaceId) && writeColor)
            {
                _depthLayer!.SetPixel(x, y, color);
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

            DrawSpanTexturedDepthToLayer(y, x0, x1, i0, i1, uv0, uv1, texture);
        }
    }

    // Draw one depth-tested scanline of a textured triangle into the depth
    // layer. The texture coordinates arrive already divided by depth and are
    // recovered per pixel.
    private void DrawSpanTexturedDepthToLayer(
        int y,
        float x0,
        float x1,
        float i0,
        float i1,
        Vector2 uv0,
        Vector2 uv1,
        FastBitmap texture)
    {
        int start = Math.Max((int)MathF.Floor(x0), 0);
        int end = Math.Min((int)MathF.Floor(x1), (int)ScreenWidth - 1);
        float span = x1 - x0;

        for (int x = start; x <= end; x++)
        {
            float t = span <= 0 ? 0f : Math.Clamp((x - x0) / span, 0f, 1f);
            float inverseDepth = i0 + ((i1 - i0) * t);
            if (DepthTestLayer(x, y, inverseDepth, surfaceId: 0))
            {
                Vector2 uv = Vector2.Lerp(uv0, uv1, t) / inverseDepth;
                _depthLayer!.SetPixel(x, y, SampleDepthLayerTexture(texture, uv));
            }
        }
    }

    // Test-and-set a depth-layer pixel's inverse depth: the draw passes when
    // at least as near as what is already there, so later draws win ties
    // (matching SoftwareGraphics.DepthTest and the original Direct3D
    // LESSEQUAL depth test).
    private bool DepthTestLayer(int x, int y, float inverseDepth, int surfaceId)
    {
        if (x < 0 || y < 0 || x >= (int)ScreenWidth || y >= (int)ScreenHeight)
        {
            return false;
        }

        int index = (y * (int)ScreenWidth) + x;
        if (inverseDepth < _depthBuffer![index])
        {
            return surfaceId != 0 && _surfaceIds![index] == surfaceId;
        }

        _depthBuffer[index] = inverseDepth;
        _surfaceIds![index] = surfaceId;
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
            foreach (KeyValuePair<string, FastBitmap> image in _images)
            {
                image.Value.Dispose();
            }

            foreach (KeyValuePair<string, nint> texture in _imageTextures)
            {
                SDL_DestroyTexture((SDL_Texture*)texture.Value);
            }

            foreach (KeyValuePair<(string, string, uint), TextTextureEntry> entry in _textTextures)
            {
                SDL_DestroyTexture((SDL_Texture*)entry.Value.Texture);
            }

            if (_depthTexture != nint.Zero)
            {
                SDL_DestroyTexture((SDL_Texture*)_depthTexture);
            }

            if (_frameTarget != nint.Zero)
            {
                SDL_DestroyTexture((SDL_Texture*)_frameTarget);
            }
        }
    }

    private void SetRenderDrawColor(in FastColor color)
    {
        // Copied out of the 'in' parameter: a lambda cannot capture a by-reference one.
        FastColor drawColor = color;
        SDLGuard.Execute(() => SDL_SetRenderDrawColor(NativeRenderer, drawColor.R, drawColor.G, drawColor.B, drawColor.A));
    }

    private sealed class TextTextureEntry
    {
        public required nint Texture { get; init; }

        public required float Width { get; init; }

        public required float Height { get; init; }

        public bool UsedThisFrame { get; set; }
    }
}
