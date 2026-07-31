// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit planet data screen: the 512-space layout, and nothing else.
/// </summary>
internal sealed class PlanetDataView16Bit : BaseView16Bit, IView<PlanetDataModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorWhite;

    internal PlanetDataView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(PlanetDataModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        DrawViewHeader(model.Header);

        if (model.Distance.Length > 0)
        {
            _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 42), "Distance:", nameof(FontType.Small), _colorGreen);
            _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 42), model.Distance, nameof(FontType.Small), _colorWhite);
        }

        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 74), "Economy:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 74), model.Economy, nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 106), "Government:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 106), model.Government, nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 138), "Tech Level:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 138), model.TechLevel, nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 170), "Population:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 170), model.Population, nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 202), "Gross Productivity:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 202), model.Productivity, nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextLeft(new(16 + _draw.Layout.ViewportLeft, 234), "Average Radius:", nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(175 + _draw.Layout.ViewportLeft, 234), model.Radius, nameof(FontType.Small), _colorWhite);
        DrawTextPretty(new(16 + _draw.Layout.ViewportLeft, 266), 400, model.Description);
    }
}
