namespace Elite.Engine
{
    using System.Numerics;
    using Elite.Engine.Enums;

    internal class Laser
    {
        private readonly IGfx _gfx;

        internal Laser(IGfx gfx)
        {
            _gfx = gfx;
        }

        internal void draw_laser_sights(int laserType)
        {
            if (laserType == 0)
            {
                return;
            }

            float x1 = 128 * gfx.GFX_SCALE;
            float y1 = (96 - 8) * gfx.GFX_SCALE;
            float y2 = (96 - 16) * gfx.GFX_SCALE;

            _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

            y1 = (96 + 8) * gfx.GFX_SCALE;
            y2 = (96 + 16) * gfx.GFX_SCALE;

            _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

            x1 = (128f - 8f) * gfx.GFX_SCALE;
            y1 = 96f * gfx.GFX_SCALE;
            float x2 = (128 - 16) * gfx.GFX_SCALE;

            _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);

            x1 = (128 + 8) * gfx.GFX_SCALE;
            x2 = (128 + 16) * gfx.GFX_SCALE;

            _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
            _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
            _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);
        }

        internal void DrawLaserLines()
        {
            Vector2 point = new()
            {
                X = RNG.Random(126, 129) * gfx.GFX_SCALE,
                Y = RNG.Random(94, 97) * gfx.GFX_SCALE,
            };

            if (elite.config.UseWireframe)
            {
                // Left laser
                _gfx.DrawTriangle(new(32 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(48 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangle(new(208 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(224 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
            else
            {
                // Left laser
                _gfx.DrawTriangleFilled(new(32 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(48 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
                // Right laser
                _gfx.DrawTriangleFilled(new(208 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), point, new(224 * gfx.GFX_SCALE, gfx.GFX_VIEW_BY), GFX_COL.GFX_COL_RED);
            }
        }
    }
}
