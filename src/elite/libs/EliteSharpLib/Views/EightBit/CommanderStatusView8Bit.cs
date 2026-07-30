// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit commander status screen: authored fresh for the 320x256 canvas
/// and its fixed 8x8 font, rather than derived from the 16-bit layout - see
/// docs/backlog-roadmap.md's "Author the 8-bit view layouts" item for why.
/// Labels are shorter than the 16-bit view's own, since the 8-bit font is
/// monospace: at 8px/character, 320px is only 40 characters wide, and the
/// widest value ("Front Military Laser", 21 chars) already needs most of a
/// row to itself, which is why equipment is one column here instead of two.
/// </summary>
internal sealed class CommanderStatusView8Bit : BaseView8Bit, IView<CommanderStatusModel>
{
    private const int LabelX = 8;
    private const int ValueX = 104;
    private const int EquipmentX = 8;
    private const int RowSpacingY = 8;
    private const int EquipmentSpacingY = 8;
    private const int FirstRowY = 40;
    private const int EquipmentHeaderY = FirstRowY + (7 * RowSpacingY) + 4;
    private const int EquipmentStartY = EquipmentHeaderY + EquipmentSpacingY;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorWhite;

    internal CommanderStatusView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGreen = draw.Palette["Green"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(CommanderStatusModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawViewHeader(model.Title);

        DrawRow(FirstRowY, "System:", model.PresentSystem);
        DrawRow(FirstRowY + RowSpacingY, "Hyperspace:", model.HyperspaceSystem);
        DrawRow(FirstRowY + (2 * RowSpacingY), "Condition:", model.Condition);
        DrawRow(FirstRowY + (3 * RowSpacingY), "Fuel:", model.Fuel);
        DrawRow(FirstRowY + (4 * RowSpacingY), "Cash:", model.Cash);
        DrawRow(FirstRowY + (5 * RowSpacingY), "Legal:", model.LegalStatus);
        DrawRow(FirstRowY + (6 * RowSpacingY), "Rating:", model.Rating);

        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Layout.Offset, EquipmentHeaderY), "EQUIPMENT:", nameof(FontType.Small), _colorGreen);

        DrawEquipment(model.Equipment);
    }

    private void DrawRow(float y, string label, string value)
    {
        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Layout.Offset, y), label, nameof(FontType.Small), _colorGreen);

        if (!string.IsNullOrEmpty(value))
        {
            _draw.Graphics.DrawTextLeft(new(ValueX + _draw.Layout.Offset, y), value, nameof(FontType.Small), _colorWhite);
        }
    }

    // One column, unlike the 16-bit view's two: even the longest equipment
    // string ("Front Military Laser") already needs most of the screen's
    // 40-character width, so a second column would overlap it.
    private void DrawEquipment(IReadOnlyList<string> equipment)
    {
        float y = EquipmentStartY;

        foreach (string item in equipment)
        {
            _draw.Graphics.DrawTextLeft(new(EquipmentX + _draw.Layout.Offset, y), item, nameof(FontType.Small), _colorWhite);
            y += EquipmentSpacingY;
        }
    }
}
