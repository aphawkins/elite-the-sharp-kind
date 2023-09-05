// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using EliteSharp.Graphics;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace EliteSharp.SDL
{
    public sealed class SDLGraphics : IGraphics
    {
        private readonly Dictionary<EColor, SDL_Color> _sdlColors = new();
        private readonly nint _fontLarge;

        private readonly string _fontPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Assets", "Fonts", "OpenSans-Regular.ttf");
        private readonly nint _fontSmall;
        private readonly ConcurrentDictionary<ImageType, nint> _images = new();
        private readonly nint _renderer;
        private readonly nint _window;
        private bool _disposedValue;

        public SDLGraphics()
        {
            // When running C# applications under the Visual Studio debugger, native code that
            // names threads with the 0x406D1388 exception will silently exit. To prevent this
            // exception from being thrown by SDL, add this line before your SDL_Init call:
            SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

            if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
            {
                SDLHelper.Throw(nameof(SDL_Init));
            }

            if (TTF_Init() < 0)
            {
                SDLHelper.Throw(nameof(TTF_Init));
            }

            _window = SDL_CreateWindow(
                "Elite - The Sharp Kind",
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                (int)ScreenWidth,
                (int)ScreenHeight,
                SDL_WindowFlags.SDL_WINDOW_SHOWN);

            _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            foreach (EColor colour in EColors.AllColors())
            {
                SDL_Color sdlColor = new()
                {
                    a = colour.A,
                    r = colour.R,
                    b = colour.B,
                    g = colour.G,
                };
                _sdlColors.Add(colour, sdlColor);
            }

            _fontLarge = TTF_OpenFont(_fontPath, 18);
            if (_fontLarge == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_OpenFont));
            }

            _fontSmall = TTF_OpenFont(_fontPath, 12);
            if (_fontLarge == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_OpenFont));
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SDLGraphics()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public float Scale { get; } = 2;

        public float ScreenHeight { get; } = 540;

        public float ScreenWidth { get; } = 960;

        public void Clear()
        {
            SetRenderDrawColor(EColors.Black);

            if (SDL_RenderClear(_renderer) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderClear));
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DrawCircle(Vector2 centre, float radius, EColor colour)
        {
        }

        public void DrawCircleFilled(Vector2 centre, float radius, EColor colour)
        {
        }

        public void DrawImage(ImageType image, Vector2 position)
        {
            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[image]);
            nint texture = SDL_CreateTextureFromSurface(_renderer, _images[image]);

            SDL_Rect dest = new()
            {
                x = (int)position.X,
                y = (int)position.Y,
                h = surface.h,
                w = surface.w,
            };

            if (SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderCopy));
            }

            SDL_DestroyTexture(texture);
        }

        public void DrawImageCentre(ImageType image, float y)
        {
            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[image]);
            float x = (ScreenWidth - surface.w) / 2;
            DrawImage(image, new(x, y));
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, EColor colour)
        {
            SetRenderDrawColor(colour);

            if (SDL_RenderDrawLineF(_renderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawLineF));
            }
        }

        public void DrawPixel(Vector2 position, EColor colour)
        {
            SetRenderDrawColor(colour);

            if (SDL_RenderDrawPointF(_renderer, position.X, position.Y) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawPointF));
            }
        }

        public void DrawPixelFast(Vector2 position, EColor colour) => DrawPixel(position, colour);

        public void DrawPolygon(Vector2[] points, EColor lineColour)
        {
            if (points == null)
            {
                return;
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], lineColour);
            }

            DrawLine(points[0], points[points.Length - 1], lineColour);
        }

        public void DrawPolygonFilled(Vector2[] points, EColor faceColour)
        {
            if (points == null)
            {
                return;
            }

            // SDL_RenderGeometry only renders triangles and quads?
            // Create triangles of which each share the first vertex
            for (int i = 1; i < points.Length - 1; i++)
            {
                DrawTriangleFilled(points[0], points[i], points[i + 1], faceColour);
            }
        }

        public void DrawRectangle(Vector2 position, float width, float height, EColor colour)
        {
            SetRenderDrawColor(colour);

            SDL_FRect rectangle = new()
            {
                x = (int)(position.X / (2 / Scale)) + 1,
                y = (int)((position.Y / (2 / Scale)) + 1),
                w = (int)width - 1,
                h = (int)height,
            };

            if (SDL_RenderDrawRectF(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawRectF));
            }
        }

        public void DrawRectangleCentre(float y, float width, float height, EColor colour)
        {
        }

        public void DrawRectangleFilled(Vector2 position, float width, float height, EColor colour)
        {
            SetRenderDrawColor(colour);

            SDL_FRect rectangle = new()
            {
                x = (int)(position.X / (2 / Scale)),
                y = (int)((position.Y / (2 / Scale)) + 1),
                w = (int)width,
                h = (int)height,
            };

            if (SDL_RenderFillRectF(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderFillRectF));
            }
        }

        public void DrawTextCentre(float y, string text, FontSize fontSize, EColor colour)
        {
            nint surfacePtr = TTF_RenderText_Solid(
                fontSize == FontSize.Large ? _fontLarge : _fontSmall,
                text,
                _sdlColors[colour]);
            if (surfacePtr == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_RenderText_Solid));
            }

            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);

            SDL_Rect dest = surface.clip_rect;
            dest.x = (int)((ScreenWidth / 2) - (dest.w / 2));
            dest.y = (int)(y / (2 / Scale));

            nint texture = SDL_CreateTextureFromSurface(_renderer, surfacePtr);
            if (texture == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateTextureFromSurface));
            }

            if (SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderCopy));
            }
        }

        public void DrawTextLeft(Vector2 position, string text, EColor colour)
        {
            nint surfacePtr = TTF_RenderText_Solid(
                _fontSmall,
                text,
                _sdlColors[colour]);
            if (surfacePtr == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_RenderText_Solid));
            }

            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);

            SDL_Rect dest = surface.clip_rect;
            dest.x = (int)(position.X / (2 / Scale));
            dest.y = (int)(position.Y / (2 / Scale));

            nint texture = SDL_CreateTextureFromSurface(_renderer, surfacePtr);
            if (texture == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateTextureFromSurface));
            }

            if (SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderCopy));
            }
        }

        public void DrawTextRight(Vector2 position, string text, EColor colour)
        {
            nint surfacePtr = TTF_RenderText_Solid(
                _fontSmall,
                text,
                _sdlColors[colour]);
            if (surfacePtr == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_RenderText_Solid));
            }

            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);

            SDL_Rect dest = surface.clip_rect;
            dest.x = (int)(position.X / (2 / Scale));
            dest.y = (int)(position.Y / (2 / Scale));

            nint texture = SDL_CreateTextureFromSurface(_renderer, surfacePtr);
            if (texture == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateTextureFromSurface));
            }

            if (SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderCopy));
            }
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
            DrawLine(a, b, colour);
            DrawLine(b, c, colour);
            DrawLine(c, a, colour);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
            SDL_Vertex[] vertices = new SDL_Vertex[3]
            {
                ConvertVertex(a, colour),
                ConvertVertex(b, colour),
                ConvertVertex(c, colour),
            };

            if (SDL_RenderGeometry(_renderer, nint.Zero, vertices, vertices.Length, null, 0) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderGeometry));
            }
        }

        public void LoadBitmap(ImageType imgType, string bitmapPath) => _images[imgType] = SDL_LoadBMP(bitmapPath);

        public void ScreenUpdate() => SDL_RenderPresent(_renderer);

        public void SetClipRegion(Vector2 position, float width, float height)
        {
            SDL_Rect rectangle = new()
            {
                x = (int)(position.X / (2 / Scale)),
                y = (int)(position.Y / (2 / Scale)),
                w = (int)width,
                h = (int)height,
            };

            if (SDL_RenderSetClipRect(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderSetClipRect));
            }
        }

        private SDL_Vertex ConvertVertex(Vector2 point, EColor colour) => new()
        {
            position = new() { x = point.X, y = point.Y },
            tex_coord = new() { x = 0.0f, y = 0.0f },
            color = _sdlColors[colour],
        };

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    // Images
                    foreach (KeyValuePair<ImageType, nint> image in _images)
                    {
                        SDL_FreeSurface(image.Value);
                    }

                    SDL_DestroyRenderer(_renderer);
                    SDL_DestroyWindow(_window);
                    SDL_Quit();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        private void SetRenderDrawColor(EColor colour)
        {
            if (SDL_SetRenderDrawColor(_renderer, colour.R, colour.G, colour.B, colour.A) < 0)
            {
                SDLHelper.Throw(nameof(SDL_SetRenderDrawColor));
            }
        }
    }
}
