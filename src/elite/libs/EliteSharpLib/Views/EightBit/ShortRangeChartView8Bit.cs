// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit short-range chart, authored for the 320x256 canvas: half the
/// 16-bit tier's cross and text offsets, matching its Scale of 1.
/// </summary>
internal sealed class ShortRangeChartView8Bit : BaseView8Bit, IView<ShortRangeChartModel>
{
    private const int TextColumn = 1;
    private const int NameRow = 22;
    private const int DistanceRow = 23;
    private const int CrossSize = 8;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorRed;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorYellow;

    internal ShortRangeChartView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorRed = draw.Palette["Red"];
        _colorWhite = draw.Palette["White"];
        _colorYellow = draw.Palette["Yellow"];
    }

    public void Draw(ShortRangeChartModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Title);

        // Fuel radius
        Vector2 centre = _draw.Layout.ViewportCentre;
        float scale = _draw.Layout.Scale;
        float radius = model.FuelLightYears * 10 * scale;
        float crossSize = CrossSize * scale;
        Graphics.DrawCircle(centre, radius, _colorGreen);
        Graphics.DrawLine(new(centre.X, centre.Y - crossSize), new(centre.X, centre.Y + crossSize), _colorWhite);
        Graphics.DrawLine(new(centre.X - crossSize, centre.Y), new(centre.X + crossSize, centre.Y), _colorWhite);

        // The packing places these against galaxy coordinates, which land
        // anywhere; the blobs they label keep their exact positions, but the
        // text snaps to the grid like every other 8-bit string.
        foreach ((Vector2 position, string name) in model.Labels)
        {
            Graphics.DrawTextLeft(SnapToGrid(position), name, nameof(FontType.Small), _colorWhite);
        }

        foreach ((Vector2 position, float size) in model.Planets)
        {
            Graphics.DrawCircleFilled(position, size, _colorYellow);
        }

        // Cross
        Graphics.DrawLine(new(model.Cross.X - CrossSize, model.Cross.Y), new(model.Cross.X + CrossSize, model.Cross.Y), _colorRed);
        Graphics.DrawLine(new(model.Cross.X, model.Cross.Y - CrossSize), new(model.Cross.X, model.Cross.Y + CrossSize), _colorRed);

        Graphics.DrawTextLeft(new(Column(TextColumn), Row(NameRow)), model.Caption, nameof(FontType.Small), _colorGreen);
        Graphics.DrawTextLeft(new(Column(TextColumn), Row(DistanceRow)), model.Detail, nameof(FontType.Small), _colorWhite);
    }
}
