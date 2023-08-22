// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Graphics
{
    public interface IGraphics : IDisposable
    {
        float ScreenHeight { get; }

        float Scale { get; }

        float ScreenWidth { get; }

        void DrawCircle(Vector2 centre, float radius, EColor colour);

        void DrawCircleFilled(Vector2 centre, float radius, EColor colour);

        void DrawImage(Image image, Vector2 position);

        void DrawImageCentre(Image image, float y);

        void DrawLine(Vector2 lineStart, Vector2 lineEnd, EColor colour);

        void DrawPixel(Vector2 position, EColor colour);

        void DrawPixelFast(Vector2 position, EColor colour);

        void DrawPolygon(Vector2[] pointList, EColor lineColour);

        void DrawPolygonFilled(Vector2[] pointList, EColor faceColour);

        void DrawRectangle(Vector2 position, float width, float height, EColor colour);

        void DrawRectangleCentre(float y, float width, float height, EColor colour);

        void DrawRectangleFilled(Vector2 position, float width, float height, EColor colour);

        void DrawTextCentre(float y, string text, FontSize fontSize, EColor colour);

        void DrawTextLeft(Vector2 position, string text, EColor colour);

        void DrawTextRight(Vector2 position, string text, EColor colour);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, EColor colour);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, EColor colour);

        void LoadBitmap(Image imgType, byte[] bitmapBytes);

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(Vector2 position, float width, float height);

        void Clear();
    }
}
