// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Types;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit Thargoid mission messages: the 512-space layout, and nothing
/// else. Each stage was laid out differently in the original, so the layout
/// keys off the model's stage.
/// </summary>
internal sealed class ThargoidMissionView16Bit : BaseView16Bit, IView<ThargoidMissionModel>
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGold;

    internal ThargoidMissionView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(ThargoidMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        switch (model.Stage)
        {
            case ThargoidStage.Summoned:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(116, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;

            case ThargoidStage.CarryingPlans:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(16, 50), 300, model.Paragraphs[0]);
                DrawTextPretty(new(16, 200), 470, model.Paragraphs[1]);
                _draw.Graphics.DrawImage(nameof(ImageType.Blake), new(352, 46));
                DrawFooter();
                break;

            case ThargoidStage.Rewarded:
                DrawViewHeader("INCOMING MESSAGE");
                _draw.Graphics.DrawTextCentre(100, model.Headline, nameof(FontType.Large), _colorGold);
                DrawTextPretty(new(116, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => _draw.Graphics.DrawTextCentre(330, "Press space to continue.", nameof(FontType.Large), _colorGold);
}
