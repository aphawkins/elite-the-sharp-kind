// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using static SDL2.SDL;

namespace EliteSharp.SDL
{
    internal static class SDLHelper
    {
        internal static void Throw(string methodName) => throw new EliteException($"SDL2 Error. Method '{methodName}' failed. Error: " + SDL_GetError());

        internal static CommandKey KeyConverter(SDL_Keycode sdlKey) => sdlKey switch
        {
            SDL_Keycode.SDLK_y => CommandKey.Yes,
            SDL_Keycode.SDLK_n => CommandKey.No,
            SDL_Keycode.SDLK_SPACE => CommandKey.SpaceBar,
            SDL_Keycode.SDLK_BACKSPACE => CommandKey.Backspace,
            SDL_Keycode.SDLK_F1 => CommandKey.F1,
            SDL_Keycode.SDLK_F2 => CommandKey.F2,
            _ => CommandKey.None,
        };
    }
}
