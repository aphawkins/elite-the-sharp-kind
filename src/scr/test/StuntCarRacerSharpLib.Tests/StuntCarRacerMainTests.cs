// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using SharpKind.Assets;
using SharpKind.Audio;
using SharpKind.Fakes.Audio;
using SharpKind.Fakes.Input;
using StuntCarRacerSharpLib.Fakes;
using Xunit;

namespace StuntCarRacerSharpLib.Tests;

public class StuntCarRacerMainTests
{
    [Fact]
    public void ConstructWithFakeAbstractionSucceeds()
    {
        // Arrange
        FakeAbstraction abstraction = new();

        // Act
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());

        // Assert
        Assert.NotNull(game);
    }

    [Fact]
    public void ConstructWithNullAbstractionThrows()
        => Assert.Throws<ArgumentNullException>(() => new StuntCarRacerMain(null!, AssetLocator.Create()));

    [Fact]
    public void ConstructWithAudioOptionsSucceeds()
    {
        // Arrange
        FakeAbstraction abstraction = new();

        // Act
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create(), new AudioOptions { MusicOn = false, EffectsOn = false });

        // Assert
        Assert.NotNull(game);
    }

    [Fact]
    public void ConstructWithNullAudioOptionsThrows()
        => Assert.Throws<ArgumentNullException>(() => new StuntCarRacerMain(new FakeAbstraction(), AssetLocator.Create(), null!));

    [Fact]
    public void PhysicsStepsEveryFrameGapTicksDuringRace()
    {
        // Arrange
        FakeAbstraction abstraction = new();
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());
        StartRace(game, abstraction);

        // Act
        List<int> movedTicks = [];
        for (int tick = 0; tick < 12; tick++)
        {
            game.Update();
            if (game.Race.FrameMoved)
            {
                movedTicks.Add(tick);
            }
        }

        // Assert: the physics ran on three of the twelve ticks, four apart
        Assert.Equal(3, movedTicks.Count);
        Assert.Equal(4, movedTicks[1] - movedTicks[0]);
        Assert.Equal(4, movedTicks[2] - movedTicks[1]);
    }

    [Fact]
    public void EngineSoundPitchesEveryTickDuringRace()
    {
        // Arrange
        FakeAbstraction abstraction = new();
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());
        FakeSound sound = (FakeSound)abstraction.Sound;
        StartRace(game, abstraction);
        int playsAtRaceStart = sound.PlayLoopCount;

        // Act
        for (int tick = 0; tick < 20; tick++)
        {
            game.Update();
        }

        // Assert: the engine loop is pitched at the full tick rate
        Assert.Equal(20, sound.PlayLoopCount - playsAtRaceStart);
    }

    [Fact]
    public void TrackMenuAndPreviewPlayNoEngineSound()
    {
        // Arrange
        FakeAbstraction abstraction = new();
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());
        FakeKeyboard keyboard = (FakeKeyboard)abstraction.Keyboard;
        FakeSound sound = (FakeSound)abstraction.Sound;

        // Act: a few menu ticks, then select the track to run the preview
        game.Update();
        game.Update();
        PressKey(game, keyboard, ConsoleKey.S);
        for (int tick = 0; tick < 10; tick++)
        {
            game.Update();
        }

        // Assert
        Assert.Equal(0, sound.PlayLoopCount);
    }

    [Fact]
    public void FrameGapKeysTuneThePhysicsRate()
    {
        // Arrange
        FakeAbstraction abstraction = new();
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());
        FakeKeyboard keyboard = (FakeKeyboard)abstraction.Keyboard;
        int frameGap = game.Race.FrameGap;

        // Act & Assert: F10 steps the physics less often, F9 more often
        PressKey(game, keyboard, ConsoleKey.F10);
        Assert.Equal(frameGap + 1, game.Race.FrameGap);

        PressKey(game, keyboard, ConsoleKey.F9);
        Assert.Equal(frameGap, game.Race.FrameGap);
    }

    [Fact]
    public void FrameGapDoesNotDropBelowOne()
    {
        // Arrange
        FakeAbstraction abstraction = new();
        StuntCarRacerMain game = new(abstraction, AssetLocator.Create());
        FakeKeyboard keyboard = (FakeKeyboard)abstraction.Keyboard;

        // Act: press F9 more times than the default gap allows
        for (int press = 0; press < game.Race.FrameGap + 3; press++)
        {
            PressKey(game, keyboard, ConsoleKey.F9);
        }

        // Assert
        Assert.Equal(1, game.Race.FrameGap);
    }

    // Drives the game from the track menu into the race: S selects the
    // track, then S again (read on a preview physics tick) starts the race.
    private static void StartRace(StuntCarRacerMain game, FakeAbstraction abstraction)
    {
        FakeKeyboard keyboard = (FakeKeyboard)abstraction.Keyboard;

        // menu -> preview
        PressKey(game, keyboard, ConsoleKey.S);

        // preview -> race; hold S until a physics tick reads it
        keyboard.KeyDown(ConsoleKey.S, ConsoleModifiers.None);
        for (int tick = 0; tick < 4; tick++)
        {
            game.Update();
        }

        keyboard.KeyUp(ConsoleKey.S, ConsoleModifiers.None);
    }

    private static void PressKey(StuntCarRacerMain game, FakeKeyboard keyboard, ConsoleKey key)
    {
        keyboard.KeyDown(key, ConsoleModifiers.None);
        game.Update();
        keyboard.KeyUp(key, ConsoleModifiers.None);
    }
}
