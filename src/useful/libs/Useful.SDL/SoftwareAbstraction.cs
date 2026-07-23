// 'Useful Libraries' - Andy Hawkins 2025.

using SDL;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;
using static SDL.SDL3;

namespace Useful.SDL;

#pragma warning disable S6640 // Avoid using this unsafe code block - required by ppy.SDL3-CS's raw pointer API
public sealed unsafe class SoftwareAbstraction : IAbstraction, IDisposable
#pragma warning restore S6640
{
    private readonly SDLRenderer _renderer;
    private readonly SDLWindow _window;
    private readonly SoftwareSoundOutput _soundOutput;
    private bool _isDisposed;

    public SoftwareAbstraction(int screenWidth, int screenHeight, string title)
    {
        _window = new(screenWidth, screenHeight, title);
        _renderer = new(_window);

        AssetLocator assetLocator = AssetLocator.Create();
        Graphics = SoftwareGraphics.Create(
            screenWidth,
            screenHeight,
            SoftwareScreenUpdate,
            assetLocator);

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
        // SDL3's named-format surface creation supersedes the previous SDL 2.x
        // mask-based SDL_CreateRGBSurfaceFrom, so a single call now covers both endiannesses.
        nint surfacePtr = SDLGuard.Execute(() => (nint)SDL_CreateSurfaceFrom(
            bitmap.Width,
            bitmap.Height,
            SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
            bitmap.BitmapHandle,
            bitmap.Width * 4));

        nint texturePtr = SDLGuard.Execute(
            () => (nint)SDL_CreateTextureFromSurface(NativeRenderer, (SDL_Surface*)surfacePtr));

        SDL_DestroySurface((SDL_Surface*)surfacePtr);

        SDLGuard.Execute(() =>
        {
            SDL_FRect dest = new()
            {
                x = 0,
                y = 0,
                w = bitmap.Width,
                h = bitmap.Height,
            };

            return SDL_RenderTexture(NativeRenderer, (SDL_Texture*)texturePtr, null, &dest);
        });

        SDL_DestroyTexture((SDL_Texture*)texturePtr);

        SDLGuard.Execute(() => SDL_RenderPresent(NativeRenderer));
    }
}
