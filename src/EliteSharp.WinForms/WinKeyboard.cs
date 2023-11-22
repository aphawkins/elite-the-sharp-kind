// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;

namespace EliteSharp.WinForms
{
    internal sealed class WinKeyboard : IKeyboard
    {
        private readonly Dictionary<CommandKey, bool> _isPressed = [];
        private CommandKey _lastKeyPressed;

        public bool Close { get; }

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
        }
    }
}
