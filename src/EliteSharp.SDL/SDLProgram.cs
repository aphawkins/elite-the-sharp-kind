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
                // TODO: Move SDL init here
                using CancellationTokenSource source = new();
                CancellationToken token = source.Token;

                using SDLGraphics graphics = new();
                using ISound sound = new SDLSound();
                SDLKeyboard keyboard = new();

                EliteMain game = new(graphics, sound, keyboard);
                game.Run(token);
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
