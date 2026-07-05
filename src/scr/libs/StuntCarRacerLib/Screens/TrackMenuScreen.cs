// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful.Abstraction;

namespace StuntCarRacerLib.Screens;

// The track list and orbiting camera (original HandleTrackMenu /
// CalcTrackMenuViewpoint). Not throttled by the frame gap.
internal sealed class TrackMenuScreen : IGameScreen
{
    private readonly StuntCarRacerMain _game;
    private int _orbitAngle;

    internal TrackMenuScreen(StuntCarRacerMain game) => _game = game;

    public void Reset()
    {
    }

    public void Update()
    {
        _orbitAngle = (_orbitAngle + 128) & (Track.MaxAngle - 1);

        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;
        const long radius = (Track.TrackCubes - 2) * (long)Track.CubeSize / AmigaTrig.Precision;

        long viewX = centre + (AmigaTrig.Sin(_orbitAngle) * radius);
        const long viewY = -3L * Track.CubeSize;
        long viewZ = centre + (AmigaTrig.Cos(_orbitAngle) * radius);

        _game.Camera.LookAt(viewX, viewY, viewZ, centre, 0, centre);

        // track selection
        for (ConsoleKey key = ConsoleKey.D1; key <= ConsoleKey.D8; key++)
        {
            if (_game.Keyboard.IsPressed(key) && _game.Track.Id != (TrackId)(key - ConsoleKey.D1))
            {
                _game.LoadTrack((TrackId)(key - ConsoleKey.D1));
            }
        }

        if (_game.Keyboard.IsPressed(ConsoleKey.S))
        {
            _game.Screens.Set(GameMode.TrackPreview);
        }
    }

    public void Draw()
    {
        // the original only shows the opponent outside the track menu
        _game.DrawWorld(showOpponent: false);

        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        float height = _game.Graphics.ScreenHeight;

        _game.Graphics.DrawTextLeft(new(2, 40), "Choose track :-", StuntCarRacerMain.SmallFont, yellow);

        for (int i = 0; i < 8; i++)
        {
            string name = (TrackId)i switch
            {
                TrackId.LittleRamp => "Little Ramp",
                TrackId.SteppingStones => "Stepping Stones",
                TrackId.HumpBack => "Hump Back",
                TrackId.BigRamp => "Big Ramp",
                TrackId.SkiJump => "Ski Jump",
                TrackId.DrawBridge => "Draw Bridge",
                TrackId.HighJump => "High Jump",
                _ => "Roller Coaster",
            };

            uint colour = _game.Track.Id == (TrackId)i ? white : yellow;
            _game.Graphics.DrawTextLeft(
                new(2, 60 + (i * 16)),
                string.Create(CultureInfo.InvariantCulture, $"'{i + 1}' -  {name}"),
                StuntCarRacerMain.SmallFont,
                colour);
        }

        _game.Graphics.DrawTextLeft(
            new(2, height - 34),
            $"Current track - {_game.Track.Name}.",
            StuntCarRacerMain.SmallFont,
            yellow);
        _game.Graphics.DrawTextLeft(
            new(2, height - 18),
            "Press 'S' to select, Escape to quit",
            StuntCarRacerMain.SmallFont,
            yellow);
    }
}
