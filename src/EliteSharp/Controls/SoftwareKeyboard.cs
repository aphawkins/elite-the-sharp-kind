// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Controls
{
    internal class SoftwareKeyboard : IKeyboard
    {
        public void ClearKeyPressed() => throw new NotImplementedException();

        public CommandKey GetKeyPressed() => throw new NotImplementedException();

        public bool IsKeyPressed(params CommandKey[] keys) => throw new NotImplementedException();

        public void KeyDown(CommandKey keyValue) => throw new NotImplementedException();

        public void KeyUp(CommandKey keyValue) => throw new NotImplementedException();
    }
}
