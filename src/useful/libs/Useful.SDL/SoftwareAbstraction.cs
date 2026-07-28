// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using SDL;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;
using static SDL.SDL3;

namespace Useful.SDL;

public sealed unsafe class SoftwareAbstraction : IAbstraction, IDisposable
{
    private readonly SDLRenderer _renderer;
    private readonly SDLWindow _window;
    private readonly SoftwareSoundOutput _soundOutput;

    // One streaming texture reused for every presented frame: the software
    // renderer hands over the same CPU framebuffer each tick, so creating a
    // surface and a texture per present was a synchronous GPU allocation and
    // upload on every frame. SDL_UpdateTexture re-uploads the pixels into
    // this one instead.
    private readonly nint _frameTexture;

    private bool _isDisposed;

    public SoftwareAbstraction(int screenWidth, int screenHeight, string title)
        : this(screenWidth, screenHeight, title, null)
    {
    }

    public SoftwareAbstraction(int screenWidth, int screenHeight, string title, ILogger? logger)
    {
        _window = new(screenWidth, screenHeight, title);
        _renderer = new(_window);

        _frameTexture = SDLGuard.Execute(() => (nint)SDL_CreateTexture(
            NativeRenderer,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
            screenWidth,
            screenHeight));

        AssetLocator assetLocator = AssetLocator.Create();
        Graphics = SoftwareGraphics.Create(
            screenWidth,
            screenHeight,
            SoftwareScreenUpdate,
            assetLocator,
            logger);

        SoftwareSound sound = new(assetLocator);
        _soundOutput = new SoftwareSoundOutput(sound);
        Sound = sound;

        Keyboard = new SoftwareKeyboard(new SDLInput());
    }

    public IGraphics Graphics { get; }

    public ISound Sound { get; }

    public IKeyboard Keyboard { get; }

    private SDL_Renderer* NativeRenderer => (SDL_Renderer*)(nint)_renderer;

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
                _soundOutput?.Dispose();
                (Sound as IDisposable)?.Dispose();

                if (_frameTexture != nint.Zero)
                {
                    SDL_DestroyTexture((SDL_Texture*)_frameTexture);
                }

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
        SDLGuard.Execute(() => SDL_UpdateTexture(
            (SDL_Texture*)_frameTexture,
            null,
            bitmap.BitmapHandle,
            bitmap.Width * 4));

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new()
            {
                x = 0,
                y = 0,
                w = bitmap.Width,
                h = bitmap.Height,
            };

            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)_frameTexture, null, &dest);
        });

        SDLGuard.Execute(() => SDL_RenderPresent(NativeRenderer));
    }
}
