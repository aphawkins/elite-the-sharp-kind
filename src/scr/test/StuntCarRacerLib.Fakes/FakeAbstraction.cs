// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Fakes.Controls;
using Useful.Graphics;
using Useful.Graphics.Fakes;

namespace StuntCarRacerLib.Fakes;

public sealed class FakeAbstraction : IAbstraction
{
    public IGraphics Graphics { get; } = new FakeGraphics();

    public ISound Sound { get; } = new FakeSound();

    public IKeyboard Keyboard { get; } = new FakeKeyboard();
}
