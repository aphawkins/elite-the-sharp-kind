// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// A direct-colour display: every channel goes to the nearest level its DAC
/// drives. Eight bits a channel is every level there is, so it passes colours
/// through untouched.
/// </summary>
public sealed class ChannelGridQuantiser(int channelBits) : IColourQuantiser
{
    // Zero means "every level there is", so nothing needs snapping.
    private readonly int _top = channelBits >= 8 ? 0 : (1 << channelBits) - 1;

    public bool IsPositionDependent => false;

    // Levels are evenly spaced, so the gap is exact. Eight bits a channel
    // leaves neighbouring values one apart.
    public float LevelGap => _top == 0 ? 1f : 255f / _top;

    public FastColor Quantise(in FastColor colour, int x, int y)
        => _top == 0
            ? colour
            : new(
                colour.A,
                (byte)AssetColourBudget.NearestLevel(colour.R, _top),
                (byte)AssetColourBudget.NearestLevel(colour.G, _top),
                (byte)AssetColourBudget.NearestLevel(colour.B, _top));
}
