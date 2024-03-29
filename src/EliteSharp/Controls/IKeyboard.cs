// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Controls
{
    public interface IKeyboard
    {
        bool Close { get; }

        void ClearKeyPressed();

        CommandKey GetKeyPressed();

        bool IsKeyPressed(params CommandKey[] keys);

        void KeyDown(CommandKey keyValue);

        void KeyUp(CommandKey keyValue);

        void Poll();
    }
}
