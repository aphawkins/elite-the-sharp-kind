// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;
using Useful.Controls;

namespace EliteSharpLib.Views.SixteenBit;

internal sealed class ShortRangeChartView16Bit : ShortRangeChartViewBase
{
    private const int TextX = 16;
    private const int NameY = 55;
    private const int DistanceY = 40;
    private const int CrossSize = 16;

    private readonly FastColor _colorGoldenrod;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorFireBrick;
    private readonly FastColor _colorWhite;

    internal ShortRangeChartView16Bit(
        GameState gameState,
        IEliteDraw draw,
        IBaseView baseView,
        IKeyboard keyboard,
        PlanetController planet,
        PlayerShip ship)
        : base(gameState, draw, baseView, keyboard, planet, ship)
    {
        _colorGoldenrod = draw.Palette["Goldenrod"];
        _colorGreen = draw.Palette["Green"];
        _colorFireBrick = draw.Palette["FireBrick"];
        _colorWhite = draw.Palette["White"];
    }

    protected override (float MinX, float MaxX, float MinY, float MaxY) CrossBounds
        => (1, EliteDraw.Layout.ViewportRight - 1, 37, EliteDraw.Layout.ScannerTop - 33);

    public override void Draw()
    {
        BaseView.DrawBorder();
        BaseView.DrawViewHeader("SHORT RANGE CHART");

        // Fuel radius
        Vector2 centre = EliteDraw.Layout.ViewportCentre;
        float scale = EliteDraw.Layout.Scale;
        float radius = Ship.Fuel * 10 * scale;
        float cross_size = CrossSize * scale;
        BaseView.Graphics.DrawCircle(centre, radius, _colorGreen);
        BaseView.Graphics.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size), _colorWhite);
        BaseView.Graphics.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y), _colorWhite);

        // Planets
        foreach ((Vector2 position, string name) in PlanetNames)
        {
            BaseView.Graphics.DrawTextLeft(position, name, nameof(FontType.Small), _colorWhite);
        }

        foreach ((Vector2 position, float size) in PlanetSizes)
        {
            BaseView.Graphics.DrawCircleFilled(position, size, _colorGoldenrod);
        }

        // Cross
        BaseView.Graphics.DrawLine(new(Cross.X - CrossSize, Cross.Y), new(Cross.X + CrossSize, Cross.Y), _colorFireBrick);
        BaseView.Graphics.DrawLine(new(Cross.X, Cross.Y - CrossSize), new(Cross.X, Cross.Y + CrossSize), _colorFireBrick);

        DrawStatusText();
    }

    private void DrawStatusText()
    {
        float x = TextX + EliteDraw.Layout.ScannerLeft;
        float nameY = EliteDraw.Layout.ScannerTop - NameY;
        float distanceY = EliteDraw.Layout.ScannerTop - DistanceY;

        if (IsFind)
        {
            BaseView.Graphics.DrawTextLeft(new(x, nameY), "Planet Name?", nameof(FontType.Small), _colorGreen);
            BaseView.Graphics.DrawTextLeft(new(x, distanceY), FindName, nameof(FontType.Small), _colorWhite);
            return;
        }

        if (string.IsNullOrEmpty(GameState.PlanetName))
        {
            BaseView.Graphics.DrawTextLeft(new(x, nameY), "Unknown Planet", nameof(FontType.Small), _colorGreen);
            BaseView.Graphics.DrawTextLeft(new(x, distanceY), FindName, nameof(FontType.Small), _colorWhite);
            return;
        }

        BaseView.Graphics.DrawTextLeft(new(x, nameY), GameState.PlanetName, nameof(FontType.Small), _colorGreen);

        if (GameState.DistanceToPlanet > 0)
        {
            BaseView.Graphics.DrawTextLeft(
                new(x, distanceY),
                $"Distance: {GameState.DistanceToPlanet:N1} Light Years ",
                nameof(FontType.Small),
                _colorWhite);
        }
    }
}
