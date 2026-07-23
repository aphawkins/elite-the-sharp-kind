// 'Useful Libraries' - Andy Hawkins 2025.

using SDL;
using static SDL.SDL3;
using static SDL.SDL3_ttf;

namespace Useful.SDL;

#pragma warning disable S6640 // Avoid using this unsafe code block - required by ppy.SDL3-CS's raw pointer API
public sealed unsafe class SDLWindow : IDisposable
#pragma warning restore S6640
{
    private readonly nint _window;
    private bool _isDisposed;

    public SDLWindow(int screenWidth, int screenHeight, string title)
    {
        // Note: SDL3 handles the Visual Studio debugger's 0x406D1388 thread-naming
        // exception internally, so the SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING hint
        // that the previous SDL 2.x binding needed for this has been removed and
        // is no longer set here.
        SDLGuard.Execute(() => SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO));
        SDLGuard.Execute(() => TTF_Init());

        _window = SDLGuard.Execute(() => (nint)SDL_CreateWindow(
            title,
            screenWidth,
            screenHeight,
            default));

        // SDL3's SDL_CreateWindow no longer takes a position, so centre the
        // window explicitly once it has been created.
        SDLGuard.Execute(() => SDL_SetWindowPosition(
            (SDL_Window*)_window,
            (int)SDL_WINDOWPOS_CENTERED,
            (int)SDL_WINDOWPOS_CENTERED));
    }

    public static implicit operator nint(SDLWindow window)
    {
        Guard.ArgumentNull(window);

        return window._window;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public nint ToIntPtr() => _window;

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
            SDL_DestroyWindow((SDL_Window*)_window);
            SDL_Quit();
        }
    }
}
