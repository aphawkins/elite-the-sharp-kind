// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class WinProgram : IDisposable
    {
        private readonly EBitmap _ebmp = new(960, 540);
        private readonly WinKeyboard _keyboard;
        private readonly WinWindow _window;
        private readonly Bitmap _bmp = new(960, 540);
        private readonly object _locker;
        private bool _disposedValue;

        public WinProgram(object locker)
        {
            _locker = locker;
            _keyboard = new();
            _window = new(960, 540, _keyboard, _locker, _bmp);
        }

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

            using WinProgram program = new(new object());
            program.Go();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _bmp?.Dispose();
                    _window?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        private void Go()
        {
            try
            {
                using ISound sound = new WinSound();
                using SoftwareGraphics graphics = new(_ebmp, ScreenUpdate);

                EliteMain game = new(graphics, sound, _keyboard);
                Task.Run(game.Run);
                Application.Run(_window);
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

        private void ScreenUpdate()
        {
            lock (_locker)
            {
                for (int y = 0; y < 540; y++)
                {
                    for (int x = 0; x < 960; x++)
                    {
                        _bmp.SetPixel(x, y, Color.FromArgb(_ebmp.GetPixel(x, y).ToArgb()));
                    }
                }

                _window.ScreenImage = _bmp;
            }
        }
    }
}
