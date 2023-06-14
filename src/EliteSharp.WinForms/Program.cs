// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class Program : IDisposable
    {
        private readonly Bitmap _bmp;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly ISound _sound;

        internal Program()
        {
            _bmp = new(512, 512);
            _graphics = new GdiGraphics(_bmp);
            _sound = new Sound();
            _keyboard = new Keyboard();
        }

        public void Dispose()
        {
            ((IDisposable)_sound)?.Dispose();
            _bmp?.Dispose();
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

            using Program program = new();
            program.Go();
        }

        private void Go()
        {
            try
            {
                using CancellationTokenSource source = new();
                CancellationToken token = source.Token;
                using GameWindow window = new(_bmp, _keyboard);
                EliteMain game = new(_graphics, _sound, _keyboard);
                game.RunAsync(token);
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
    }
}
