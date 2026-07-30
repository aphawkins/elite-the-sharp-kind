// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit planet data screen: a first-draft 320x256 layout, not derived
/// from the 16-bit one - see docs/backlog-roadmap.md's "Author the 8-bit
/// view layouts" item. Labels are shortened from the 16-bit view's own
/// (label text is view chrome, not model content) so the value column can
/// start well before 320px; exact spacing is expected to be refined visually.
/// </summary>
internal sealed class PlanetDataView8Bit : IView<PlanetDataModel>
{
    private const int LabelX = 8;
    private const int ValueX = 88;
    private const int FirstRowY = 32;
    private const int RowSpacingY = 8;
    private const int DescriptionY = 96;

    private readonly IEliteDraw _draw;
    private readonly uint _colorGreen;
    private readonly uint _colorWhite;

    internal PlanetDataView8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(PlanetDataModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.DrawViewHeader(model.Header);

        float y = FirstRowY;

        if (model.Distance.Length > 0)
        {
            DrawRow(ref y, "Distance:", model.Distance);
        }

        DrawRow(ref y, "Economy:", model.Economy);
        DrawRow(ref y, "Govt:", model.Government);
        DrawRow(ref y, "Tech:", model.TechLevel);
        DrawRow(ref y, "Pop:", model.Population);
        DrawRow(ref y, "Product:", model.Productivity);
        DrawRow(ref y, "Radius:", model.Radius);

        _draw.DrawTextPretty(new(LabelX + _draw.Offset, DescriptionY), 304, model.Description);
    }

    private void DrawRow(ref float y, string label, string value)
    {
        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Offset, y), label, nameof(FontType.Small), _colorGreen);
        _draw.Graphics.DrawTextLeft(new(ValueX + _draw.Offset, y), value, nameof(FontType.Small), _colorWhite);
        y += RowSpacingY;
    }
}
