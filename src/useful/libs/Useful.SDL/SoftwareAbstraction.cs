// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;
using static SDL2.SDL;

namespace Useful.SDL;

public sealed class SoftwareAbstraction : IAbstraction, IDisposable
{
    private readonly SDLRenderer _renderer;
    private readonly SDLWindow _window;
    private bool _isDisposed;

    public SoftwareAbstraction(int screenWidth, int screenHeight, string title)
    {
        _window = new(screenWidth, screenHeight, title);
        _renderer = new(_window);

        Graphics = new SoftwareGraphics(
            screenWidth,
            screenHeight,
            SoftwareScreenUpdate);
        Sound = new SoftwareSound();
        Keyboard = new SoftwareKeyboard(new SDLInput());
    }

    public IGraphics Graphics { get; }

    public ISound Sound { get; }

    public IKeyboard Keyboard { get; }

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
                (Graphics as IDisposable)?.Dispose();
                (Sound as IDisposable)?.Dispose();
                _renderer?.Dispose();
                _window?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }

    private void SoftwareScreenUpdate(FastBitmap bitmap)
    {
        IntPtr surface = SDLGuard.Execute(() => BitConverter.IsLittleEndian
            ? SDL_CreateRGBSurfaceFrom(
                bitmap.BitmapHandle,
                bitmap.Width,
                bitmap.Height,
                bitmap.BitsPerPixel,
                bitmap.Width * 4,
                0x00FF0000,
                0x0000FF00,
                0x000000FF,
                0xFF000000)
            : SDL_CreateRGBSurfaceFrom(
                bitmap.BitmapHandle,
                bitmap.Width,
                bitmap.Height,
                bitmap.BitsPerPixel,
                bitmap.Width * 4,
                0x0000FF00,
                0x00FF0000,
                0xFF000000,
                0x000000FF));

        IntPtr texture = SDLGuard.Execute(() => SDL_CreateTextureFromSurface(_renderer, surface));

        SDL_FreeSurface(surface);

        SDL_Rect dest = new()
        {
            x = 0,
            y = 0,
            w = bitmap.Width,
            h = bitmap.Height,
        };

        SDLGuard.Execute(() => SDL_RenderCopy(_renderer, texture, nint.Zero, ref dest));

        SDL_DestroyTexture(texture);

        SDL_RenderPresent(_renderer);
    }
}
