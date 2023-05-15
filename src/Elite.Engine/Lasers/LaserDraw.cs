// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    internal sealed class LaserDraw
    {
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;

        internal LaserDraw(GameState gameState, IGraphics graphics)
        {
            _gameState = gameState;
            _graphics = graphics;
        }

        internal void DrawLaserLines()
        {
            Vector2 point = new()
            {
                X = RNG.Random(126, 129) * _graphics.Scale,
                Y = RNG.Random(94, 97) * _graphics.Scale,
            };

            if (_gameState.Config.UseWireframe)
            {
                // Left laser
                _graphics.DrawTriangle(new(32 * _graphics.Scale, _graphics.ViewB.Y), point, new(48 * _graphics.Scale, _graphics.ViewB.Y), Colour.Red1);

                // Right laser
                _graphics.DrawTriangle(new(208 * _graphics.Scale, _graphics.ViewB.Y), point, new(224 * _graphics.Scale, _graphics.ViewB.Y), Colour.Red1);
            }
            else
            {
                // Left laser
                _graphics.DrawTriangleFilled(new(32 * _graphics.Scale, _graphics.ViewB.Y), point, new(48 * _graphics.Scale, _graphics.ViewB.Y), Colour.Red1);

                // Right laser
                _graphics.DrawTriangleFilled(new(208 * _graphics.Scale, _graphics.ViewB.Y), point, new(224 * _graphics.Scale, _graphics.ViewB.Y), Colour.Red1);
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

            _graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), Colour.Grey1);
            _graphics.DrawLine(new(x1, y1), new(x1, y2), Colour.White1);
            _graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), Colour.Grey1);

            y1 = (96 + 8) * _graphics.Scale;
            y2 = (96 + 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), Colour.Grey1);
            _graphics.DrawLine(new(x1, y1), new(x1, y2), Colour.White1);
            _graphics.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), Colour.Grey1);

            x1 = (128f - 8f) * _graphics.Scale;
            y1 = 96f * _graphics.Scale;
            float x2 = (128 - 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), Colour.Grey1);
            _graphics.DrawLine(new(x1, y1), new(x2, y1), Colour.White1);
            _graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), Colour.Grey1);

            x1 = (128 + 8) * _graphics.Scale;
            x2 = (128 + 16) * _graphics.Scale;

            _graphics.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), Colour.Grey1);
            _graphics.DrawLine(new(x1, y1), new(x2, y1), Colour.White1);
            _graphics.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), Colour.Grey1);
        }
    }
}
