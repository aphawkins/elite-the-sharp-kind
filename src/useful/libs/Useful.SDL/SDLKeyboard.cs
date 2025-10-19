// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Controls;
using static SDL2.SDL;

namespace Useful.SDL;

public sealed class SDLKeyboard : IKeyboard
{
    private readonly Dictionary<ConsoleKey, bool> _pressedKeys = [];
    private ConsoleKey _lastKeyPressed;
    private ConsoleModifiers _lastModifierPressed;
    private ConsoleModifiers _pressedModifiers = ConsoleModifiers.None;

    public bool Close { get; private set; }

    public void ClearPressed()
    {
        _lastKeyPressed = ConsoleKey.None;
        _pressedKeys.Clear();
        _lastModifierPressed = ConsoleModifiers.None;
        _pressedModifiers = ConsoleModifiers.None;
    }

    public bool IsPressed(ConsoleKey key)
    {
        if (key == ConsoleKey.None)
        {
            return false;
        }

        if (_lastKeyPressed == key)
        {
            _lastKeyPressed = ConsoleKey.None;
            ClearPressed(key);
            return true;
        }

        if (_pressedKeys.TryGetValue(key, out bool value) && value)
        {
            ClearPressed(key);
            return true;
        }

        return false;
    }

    public bool IsPressed(ConsoleModifiers modifiers)
    {
        if (_lastModifierPressed.HasFlag(modifiers))
        {
            _lastModifierPressed &= ~modifiers;
            ClearPressed(modifiers);
            return true;
        }

        if (_pressedModifiers.HasFlag(modifiers))
        {
            ClearPressed(modifiers);
            return true;
        }

        return false;
    }

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers)
    {
        _lastKeyPressed = key;
        _lastModifierPressed = modifiers;
        _pressedKeys[key] = true;
        _pressedModifiers |= modifiers;
    }

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers)
    {
        _pressedKeys[key] = false;
        ClearPressed(key);
        if (_lastKeyPressed == key)
        {
            _lastKeyPressed = ConsoleKey.None;
        }

        ClearPressed(modifiers);
        if (_lastModifierPressed.HasFlag(modifiers))
        {
            _lastModifierPressed &= ~modifiers;
        }
    }

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed()
    {
        if ((_lastKeyPressed == ConsoleKey.None) && (_lastModifierPressed == ConsoleModifiers.None))
        {
            return (ConsoleKey.None, ConsoleModifiers.None);
        }

        (ConsoleKey Key, ConsoleModifiers Modifiers) keys = (_lastKeyPressed, _lastModifierPressed);
        _lastKeyPressed = ConsoleKey.None;
        _lastModifierPressed = ConsoleModifiers.None;
        return keys;
    }

    public void Poll()
    {
        while (PollEvent(out SDL_Event sdlEvent) &&
            sdlEvent.type != SDL_EventType.SDL_POLLSENTINEL &&
            !Close)
        {
            switch (sdlEvent.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    break;

                case SDL_EventType.SDL_KEYDOWN:
                    (ConsoleKey key, ConsoleModifiers modifiers) = SDLHelper.KeyConverter(sdlEvent.key.keysym.sym);
                    KeyDown(key, modifiers);
                    if (sdlEvent.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                    {
                        Close = true;
                        break;
                    }

                    break;

                case SDL_EventType.SDL_KEYUP:
                    (ConsoleKey key1, ConsoleModifiers modifiers1) = SDLHelper.KeyConverter(sdlEvent.key.keysym.sym);
                    KeyUp(key1, modifiers1);
                    break;

                case SDL_EventType.SDL_QUIT:
                    Close = true;
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

    private void ClearPressed(ConsoleKey key) => _pressedKeys[key] = false;

    private void ClearPressed(ConsoleModifiers modifiers) => _pressedModifiers &= ~modifiers;
}
