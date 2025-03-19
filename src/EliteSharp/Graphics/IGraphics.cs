// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets.Fonts;

namespace EliteSharp.Graphics;

public interface IGraphics : IDisposable
{
    public float ScreenHeight { get; }

    public float Scale { get; }

    public float ScreenWidth { get; }

    public void DrawCircle(Vector2 centre, float radius, FastColor color);

    public void DrawCircleFilled(Vector2 centre, float radius, FastColor color);

    public void DrawImage(ImageType image, Vector2 position);

    public void DrawImageCentre(ImageType image, float y);

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color);

    public void DrawPixel(Vector2 position, FastColor color);

    public void DrawPolygon(Vector2[] points, FastColor lineColor);

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor);

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color);

    public void DrawRectangleCentre(float y, float width, float height, FastColor color);

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color);

    public void DrawTextCentre(float y, string text, FontType fontType, FastColor color);

    public void DrawTextLeft(Vector2 position, string text, FastColor color);

    public void DrawTextRight(Vector2 position, string text, FastColor color);

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color);

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color);

    /// <summary>
    /// Blit the back buffer to the screen.
    /// </summary>
    public void ScreenUpdate();

    public void SetClipRegion(Vector2 position, float width, float height);

    public void Clear();
}
