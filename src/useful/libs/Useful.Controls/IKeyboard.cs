// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public interface IKeyboard
{
    public bool Close { get; }

    public void ClearKeyPressed();

    public CommandKey GetKeyPressed();

    public bool IsKeyPressed(params CommandKey[] keys);

    public void KeyDown(CommandKey keyValue);

    public void KeyUp(CommandKey keyValue);

    public void Poll();
}
