// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful;

/// <summary>
/// Default <see cref="IRandomSource"/> implementation: wraps an injected
/// <see cref="System.Random"/>, so it is fast (unlike a cryptographic RNG)
/// and seedable (unlike static state).
/// </summary>
public sealed class RandomSource(Random random) : IRandomSource
{
    private readonly Random _random = random;

    public int NextInt() => _random.Next();

    public int Random(int toExclusive) => Random(0, toExclusive);

    public int Random(int fromInclusive, int toExclusive) => _random.Next(fromInclusive, toExclusive);

    public bool TrueOrFalse() => Random(0, 2) != 0;

    public int GaussianRandom(int min, int max)
    {
        const int iterations = 12;
        int r = 0;
        for (int i = 0; i < iterations; i++)
        {
            r += Random(min, max);
        }

        r /= iterations;

        return r;
    }
}
