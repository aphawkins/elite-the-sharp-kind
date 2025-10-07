// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Useful.Controls;

public class SoftwareKeyboard : IKeyboard
{
    public bool Close { get; }

    public void ClearKeyPressed()
    {
    }

    public CommandKey GetKeyPressed()
        => CommandKey.None;

    public bool IsKeyPressed(params CommandKey[] keys)
        => false;

    public void KeyDown(CommandKey keyValue)
    {
    }

    public void KeyUp(CommandKey keyValue)
    {
    }

    public void Poll()
    {
    }
}
