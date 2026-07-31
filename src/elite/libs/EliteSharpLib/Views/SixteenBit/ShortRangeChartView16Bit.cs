// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

internal sealed class ShortRangeChartView16Bit : BaseView16Bit, IView<ShortRangeChartModel>
{
    private const int TextX = 16;
    private const int NameY = 55;
    private const int DistanceY = 40;
    private const int CrossSize = 16;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGoldenrod;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorFireBrick;
    private readonly FastColor _colorWhite;

    internal ShortRangeChartView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGoldenrod = draw.Palette["Goldenrod"];
        _colorGreen = draw.Palette["Green"];
        _colorFireBrick = draw.Palette["FireBrick"];
        _colorWhite = draw.Palette["White"];
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

        foreach ((Vector2 position, string name) in model.Labels)
        {
            Graphics.DrawTextLeft(position, name, nameof(FontType.Small), _colorWhite);
        }

        foreach ((Vector2 position, float size) in model.Planets)
        {
            Graphics.DrawCircleFilled(position, size, _colorGoldenrod);
        }

        // Cross
        Graphics.DrawLine(new(model.Cross.X - CrossSize, model.Cross.Y), new(model.Cross.X + CrossSize, model.Cross.Y), _colorFireBrick);
        Graphics.DrawLine(new(model.Cross.X, model.Cross.Y - CrossSize), new(model.Cross.X, model.Cross.Y + CrossSize), _colorFireBrick);

        float x = TextX + _draw.Layout.ViewportLeft;
        Graphics.DrawTextLeft(
            new(x, _draw.Layout.ViewportHeight - NameY),
            model.Caption,
            nameof(FontType.Small),
            _colorGreen);
        Graphics.DrawTextLeft(
            new(x, _draw.Layout.ViewportHeight - DistanceY),
            model.Detail,
            nameof(FontType.Small),
            _colorWhite);
    }
}
