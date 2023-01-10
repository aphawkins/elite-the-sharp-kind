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
        void ClearArea(int x, int y, int width, int height);

        void ClearDisplay();

        void ClearTextArea();

        void DrawCircle(int cx, int cy, int radius, GFX_COL colour);

        void DrawCircleFilled(int cx, int cy, int radius, GFX_COL colour);

        void DrawLine(float x1, float y1, float x2, float y2, GFX_COL colour);

        void DrawLine(float x1, float y1, float x2, float y2);

        void DrawLine(int x1, int y1, int x2, int y2, int dist, GFX_COL colour);

        void DrawLineXor(float x1, float y1, float x2, float y2, GFX_COL colour);

        void DrawPolygon(Vector2[] point_list, GFX_COL face_colour, int zavg);

        void DrawRectangle(int x, int y, int width, int height, GFX_COL colour);

        void DrawScanner();

        void DrawSprite(IMG spriteImgage, int x, int y);

        void DrawText(int x, int y, string text, GFX_COL colour);

        void DrawText(int x, int y, string text);

        void DrawTextCentre(int y, string text, int psize, GFX_COL colour);

        void DrawTextPretty(int tx, int ty, int bx, int by, string text);

        void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, GFX_COL colour);

        void GraphicsShutdown();

        bool GraphicsStartup();

        void PlotPixel(int x, int y, GFX_COL colour);

        void PlotPixelFast(int x, int y, GFX_COL colour);

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