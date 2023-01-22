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
        int SpeedCap { get; set; }

        void ClearArea(float x, float y, float width, float height);

        void ClearDisplay();

        void ClearTextArea();

        void DrawCircle(Vector2 centre, float radius, GFX_COL colour);

        void DrawCircleFilled(Vector2 centre, float radius, GFX_COL colour);

        void DrawLine(Vector2 start, Vector2 end, GFX_COL colour);

        void DrawLine(Vector2 start, Vector2 end);

        void DrawPolygonFilled(Vector2[] point_list, GFX_COL face_colour);

        void DrawRectangleFilled(float x, float y, float width, float height, GFX_COL colour);

        void DrawRectangle(float x, float y, float width, float height, GFX_COL colour);

        void DrawScanner();

        void DrawSprite(IMG spriteImgage, Vector2 location);

        void DrawTextLeft(float x, float y, string text, GFX_COL colour);

        void DrawTextRight(float x, float y, string text, GFX_COL colour);

        void DrawTextCentre(float y, string text, int psize, GFX_COL colour);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour);

        void PlotPixel(Vector2 position, GFX_COL colour);

        void PlotPixelFast(Vector2 position, GFX_COL colour);
        void ScreenAcquire();

        void ScreenRelease();

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(float x, float y, float width, float height);
    }
}