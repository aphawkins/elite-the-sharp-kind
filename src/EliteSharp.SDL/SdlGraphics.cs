// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.SDL
{
    public sealed class SdlGraphics : IGraphics
    {
        private readonly IntPtr _window;
        private readonly IntPtr _renderer;
        private bool _disposedValue;

        public SdlGraphics()
        {
            if (SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_VIDEO) < 0)
            {
                return;
            }

            _window = SDL2.SDL.SDL_CreateWindow(
                "Elite - The Sharp Kind",
                SDL2.SDL.SDL_WINDOWPOS_CENTERED,
                SDL2.SDL.SDL_WINDOWPOS_CENTERED,
                512,
                512,
                SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            _renderer = SDL2.SDL.SDL_CreateRenderer(_window, -1, SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SdlGraphics()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public float Scale { get; }

        public float ScreenHeight { get; } = 512;

        public float ScreenWidth { get; } = 512;

        public void Clear()
        {
            if (SDL2.SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255) < 0)
            {
                return;
            }

            if (SDL2.SDL.SDL_RenderClear(_renderer) < 0)
            {
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

        public void DrawImage(Image image, Vector2 position)
        {
        }

        public void DrawImageCentre(Image image, float y)
        {
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, EColor colour)
        {
            if (SDL2.SDL.SDL_SetRenderDrawColor(_renderer, colour.R, colour.G, colour.B, colour.A) < 0)
            {
                return;
            }

            if (SDL2.SDL.SDL_RenderDrawLineF(_renderer, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y) < 0)
            {
            }
        }

        public void DrawPixel(Vector2 position, EColor colour)
        {
        }

        public void DrawPixelFast(Vector2 position, EColor colour)
        {
        }

        public void DrawPolygon(Vector2[] pointList, EColor lineColour)
        {
        }

        public void DrawPolygonFilled(Vector2[] pointList, EColor faceColour)
        {
        }

        public void DrawRectangle(Vector2 position, float width, float height, EColor colour)
        {
        }

        public void DrawRectangleCentre(float y, float width, float height, EColor colour)
        {
        }

        public void DrawRectangleFilled(Vector2 position, float width, float height, EColor colour)
        {
        }

        public void DrawTextCentre(float y, string text, FontSize fontSize, EColor colour)
        {
        }

        public void DrawTextLeft(Vector2 position, string text, EColor colour)
        {
        }

        public void DrawTextRight(Vector2 position, string text, EColor colour)
        {
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, EColor colour)
        {
        }

        public void LoadBitmap(Image imgType, byte[] bitmapBytes)
        {
        }

        public void ScreenUpdate() => SDL2.SDL.SDL_RenderPresent(_renderer);

        public void SetClipRegion(Vector2 position, float width, float height)
        {
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    SDL2.SDL.SDL_DestroyRenderer(_renderer);
                    SDL2.SDL.SDL_DestroyWindow(_window);
                    SDL2.SDL.SDL_Quit();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}
