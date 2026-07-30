// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
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
    private readonly FastColor _colorGoldenrod;

    internal ThargoidMissionView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGoldenrod = draw.Palette["Goldenrod"];
    }

    public void Draw(ThargoidMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        switch (model.Stage)
        {
            case 4:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(116, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;

            case 5:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(16, 50), 300, model.Paragraphs[0]);
                DrawTextPretty(new(16, 200), 470, model.Paragraphs[1]);
                _draw.Graphics.DrawImage(nameof(ImageType.Blake), new(352, 46));
                DrawFooter();
                break;

            case 6:
                DrawViewHeader("INCOMING MESSAGE");
                _draw.Graphics.DrawTextCentre(100, model.Headline, nameof(FontType.Large), _colorGoldenrod);
                DrawTextPretty(new(116, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => _draw.Graphics.DrawTextCentre(330, "Press space to continue.", nameof(FontType.Large), _colorGoldenrod);
}
