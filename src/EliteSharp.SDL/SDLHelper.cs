// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using static SDL2.SDL;

namespace EliteSharp.SDL;

internal static class SDLHelper
{
    internal static void Throw(string? methodName)
        => throw new SDLException($"SDL2 Error. Method '{methodName}' failed. Error: " + SDL_GetError());

    internal static CommandKey KeyConverter(SDL_Keycode sdlKey) => sdlKey switch
    {
        SDL_Keycode.SDLK_BACKSPACE => CommandKey.Backspace,
        SDL_Keycode.SDLK_LCTRL => CommandKey.Ctrl,
        SDL_Keycode.SDLK_RCTRL => CommandKey.Ctrl,
        SDL_Keycode.SDLK_SLASH => CommandKey.DecreaseSpeed,
        SDL_Keycode.SDLK_c => CommandKey.DockingComputerOn,
        SDL_Keycode.SDLK_d => CommandKey.DockingComputerOff,
        SDL_Keycode.SDLK_x => CommandKey.Down,
        SDL_Keycode.SDLK_DOWN => CommandKey.DownArrow,
        SDL_Keycode.SDLK_e => CommandKey.ECM,
        SDL_Keycode.SDLK_9 => CommandKey.Tab,
        SDL_Keycode.SDLK_TAB => CommandKey.EnergyBomb,
        SDL_Keycode.SDLK_RETURN => CommandKey.Enter,
        SDL_Keycode.SDLK_ESCAPE => CommandKey.Esc,
        SDL_Keycode.SDLK_F1 => CommandKey.F1,
        SDL_Keycode.SDLK_F2 => CommandKey.F2,
        SDL_Keycode.SDLK_F3 => CommandKey.F3,
        SDL_Keycode.SDLK_F4 => CommandKey.F4,
        SDL_Keycode.SDLK_F5 => CommandKey.F5,
        SDL_Keycode.SDLK_F6 => CommandKey.F6,
        SDL_Keycode.SDLK_F7 => CommandKey.F7,
        SDL_Keycode.SDLK_F8 => CommandKey.F8,
        SDL_Keycode.SDLK_F9 => CommandKey.F9,
        SDL_Keycode.SDLK_F10 => CommandKey.F10,
        SDL_Keycode.SDLK_F11 => CommandKey.F11,
        SDL_Keycode.SDLK_F12 => CommandKey.F12,
        SDL_Keycode.SDLK_f => CommandKey.Find,
        SDL_Keycode.SDLK_a => CommandKey.Fire,
        SDL_Keycode.SDLK_m => CommandKey.FireMissile,
        SDL_Keycode.SDLK_h => CommandKey.Hyperspace,
        SDL_Keycode.SDLK_SPACE => CommandKey.SpaceBar,
        SDL_Keycode.SDLK_j => CommandKey.Jump,
        SDL_Keycode.SDLK_LEFTBRACKET => CommandKey.Left,
        SDL_Keycode.SDLK_LEFT => CommandKey.LeftArrow,
        SDL_Keycode.SDLK_n => CommandKey.No,
        SDL_Keycode.SDLK_o => CommandKey.Origin,
        SDL_Keycode.SDLK_p => CommandKey.Pause,
        SDL_Keycode.SDLK_r => CommandKey.Resume,
        SDL_Keycode.SDLK_RIGHT => CommandKey.Right,
        SDL_Keycode.SDLK_RIGHTBRACKET => CommandKey.RightArrow,
        SDL_Keycode.SDLK_t => CommandKey.TargetMissile,
        SDL_Keycode.SDLK_u => CommandKey.UnarmMissile,
        SDL_Keycode.SDLK_s => CommandKey.Up,
        SDL_Keycode.SDLK_UP => CommandKey.UpArrow,
        SDL_Keycode.SDLK_y => CommandKey.Yes,
        _ => CommandKey.None,
    };
}
