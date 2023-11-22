// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

////using EliteSharp.Graphics;

using System.Drawing.Imaging;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class WinProgram : IDisposable
    {
#if QHD
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
#else
        private const int ScreenWidth = 512;
        private const int ScreenHeight = 512;
#endif

#if SOFTWAREGRAPHICS
        private const GraphicsType GraphicsRender = GraphicsType.Software;
#else
        private const GraphicsType GraphicsRender = GraphicsType.GDI;
#endif
        private static readonly WinKeyboard s_keyboard = new();
        private static readonly WinSound s_sound = new();
        private static readonly IGraphics s_graphics = GraphicsRender switch
        {
            GraphicsType.Software => new SoftwareGraphics(ScreenWidth, ScreenHeight, SoftwareScreenUpdate),
            _ => new GDIGraphics(ScreenWidth, ScreenHeight, ScreenUpdate),
        };

        private static readonly WinWindow s_window = new(ScreenWidth, ScreenHeight, s_keyboard);
        private static readonly EliteMain s_game = new(s_graphics, s_sound, s_keyboard);
        private bool _disposedValue;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            try
            {
                Task.Run(s_game.Run);
                Application.Run(s_window);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.ToString(), "Critial Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ex.Message, "Critial Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                throw;
            }
        }

        private static void ScreenUpdate(Bitmap bitmap) => s_window.SetImage(bitmap);

        private static void SoftwareScreenUpdate(FastBitmap fastBitmap)
        {
            Bitmap bitmap = new(fastBitmap.Width, fastBitmap.Height, PixelFormat.Format32bppArgb);

            for (int y = 0; y < 540; y++)
            {
                for (int x = 0; x < 960; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(fastBitmap.GetPixel(x, y).Argb));
                }
            }

            ScreenUpdate(bitmap);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    s_window?.Dispose();
                    s_graphics?.Dispose();
                    s_sound?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }
    }
}
