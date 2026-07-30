// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit Thargoid mission messages: a first-draft 320x256 layout, not
/// derived from the 16-bit one - see docs/backlog-roadmap.md's "Author the
/// 8-bit view layouts" item. Exact spacing is expected to be refined
/// visually.
/// </summary>
internal sealed class ThargoidMissionView8Bit : IView<ThargoidMissionModel>
{
    private const int TextX = 8;
    private const int TextWidth = 304;
    private const int FooterY = 180;

    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;

    internal ThargoidMissionView8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(ThargoidMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        switch (model.Stage)
        {
            case 4:
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(new(TextX + _draw.Offset, 64), TextWidth, model.Paragraphs[0]);
                DrawFooter();
                break;

            case 5:
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(new(TextX + _draw.Offset, 32), TextWidth, model.Paragraphs[0]);
                _draw.DrawTextPretty(new(TextX + _draw.Offset, 96), TextWidth, model.Paragraphs[1]);
                _draw.Graphics.DrawImage(nameof(ImageType.Blake), new(232 + _draw.Offset, 32));
                DrawFooter();
                break;

            case 6:
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.Graphics.DrawTextCentre(48, model.Headline, nameof(FontType.Large), _colorGold);
                _draw.DrawTextPretty(new(TextX + _draw.Offset, 64), TextWidth, model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => _draw.Graphics.DrawTextCentre(FooterY, "Press space to continue.", nameof(FontType.Small), _colorGold);
}
