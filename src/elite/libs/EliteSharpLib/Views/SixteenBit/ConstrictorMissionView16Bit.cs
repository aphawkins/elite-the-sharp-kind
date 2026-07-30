// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit Constrictor mission messages: the 512-space layout, and nothing
/// else. The brief and the debrief were laid out differently in the original,
/// so the layout keys off the model's stage. The Constrictor posing behind
/// the brief is drawn by the universe, not here.
/// </summary>
internal sealed class ConstrictorMissionView16Bit : IView<ConstrictorMissionModel>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;

    internal ConstrictorMissionView16Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public void Draw(ConstrictorMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        switch (model.Stage)
        {
            case 1:
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(new(16 + _draw.Offset, 50), 300, model.Paragraphs[0]);
                _draw.DrawTextPretty(new(16 + _draw.Offset, 200), 470, model.Paragraphs[1]);
                DrawFooter();
                break;

            case 3:
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.Graphics.DrawTextCentre(100, model.Headline, nameof(FontType.Large), _colorGold);
                _draw.DrawTextPretty(new(116 + _draw.Offset, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => _draw.Graphics.DrawTextCentre(330, "Press space to continue.", nameof(FontType.Large), _colorGold);
}
