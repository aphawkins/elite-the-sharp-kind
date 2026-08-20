// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Abstraction.Config;

namespace SharpKind.UI.Gallery;

/// <summary>
/// The root of gallery.sharp. The engine half only - backend, window scale,
/// frame rate and logging - because the gallery has no settings of its own to
/// keep. Deriving from the non-generic <see cref="ConfigSettings"/> rather
/// than the generic one is how a game says it has no game section.
/// <para>
/// It exists so the gallery can be pointed at either backend and at any of
/// the font kinds: alignment is what a change of font moves, and a control
/// laid out against one kind's metrics has to be checked against the others.
/// </para>
/// </summary>
internal sealed class GalleryConfig : ConfigSettings
{
    internal const int DefaultWindowScale = 3;

    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryConfig"/> class,
    /// magnified by default. A game's canvas is its own business, but this one
    /// is the 8-bit tier's 320x256 and exists to be looked at closely - and an
    /// unmagnified window is a postage stamp on a modern display. Set in the
    /// constructor rather than repaired, so a windowScale the file does name
    /// still wins.
    /// </summary>
    public GalleryConfig() => Engine.WindowScale = DefaultWindowScale;
}
