// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
using Useful.Assets;

namespace Useful.Graphics;

public interface IGraphics
{
    public bool IsInitialized { get; }

    public float Scale { get; }

    public float ScreenHeight { get; }

    public float ScreenWidth { get; }

    public void Clear();

    public void DrawCircle(Vector2 centre, float radius, FastColor color);

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color);

    public void DrawImage(int imageType, Vector2 position);

    public void DrawImageCentre(int imageType, float y);

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color);

    public void DrawPixel(Vector2 position, FastColor color);

    public void DrawPolygon(Vector2[] points, FastColor lineColor);

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor);

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color);

    public void DrawRectangleCentre(float y, float width, float height, FastColor color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color);

    public void DrawTextCentre(float y, string text, int fontType, FastColor color);

    public void DrawTextLeft(Vector2 position, string text, int fontType, FastColor color);

    public void DrawTextRight(Vector2 position, string text, int fontType, FastColor color);

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color);

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color);

    public void Initialize(IAssetLocator assetLocator, IEnumerable<FastColor> colors);

    /// <summary>
    /// Blit the back buffer to the screen.
    /// </summary>
    public void ScreenUpdate();

    public void SetClipRegion(Vector2 position, float width, float height);
}
