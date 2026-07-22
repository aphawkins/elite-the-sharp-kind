// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerSharpLib.Fakes;

// Records drawing calls so tests can assert on rendered output.
public sealed class RecordingGraphics(float screenWidth, float screenHeight) : IGraphics
{
    public IList<(Vector2[] Points, FastColor Colour)> FilledPolygons { get; } = [];

    public IList<(Vector2[] Points, Vector2[] TextureCoords, FastBitmap Texture)> TexturedPolygons { get; } = [];

    public IList<(string ImageType, Vector2 Position, Vector2 Size, Vector2 SourcePosition, Vector2 SourceSize)> ImageParts { get; } = [];

    public IList<(Vector2 Position, float Width, float Height, FastColor Colour)> FilledRectangles { get; } = [];

    public IList<(Vector2 Position, string Text, string FontType, FastColor Colour)> LeftTexts { get; } = [];

    public IList<(Vector2 Position, string Text, string FontType, FastColor Colour)> RightTexts { get; } = [];

    public int ClearCount { get; private set; }

    public int ClearDepthCount { get; private set; }

    public int ScreenUpdateCount { get; private set; }

    public IList<string> SavedScreenPaths { get; } = [];

    public float Scale => 1;

    public float ScreenHeight { get; } = screenHeight;

    public float ScreenWidth { get; } = screenWidth;

    public void Clear() => ClearCount++;

    public void ClearDepth() => ClearDepthCount++;

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
        => ImageParts.Add((imageType, position, size, sourcePosition, sourceSize));

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
        => FilledPolygons.Add((points, faceColor));

    public void DrawPolygonFilledDepth(Vector2[] points, float[] depths, FastColor faceColor)
        => FilledPolygons.Add((points, faceColor));

    public void DrawPolygonTextured(Vector2[] points, Vector2[] textureCoords, FastBitmap texture)
        => TexturedPolygons.Add((points, textureCoords, texture));

    public void DrawPolygonTexturedDepth(Vector2[] points, float[] depths, Vector2[] textureCoords, FastBitmap texture)
        => TexturedPolygons.Add((points, textureCoords, texture));

    public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
    {
    }

    public void DrawRectangleCentre(float y, float width, float height, FastColor color)
    {
    }

    public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
        => FilledRectangles.Add((position, width, height, color));

    public void DrawTextCentre(float y, string text, string fontType, FastColor color)
    {
    }

    public void DrawTextLeft(Vector2 position, string text, string fontType, FastColor color)
        => LeftTexts.Add((position, text, fontType, color));

    public void DrawTextRight(Vector2 position, string text, string fontType, FastColor color)
        => RightTexts.Add((position, text, fontType, color));

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
    }

    public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color)
    {
    }

    public void ScreenUpdate() => ScreenUpdateCount++;

    public void SaveScreen(string path) => SavedScreenPaths.Add(path);

    public void SetClipRegion(Vector2 position, float width, float height)
    {
    }
}
