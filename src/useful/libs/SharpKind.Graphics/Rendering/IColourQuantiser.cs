// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// How a computed colour is reduced to one the display can actually show. The
/// pipeline's output stage, independent of how the colour was arrived at: a
/// rendition standing in for limited hardware quantises whether or not
/// anything was lit.
/// </summary>
/// <remarks>
/// The pixel position is a parameter because the useful answers are not all
/// per-colour. Nearest-level and nearest-palette-entry ignore it; ordered
/// dithering cannot, since its whole method is to alternate between the two
/// levels that straddle the wanted colour according to where the pixel falls
/// in a threshold matrix. What a dither depends on is only the pixel's place
/// in that matrix, though, so a flat fill resolves its handful of answers
/// once per face - see <see cref="Period"/>.
/// </remarks>
public interface IColourQuantiser
{
    /// <summary>
    /// Gets how many pixels apart, on both axes, this quantiser's answer
    /// repeats. One means the answer never depends on the pixel at all.
    /// </summary>
    /// <remarks>
    /// A fill resolves a flat face's colours once per cell of this tile
    /// rather than once per pixel, so the period has to be declared rather
    /// than assumed: a quantiser that repeats on some other stride would
    /// otherwise be tiled wrongly and silently.
    /// </remarks>
    public int Period { get; }

    /// <summary>
    /// Gets a value indicating whether this quantiser's answer depends on
    /// where the pixel is, and so has to be asked per pixel rather than once
    /// for a whole face.
    /// </summary>
    public bool IsPositionDependent => Period > 1;

    /// <summary>
    /// Gets the typical gap, per channel, between neighbouring colours this
    /// display can show. What a dither has to span to reach the colour either
    /// side of the one wanted.
    /// </summary>
    public float LevelGap { get; }

    /// <summary>
    /// The displayable colour nearest the one wanted.
    /// </summary>
    /// <param name="colour">The colour the shading stage produced.</param>
    /// <param name="x">Pixel column, for a quantiser that dithers.</param>
    /// <param name="y">Pixel row, for a quantiser that dithers.</param>
    public FastColor Quantise(in FastColor colour, int x, int y);
}
