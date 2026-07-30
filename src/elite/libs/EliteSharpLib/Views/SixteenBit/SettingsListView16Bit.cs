// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit settings list: the 512-space layout, and nothing else. Shared
/// by the game and engine settings screens, since neither varies the layout.
/// </summary>
internal sealed class SettingsListView16Bit : BaseView16Bit, IView<SettingsListModel>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;
    private readonly uint _colorLightRed;

    internal SettingsListView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw(SettingsListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawViewHeader(model.Header);

        for (int i = 0; i < model.Rows.Count; i++)
        {
            Vector2 position;

            if (i == model.Rows.Count - 1)
            {
                position.Y = ((model.Rows.Count + 1) / 2 * 30) + (_draw.Layout.Centre.Y / 2) + 32;
                if (i == model.HighlightedIndex)
                {
                    position.X = _draw.Layout.Centre.X - 200;
                    _draw.Graphics.DrawRectangleFilled(position, 400, 15, _colorLightRed);
                }

                _draw.Graphics.DrawTextCentre(position.Y, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);

                if (model.Footer.Length > 0)
                {
                    _draw.Graphics.DrawTextCentre(position.Y + 40, model.Footer, nameof(FontType.Small), _colorWhite);
                }

                return;
            }

            position.X = ((i & 1) * 250) + 32 + _draw.Layout.Offset;
            position.Y = (i / 2 * 30) + (_draw.Layout.Centre.Y / 2);

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, 100, 15, _colorLightRed);
            }

            _draw.Graphics.DrawTextLeft(position, model.Rows[i].Name, nameof(FontType.Small), _colorWhite);
            position.X += 120;
            _draw.Graphics.DrawTextLeft(position, model.Rows[i].Value, nameof(FontType.Small), _colorWhite);
        }
    }
}
