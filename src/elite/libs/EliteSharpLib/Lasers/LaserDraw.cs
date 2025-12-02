// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;

namespace EliteSharpLib.Lasers;

internal sealed class LaserDraw
{
    private readonly GameState _gameState;
    private readonly IEliteDraw _draw;
    private readonly uint _colorLighterRed;
    private readonly uint _colorLightGrey;
    private readonly uint _colorWhite;

    internal LaserDraw(GameState gameState, IEliteDraw draw)
    {
        _gameState = gameState;
        _draw = draw;
        _colorLighterRed = _draw.Palette["LighterRed"];
        _colorLightGrey = _draw.Palette["LightGrey"];
        _colorWhite = _draw.Palette["White"];
    }

    internal void DrawLaserLines()
    {
        Vector2 target = new()
        {
            X = RNG.Random((int)_draw.Centre.X / 2, (int)(_draw.Centre.X / 2) + 2) * _draw.Graphics.Scale,
            Y = RNG.Random((int)_draw.Centre.Y / 2, (int)(_draw.Centre.Y / 2) + 2) * _draw.Graphics.Scale,
        };

        Vector2 leftA = new((32 + (_draw.ScannerLeft / 2)) * _draw.Graphics.Scale, _draw.Bottom);
        Vector2 leftB = new((48 + (_draw.ScannerLeft / 2)) * _draw.Graphics.Scale, _draw.Bottom);

        Vector2 rightA = new(((_draw.ScannerRight / 2) - 32) * _draw.Graphics.Scale, _draw.Bottom);
        Vector2 rightB = new(((_draw.ScannerRight / 2) - 48) * _draw.Graphics.Scale, _draw.Bottom);

        if (_gameState.Config.ShipWireframe)
        {
            // Left laser
            _draw.Graphics.DrawTriangle(leftA, target, leftB, _colorLighterRed);

            // Right laser
            _draw.Graphics.DrawTriangle(rightA, target, rightB, _colorLighterRed);
        }
        else
        {
            // Left laser
            _draw.Graphics.DrawTriangleFilled(leftA, target, leftB, _colorLighterRed);

            // Right laser
            _draw.Graphics.DrawTriangleFilled(rightA, target, rightB, _colorLighterRed);
        }
    }

    internal void DrawLaserSights(LaserType laserType)
    {
        if (laserType == LaserType.None)
        {
            return;
        }

        // Top line
        float x1 = _draw.Centre.X / 2 * _draw.Graphics.Scale;
        float y1 = ((_draw.Centre.Y / 2) - 8) * _draw.Graphics.Scale;
        float y2 = ((_draw.Centre.Y / 2) - 16) * _draw.Graphics.Scale;
        _draw.Graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), _colorLightGrey);
        _draw.Graphics.DrawLine(new(x1, y1), new(x1, y2), _colorWhite);
        _draw.Graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), _colorLightGrey);

        // Bottom line
        y1 = ((_draw.Centre.Y / 2) + 8) * _draw.Graphics.Scale;
        y2 = ((_draw.Centre.Y / 2) + 16) * _draw.Graphics.Scale;
        _draw.Graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), _colorLightGrey);
        _draw.Graphics.DrawLine(new(x1, y1), new(x1, y2), _colorWhite);
        _draw.Graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), _colorLightGrey);

        // Left line
        x1 = ((_draw.Centre.X / 2) - 8) * _draw.Graphics.Scale;
        y1 = _draw.Centre.Y / 2 * _draw.Graphics.Scale;
        float x2 = ((_draw.Centre.X / 2) - 16) * _draw.Graphics.Scale;
        _draw.Graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), _colorLightGrey);
        _draw.Graphics.DrawLine(new(x1, y1), new(x2, y1), _colorWhite);
        _draw.Graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), _colorLightGrey);

        // Right line
        x1 = ((_draw.Centre.X / 2) + 8) * _draw.Graphics.Scale;
        x2 = ((_draw.Centre.X / 2) + 16) * _draw.Graphics.Scale;
        _draw.Graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), _colorLightGrey);
        _draw.Graphics.DrawLine(new(x1, y1), new(x2, y1), _colorWhite);
        _draw.Graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), _colorLightGrey);
    }
}
