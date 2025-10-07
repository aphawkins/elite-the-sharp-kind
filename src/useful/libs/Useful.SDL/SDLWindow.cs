// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace Useful.SDL;

internal sealed class SDLWindow : IDisposable
{
    private readonly nint _window;
    private bool _isDisposed;

    internal SDLWindow(int screenWidth, int screenHeight, string title)
    {
        // When running C# applications under the Visual Studio debugger, native code that
        // names threads with the 0x406D1388 exception will silently exit. To prevent this
        // exception from being thrown by SDL, add this line before your SDL_Init call:
        SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

        SDLGuard.Execute(() => SDL_Init(SDL_INIT_VIDEO));
        SDLGuard.Execute(TTF_Init);

        _window = SDLGuard.Execute(() => SDL_CreateWindow(
            title,
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            screenWidth,
            screenHeight,
            SDL_WindowFlags.SDL_WINDOW_SHOWN));
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
            SDL_DestroyWindow(_window);
            SDL_Quit();
        }
    }
}
