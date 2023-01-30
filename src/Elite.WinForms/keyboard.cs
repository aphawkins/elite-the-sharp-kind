/*
 * Elite - The New Kind.
 *
 * Allegro version of the keyboard routines.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999-2001.
 * email: <christian@newkind.co.uk>
 *
 */

namespace Elite.WinForms
{
    using Elite.Engine;
    using Elite.Engine.Enums;

    public class Keyboard : IKeyboard
    {
        private int _lastKeyPressed;
        private readonly Dictionary<int, bool> _isPressed = new();

        public void KeyDown(int keyValue)
        {
            _lastKeyPressed = keyValue;
            _isPressed[keyValue] = true;
        }

        public void KeyUp(int keyValue)
        {
            _isPressed[keyValue] = false;
        }

        public bool IsKeyPressed(CommandKey key)
        {
            bool isPressed = _isPressed.ContainsKey((int)key) && _isPressed[(int)key];
            return isPressed;
        }

        /// <inheritdoc />
        public int ReadKey()
        {
            _lastKeyPressed = 0;

            while (_lastKeyPressed == 0) 
            {
                //Thread.Sleep(100);
            }

            return _lastKeyPressed;
        }
    }
}