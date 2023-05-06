using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.Engine.Lasers
{
    internal class LaserDraw
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;

        internal LaserDraw(GameState gameState, IGfx gfx)
        {
            _gameState = gameState;
            _gfx = gfx;
        }

        internal void DrawLaserSights(LaserType laserType)
        {
            if (laserType == LaserType.None)
            {
                return;
            }

            float x1 = 128 * Graphics.GFX_SCALE;
            float y1 = (96 - 8) * Graphics.GFX_SCALE;
            float y2 = (96 - 16) * Graphics.GFX_SCALE;

            _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

            y1 = (96 + 8) * Graphics.GFX_SCALE;
            y2 = (96 + 16) * Graphics.GFX_SCALE;

            _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

            x1 = (128f - 8f) * Graphics.GFX_SCALE;
            y1 = 96f * Graphics.GFX_SCALE;
            float x2 = (128 - 16) * Graphics.GFX_SCALE;

            _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);

            x1 = (128 + 8) * Graphics.GFX_SCALE;
            x2 = (128 + 16) * Graphics.GFX_SCALE;

            _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);
        }

        internal void DrawLaserLines()
        {
            Vector2 point = new()
            {
                X = RNG.Random(126, 129) * Graphics.GFX_SCALE,
                Y = RNG.Random(94, 97) * Graphics.GFX_SCALE,
            };

            if (_gameState.Config.UseWireframe)
            {
                // Left laser
                _gfx.DrawTriangle(new(32 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), point, new(48 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangle(new(208 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), point, new(224 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
            else
            {
                // Left laser
                _gfx.DrawTriangleFilled(new(32 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), point, new(48 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangleFilled(new(208 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), point, new(224 * Graphics.GFX_SCALE, Graphics.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
        }
    }
}
