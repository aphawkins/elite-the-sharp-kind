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

namespace Elite.Engine
{
    using System.Numerics;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    public interface IGfx
    {
        void LoadBitmap(Image imgType, Stream bitmapStream);

        void ClearArea(float x, float y, float width, float height);

        void DrawCircle(Vector2 centre, float radius, GFX_COL colour);

        void DrawCircleFilled(Vector2 centre, float radius, GFX_COL colour);

        void DrawImage(Image spriteImgage, Vector2 location);

        void DrawLine(Vector2 start, Vector2 end, GFX_COL colour);

        void DrawLine(Vector2 start, Vector2 end);

        void DrawPixel(Vector2 position, GFX_COL colour);

        void DrawPixelFast(Vector2 position, GFX_COL colour);

        void DrawPolygonFilled(Vector2[] pointList, GFX_COL faceColour);

        void DrawPolygon(Vector2[] pointList, GFX_COL lineColour);

        void DrawRectangle(float x, float y, float width, float height, GFX_COL colour);

        void DrawRectangleFilled(float x, float y, float width, float height, GFX_COL colour);

        void DrawTextCentre(float y, string text, int psize, GFX_COL colour);

        void DrawTextLeft(float x, float y, string text, GFX_COL colour);

        void DrawTextRight(float x, float y, string text, GFX_COL colour);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour);

        void ScreenAcquire();

        void ScreenRelease();

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(float x, float y, float width, float height);
    }
}