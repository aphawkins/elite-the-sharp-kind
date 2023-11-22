// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class GraphicsFactory : IDisposable
    {
        private readonly WinWindow _window;
        private bool _disposedValue;

        internal GraphicsFactory(WinWindow window) => _window = window;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal IGraphics GetGraphics(float screenWidth, float screenHeight, GraphicsType graphicsType) => graphicsType switch
        {
            GraphicsType.Software => new SoftwareGraphics(screenWidth, screenHeight, SoftwareScreenUpdate),
            _ => new GDIGraphics(screenWidth, screenHeight, GDIScreenUpdate),
        };

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    ////_window?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        private void GDIScreenUpdate(Bitmap bitmap) => _window.SetImage(bitmap);

        private void SoftwareScreenUpdate(FastBitmap fastBitmap)
        {
            Bitmap bitmap = new(fastBitmap.Width, fastBitmap.Height, PixelFormat.Format32bppArgb);

            for (int y = 0; y < 540; y++)
            {
                for (int x = 0; x < 960; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(fastBitmap.GetPixel(x, y).Argb));
                }
            }

            _window.SetImage(bitmap);
        }
    }
}
