// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;

namespace EliteSharp.SDL
{
    internal static class SDLProgram
    {
        public static void Main()
        {
            try
            {
                using ISound sound = new Sound();
                using CancellationTokenSource source = new();
                CancellationToken token = source.Token;

                using SDLGraphics graphics = new();
                SDLKeyboard keyboard = new();

                EliteMain game = new(graphics, sound, keyboard);
                game.RunAsync(token).ConfigureAwait(false);

                while (!keyboard.Close)
                {
                    keyboard.Poll();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(-1);
                throw;
            }
        }
    }
}
