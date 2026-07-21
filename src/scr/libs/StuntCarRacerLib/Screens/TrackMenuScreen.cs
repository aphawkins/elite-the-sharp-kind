// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using System.Numerics;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Abstraction;
using Useful.Controls;
using Useful.Graphics;

namespace StuntCarRacerLib.Screens;

// The track list and orbiting camera (original HandleTrackMenu /
// CalcTrackMenuViewpoint). Not throttled by the frame gap.
//
// The original's HandleTrackMenu is plain text over the 3D world (the
// Bitmap/menu.png background art in stuntcarremake is never actually
// loaded by that game's code). This drawn overlay is a cosmetic addition
// beyond strict porting: menu.png's centre panel is transparent, so the
// orbiting track view still shows through, with the track list positioned
// inside the panel.
internal sealed class TrackMenuScreen : IGameScreen
{
    private const string MenuImage = "Menu";

    // menu.png is 320x200; its centre panel (inset from the measured
    // transparent bounds to clear the wooden frame/logo) in that space.
    private const float CanvasWidth = 320f;
    private const float PanelLeft = 38f;
    private const float PanelTop = 70f;
    private const float PanelBottom = 190f;
    private const float RowHeight = 11f;

    private readonly Race _race;
    private readonly IKeyboard _keyboard;
    private readonly ScreenManager<GameMode, IGameScreen> _screens;
    private readonly IGraphics _graphics;
    private readonly ScrPalette _palette;
    private int _orbitAngle;

    internal TrackMenuScreen(
        Race race,
        IKeyboard keyboard,
        ScreenManager<GameMode, IGameScreen> screens,
        IGraphics graphics,
        ScrPalette palette)
    {
        _race = race;
        _keyboard = keyboard;
        _screens = screens;
        _graphics = graphics;
        _palette = palette;
    }

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

        _race.Camera.LookAt(viewX, viewY, viewZ, centre, 0, centre);

        // track selection
        for (ConsoleKey key = ConsoleKey.D1; key <= ConsoleKey.D8; key++)
        {
            if (_keyboard.IsPressed(key) && _race.Track.Id != (TrackId)(key - ConsoleKey.D1))
            {
                _race.LoadTrack((TrackId)(key - ConsoleKey.D1));
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.S))
        {
            _screens.Set(GameMode.TrackPreview);
        }
    }

    public void Draw()
    {
        // the original only shows the opponent outside the track menu
        _race.DrawWorld(showOpponent: false);

        // menu.png's frame draws over the world; its centre panel is
        // transparent so the orbiting track view still shows through
        _graphics.DrawImagePart(
            MenuImage,
            Vector2.Zero,
            new(_graphics.ScreenWidth, _graphics.ScreenHeight),
            Vector2.Zero,
            new(CanvasWidth, 200f));

        float scale = _graphics.ScreenWidth / CanvasWidth;
        FastColor yellow = _palette.Colour(Track.ScrBaseColour + 3);
        FastColor white = _palette.Colour(Track.ScrBaseColour + 15);

        _graphics.DrawTextLeft(new(PanelLeft * scale, PanelTop * scale), "Choose track :-", StuntCarRacerMain.SmallFont, yellow);

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

            FastColor colour = _race.Track.Id == (TrackId)i ? white : yellow;
            float y = PanelTop + RowHeight + (i * RowHeight);
            _graphics.DrawTextLeft(
                new(PanelLeft * scale, y * scale),
                string.Create(CultureInfo.InvariantCulture, $"'{i + 1}' -  {name}"),
                StuntCarRacerMain.SmallFont,
                colour);
        }

        _graphics.DrawTextLeft(
            new(PanelLeft * scale, (PanelBottom - 18) * scale),
            $"Current track - {_race.Track.Name}.",
            StuntCarRacerMain.SmallFont,
            yellow);
        _graphics.DrawTextLeft(
            new(PanelLeft * scale, (PanelBottom - 6) * scale),
            "Press 'S' to select, Escape to quit",
            StuntCarRacerMain.SmallFont,
            yellow);
    }
}
