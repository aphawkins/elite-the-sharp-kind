// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Fakes;

public sealed class FakeGraphics : IGraphics
{
    public float Scale => 2;

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    public void Clear()
    {
    }

    public void ClearDepth()
    {
    }

    public void DrawCircle(Vector2 centre, float radius, FastColor color)
    {
    }

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color)
    {
    }

    public void DrawImage(string imageType, Vector2 position)
    {
    }

    public void DrawImageCentre(string imageType, float y)
    {
    }

    public void DrawImagePart(string imageType, Vector2 position, Vector2 size, Vector2 sourcePosition, Vector2 sourceSize)
    {
    }

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color)
    {
    }

    public void DrawPixel(Vector2 position, FastColor color)
    {
    }

    public void DrawPolygon(Vector2[] points, FastColor lineColor)
    {
    }

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor)
    {
    }

    public void DrawPolygonFilledDepth(Vector2[] points, float[] depths, FastColor faceColor)
    {
    }

    public void DrawPolygonTextured(Vector2[] points, Vector2[] textureCoords, FastBitmap texture)
    {
    }

    public void DrawPolygonTexturedDepth(Vector2[] points, float[] depths, Vector2[] textureCoords, FastBitmap texture)
    {
    }

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
    {
    }

    public void DrawRectangleCentre(float y, float width, float height, FastColor color)
    {
    }

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
    {
    }

    public void DrawTextCentre(float y, string text, string fontType, FastColor color)
    {
    }

    public void DrawTextLeft(Vector2 position, string text, string fontType, FastColor color)
    {
    }

    public void DrawTextRight(Vector2 position, string text, string fontType, FastColor color)
    {
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
    }

    public void ScreenUpdate()
    {
    }

    public void SetClipRegion(Vector2 position, float width, float height)
    {
    }
}
