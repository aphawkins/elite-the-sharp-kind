// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Audio;
using SharpKind.Graphics;
using SharpKind.Input;

namespace SharpKind.Abstraction;

public interface IAbstraction
{
    public IGraphics Graphics { get; }

    // The render target's size, which IGraphics deliberately does not
    // expose - see ScreenLayout.
    public ScreenLayout Layout { get; }

    public ISound Sound { get; }

    public IKeyboard Keyboard { get; }

    public IGamepad Gamepad { get; }
}
