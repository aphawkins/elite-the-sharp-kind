// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using EliteSharp.Assets;
using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class GameFactory : IDisposable
    {
        private readonly IGraphics _graphics;
        private readonly WinKeyboard _keyboard;
        private readonly int _screenHeight;
        private readonly int _screenWidth;
        private readonly ISound _sound;
        private bool _isDisposed;

        internal GameFactory(int screenWidth, int screenHeight, string type = "GDI")
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _keyboard = new();
            Window = new(_screenWidth, _screenHeight, _keyboard);

            if (type == "SOFTWARE")
            {
                _graphics = new SoftwareGraphics(
                    _screenWidth,
                    _screenHeight,
                    new SoftwareAssetLoader(new AssetPaths()),
                    SoftwareScreenUpdate);
                _sound = new SoftwareSound(new SoftwareAssetLoader(new AssetPaths()));
                Game = new EliteMain(_graphics, _sound, _keyboard);
            }
            else
            {
                _graphics = new GDIGraphics(_screenWidth, _screenHeight, new GDIAssetLoader(new AssetPaths()), ScreenUpdate);
                _sound = new WinSound(new GDIAssetLoader(new AssetPaths()));

                Game = new EliteMain(_graphics, _sound, _keyboard);
            }
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
}
