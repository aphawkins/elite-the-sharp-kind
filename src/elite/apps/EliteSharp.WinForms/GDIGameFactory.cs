// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using System.Runtime.Versioning;
using EliteSharp.Assets;
using EliteSharp.Audio;
using EliteSharp.GDI;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms;

[SupportedOSPlatform("windows")]
internal sealed class GDIGameFactory : IDisposable
{
    private readonly IGraphics _graphics;
    private readonly WinKeyboard _keyboard;
    private readonly int _screenHeight;
    private readonly int _screenWidth;
    private readonly SoftwareSound _sound;
    private bool _isDisposed;

    internal GDIGameFactory(int screenWidth, int screenHeight, string title, string type)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _keyboard = new();
        Window = new(_screenWidth, _screenHeight, title, _keyboard);

        if (type == "SOFTWARE")
        {
            SoftwareAssetLoader assetLoader = new(new SoftwareAssetLocator());
            _graphics = new SoftwareGraphics(
                _screenWidth,
                _screenHeight,
                assetLoader,
                SoftwareScreenUpdate);
            _sound = new(assetLoader);
        }
        else
        {
            GDIAssetLoader assetLoader = new(new AssetLocator());
            _graphics = new GDIGraphics(_screenWidth, _screenHeight, assetLoader, ScreenUpdate);
            _sound = new(new(new AssetLocator()));
        }

        Game = new EliteMain(_graphics, _sound, _keyboard);
    }

    internal EliteMain Game { get; }

    internal WinWindow Window { get; }

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
                Window?.Dispose();
                _graphics?.Dispose();
                _sound?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }

    private void ScreenUpdate(Bitmap bitmap) => Window.SetImage(bitmap);

    private void SoftwareScreenUpdate(FastBitmap fastBitmap)
    {
        Bitmap bitmap = new(_screenWidth, _screenHeight, _screenWidth * 4, PixelFormat.Format32bppArgb, fastBitmap.BitmapHandle);
        ScreenUpdate(bitmap);
    }
}
