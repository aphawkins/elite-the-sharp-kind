// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Fakes;
using StuntCarRacerSharpLib.Screens;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Fakes.Harness;

namespace StuntCarRacerSharpLib.Tests;

// Drives the real StuntCarRacerMain against a real SoftwareGraphics with no
// SDL window (generalising StuntCarRacerMainTests.StartRace's menu->race
// key sequence into a scripted timeline any test can replay), for tests
// that need several ticks of real gameplay and, occasionally, a rendered
// frame to eyeball.
internal sealed class HeadlessGameHarness : HeadlessGameHarnessBase<GameStateSummary>
{
    public HeadlessGameHarness(int width = 640, int height = 400)
        : base(width, height, AssetLocator.Create())
    {
        FakeAbstraction abstraction = new(Graphics);
        Keyboard = (FakeKeyboard)abstraction.Keyboard;
        Game = new(abstraction);
    }

    public StuntCarRacerMain Game { get; }

    public override GameStateSummary State => new(
        Game.Screens.CurrentId,
        Game.Screens.CurrentId is GameMode.GameInProgress or GameMode.GameOver,
        Game.Race.Car.CurrentPiece,
        Game.Race.Opponent.CurrentPiece,
        Game.Race.Opponent.DistanceToPlayer());

    protected override void UpdateGame() => Game.Update();

    protected override void DrawGame() => Game.Draw();
}
