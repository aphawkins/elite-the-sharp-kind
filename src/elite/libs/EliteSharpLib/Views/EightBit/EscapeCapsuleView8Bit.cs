// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit escape capsule alert: authored for the 320x256 canvas and its
/// fixed 8x8 font. The doomed Cobra ahead of it is drawn by the universe, not
/// here.
/// </summary>
internal sealed class EscapeCapsuleView8Bit : IView<EscapeCapsuleModel>
{
    // The alert sits above the scanner, clear of the dashboard. Half the
    // 16-bit offset, matching the halved font and scanner heights.
    private const float AlertOffset = 20;

    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;

    internal EscapeCapsuleView8Bit(IEliteDraw draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
    }

    public void Draw(EscapeCapsuleModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.IsAlertVisible)
        {
            _draw.Graphics.DrawTextCentre(
                _draw.ScannerTop - AlertOffset,
                model.Alert,
                nameof(FontType.Small),
                _colorWhite);
        }
    }
}
