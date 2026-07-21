// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Controls;
using static SDL2.SDL;

namespace Useful.SDL;

public sealed class SDLInput : IInput
{
    private IKeyboardSink? _keyboard;

    public void Register(IKeyboardSink keyboard) => _keyboard = keyboard;

    public void Poll()
    {
        while (_keyboard != null &&
            PollEvent(out SDL_Event sdlEvent) &&
            sdlEvent.type != SDL_EventType.SDL_POLLSENTINEL &&
            !_keyboard.Close)
        {
            switch (sdlEvent.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    break;

                case SDL_EventType.SDL_KEYDOWN:
                    (ConsoleKey key, ConsoleModifiers modifiers) = SDLHelper.KeyConverter(sdlEvent.key.keysym.sym);
                    _keyboard.KeyDown(key, modifiers);
                    break;

                case SDL_EventType.SDL_KEYUP:
                    (ConsoleKey key1, ConsoleModifiers modifiers1) = SDLHelper.KeyConverter(sdlEvent.key.keysym.sym);
                    _keyboard.KeyUp(key1, modifiers1);
                    break;

                case SDL_EventType.SDL_QUIT:
                    _keyboard.Close = true;
                    break;
            }
        }
    }

    private static bool PollEvent(out SDL_Event sdlEvent)
    {
        if (SDL_PollEvent(out sdlEvent) < 0)
        {
            SDLHelper.Throw(nameof(SDL_PollEvent));
        }

        return true;
    }
}
