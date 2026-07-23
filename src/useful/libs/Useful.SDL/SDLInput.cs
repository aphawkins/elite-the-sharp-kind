// 'Useful Libraries' - Andy Hawkins 2025.

using SDL;
using Useful.Controls;
using static SDL.SDL3;

namespace Useful.SDL;

public sealed class SDLInput : IInput
{
    private IKeyboardSink? _keyboard;

    public void Register(IKeyboardSink keyboard) => _keyboard = keyboard;

    public void Poll()
    {
        while (_keyboard != null &&
            PollEvent(out SDL_Event sdlEvent) &&
            sdlEvent.type != (uint)SDL_EventType.SDL_EVENT_POLL_SENTINEL &&
            !_keyboard.Close)
        {
            switch ((SDL_EventType)sdlEvent.type)
            {
                case SDL_EventType.SDL_EVENT_KEY_DOWN:
                    (ConsoleKey key, ConsoleModifiers modifiers) = SDLHelper.KeyConverter(sdlEvent.key.key);
                    _keyboard.KeyDown(key, modifiers);
                    break;

                case SDL_EventType.SDL_EVENT_KEY_UP:
                    (ConsoleKey key1, ConsoleModifiers modifiers1) = SDLHelper.KeyConverter(sdlEvent.key.key);
                    _keyboard.KeyUp(key1, modifiers1);
                    break;

                case SDL_EventType.SDL_EVENT_QUIT:
                    _keyboard.Close = true;
                    break;
            }
        }
    }

#pragma warning disable S6640 // Avoid using this unsafe code block - required to fix the SDL_Event pointer for SDL_PollEvent
    private static unsafe bool PollEvent(out SDL_Event sdlEvent)
#pragma warning restore S6640
    {
        // SDL3's bool result signals whether an event was returned, not
        // failure - the end of the queue is instead detected via the
        // SDL_EVENT_POLL_SENTINEL check in the caller's loop condition, so
        // there is no error case to guard here.
        sdlEvent = default;

        fixed (SDL_Event* eventPtr = &sdlEvent)
        {
            _ = SDL_PollEvent(eventPtr);
        }

        return true;
    }
}
