// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets;
using static SDL2.SDL;

namespace EliteSharp.SDL
{
    internal static class SDLProgram
    {
        public static void Main()
        {
            try
            {
                // When running C# applications under the Visual Studio debugger, native code that
                // names threads with the 0x406D1388 exception will silently exit. To prevent this
                // exception from being thrown by SDL, add this line before your SDL_Init call:
                SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
                SDLAssetLoader assetLoader = new(new AssetLocator());
                using SDLGraphics graphics = new(960, 540, assetLoader);
                using SDLSound sound = new(assetLoader);
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
