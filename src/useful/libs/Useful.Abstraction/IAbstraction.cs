// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

namespace Useful.Abstraction;

public interface IAbstraction
{
    public IGraphics Graphics { get; }

    public ISound Sound { get; }

    public IKeyboard Keyboard { get; }
}
