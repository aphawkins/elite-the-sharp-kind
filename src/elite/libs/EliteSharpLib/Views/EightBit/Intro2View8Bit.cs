// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit ship parade screen: authored for the 320x256 canvas and its
/// fixed 8x8 font. The ship itself is drawn by the universe, not here.
/// </summary>
internal sealed class Intro2View8Bit : IView<Intro2Model>
{
    private const float PromptOffset = 16;
    private const float ShipNameOffset = 30;

    private readonly IEliteDraw _draw;
    private readonly uint _colorGold;
    private readonly uint _colorWhite;

    internal Intro2View8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(Intro2Model model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Top + 4);

        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - PromptOffset, model.Prompt, nameof(FontType.Small), _colorGold);

        if (model.ShipName.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(
                _draw.ScannerTop - ShipNameOffset,
                model.ShipName,
                nameof(FontType.Small),
                _colorWhite);
        }
    }
}
