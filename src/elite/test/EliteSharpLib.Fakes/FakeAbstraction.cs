// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;
using Useful.Graphics;
using Useful.Graphics.Fakes;

namespace EliteSharpLib.Fakes;

internal sealed class FakeAbstraction(IGraphics graphics) : IAbstraction
{
    // 512x512, matching SDLProgram's real ScreenWidth/ScreenHeight: EliteDraw
    // derives its layout (Centre, ScannerTop, ...) from these, and a 0x0
    // fake screen produces negative ranges that blow up star generation.
    public FakeAbstraction()
        : this(new FakeGraphics(512, 512))
    {
    }

    public IGraphics Graphics { get; } = graphics;

    public ISound Sound { get; } = new FakeSound();

    public IKeyboard Keyboard { get; } = new FakeKeyboard();
}
