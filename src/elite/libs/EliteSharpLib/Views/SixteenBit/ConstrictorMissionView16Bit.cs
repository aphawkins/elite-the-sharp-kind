// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Missions;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit Constrictor mission messages: the 512-space layout, and nothing
/// else. The brief and the debrief were laid out differently in the original,
/// so the layout keys off the model's stage. The Constrictor posing behind
/// the brief is drawn by the universe, not here, but this tier picks where it
/// sits.
/// </summary>
internal sealed class ConstrictorMissionView16Bit : BaseView16Bit, IConstrictorMissionView
{
    private readonly IEliteDraw _draw;
    private readonly FastColor _colorGold;

    internal ConstrictorMissionView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
    }

    public Vector4 ShipLocation => new(200, 90, 600, 0);

    public void Draw(ConstrictorMissionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        switch (model.Stage)
        {
            case ConstrictorMission.Briefed:
                DrawViewHeader("INCOMING MESSAGE");
                DrawTextPretty(new(16 + _draw.Layout.ViewportLeft, 50), 300, model.Paragraphs[0]);
                DrawTextPretty(new(16 + _draw.Layout.ViewportLeft, 200), 470, model.Paragraphs[1]);
                DrawFooter();
                break;

            case ConstrictorMission.Rewarded:
                DrawViewHeader("INCOMING MESSAGE");
                _draw.Graphics.DrawTextCentre(100, model.Headline, nameof(FontType.Large), _colorGold);
                DrawTextPretty(new(116 + _draw.Layout.ViewportLeft, 132), 400, model.Paragraphs[0]);
                DrawFooter();
                break;
        }
    }

    private void DrawFooter()
        => _draw.Graphics.DrawTextCentre(330, "Press space to continue.", nameof(FontType.Large), _colorGold);
}
