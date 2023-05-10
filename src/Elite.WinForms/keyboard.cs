// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine;
using Elite.Engine.Enums;

namespace Elite.WinForms
{
    public class Keyboard : IKeyboard
    {
        private CommandKey _lastKeyPressed;
        private readonly Dictionary<CommandKey, bool> _isPressed = new();

        public void KeyDown(CommandKey keyValue)
        {
            _lastKeyPressed = keyValue;
            _isPressed[keyValue] = true;
        }

        public void KeyUp(CommandKey keyValue) => _isPressed[keyValue] = false;

        public bool IsKeyPressed(params CommandKey[] keys)
        {
            foreach (CommandKey key in keys)
            {
                if (_isPressed.TryGetValue(key, out bool value) && value)
                {
                    return true;
                }
            }

            return false;
        }

        public CommandKey GetKeyPressed()
        {
            CommandKey key = _lastKeyPressed;
            _lastKeyPressed = 0;
            return key;
        }

        public void ClearKeyPressed()
        {
            _lastKeyPressed = 0;
            _isPressed.Clear();
        }
    }
}
