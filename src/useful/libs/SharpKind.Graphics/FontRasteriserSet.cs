// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics;

/// <summary>
/// The font kinds a rendition can be drawn with, and which of them is in use.
/// Backends hold one of these rather than a single rasteriser, so the choice
/// can change while the game is running instead of only at launch.
/// </summary>
/// <remarks>
/// A rendition need not offer every kind - only its own sheets are certain to
/// exist, since those are what it ships. Asking for one it does not have
/// gives the sheets instead: a font the commander picked being unavailable is
/// a reason to draw the rendition's own text, not a reason to fail.
/// </remarks>
public sealed class FontRasteriserSet : IFontRasteriser, IDisposable
{
    private readonly Dictionary<FontKind, IFontRasteriser> _rasterisers;
    private bool _isDisposed;

    public FontRasteriserSet(Dictionary<FontKind, IFontRasteriser> rasterisers, FontKind kind)
    {
        ArgumentNullException.ThrowIfNull(rasterisers);

        if (!rasterisers.ContainsKey(FontKind.Bitmap))
        {
            throw new SharpKindException(
                "A rendition's own sheets are what every other font kind falls back to, so they have to be there.");
        }

        _rasterisers = rasterisers;
        Kind = Resolve(kind);
    }

    /// <summary>
    /// Gets the kind actually in use, which is the one asked for whenever
    /// the rendition has it.
    /// </summary>
    public FontKind Kind { get; private set; }

    /// <summary>
    /// Wraps one rasteriser as a set that offers only it, for callers with
    /// nothing to choose between.
    /// </summary>
    /// <param name="rasteriser">The only rasteriser available.</param>
    /// <returns>A set holding just that one.</returns>
    public static FontRasteriserSet Only(IFontRasteriser rasteriser)
        => new(new() { { FontKind.Bitmap, rasteriser } }, FontKind.Bitmap);

    /// <summary>
    /// Whether the rendition offers <paramref name="kind"/>.
    /// </summary>
    /// <param name="kind">The kind to look for.</param>
    /// <returns><see langword="true"/> if it can be drawn with.</returns>
    public bool Has(FontKind kind) => _rasterisers.ContainsKey(kind);

    /// <summary>
    /// Draws with <paramref name="kind"/> from now on, or with the
    /// rendition's own sheets if it has no such font.
    /// </summary>
    /// <param name="kind">The kind wanted.</param>
    /// <returns>The kind now in use, which differs if the wanted one is absent.</returns>
    public FontKind Select(FontKind kind)
    {
        Kind = Resolve(kind);

        return Kind;
    }

    public FastBitmap Rasterise(string text, string fontType, FastColor color)
        => _rasterisers[Kind].Rasterise(text, fontType, color);

    public Vector2 Measure(string text, string fontType) => _rasterisers[Kind].Measure(text, fontType);

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        // Only the TrueType one holds anything unmanaged, but which kinds are
        // present is the caller's choice, so every one is asked.
        foreach (KeyValuePair<FontKind, IFontRasteriser> rasteriser in _rasterisers)
        {
            (rasteriser.Value as IDisposable)?.Dispose();
        }

        _rasterisers.Clear();
        _isDisposed = true;
    }

    private FontKind Resolve(FontKind kind) => _rasterisers.ContainsKey(kind) ? kind : FontKind.Bitmap;
}
