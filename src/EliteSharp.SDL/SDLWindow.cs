// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace EliteSharp.SDL
{
    public sealed class SDLWindow : IDisposable
    {
        private readonly IKeyboard _keyboard;
        private bool _isDisposed;

        internal SDLWindow(int screenWidth, int screenHeight, string title, IKeyboard keyboard)
        {
            // When running C# applications under the Visual Studio debugger, native code that
            // names threads with the 0x406D1388 exception will silently exit. To prevent this
            // exception from being thrown by SDL, add this line before your SDL_Init call:
            SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                SDLHelper.Throw(nameof(SDL_Init));
            }

            if (TTF_Init() < 0)
            {
                SDLHelper.Throw(nameof(TTF_Init));
            }

            Window = SDL_CreateWindow(
                title,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                screenWidth,
                screenHeight,
                SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (Window == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateWindow));
            }

            Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (Renderer == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_CreateRenderer));
            }

            _keyboard = keyboard;
        }

        public nint Window { get; }

        public nint Renderer { get; }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    SDL_DestroyRenderer(Renderer);
                    SDL_DestroyWindow(Window);
                    SDL_Quit();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }
    }
}
