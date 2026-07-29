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

    /// <summary>
    /// Fixes the coordinate space every draw call is expressed in, so a
    /// window larger than <paramref name="screenWidth"/> x
    /// <paramref name="screenHeight"/> shows the same pixels magnified by a
    /// whole number rather than more of the scene. Nothing above this needs
    /// to know the window size. Logical presentation applies only when
    /// drawing to the window, so anything rendered into a texture target
    /// stays at its native size.
    /// </summary>
    public void SetLogicalSize(int screenWidth, int screenHeight)
        => SDLGuard.Execute(() => SDL_SetRenderLogicalPresentation(
            (SDL_Renderer*)_renderer,
            screenWidth,
            screenHeight,
            SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_INTEGER_SCALE));

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
