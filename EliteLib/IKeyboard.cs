namespace Elite
{
    using Elite.Enums;

    public interface IKeyboard
    {
        int kbd_keyboard_startup();

        void kbd_poll_keyboard();

        char kbd_read_key();

        void KeyPressed(int keyValue);

        bool IsKeyPressed(CommandKey key);

        CommandKey ReadKey();
    }
}