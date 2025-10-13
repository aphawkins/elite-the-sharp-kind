// 'Useful Libraries' - Andy Hawkins 2025.

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
