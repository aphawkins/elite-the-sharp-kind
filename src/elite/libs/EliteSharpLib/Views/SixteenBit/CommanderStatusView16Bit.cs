// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit commander status screen: the 512-space layout, and nothing
/// else. Labels sit at x=16 with their values at x=200, on 16px rows.
/// </summary>
internal sealed class CommanderStatusView16Bit : BaseView16Bit, IView<CommanderStatusModel>
{
    private const int LabelX = 16;
    private const int ValueX = 200;
    private const int EquipmentX = 50;
    private const int EquipmentStartY = 202;
    private const int EquipmentMaxY = 290;
    private const int EquipmentColumnWidth = 200;
    private const int SpacingY = 16;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGreen;
    private readonly FastColor _colorWhite;

    internal CommanderStatusView16Bit(IEliteDraw draw)
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

        DrawRow(58, "Present System:", model.PresentSystem);
        DrawRow(74, "Hyperspace System:", model.HyperspaceSystem);
        DrawRow(90, "Condition:", model.Condition);
        DrawRow(106, "Fuel:", model.Fuel);
        DrawRow(122, "Cash:", model.Cash);
        DrawRow(138, "Legal Status:", model.LegalStatus);
        DrawRow(154, "Rating:", model.Rating);

        _draw.Graphics.DrawTextLeft(new(LabelX + _draw.Layout.Offset, 186), "EQUIPMENT:", nameof(FontType.Small), _colorGreen);

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

    // The equipment list, filling the left column before wrapping to the right.
    private void DrawEquipment(IReadOnlyList<string> equipment)
    {
        Vector2 position = new(EquipmentX + _draw.Layout.Offset, EquipmentStartY);

        foreach (string item in equipment)
        {
            _draw.Graphics.DrawTextLeft(position, item, nameof(FontType.Small), _colorWhite);

            position.Y += SpacingY;
            if (position.Y > EquipmentMaxY)
            {
                position.Y = EquipmentStartY;
                position.X += EquipmentColumnWidth;
            }
        }
    }
}
