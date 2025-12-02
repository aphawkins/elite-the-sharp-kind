// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Fakes;

public sealed class FakeGraphics : IGraphics
{
    public float Scale => 2;
    public float ScreenHeight { get; }
    public float ScreenWidth { get; }

    public void Clear() => throw new NotImplementedException();
    public void DrawCircle(Vector2 centre, float radius, uint color) => throw new NotImplementedException();
    public void DrawCircleFilled(Vector2 centre, float radius, uint color) => throw new NotImplementedException();
    public void DrawImage(int imageType, Vector2 position) => throw new NotImplementedException();
    public void DrawImageCentre(int imageType, float y) => throw new NotImplementedException();
    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, uint color) => throw new NotImplementedException();
    public void DrawPixel(Vector2 position, uint color) => throw new NotImplementedException();
    public void DrawPolygon(Vector2[] points, uint lineColor) => throw new NotImplementedException();
    public void DrawPolygonFilled(Vector2[] points, uint faceColor) => throw new NotImplementedException();
    public void DrawRectangle(Vector2 position, float width, float height, uint color) => throw new NotImplementedException();
    public void DrawRectangleCentre(float y, float width, float height, uint color) => throw new NotImplementedException();
    public void DrawRectangleFilled(Vector2 position, float width, float height, uint color) => throw new NotImplementedException();
    public void DrawTextCentre(float y, string text, int fontType, uint color) => throw new NotImplementedException();
    public void DrawTextLeft(Vector2 position, string text, int fontType, uint color) => throw new NotImplementedException();
    public void DrawTextRight(Vector2 position, string text, int fontType, uint color) => throw new NotImplementedException();
    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, uint color) => throw new NotImplementedException();
    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, uint color) => throw new NotImplementedException();
    public void ScreenUpdate() => throw new NotImplementedException();
    public void SetClipRegion(Vector2 position, float width, float height) => throw new NotImplementedException();
}
