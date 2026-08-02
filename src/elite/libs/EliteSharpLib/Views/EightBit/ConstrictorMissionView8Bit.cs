// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Missions;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit Constrictor mission messages: a first-draft 320x256 layout, not
/// derived from the 16-bit one - see docs/backlog-roadmap.md's "Author the
/// 8-bit view layouts" item. Exact spacing is expected to be refined
/// visually.
/// </summary>
internal sealed class ConstrictorMissionView8Bit : BaseView8Bit, IConstrictorMissionView
{
    private const int TextColumn = 1;
    private const int TextRowUpper = 3;
    private const int TextColumnsUpper = 32;
    private const int TextRowLower = 13;
    private const int TextColumnsLower = 39;
    private const int FooterRow = 23;

    private readonly FastColor _colorYellow;

    internal ConstrictorMissionView8Bit(IEliteDraw draw)
        : base(draw)
        => _colorYellow = draw.Palette["Yellow"];

    public Vector4 ShipLocation => new(330, 80, 700, 0);

    public void Draw(ConstrictorMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        switch (model.Stage)
        {
            case ConstrictorMission.Briefed:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(Column(TextColumn), Row(TextRowUpper)), Column(TextColumnsUpper), model.Paragraphs[0]);
                DrawTextPretty(new(Column(TextColumn), Row(TextRowLower)), Column(TextColumnsLower), model.Paragraphs[1]);
                DrawFooter();
                break;

            case ConstrictorMission.Rewarded:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextCentreOnGrid(6, model.Headline, nameof(FontType.Large), _colorYellow);
                DrawTextPretty(new(Column(TextColumn), Row(8)), Column(TextColumnsUpper), model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => DrawTextCentreOnGrid(FooterRow, "Press space to continue.", nameof(FontType.Small), _colorYellow);
}
