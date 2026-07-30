// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit title screen: the 512-space layout, and nothing else. The
/// rolling Cobra behind it is drawn by the universe, not here.
/// </summary>
internal sealed class Intro1View16Bit : IView<Intro1Model>
{
    // The credits stack upwards from the prompt, one line every 20px.
    private const float CreditSpacing = 20;

    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;
    private readonly uint _colorWhite;

    internal Intro1View16Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(Intro1Model model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Top + 10);

        float y = _draw.ScannerTop - 90;
        foreach (string credit in model.Credits)
        {
            _draw.Graphics.DrawTextCentre(y, credit, nameof(FontType.Small), _colorWhite);
            y += CreditSpacing;
        }

        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, model.Prompt, nameof(FontType.Large), _colorGold);
    }
}
