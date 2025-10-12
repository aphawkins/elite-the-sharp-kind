// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
