// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;
using Useful.Controls;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit short-range chart, authored for the 320x256 canvas: half the
/// 16-bit tier's cross and text offsets, matching its Scale of 1.
/// </summary>
internal sealed class ShortRangeChartView8Bit : ShortRangeChartViewBase
{
    private const int TextX = 8;
    private const int NameY = 28;
    private const int DistanceY = 20;
    private const int CrossSize = 8;

    private readonly FastColor _colorGreen;
    private readonly FastColor _colorRed;
    private readonly FastColor _colorWhite;
    private readonly FastColor _colorYellow;

    internal ShortRangeChartView8Bit(
        GameState gameState,
        IEliteDraw draw,
        IBaseView baseView,
        IKeyboard keyboard,
        PlanetController planet,
        PlayerShip ship)
        : base(gameState, draw, baseView, keyboard, planet, ship)
    {
        _colorGreen = draw.Palette["Green"];
        _colorRed = draw.Palette["Red"];
        _colorWhite = draw.Palette["White"];
        _colorYellow = draw.Palette["Yellow"];
    }

    protected override (float MinX, float MaxX, float MinY, float MaxY) CrossBounds
        => (1, EliteDraw.Layout.ViewportRight - 1, 19, EliteDraw.Layout.ScannerTop - 17);

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
            BaseView.Graphics.DrawCircleFilled(position, size, _colorYellow);
        }

        // Cross
        BaseView.Graphics.DrawLine(new(Cross.X - CrossSize, Cross.Y), new(Cross.X + CrossSize, Cross.Y), _colorRed);
        BaseView.Graphics.DrawLine(new(Cross.X, Cross.Y - CrossSize), new(Cross.X, Cross.Y + CrossSize), _colorRed);

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
            // "Distance:" alone is 9 characters of the 40 available, so the
            // 8-bit row drops the label's "Light Years" suffix.
            BaseView.Graphics.DrawTextLeft(
                new(x, distanceY),
                $"Dist: {GameState.DistanceToPlanet:N1} LY",
                nameof(FontType.Small),
                _colorWhite);
        }
    }
}
