// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Audio;
using SharpKind.Graphics;
using SharpKind.Input;

namespace SharpKind.Abstraction;

public interface IAbstraction
{
    public IGraphics Graphics { get; }

    public ISound Sound { get; }

    public IKeyboard Keyboard { get; }
}
