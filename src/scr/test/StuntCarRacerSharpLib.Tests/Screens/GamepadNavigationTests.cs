// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using SharpKind.Assets;
using SharpKind.Fakes.Input;
using SharpKind.Graphics.Fakes;
using SharpKind.Input;
using StuntCarRacerSharpLib.Fakes;
using StuntCarRacerSharpLib.Screens;
using StuntCarRacerSharpLib.Tracks;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Screens;

public class GamepadNavigationTests
{
    [Fact]
    public void AStartsTheTrackPreviewFromTheMenu()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.ButtonDown(GamepadButton.A);
        game.Update();

        Assert.Equal(GameMode.TrackPreview, game.Screens.CurrentId);
    }

    [Fact]
    public void AStartsTheRaceFromThePreviewAndBGoesBack()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.ButtonDown(GamepadButton.A);
        game.Update();
        pad.ButtonUp(GamepadButton.A);

        pad.ButtonDown(GamepadButton.B);
        game.Update();

        Assert.Equal(GameMode.TrackMenu, game.Screens.CurrentId);
    }

    [Fact]
    public void BackAbandonsTheRace()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.ButtonDown(GamepadButton.A);
        game.Update();
        pad.ButtonUp(GamepadButton.A);

        pad.ButtonDown(GamepadButton.A);
        game.Update();
        pad.ButtonUp(GamepadButton.A);
        Assert.Equal(GameMode.GameInProgress, game.Screens.CurrentId);

        pad.ButtonDown(GamepadButton.Back);
        game.Update();

        Assert.Equal(GameMode.TrackMenu, game.Screens.CurrentId);
    }

    [Fact]
    public void TheStickStepsThroughTheTrackList()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        Assert.Equal(TrackId.LittleRamp, game.Race.Track.Id);

        pad.AxisMoved(GamepadAxis.LeftX, 1f);
        game.Update();

        Assert.Equal(TrackId.SteppingStones, game.Race.Track.Id);
    }

    // The stick is read every tick, so without an edge latch one flick would
    // run through the whole list.
    [Fact]
    public void AHeldStickStepsOnlyOnce()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.AxisMoved(GamepadAxis.LeftX, 1f);
        game.Update();
        game.Update();
        game.Update();

        Assert.Equal(TrackId.SteppingStones, game.Race.Track.Id);
    }

    [Fact]
    public void ReleasingAndFlickingAgainStepsOnceMore()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.AxisMoved(GamepadAxis.LeftX, 1f);
        game.Update();

        pad.AxisMoved(GamepadAxis.LeftX, 0f);
        game.Update();

        pad.AxisMoved(GamepadAxis.LeftX, 1f);
        game.Update();

        Assert.Equal(TrackId.HumpBack, game.Race.Track.Id);
    }

    // The first track is the first track; the list does not wrap round.
    [Fact]
    public void TheStickStopsAtTheEndsOfTheList()
    {
        (StuntCarRacerMain game, FakeGamepad pad) = NewGame();

        pad.AxisMoved(GamepadAxis.LeftX, -1f);
        game.Update();

        Assert.Equal(TrackId.LittleRamp, game.Race.Track.Id);
    }

    private static (StuntCarRacerMain Game, FakeGamepad Pad) NewGame()
    {
        RecordingGraphics graphics = new(640, 400);
        FakeAbstraction abstraction = new(graphics, graphics.Layout);
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());

        return (game, (FakeGamepad)abstraction.Gamepad);
    }
}
