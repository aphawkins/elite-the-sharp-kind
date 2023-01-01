/**
 *
 * Elite - The New Kind.
 *
 * Allegro version of Graphics routines.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999-2001.
 * email: <christian@newkind.co.uk>
 *
 * Routines for drawing anti-aliased lines and circles by T.Harte.
 *
 **/

namespace Elite
{
    using System.Numerics;
    using Elite.Enums;

    public interface IGfx
    {
        void ClearArea(int tx, int ty, int bx, int by);

        void ClearDisplay();

        void ClearTextArea();

        void DisplayText(int x, int y, string txt, GFX_COL col);

        void DisplayText(int x, int y, string txt);

        void DisplayTextCentre(int y, string str, int psize, GFX_COL col);

        void DisplayTextPretty(int tx, int ty, int bx, int by, string txt);

        void DrawCircle(int cx, int cy, int radius, GFX_COL circle_colour);

        void DrawCircleFilled(int cx, int cy, int radius, GFX_COL circle_colour);

        void DrawLine(int x1, int y1, int x2, int y2, GFX_COL line_colour);

        void DrawLine(int x1, int y1, int x2, int y2);

        void DrawLine(int x1, int y1, int x2, int y2, int dist, GFX_COL col);

        void DrawLineXor(int x1, int y1, int x2, int y2, GFX_COL line_colour);

        void DrawPolygon(Vector2[] point_list, GFX_COL face_colour, int zavg);

        void DrawRectangle(int tx, int ty, int bx, int by, GFX_COL col);

        void DrawScanner();

        void DrawSprite(IMG sprite_no, int x, int y);

        void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, GFX_COL col);

        void GraphicsShutdown();

        int GraphicsStartup();

        void PlotPixel(int x, int y, GFX_COL col);

        void PlotPixelFast(int x, int y, GFX_COL col);

        void RenderFinish();

        void RenderStart();

        bool RequestFile(string title, string path, string ext);

        void ScreenAcquire();

        void ScreenRelease();

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(int tx, int ty, int bx, int by);
    }
}