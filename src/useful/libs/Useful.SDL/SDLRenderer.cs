// 'Useful Libraries' - Andy Hawkins 2023-2026.

using SDL;
using static SDL.SDL3;

namespace Useful.SDL;

public sealed unsafe class SDLRenderer(SDLWindow window) : IDisposable
{
    private readonly nint _renderer = SDLGuard.Execute(() => (nint)SDL_CreateRenderer((SDL_Window*)(nint)window, (byte*)null));
    private bool _isDisposed;

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~SDLRenderer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public static implicit operator nint(SDLRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        return renderer._renderer;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public nint ToIntPtr() => _renderer;

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
            SDL_DestroyRenderer((SDL_Renderer*)_renderer);
        }
    }
}
