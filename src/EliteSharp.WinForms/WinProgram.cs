// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.Versioning;

[assembly: CLSCompliant(false)]

namespace EliteSharp.WinForms
{
    [SupportedOSPlatform("windows")]
    internal sealed class WinProgram : IDisposable
    {
        private const string Title = "Elite - The Sharp Kind";

#if QHD
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
#else
        private const int ScreenWidth = 512;
        private const int ScreenHeight = 512;
#endif

#if !SOFTWARERENDERER
        private static readonly GDIGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "SOFTWARE");
#else
        private static readonly GDIGameFactory s_gameFactory = new(ScreenWidth, ScreenHeight, Title, "GDI");
#endif

        private bool _isDisposed;

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
                Task.Run(s_gameFactory.Game.Run);
                Application.Run(s_gameFactory.Window);
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

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    s_gameFactory?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }
    }
}
