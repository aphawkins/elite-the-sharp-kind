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

        void DrawCircle(Vector2 centre, float radius, GFX_COL colour);

        void DrawCircleFilled(Vector2 centre, float radius, GFX_COL colour);

        void DrawLine(float x1, float y1, float x2, float y2, GFX_COL colour);

        void DrawLine(float x1, float y1, float x2, float y2);

        void DrawLine(float x1, float y1, float x2, float y2, int dist, GFX_COL colour);

        void DrawPolygon(Vector2[] point_list, GFX_COL face_colour, int zavg);

        void DrawRectangle(int x, int y, int width, int height, GFX_COL colour);

        void DrawScanner();

        void DrawSprite(IMG spriteImgage, int x, int y);

        void DrawText(int x, int y, string text, GFX_COL colour);

        void DrawText(int x, int y, string text);

        void DrawTextCentre(int y, string text, int psize, GFX_COL colour);

        void DrawTextPretty(int x, int y, int width, int height, string text);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour);

        void GraphicsShutdown();

        bool GraphicsStartup();

        void PlotPixel(Vector2 position, GFX_COL colour);

        void PlotPixelFast(Vector2 position, GFX_COL colour);

        void RenderFinish();

        void RenderStart();

        bool RequestFile(string title, string path, string ext);

        void ScreenAcquire();

        void ScreenRelease();

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(int x, int y, int width, int height);
    }
}