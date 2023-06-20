// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Graphics
{
    public interface IGraphics : IDisposable
    {
        Vector2 Centre { get; }

        Vector2 Offset { get; }

        float Scale { get; }

        Vector2 ViewB { get; }

        Vector2 ViewT { get; }

        void ClearArea(float x, float y, float width, float height);

        void DrawCircle(Vector2 centre, float radius, Colour colour);

        void DrawCircleFilled(Vector2 centre, float radius, Colour colour);

        void DrawImage(Image spriteImgage, Vector2 location);

        void DrawLine(Vector2 lineStart, Vector2 lineEnd, Colour colour);

        void DrawLine(Vector2 lineStart, Vector2 lineEnd);

        void DrawPixel(Vector2 position, Colour colour);

        void DrawPixelFast(Vector2 position, Colour colour);

        void DrawPolygon(Vector2[] pointList, Colour lineColour);

        void DrawPolygonFilled(Vector2[] pointList, Colour faceColour);

        void DrawRectangle(float x, float y, float width, float height, Colour colour);

        void DrawRectangleFilled(float x, float y, float width, float height, Colour colour);

        void DrawTextCentre(float y, string text, FontSize fontSize, Colour colour);

        void DrawTextLeft(float x, float y, string text, Colour colour);

        void DrawTextRight(float x, float y, string text, Colour colour);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Colour colour);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, Colour colour);

        void LoadBitmap(Image imgType, byte[] bitmapBytes);

        void ScreenAcquire();

        void ScreenRelease();

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(float x, float y, float width, float height);
    }
}
