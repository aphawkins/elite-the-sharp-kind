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

        void DrawCircle(Vector2 centre, float radius, FastColor colour);

        void DrawCircleFilled(Vector2 centre, float radius, FastColor colour);

        void DrawImage(ImageType image, Vector2 position);

        void DrawImageCentre(ImageType image, float y);

        void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor colour);

        void DrawPixel(Vector2 position, FastColor colour);

        void DrawPixelFast(Vector2 position, FastColor colour);

        void DrawPolygon(Vector2[] points, FastColor lineColour);

        void DrawPolygonFilled(Vector2[] points, FastColor faceColour);

        void DrawRectangle(Vector2 position, float width, float height, FastColor colour);

        void DrawRectangleCentre(float y, float width, float height, FastColor colour);

        void DrawRectangleFilled(Vector2 position, float width, float height, FastColor colour);

        void DrawTextCentre(float y, string text, FontSize fontSize, FastColor colour);

        void DrawTextLeft(Vector2 position, string text, FastColor colour);

        void DrawTextRight(Vector2 position, string text, FastColor colour);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor colour);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor colour);

        void LoadBitmap(ImageType imgType, string bitmapPath);

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(Vector2 position, float width, float height);

        void Clear();
    }
}
