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
        : this(screenWidth, screenHeight, title, AssetLocator.Create(), logger)
    {
    }

    public SoftwareAbstraction(
        int screenWidth,
        int screenHeight,
        string title,
        IAssetLocator assetLocator,
        ILogger? logger)
        : this(screenWidth, screenHeight, 1, title, assetLocator, logger)
    {
    }

    public SoftwareAbstraction(
        int screenWidth,
        int screenHeight,
        int windowScale,
        string title,
        IAssetLocator assetLocator,
        ILogger? logger)
    {
        // The framebuffer stays at the native resolution whatever the scale
        // is; only the window and the blit that presents it grow.
        _window = new(screenWidth * windowScale, screenHeight * windowScale, title);
        _renderer = new(_window);
        _renderer.SetLogicalSize(screenWidth, screenHeight);

        _frameTexture = SDLGuard.Execute(() => (nint)SDL_CreateTexture(
            NativeRenderer,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
            screenWidth,
            screenHeight));

        // Magnifying must duplicate pixels rather than blend them: SDL's
        // default is linear, which would show a filtered image instead of
        // the tier's own.
        SDLGuard.Execute(() => SDL_SetTextureScaleMode(
            (SDL_Texture*)_frameTexture,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST));

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
