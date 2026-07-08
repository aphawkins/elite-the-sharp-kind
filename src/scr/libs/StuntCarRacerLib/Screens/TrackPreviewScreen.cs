// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful.Abstraction;

namespace StuntCarRacerLib.Screens;

// The opponent drives the selected track, watched from the middle of the
// world (original HandleTrackPreview / CalcTrackPreviewViewpoint).
internal sealed class TrackPreviewScreen : IGameScreen
{
    private readonly StuntCarRacerMain _game;

    internal TrackPreviewScreen(StuntCarRacerMain game) => _game = game;

    public void Reset()
    {
        // reset the cars so the opponent is shown during the preview
        _game.Car.Reset();
        _game.Opponent.StartRace();
    }

    public void Update()
    {
        if (!_game.PhysicsDue())
        {
            return;
        }

        _game.FrameMoved = true;
        _game.Opponent.Update();
        _game.Bridge.Move(_game.Car.CurrentPiece, _game.Opponent.CurrentPiece, _game.Opponent);

        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;

        // the Draw Bridge requires a higher viewpoint
        long viewY = _game.Track.Id == TrackId.DrawBridge
            ? _game.Opponent.Y - (5L * Track.CubeSize / 2)
            : _game.Opponent.Y - ((long)Track.CubeSize / 2);

        long viewX = centre + ((_game.Opponent.X - centre) / 2);
        long viewZ = centre + ((_game.Opponent.Z - centre) / 2);

        _game.Camera.LookAt(viewX, viewY, viewZ, _game.Opponent.X, _game.Opponent.Y, _game.Opponent.Z);

        if (_game.Keyboard.IsPressed(ConsoleKey.S))
        {
            _game.Screens.Set(GameMode.GameInProgress);
        }
        else if (_game.Keyboard.IsPressed(ConsoleKey.M))
        {
            _game.Screens.Set(GameMode.TrackMenu);
        }
    }

    public void Draw()
    {
        _game.DrawWorld(showOpponent: true);

        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        float height = _game.Graphics.ScreenHeight;

        _game.Graphics.DrawTextLeft(
            new(2, height - 130),
            $"Selected track - {_game.Track.Name}.",
            StuntCarRacerMain.SmallFont,
            yellow);
        _game.Graphics.DrawTextLeft(
            new(2, height - 114),
            "Press 'S' to start game, 'M' for track menu, Escape to quit",
            StuntCarRacerMain.SmallFont,
            yellow);

        _game.Graphics.DrawTextLeft(
            new(2, height - 82),
            "Keyboard controls during game :-",
            StuntCarRacerMain.SmallFont,
            yellow);
        _game.Graphics.DrawTextLeft(
            new(2, height - 66),
            "  Left/Right arrows = Steer, Up = Accelerate, Down = Brake",
            StuntCarRacerMain.SmallFont,
            yellow);
        _game.Graphics.DrawTextLeft(
            new(2, height - 50),
            "  Space = Boost, N = Change scenery",
            StuntCarRacerMain.SmallFont,
            yellow);
        _game.Graphics.DrawTextLeft(
            new(2, height - 34),
            "  M = Back to track menu, Escape = Quit",
            StuntCarRacerMain.SmallFont,
            yellow);
    }
}
