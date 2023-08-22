// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class Program
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
                using ISound sound = new Sound();
                IKeyboard keyboard = new Keyboard();
                using CancellationTokenSource source = new();
                CancellationToken token = source.Token;
#if QHD
                using GameWindow window = new(960, 540, keyboard);
#else
                using GameWindow window = new(512, 512, keyboard);
#endif
#pragma warning disable CA2000 // Dispose objects before losing scope
                IGraphics graphics = new GdiGraphics(window.ScreenBitmap);
#pragma warning restore CA2000 // Dispose objects before losing scope
                EliteMain game = new(graphics, sound, keyboard);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                game.RunAsync(token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
