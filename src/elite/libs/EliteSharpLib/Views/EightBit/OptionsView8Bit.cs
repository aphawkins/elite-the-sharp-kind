// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit options menu: authored for the 320x256 canvas and its fixed 8x8
/// font. The 16-bit view's 400px selection bar overflows a 320-wide screen,
/// so this tier has its own width, and the credits are word-wrapped rather
/// than drawn one centred line each: the longest ("The New Kind - Christian
/// Pinder 1999-2001", 41 characters) is one character wider than the screen's
/// 40-character row. <see cref="IBaseView.DrawTextPretty"/> is deliberately
/// not used for that - it breaks even text that already fits, and it draws
/// left-aligned where these lines are centred.
/// </summary>
internal sealed class OptionsView8Bit : BaseView8Bit, IView<OptionsModel>
{
    private const int OptionBarHeight = 8;
    private const int OptionBarWidth = 240;
    private const int FirstRowY = 56;
    private const int RowSpacingY = 16;
    private const int CreditSpacingY = 10;
    private const int CreditsBottomGapY = 6;
    private const int VersionGapY = 12;

    // 320px of 8x8 characters, less a character of margin either side.
    private const int MaxCharsPerLine = 38;

    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;
    private readonly uint _colorRed;
    private readonly uint _colorLightGray;

    internal OptionsView8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
        _colorRed = draw.Palette["Red"];
        _colorLightGray = draw.Palette["LightGray"];
    }

    public void Draw(OptionsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawViewHeader("OPTIONS");

        for (int i = 0; i < model.Options.Count; i++)
        {
            Vector2 position = new(_draw.Layout.Centre.X - (OptionBarWidth / 2), FirstRowY + (i * RowSpacingY));

            if (i == model.HighlightedIndex)
            {
                _draw.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorRed);
            }

            uint col = model.Options[i].IsEnabled ? _colorWhite : _colorLightGray;

            _draw.Graphics.DrawTextCentre(position.Y, model.Options[i].Label, nameof(FontType.Small), col);
        }

        List<string> lines = [];
        foreach (string credit in model.Credits)
        {
            Wrap(credit, lines);
        }

        float y = _draw.Layout.ScannerTop - CreditsBottomGapY - (lines.Count * CreditSpacingY);

        _draw.Graphics.DrawTextCentre(y - VersionGapY, model.Version, nameof(FontType.Small), _colorWhite);

        foreach (string line in lines)
        {
            _draw.Graphics.DrawTextCentre(y, line, nameof(FontType.Small), _colorWhite);
            y += CreditSpacingY;
        }
    }

    // Break on spaces at the row width. Only one credit is long enough to
    // need it, but which one that is is the controller's business, not this
    // view's.
    private static void Wrap(string text, List<string> lines)
    {
        int start = 0;

        while (text.Length - start > MaxCharsPerLine)
        {
            int split = text.LastIndexOf(' ', start + MaxCharsPerLine, MaxCharsPerLine);

            if (split <= start)
            {
                split = start + MaxCharsPerLine;
            }

            lines.Add(text[start..split]);
            start = split + 1;
        }

        lines.Add(text[start..]);
    }
}
