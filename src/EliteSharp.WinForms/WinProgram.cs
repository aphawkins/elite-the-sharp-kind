// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Controls;

namespace EliteSharp.WinForms
{
    internal sealed class WinProgram
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Go();
        }

        private static void Go()
        {
            try
            {
                using ISound sound = new WinSound();
                IKeyboard keyboard = new WinKeyboard();
#if QHD
                using WinWindow window = new(960, 540, keyboard);
#else
                using GameWindow window = new(512, 512, keyboard);
#endif
                using GDIGraphics graphics = new(window.ScreenBitmap);

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
    }
}
