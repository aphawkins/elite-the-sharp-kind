// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

namespace Useful.SDL;

public sealed class SDLAbstraction : IAbstraction, IDisposable
{
    private readonly int _screenHeight;
    private readonly int _screenWidth;
    private readonly SDLRenderer _renderer;
    private readonly SDLWindow _window;
    private bool _isDisposed;

    public SDLAbstraction(int screenWidth, int screenHeight, string title)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _window = new(_screenWidth, _screenHeight, title);
        _renderer = new(_window);

        Graphics = new SDLGraphics(_renderer, _screenWidth, _screenHeight);
        Sound = new SDLSound();
        Keyboard = new SDLKeyboard();
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
