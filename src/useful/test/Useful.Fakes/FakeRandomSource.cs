// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Fakes;

// Configurable fake of IRandomSource: every member returns a fixed,
// test-set value instead of drawing from a PRNG, so a test can force an
// exact branch (e.g. "the 1-in-256 roll succeeds") without hunting for a
// seed that happens to produce it.
public sealed class FakeRandomSource : IRandomSource
{
    public int NextIntValue { get; set; }

    public int RandomValue { get; set; }

    public bool TrueOrFalseValue { get; set; }

    public int GaussianRandomValue { get; set; }

    public int NextInt() => NextIntValue;

    public int Random(int toExclusive) => RandomValue;

    public int Random(int fromInclusive, int toExclusive) => RandomValue;

    public bool TrueOrFalse() => TrueOrFalseValue;

    public int GaussianRandom(int min, int max) => GaussianRandomValue;
}
