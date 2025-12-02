// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics;

public interface IGraphics
{
    public float Scale { get; }

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    public void Clear();

    public void DrawCircle(Vector2 centre, float radius, uint color);

    public void DrawCircleFilled(Vector2 centre, float radius, uint color);

    public void DrawImage(int imageType, Vector2 position);

    public void DrawImageCentre(int imageType, float y);

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, uint color);

    public void DrawPixel(Vector2 position, uint color);

    public void DrawPolygon(Vector2[] points, uint lineColor);

    public void DrawPolygonFilled(Vector2[] points, uint faceColor);

    public void DrawRectangle(Vector2 position, float width, float height, uint color);

    public void DrawRectangleCentre(float y, float width, float height, uint color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, uint color);

    public void DrawTextCentre(float y, string text, int fontType, uint color);

    public void DrawTextLeft(Vector2 position, string text, int fontType, uint color);

    public void DrawTextRight(Vector2 position, string text, int fontType, uint color);

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, uint color);

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, uint color);

    /// <summary>
    /// Blit the back buffer to the screen.
    /// </summary>
    public void ScreenUpdate();

    public void SetClipRegion(Vector2 position, float width, float height);
}
