// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using SharpKind.Abstraction;
using SharpKind.Audio;
using SharpKind.Fakes.Audio;
using SharpKind.Fakes.Input;
using SharpKind.Graphics;
using SharpKind.Graphics.Fakes;
using SharpKind.Input;

namespace StuntCarRacerSharpLib.Fakes;

public sealed class FakeAbstraction(IGraphics graphics, ScreenLayout layout) : IAbstraction
{
    public FakeAbstraction()
        : this(new RecordingGraphics(), new(0, 0))
    {
    }

    public IGraphics Graphics { get; } = graphics;

    public ScreenLayout Layout { get; } = layout;

    public ISound Sound { get; } = new FakeSound();

    public IKeyboard Keyboard { get; } = new FakeKeyboard();
}
