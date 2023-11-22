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
        private readonly nint _fontLarge;
        private readonly string _fontPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty,
            "Assets",
            "Fonts",
            "OpenSans-Regular.ttf");

        private readonly nint _fontSmall;
        private readonly ConcurrentDictionary<ImageType, nint> _images = new();
        private readonly nint _renderer;
        private readonly Dictionary<EColor, SDL_Color> _sdlColors = [];
        private readonly nint _window;
        private bool _isDisposed;

        public SDLGraphics()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
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
            if (_window == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateWindow));
            }

            _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (_renderer == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateRenderer));
            }

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

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
            if (_isDisposed)
            {
                return;
            }

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
            float diameter = radius * 2;
            float x = MathF.Floor(radius);
            float y = 0;
            float tx = 1;
            float ty = 1;
            float error = tx - diameter;

            List<SDL_FPoint> points = [];

            while (x >= y)
            {
                points.Add(new() { x = centre.X + x, y = centre.Y + y });
                points.Add(new() { x = centre.X + x, y = centre.Y - y });
                points.Add(new() { x = centre.X - x, y = centre.Y + y });
                points.Add(new() { x = centre.X - x, y = centre.Y - y });
                points.Add(new() { x = centre.X + y, y = centre.Y + x });
                points.Add(new() { x = centre.X + y, y = centre.Y - x });
                points.Add(new() { x = centre.X - y, y = centre.Y + x });
                points.Add(new() { x = centre.X - y, y = centre.Y - x });

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

            DrawPixels([.. points], colour);
        }

        public void DrawCircleFilled(Vector2 centre, float radius, EColor colour)
        {
            float diameter = radius * 2;
            float x = MathF.Floor(radius);
            float y = 0;
            float tx = 1;
            float ty = 1;
            float error = tx - diameter;

            List<(SDL_FPoint Start, SDL_FPoint End)> lines = [];

            while (x >= y)
            {
                lines.Add((new() { x = centre.X + x, y = centre.Y + y }, new() { x = centre.X + x, y = centre.Y - y }));
                lines.Add((new() { x = centre.X - x, y = centre.Y + y }, new() { x = centre.X - x, y = centre.Y - y }));
                lines.Add((new() { x = centre.X + y, y = centre.Y + x }, new() { x = centre.X + y, y = centre.Y - x }));
                lines.Add((new() { x = centre.X - y, y = centre.Y + x }, new() { x = centre.X - y, y = centre.Y - x }));

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

            DrawLines(lines, colour);
        }

        public void DrawImage(ImageType image, Vector2 position)
        {
            if (_isDisposed)
            {
                return;
            }

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
            if (_isDisposed)
            {
                return;
            }

            SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(_images[image]);
            float x = (ScreenWidth - surface.w) / 2;
            DrawImage(image, new(x, y));
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, EColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            SetRenderDrawColor(colour);

            if (SDL_RenderDrawLineF(_renderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawLineF));
            }
        }

        public void DrawPixel(Vector2 position, EColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

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

            DrawLine(points[0], points[^1], lineColour);
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
            if (_isDisposed)
            {
                return;
            }

            SetRenderDrawColor(colour);

            SDL_FRect rectangle = new()
            {
                x = position.X / (2 / Scale),
                y = position.Y / (2 / Scale),
                w = width + 1,
                h = height + 1,
            };

            if (SDL_RenderDrawRectF(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawRectF));
            }
        }

        public void DrawRectangleCentre(float y, float width, float height, EColor colour)
            => DrawRectangle(new((ScreenWidth - width) / Scale, y), width, height, colour);

        public void DrawRectangleFilled(Vector2 position, float width, float height, EColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            SetRenderDrawColor(colour);

            SDL_FRect rectangle = new()
            {
                x = position.X / (2 / Scale),
                y = position.Y / (2 / Scale),
                w = width + 1,
                h = height + 1,
            };

            if (SDL_RenderFillRectF(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderFillRectF));
            }
        }

        public void DrawTextCentre(float y, string text, FontSize fontSize, EColor colour)
        {
            if (_isDisposed || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

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

            SDL_FreeSurface(surfacePtr);
            SDL_DestroyTexture(texture);
        }

        public void DrawTextLeft(Vector2 position, string text, EColor colour)
        {
            if (_isDisposed || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

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

            SDL_FreeSurface(surfacePtr);
            SDL_DestroyTexture(texture);
        }

        public void DrawTextRight(Vector2 position, string text, EColor colour)
        {
            if (_isDisposed || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

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
            dest.x = (int)((position.X - dest.w) / (2 / Scale));
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

            SDL_FreeSurface(surfacePtr);
            SDL_DestroyTexture(texture);
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
            DrawLine(a, b, colour);
            DrawLine(b, c, colour);
            DrawLine(c, a, colour);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            SDL_Vertex[] vertices =
            [
                ConvertVertex(a, colour),
                ConvertVertex(b, colour),
                ConvertVertex(c, colour),
            ];

            if (SDL_RenderGeometry(_renderer, nint.Zero, vertices, vertices.Length, null, 0) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderGeometry));
            }
        }

        public void LoadBitmap(ImageType imgType, string bitmapPath)
        {
            if (_isDisposed)
            {
                return;
            }

            _images[imgType] = SDL_LoadBMP(bitmapPath);
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

            if (SDL_RenderSetClipRect(_renderer, ref rectangle) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderSetClipRect));
            }
        }

        private SDL_Vertex ConvertVertex(Vector2 point, in EColor colour) => new()
        {
            position = new() { x = point.X, y = point.Y },
            tex_coord = new() { x = 0.0f, y = 0.0f },
            color = _sdlColors[colour],
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

                //Fonts
                TTF_CloseFont(_fontSmall);
                TTF_CloseFont(_fontLarge);

                // Images
                foreach (KeyValuePair<ImageType, nint> image in _images)
                {
                    SDL_FreeSurface(image.Value);
                }

                SDL_DestroyRenderer(_renderer);
                SDL_DestroyWindow(_window);
                SDL_Quit();
            }
        }

        private void DrawLines(List<(SDL_FPoint Start, SDL_FPoint End)> points, in EColor colour)
        {
            SetRenderDrawColor(colour);

            foreach ((SDL_FPoint start, SDL_FPoint end) in points)
            {
                if (SDL_RenderDrawLineF(_renderer, start.x, start.y, end.x, end.y) < 0)
                {
                    SDLHelper.Throw(nameof(SDL_RenderDrawLinesF));
                }
            }
        }

        private void DrawPixels(SDL_FPoint[] points, in EColor colour)
        {
            SetRenderDrawColor(colour);

            if (SDL_RenderDrawPointsF(_renderer, points, points.Length) < 0)
            {
                SDLHelper.Throw(nameof(SDL_RenderDrawPointF));
            }
        }

        private void SetRenderDrawColor(in EColor colour)
        {
            if (SDL_SetRenderDrawColor(_renderer, colour.R, colour.G, colour.B, colour.A) < 0)
            {
                SDLHelper.Throw(nameof(SDL_SetRenderDrawColor));
            }
        }
    }
}
