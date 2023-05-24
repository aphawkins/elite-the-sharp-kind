// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine;
using Elite.Engine.Audio;

namespace Elite.WinForms
{
    internal sealed class Program : IDisposable
    {
        private readonly Bitmap _bmp;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly ISound _sound;

        public Program()
        {
            _bmp = new(512, 512);
            _graphics = new GdiGraphics(ref _bmp);
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
                using GameWindow window = new(_bmp, _keyboard);

                EliteMain game = new(_graphics, _sound, _keyboard);

                Task.Run(() =>
                {
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
                        game.Run();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Critial Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                });

                Application.Run(window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Critial Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
