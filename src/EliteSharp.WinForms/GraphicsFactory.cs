// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class GraphicsFactory : IDisposable
    {
        private readonly Bitmap _bmp = new(960, 540);
        private readonly EBitmap _ebmp = new(960, 540);
        private readonly System.Drawing.Graphics _screenGraphics;
        private readonly WinWindow _window;
        private bool _disposedValue;

        internal GraphicsFactory(WinWindow window)
        {
            _window = window;
            _screenGraphics = System.Drawing.Graphics.FromImage(_bmp);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal IGraphics GetGraphics(GraphicsType graphicsType) => graphicsType switch
        {
            GraphicsType.Software => new SoftwareGraphics(_ebmp, SoftwareScreenUpdate),
            _ => new GDIGraphics(_bmp, GDIScreenUpdate),
        };

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _screenGraphics?.Dispose();
                    _bmp?.Dispose();
                    _window?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        private void GDIScreenUpdate() => _window.SetImage(_bmp);

        private void SoftwareScreenUpdate()
        {
            _screenGraphics.DrawImage(_bmp, 0, 0);
            for (int y = 0; y < 540; y++)
            {
                for (int x = 0; x < 960; x++)
                {
                    _bmp.SetPixel(x, y, Color.FromArgb(_ebmp.GetPixel(x, y).ToArgb()));
                }
            }

            _window.SetImage(_bmp);
        }
    }
}
