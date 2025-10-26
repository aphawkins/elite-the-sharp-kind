// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public interface IInput
{
    public void Poll();

    public void Register(IKeyboard keyboard);
}
