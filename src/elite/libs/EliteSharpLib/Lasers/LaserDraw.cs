// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;

namespace EliteSharpLib.Lasers;

internal sealed class LaserDraw
{
    private readonly GameState _gameState;
    private readonly IEliteDraw _draw;
    private readonly uint _colorPaleYellow;
    private readonly uint _colorRedOrange;
    private readonly uint _colorBrightPurple;
    private readonly RNG _rng;

    internal LaserDraw(GameState gameState, IEliteDraw draw, RNG rng)
    {
        _gameState = gameState;
        _draw = draw;
        _rng = rng;

        // The beam colour per laser type; beam and mining match their
        // crosshair sprite's shade, pulse and military fire red.
        _colorPaleYellow = _draw.Palette["PaleYellow"];
        _colorRedOrange = _draw.Palette["RedOrange"];
        _colorBrightPurple = _draw.Palette["BrightPurple"];
    }

    internal void DrawLaserLines(LaserType laserType)
    {
        uint color = BeamColor(laserType);

        Vector2 target = new()
        {
            X = _draw.Centre.X + (_rng.Random(0, 2) * _draw.Scale),
            Y = _draw.Centre.Y + (_rng.Random(0, 2) * _draw.Scale),
        };

        Vector2 leftA = new(_draw.ScannerLeft + (32 * _draw.Scale), _draw.Bottom);
        Vector2 leftB = new(_draw.ScannerLeft + (48 * _draw.Scale), _draw.Bottom);

        Vector2 rightA = new(_draw.ScannerRight - (32 * _draw.Scale), _draw.Bottom);
        Vector2 rightB = new(_draw.ScannerRight - (48 * _draw.Scale), _draw.Bottom);

        if (_gameState.Config.Game.LaserWireframe)
        {
            // Left laser
            _draw.Graphics.DrawTriangle(leftA, target, leftB, color);

            // Right laser
            _draw.Graphics.DrawTriangle(rightA, target, rightB, color);
        }
        else
        {
            // Left laser
            _draw.Graphics.DrawTriangleFilled(leftA, target, leftB, color);

            // Right laser
            _draw.Graphics.DrawTriangleFilled(rightA, target, rightB, color);
        }
    }

    // Each laser type has its own crosshair sprite, centred on the view.
    internal void DrawLaserSights(LaserType laserType)
    {
        if (laserType == LaserType.None)
        {
            return;
        }

        string image = CrosshairImage(laserType);
        _draw.Graphics.DrawImage(image, _draw.Centre - (_draw.Graphics.ImageSize(image) / 2));
    }

    private static string CrosshairImage(LaserType laserType) => laserType switch
    {
        LaserType.Pulse => nameof(ImageType.LaserPulse),
        LaserType.Beam => nameof(ImageType.LaserBeam),
        LaserType.Military => nameof(ImageType.LaserMilitary),
        LaserType.Mining => nameof(ImageType.LaserMining),
        _ => throw new ArgumentOutOfRangeException(nameof(laserType)),
    };

    private uint BeamColor(LaserType laserType) => laserType switch
    {
        LaserType.Beam => _colorPaleYellow,
        LaserType.Mining => _colorBrightPurple,
        _ => _colorRedOrange,
    };
}
