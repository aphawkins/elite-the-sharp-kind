// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Audio;
using Useful.Graphics;
using Useful.Input;

namespace Useful.SDL;

public sealed class SDLAbstraction : IAbstraction, IDisposable
{
    private readonly SDLRenderer _renderer;
    private readonly SDLWindow _window;
    private bool _isDisposed;

    public SDLAbstraction(int screenWidth, int screenHeight, string title, IAssetLocator assetLocator)
        : this(screenWidth, screenHeight, title, assetLocator, null)
    {
    }

    public SDLAbstraction(int screenWidth, int screenHeight, string title, IAssetLocator assetLocator, ILogger? logger)
        : this(screenWidth, screenHeight, 1, title, assetLocator, logger)
    {
    }

    public SDLAbstraction(
        int screenWidth,
        int screenHeight,
        int windowScale,
        string title,
        IAssetLocator assetLocator,
        ILogger? logger)
    {
        // The frame is composed at the native resolution whatever the scale
        // is; only the window and the blit that presents it grow.
        _window = new(screenWidth * windowScale, screenHeight * windowScale, title);
        _renderer = new(_window);
        _renderer.SetLogicalSize(screenWidth, screenHeight);

        Graphics = SDLGraphics.Create(_renderer, screenWidth, screenHeight, assetLocator, logger);
        Sound = new SDLSound(assetLocator);
        SDLInput input = new();
        Keyboard = new SoftwareKeyboard(input);
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
}
