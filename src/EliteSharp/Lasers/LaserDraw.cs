// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp.Lasers
{
    internal sealed class LaserDraw
    {
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IDraw _draw;

        internal LaserDraw(GameState gameState, IGraphics graphics, IDraw draw)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
        }

        internal void DrawLaserLines()
        {
            Vector2 point = new()
            {
                X = RNG.Random(126, 130) * _graphics.Scale,
                Y = RNG.Random(94, 98) * _graphics.Scale,
            };

            if (_gameState.Config.UseWireframe)
            {
                // Left laser
                _graphics.DrawTriangle(new(32 * _graphics.Scale, _draw.Bottom), point, new(48 * _graphics.Scale, _draw.Bottom), Colour.LighterRed);

                // Right laser
                _graphics.DrawTriangle(new(208 * _graphics.Scale, _draw.Bottom), point, new(224 * _graphics.Scale, _draw.Bottom), Colour.LighterRed);
            }
            else
            {
                // Left laser
                _graphics.DrawTriangleFilled(new(32 * _graphics.Scale, _draw.Bottom), point, new(48 * _graphics.Scale, _draw.Bottom), Colour.LighterRed);

                // Right laser
                _graphics.DrawTriangleFilled(new(208 * _graphics.Scale, _draw.Bottom), point, new(224 * _graphics.Scale, _draw.Bottom), Colour.LighterRed);
            }
        }

        internal void DrawLaserSights(LaserType laserType)
        {
            if (laserType == LaserType.None)
            {
                return;
            }

            float x1 = 128 * _graphics.Scale;
            float y1 = (96 - 8) * _graphics.Scale;
            float y2 = (96 - 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), Colour.LightGrey);
            _graphics.DrawLine(new(x1, y1), new(x1, y2), Colour.White);
            _graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), Colour.LightGrey);

            y1 = (96 + 8) * _graphics.Scale;
            y2 = (96 + 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), Colour.LightGrey);
            _graphics.DrawLine(new(x1, y1), new(x1, y2), Colour.White);
            _graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), Colour.LightGrey);

            x1 = (128f - 8f) * _graphics.Scale;
            y1 = 96f * _graphics.Scale;
            float x2 = (128 - 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), Colour.LightGrey);
            _graphics.DrawLine(new(x1, y1), new(x2, y1), Colour.White);
            _graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), Colour.LightGrey);

            x1 = (128 + 8) * _graphics.Scale;
            x2 = (128 + 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), Colour.LightGrey);
            _graphics.DrawLine(new(x1, y1), new(x2, y1), Colour.White);
            _graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), Colour.LightGrey);
        }
    }
}
