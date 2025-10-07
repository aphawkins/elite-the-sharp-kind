// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Ships;
using Useful.Graphics;

namespace EliteSharp.Graphics;

internal interface IEliteDraw
{
    public float Bottom { get; }

    public Vector2 Centre { get; }

    public IGraphics Graphics { get; }

    public float Left { get; }

    public float Offset { get; }

    public float Right { get; }

    public float ScannerLeft { get; }

    public float ScannerRight { get; }

    public float ScannerTop { get; }

    public float Top { get; }

    public void DrawBorder();

    public void DrawHyperspaceCountdown(int countdown);

    public void DrawObject(IObject obj);

    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor, float averageZ);

    public void DrawTextPretty(Vector2 position, float width, string text);

    public void DrawViewHeader(string title);

    public void RenderEnd();

    public void RenderStart();

    public void SetFullScreenClipRegion();

    public void SetViewClipRegion();
}
