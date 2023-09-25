// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.SDL
{
    internal static class SDLProgram
    {
        public static void Main()
        {
            try
            {
                SDLHelper.Initialise();
                using SDLGraphics graphics = new();
                using SDLSound sound = new();
                SDLKeyboard keyboard = new();
                EliteMain game = new(graphics, sound, keyboard);
                game.Run();
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
