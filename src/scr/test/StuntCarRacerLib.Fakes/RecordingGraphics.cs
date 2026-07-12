// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using Useful.Graphics;

namespace StuntCarRacerLib.Fakes;

// Records drawing calls so tests can assert on rendered output.
public sealed class RecordingGraphics(float screenWidth, float screenHeight) : IGraphics
{
    public IList<(Vector2[] Points, uint Colour)> FilledPolygons { get; } = [];

    public IList<(Vector2[] Points, Vector2[] TextureCoords, FastBitmap Texture)> TexturedPolygons { get; } = [];

    public IList<(string ImageType, Vector2 Position, Vector2 Size, Vector2 SourcePosition, Vector2 SourceSize)> ImageParts { get; } = [];

    public IList<(Vector2 Position, float Width, float Height, uint Colour)> FilledRectangles { get; } = [];

    public IList<(Vector2 Position, string Text, string FontType, uint Colour)> LeftTexts { get; } = [];

    public int ClearCount { get; private set; }

    public int ClearDepthCount { get; private set; }

    public int ScreenUpdateCount { get; private set; }

    public float Scale => 1;

    public float ScreenHeight { get; } = screenHeight;

    public float ScreenWidth { get; } = screenWidth;

    public void Clear() => ClearCount++;

    public void ClearDepth() => ClearDepthCount++;

    public void DrawCircle(Vector2 centre, float radius, uint color)
    {
    }

    public void DrawCircleFilled(Vector2 centre, float radius, uint color)
    {
    }

    public void DrawImage(string imageType, Vector2 position)
    {
    }

    public void DrawImageCentre(string imageType, float y)
    {
    }

    public void DrawImagePart(string imageType, Vector2 position, Vector2 size, Vector2 sourcePosition, Vector2 sourceSize)
        => ImageParts.Add((imageType, position, size, sourcePosition, sourceSize));

    public void DrawLine(Vector2 lineStart, Vector2 lineEnd, uint color)
    {
    }

    public void DrawPixel(Vector2 position, uint color)
    {
    }

    public void DrawPolygon(Vector2[] points, uint lineColor)
    {
    }

    public void DrawPolygonFilled(Vector2[] points, uint faceColor)
        => FilledPolygons.Add((points, faceColor));

    public void DrawPolygonFilledDepth(Vector2[] points, float[] depths, uint faceColor)
        => FilledPolygons.Add((points, faceColor));

    public void DrawPolygonTextured(Vector2[] points, Vector2[] textureCoords, FastBitmap texture)
        => TexturedPolygons.Add((points, textureCoords, texture));

    public void DrawPolygonTexturedDepth(Vector2[] points, float[] depths, Vector2[] textureCoords, FastBitmap texture)
        => TexturedPolygons.Add((points, textureCoords, texture));

    public void DrawRectangle(Vector2 position, float width, float height, uint color)
    {
    }

    public void DrawRectangleCentre(float y, float width, float height, uint color)
    {
    }

    public void DrawRectangleFilled(Vector2 position, float width, float height, uint color)
        => FilledRectangles.Add((position, width, height, color));

    public void DrawTextCentre(float y, string text, string fontType, uint color)
    {
    }

    public void DrawTextLeft(Vector2 position, string text, string fontType, uint color)
        => LeftTexts.Add((position, text, fontType, color));

    public void DrawTextRight(Vector2 position, string text, string fontType, uint color)
    {
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, uint color)
    {
    }

    public void ScreenUpdate() => ScreenUpdateCount++;

    public void SetClipRegion(Vector2 position, float width, float height)
    {
    }
}
