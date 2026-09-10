// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

// Every displayable colour one flat face can resolve to, worked out once for
// the face instead of once for each of its pixels.
//
// A flat fill asks the quantiser for the same colour at every pixel, and a
// quantiser's answer repeats every Period pixels on both axes, so the whole
// face has only a handful of distinct answers. Resolving them up front and
// indexing by where the pixel falls in the tile is byte-identical to asking
// per pixel, because the table holds the results of those same calls.
//
// The tile is always 4x4, which any period that divides four fills; a period
// that does not is rejected rather than silently mis-tiled. The caller owns
// the storage - a stack buffer at the top of the fill - so a face costs no
// allocation.
internal readonly ref struct DitherCells
{
    internal const int Count = Side * Side;

    private const int Side = 4;

    private const int Mask = Side - 1;

    private readonly ReadOnlySpan<FastColor> _cells;

    internal DitherCells(Span<FastColor> storage, in FastColor colour, IColourQuantiser? quantiser)
    {
        int period = quantiser?.Period ?? 1;

        if (Side % period != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantiser),
                period,
                "A quantiser whose pattern does not tile a 4x4 cell cannot be resolved once per face.");
        }

        for (int y = 0; y < Side; y++)
        {
            for (int x = 0; x < Side; x++)
            {
                storage[(y << 2) | x] = quantiser == null ? colour : quantiser.Quantise(colour, x, y);
            }
        }

        _cells = storage;
    }

    internal FastColor this[int x, int y] => _cells[((y & Mask) << 2) | (x & Mask)];
}
