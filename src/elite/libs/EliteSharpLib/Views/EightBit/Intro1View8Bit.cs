// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit title screen: authored for the 320x256 canvas and its fixed 8x8
/// font. The rolling Cobra behind it is drawn by the universe, not here. The
/// longest credit line ("Original Game (C) I.Bell &amp; D.Braben", 35
/// characters) fits the 40-character row width with room to spare.
/// </summary>
internal sealed class Intro1View8Bit : BaseView8Bit, IView<Intro1Model>
{
    // The credits stack upwards from the prompt, one line every 10px: the
    // 8x8 font plus a two-pixel gap.
    private const float CreditSpacing = 10;
    private const float CreditsOffset = 60;
    private const float PromptOffset = 16;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorYellow;
    private readonly FastColor _colorWhite;

    internal Intro1View8Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorYellow = draw.Palette["Yellow"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(Intro1Model model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Layout.Top + 4);

        float y = _draw.Layout.ScannerTop - CreditsOffset;
        foreach (string credit in model.Credits)
        {
            _draw.Graphics.DrawTextCentre(y, credit, nameof(FontType.Small), _colorWhite);
            y += CreditSpacing;
        }

        _draw.Graphics.DrawTextCentre(_draw.Layout.ScannerTop - PromptOffset, model.Prompt, nameof(FontType.Small), _colorYellow);
    }
}
