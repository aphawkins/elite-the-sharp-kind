// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

////using EliteSharp.Graphics;

using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class WinProgram : IDisposable
    {
        private bool _disposedValue;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static void Go()
        {
            try
            {
                using WinSound sound = new();
                WinKeyboard keyboard = new();

#if QHD
                using WinWindow window = new(960, 540, keyboard);
#else
                using WinWindow window = new(512, 512, keyboard);
#endif

                using GraphicsFactory graphicsFactory = new(window);
                using IGraphics graphics = graphicsFactory.GetGraphics(GraphicsType.Software);

                EliteMain game = new(graphics, sound, keyboard);
                Task.Run(game.Run);
                Application.Run(window);
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

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            using WinProgram program = new();
            Go();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }
    }
}
