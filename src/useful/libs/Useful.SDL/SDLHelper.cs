// 'Useful Libraries' - Andy Hawkins 2023-2026.

using SDL;
using static SDL.SDL3;

namespace Useful.SDL;

internal static class SDLHelper
{
    internal static void Throw(string? methodName)
        => throw new SDLException($"SDL3 Error. Method '{methodName}' failed. Error: " + SDL_GetError());

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
        SDL_Keycode.SDLK_COMMA => (ConsoleKey.OemComma, ConsoleModifiers.None),
        SDL_Keycode.SDLK_LEFT => (ConsoleKey.LeftArrow, ConsoleModifiers.None),
        SDL_Keycode.SDLK_PERIOD => (ConsoleKey.OemPeriod, ConsoleModifiers.None),
        SDL_Keycode.SDLK_RIGHT => (ConsoleKey.RightArrow, ConsoleModifiers.None),
        SDL_Keycode.SDLK_UP => (ConsoleKey.UpArrow, ConsoleModifiers.None),

        SDL_Keycode.SDLK_A => (ConsoleKey.A, ConsoleModifiers.None),
        SDL_Keycode.SDLK_B => (ConsoleKey.B, ConsoleModifiers.None),
        SDL_Keycode.SDLK_C => (ConsoleKey.C, ConsoleModifiers.None),
        SDL_Keycode.SDLK_D => (ConsoleKey.D, ConsoleModifiers.None),
        SDL_Keycode.SDLK_E => (ConsoleKey.E, ConsoleModifiers.None),
        SDL_Keycode.SDLK_F => (ConsoleKey.F, ConsoleModifiers.None),
        SDL_Keycode.SDLK_G => (ConsoleKey.G, ConsoleModifiers.None),
        SDL_Keycode.SDLK_H => (ConsoleKey.H, ConsoleModifiers.None),
        SDL_Keycode.SDLK_I => (ConsoleKey.I, ConsoleModifiers.None),
        SDL_Keycode.SDLK_J => (ConsoleKey.J, ConsoleModifiers.None),
        SDL_Keycode.SDLK_K => (ConsoleKey.K, ConsoleModifiers.None),
        SDL_Keycode.SDLK_L => (ConsoleKey.L, ConsoleModifiers.None),
        SDL_Keycode.SDLK_M => (ConsoleKey.M, ConsoleModifiers.None),
        SDL_Keycode.SDLK_N => (ConsoleKey.N, ConsoleModifiers.None),
        SDL_Keycode.SDLK_O => (ConsoleKey.O, ConsoleModifiers.None),
        SDL_Keycode.SDLK_P => (ConsoleKey.P, ConsoleModifiers.None),
        SDL_Keycode.SDLK_Q => (ConsoleKey.Q, ConsoleModifiers.None),
        SDL_Keycode.SDLK_R => (ConsoleKey.R, ConsoleModifiers.None),
        SDL_Keycode.SDLK_S => (ConsoleKey.S, ConsoleModifiers.None),
        SDL_Keycode.SDLK_T => (ConsoleKey.T, ConsoleModifiers.None),
        SDL_Keycode.SDLK_U => (ConsoleKey.U, ConsoleModifiers.None),
        SDL_Keycode.SDLK_V => (ConsoleKey.V, ConsoleModifiers.None),
        SDL_Keycode.SDLK_W => (ConsoleKey.W, ConsoleModifiers.None),
        SDL_Keycode.SDLK_X => (ConsoleKey.X, ConsoleModifiers.None),
        SDL_Keycode.SDLK_Y => (ConsoleKey.Y, ConsoleModifiers.None),
        SDL_Keycode.SDLK_Z => (ConsoleKey.Z, ConsoleModifiers.None),

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
