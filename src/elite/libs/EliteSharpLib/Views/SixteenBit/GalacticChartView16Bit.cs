// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit galactic chart: the 512-space layout, and nothing else.
/// </summary>
internal sealed class GalacticChartView16Bit : BaseView16Bit, IView<GalacticChartModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorFireBrick;
    private readonly FastColor _colorWhite;

    internal GalacticChartView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorFireBrick = draw.Palette["FireBrick"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(GalacticChartModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        // Header
        DrawViewHeader(model.Title);

        _draw.Graphics.DrawLine(new(0 + _draw.Layout.ScannerLeft, 36 + 258), new(_draw.Layout.ScannerRight, 36 + 258), _colorWhite);

        // Fuel radius
        Vector2 centre = ToScreen(model.DockedPlanet);
        float radius = model.FuelLightYears * 2.5f * _draw.Layout.Scale;
        float cross_size = 7 * _draw.Layout.Scale;
        _draw.Graphics.DrawCircle(centre, radius, _colorGreen);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size), _colorWhite);
        _draw.Graphics.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y), _colorWhite);

        // Planets
        foreach (GalacticChartStar star in model.Stars)
        {
            Vector2 pixel = ToScreen(star.Position);
            _draw.Graphics.DrawPixel(pixel, _colorWhite);

            if (star.IsWide)
            {
                _draw.Graphics.DrawPixel(new(pixel.X + 1, pixel.Y), _colorWhite);
            }
        }

        // Cross
        centre = ToScreen(model.Cross);

        _draw.Graphics.DrawLine(new(centre.X - 8, centre.Y), new(centre.X + 8, centre.Y), _colorFireBrick);
        _draw.Graphics.DrawLine(new(centre.X, centre.Y - 8), new(centre.X, centre.Y + 8), _colorFireBrick);

        // Text
        _draw.Graphics
            .DrawTextLeft(
                new(16 + _draw.Layout.ScannerLeft, _draw.Layout.ScannerTop - 55),
                model.Caption,
                nameof(FontType.Small),
                _colorGreen);
        _draw.Graphics
            .DrawTextLeft(
                new(16 + _draw.Layout.ScannerLeft, _draw.Layout.ScannerTop - 40),
                model.Detail,
                nameof(FontType.Small),
                _colorWhite);
    }

    // Galaxy space (D, B) to this tier's screen coordinates.
    private Vector2 ToScreen(Vector2 galaxy) => new(
        (galaxy.X * _draw.Layout.Scale) + _draw.Layout.ScannerLeft,
        (galaxy.Y * _draw.Layout.Scale / 2) + (18 * _draw.Layout.Scale) + 1);
}
