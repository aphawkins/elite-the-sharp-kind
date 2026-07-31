// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit escape capsule alert: the 512-space layout, and nothing else.
/// The doomed Cobra ahead of it is drawn by the universe, not here.
/// </summary>
internal sealed class EscapeCapsuleView16Bit : BaseView16Bit, IView<EscapeCapsuleModel>
{
    // The alert sits above the scanner, clear of the dashboard.
    private const float AlertOffset = 40;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorWhite;

    internal EscapeCapsuleView16Bit(IEliteDraw draw)
        : base(draw)
    {
        _draw = draw;

        _colorWhite = draw.Palette["White"];
    }

    public void Draw(EscapeCapsuleModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        DrawBorder();

        if (model.IsAlertVisible)
        {
            _draw.Graphics.DrawTextCentre(
                _draw.Layout.ScannerTop - AlertOffset,
                model.Alert,
                nameof(FontType.Small),
                _colorWhite);
        }
    }
}
