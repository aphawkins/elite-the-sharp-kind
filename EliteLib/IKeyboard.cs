namespace Elite
{
    using Elite.Enums;

    public interface IKeyboard
    {
        int kbd_keyboard_startup();

        void kbd_poll_keyboard();

        char kbd_read_key();

        void KeyDown(int keyValue);

        void KeyUp(int keyValue);

        bool IsKeyPressed(CommandKey key);

        /// <summary>
        /// Blocks until the next key is pressed.
        /// </summary>
        /// <returns></returns>
        int ReadKey();
    }
}