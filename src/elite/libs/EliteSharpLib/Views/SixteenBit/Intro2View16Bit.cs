// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit ship parade screen: the 512-space layout, and nothing else.
/// The ship itself is drawn by the universe, not here.
/// </summary>
internal sealed class Intro2View16Bit : BaseView16Bit, IView<Intro2Model>
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorGoldenrod;
    private readonly uint _colorWhite;

    internal Intro2View16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorGoldenrod = draw.Palette["Goldenrod"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw(Intro2Model model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Layout.Top + 10);

        _draw.Graphics.DrawTextCentre(_draw.Layout.ScannerTop - 30, model.Prompt, nameof(FontType.Large), _colorGoldenrod);

        if (model.ShipName.Length > 0)
        {
            _draw.Graphics.DrawTextCentre(_draw.Layout.ScannerTop - 60, model.ShipName, nameof(FontType.Small), _colorWhite);
        }
    }
}
