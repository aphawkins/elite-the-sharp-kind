// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit Thargoid mission messages: a first-draft 320x256 layout, not
/// derived from the 16-bit one - see docs/backlog-roadmap.md's "Author the
/// 8-bit view layouts" item. Exact spacing is expected to be refined
/// visually.
/// </summary>
internal sealed class ThargoidMissionView8Bit : BaseView8Bit, IView<ThargoidMissionModel>
{
    private const int TextColumn = 1;
    private const int TextColumns = 38;
    private const int FooterRow = 22;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorYellow;

    internal ThargoidMissionView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorYellow = draw.Palette["Yellow"];
    }

    public void Draw(ThargoidMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        switch (model.Stage)
        {
            case 4:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(Column(TextColumn), Row(8)), Column(TextColumns), model.Paragraphs[0]);
                DrawFooter();
                break;

            case 5:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(Column(TextColumn), Row(4)), Column(TextColumns), model.Paragraphs[0]);
                DrawTextPretty(new(Column(TextColumn), Row(12)), Column(TextColumns), model.Paragraphs[1]);
                _draw.Graphics.DrawImage(nameof(ImageType.Blake), new(Column(29), Row(4)));
                DrawFooter();
                break;

            case 6:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextCentreOnGrid(6, model.Headline, nameof(FontType.Large), _colorYellow);
                DrawTextPretty(new(Column(TextColumn), Row(8)), Column(TextColumns), model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => DrawTextCentreOnGrid(FooterRow, "Press space to continue.", nameof(FontType.Small), _colorYellow);
}
