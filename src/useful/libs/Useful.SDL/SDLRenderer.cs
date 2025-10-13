// 'Useful Libraries' - Andy Hawkins 2025.

using static SDL2.SDL;

namespace Useful.SDL;

public sealed class SDLRenderer : IDisposable
{
    private readonly nint _renderer;
    private bool _isDisposed;

    public SDLRenderer(SDLWindow window)
        => _renderer = SDLGuard.Execute(() => SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED));

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~SDLRenderer()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public static implicit operator nint(SDLRenderer renderer)
    {
        Guard.ArgumentNull(renderer);

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
            SDL_DestroyRenderer(_renderer);
        }
    }
}
