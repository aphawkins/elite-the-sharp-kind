// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using static SDL2.SDL;

namespace EliteSharp.SDL
{
    internal sealed class SDLKeyboard : IKeyboard
    {
        private readonly Dictionary<CommandKey, bool> _isPressed = [];
        private CommandKey _lastKeyPressed;

        public bool Close { get; private set; }

        public void ClearKeyPressed()
        {
            _lastKeyPressed = 0;
            _isPressed.Clear();
        }

        public CommandKey GetKeyPressed()
        {
            CommandKey key = _lastKeyPressed;
            _lastKeyPressed = 0;
            return key;
        }

        public bool IsKeyPressed(params CommandKey[] keys)
        {
            if (keys == null)
            {
                return false;
            }

            foreach (CommandKey key in keys)
            {
                if (_isPressed.TryGetValue(key, out bool value) && value)
                {
                    return true;
                }
            }

            return false;
        }

        public void KeyDown(CommandKey keyValue)
        {
            _lastKeyPressed = keyValue;
            _isPressed[keyValue] = true;
        }

        public void KeyUp(CommandKey keyValue) => _isPressed[keyValue] = false;

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
                        KeyDown(SDLHelper.KeyConverter(sdlEvent.key.keysym.sym));
                        if (sdlEvent.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                        {
                            Close = true;
                            break;
                        }

                        break;

                    case SDL_EventType.SDL_KEYUP:
                        KeyUp(SDLHelper.KeyConverter(sdlEvent.key.keysym.sym));
                        break;

                    case SDL_EventType.SDL_QUIT:
                        Close = true;
                        break;
                }
            }
        }

        private bool PollEvent(out SDL_Event sdlEvent)
        {
            if (SDL_PollEvent(out sdlEvent) < 0)
            {
                SDLHelper.Throw(nameof(SDL_PollEvent));
            }

            return true;
        }
    }
}
