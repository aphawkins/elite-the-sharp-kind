// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets.Fonts;

namespace EliteSharp.Graphics
{
    public interface IGraphics : IDisposable
    {
        float ScreenHeight { get; }

        float Scale { get; }

        float ScreenWidth { get; }

        void DrawCircle(Vector2 centre, float radius, FastColor color);

        void DrawCircleFilled(Vector2 centre, float radius, FastColor color);

        void DrawImage(ImageType image, Vector2 position);

        void DrawImageCentre(ImageType image, float y);

        void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color);

        void DrawPixel(Vector2 position, FastColor color);

        void DrawPolygon(Vector2[] points, FastColor lineColor);

        void DrawPolygonFilled(Vector2[] points, FastColor faceColor);

        void DrawRectangle(Vector2 position, float width, float height, FastColor color);

        void DrawRectangleCentre(float y, float width, float height, FastColor color);

        void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color);

        void DrawTextCentre(float y, string text, FontType fontType, FastColor color);

        void DrawTextLeft(Vector2 position, string text, FastColor color);

        void DrawTextRight(Vector2 position, string text, FastColor color);

        void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color);

        void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color);

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        void ScreenUpdate();

        void SetClipRegion(Vector2 position, float width, float height);

        void Clear();
    }
}
