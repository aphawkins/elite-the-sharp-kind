// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Views;

/// <summary>
/// The 16-bit escape capsule alert: the 512-space layout, and nothing else.
/// The doomed Cobra ahead of it is drawn by the universe, not here.
/// </summary>
internal sealed class EscapeCapsuleView : IView<EscapeCapsuleModel>
{
    // The alert sits above the scanner, clear of the dashboard.
    private const float AlertOffset = 40;

    private readonly IEliteDraw _draw;
    private readonly uint _colorWhite;

    internal EscapeCapsuleView(IEliteDraw draw)
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
