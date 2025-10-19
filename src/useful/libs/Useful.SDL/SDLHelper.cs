// 'Useful Libraries' - Andy Hawkins 2025.

using static SDL2.SDL;

namespace Useful.SDL;

internal static class SDLHelper
{
    internal static void Throw(string? methodName)
        => throw new SDLException($"SDL2 Error. Method '{methodName}' failed. Error: " + SDL_GetError());

    internal static (ConsoleKey Key, ConsoleModifiers Modifiers) KeyConverter(SDL_Keycode sdlKey) => sdlKey switch
    {
        SDL_Keycode.SDLK_BACKSPACE => (ConsoleKey.Backspace, ConsoleModifiers.None),
        SDL_Keycode.SDLK_LCTRL => (ConsoleKey.None, ConsoleModifiers.Control),
        SDL_Keycode.SDLK_RCTRL => (ConsoleKey.None, ConsoleModifiers.Control),
        SDL_Keycode.SDLK_SLASH => (ConsoleKey.Oem2, ConsoleModifiers.None),
        SDL_Keycode.SDLK_DOWN => (ConsoleKey.DownArrow, ConsoleModifiers.None),
        SDL_Keycode.SDLK_TAB => (ConsoleKey.Tab, ConsoleModifiers.None),
        SDL_Keycode.SDLK_RETURN => (ConsoleKey.Enter, ConsoleModifiers.None),
        SDL_Keycode.SDLK_ESCAPE => (ConsoleKey.Escape, ConsoleModifiers.None),
        SDL_Keycode.SDLK_SPACE => (ConsoleKey.Spacebar, ConsoleModifiers.None),
        SDL_Keycode.SDLK_LEFTBRACKET => (ConsoleKey.OemComma, ConsoleModifiers.None),
        SDL_Keycode.SDLK_LEFT => (ConsoleKey.LeftArrow, ConsoleModifiers.None),
        SDL_Keycode.SDLK_RIGHT => (ConsoleKey.OemPeriod, ConsoleModifiers.None),
        SDL_Keycode.SDLK_RIGHTBRACKET => (ConsoleKey.RightArrow, ConsoleModifiers.None),
        SDL_Keycode.SDLK_UP => (ConsoleKey.UpArrow, ConsoleModifiers.None),

        SDL_Keycode.SDLK_a => (ConsoleKey.A, ConsoleModifiers.None),
        SDL_Keycode.SDLK_b => (ConsoleKey.B, ConsoleModifiers.None),
        SDL_Keycode.SDLK_c => (ConsoleKey.C, ConsoleModifiers.None),
        SDL_Keycode.SDLK_d => (ConsoleKey.D, ConsoleModifiers.None),
        SDL_Keycode.SDLK_e => (ConsoleKey.E, ConsoleModifiers.None),
        SDL_Keycode.SDLK_f => (ConsoleKey.F, ConsoleModifiers.None),
        SDL_Keycode.SDLK_g => (ConsoleKey.G, ConsoleModifiers.None),
        SDL_Keycode.SDLK_h => (ConsoleKey.H, ConsoleModifiers.None),
        SDL_Keycode.SDLK_i => (ConsoleKey.I, ConsoleModifiers.None),
        SDL_Keycode.SDLK_j => (ConsoleKey.J, ConsoleModifiers.None),
        SDL_Keycode.SDLK_k => (ConsoleKey.K, ConsoleModifiers.None),
        SDL_Keycode.SDLK_l => (ConsoleKey.L, ConsoleModifiers.None),
        SDL_Keycode.SDLK_m => (ConsoleKey.M, ConsoleModifiers.None),
        SDL_Keycode.SDLK_n => (ConsoleKey.N, ConsoleModifiers.None),
        SDL_Keycode.SDLK_o => (ConsoleKey.O, ConsoleModifiers.None),
        SDL_Keycode.SDLK_p => (ConsoleKey.P, ConsoleModifiers.None),
        SDL_Keycode.SDLK_q => (ConsoleKey.Q, ConsoleModifiers.None),
        SDL_Keycode.SDLK_r => (ConsoleKey.R, ConsoleModifiers.None),
        SDL_Keycode.SDLK_s => (ConsoleKey.S, ConsoleModifiers.None),
        SDL_Keycode.SDLK_t => (ConsoleKey.T, ConsoleModifiers.None),
        SDL_Keycode.SDLK_u => (ConsoleKey.U, ConsoleModifiers.None),
        SDL_Keycode.SDLK_v => (ConsoleKey.V, ConsoleModifiers.None),
        SDL_Keycode.SDLK_w => (ConsoleKey.W, ConsoleModifiers.None),
        SDL_Keycode.SDLK_x => (ConsoleKey.X, ConsoleModifiers.None),
        SDL_Keycode.SDLK_y => (ConsoleKey.Y, ConsoleModifiers.None),
        SDL_Keycode.SDLK_z => (ConsoleKey.Z, ConsoleModifiers.None),

        SDL_Keycode.SDLK_0 => (ConsoleKey.D0, ConsoleModifiers.None),
        SDL_Keycode.SDLK_1 => (ConsoleKey.D1, ConsoleModifiers.None),
        SDL_Keycode.SDLK_2 => (ConsoleKey.D2, ConsoleModifiers.None),
        SDL_Keycode.SDLK_3 => (ConsoleKey.D3, ConsoleModifiers.None),
        SDL_Keycode.SDLK_4 => (ConsoleKey.D4, ConsoleModifiers.None),
        SDL_Keycode.SDLK_5 => (ConsoleKey.D5, ConsoleModifiers.None),
        SDL_Keycode.SDLK_6 => (ConsoleKey.D6, ConsoleModifiers.None),
        SDL_Keycode.SDLK_7 => (ConsoleKey.D7, ConsoleModifiers.None),
        SDL_Keycode.SDLK_8 => (ConsoleKey.D8, ConsoleModifiers.None),
        SDL_Keycode.SDLK_9 => (ConsoleKey.D9, ConsoleModifiers.None),

        SDL_Keycode.SDLK_F1 => (ConsoleKey.F1, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F2 => (ConsoleKey.F2, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F3 => (ConsoleKey.F3, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F4 => (ConsoleKey.F4, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F5 => (ConsoleKey.F5, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F6 => (ConsoleKey.F6, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F7 => (ConsoleKey.F7, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F8 => (ConsoleKey.F8, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F9 => (ConsoleKey.F9, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F10 => (ConsoleKey.F10, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F11 => (ConsoleKey.F11, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F12 => (ConsoleKey.F12, ConsoleModifiers.None),

        _ => (ConsoleKey.None, ConsoleModifiers.None),
    };
}
