// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Controls;

public interface IInput
{
    public void Poll();

    public void Register(IKeyboardSink keyboard);
}
