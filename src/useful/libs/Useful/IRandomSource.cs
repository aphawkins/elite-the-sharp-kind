// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful;

/// <summary>
/// An injected, seedable source of randomness for game logic.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Generates a random, non-negative number.
    /// </summary>
    /// <returns>A random number.</returns>
    public int NextInt();

    /// <summary>
    /// Generates a random number from zero to the exclusive upper bound.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random number.</returns>
    public int Random(int toExclusive);

    /// <summary>
    /// Generates a random number.
    /// </summary>
    /// <param name="fromInclusive">The exclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random number.</returns>
    public int Random(int fromInclusive, int toExclusive);

    /// <summary>
    /// Generates a random boolean.
    /// </summary>
    /// <returns>A random boolean.</returns>
    public bool TrueOrFalse();

    /// <summary>
    /// Guassian random number generator.
    /// </summary>
    /// <param name="min">The lower bound of the distribution (inclusive).</param>
    /// <param name="max">The upper bound of the distribution (exclusive).</param>
    /// <returns>A number between min and max with Gaussian distribution.</returns>
    public int GaussianRandom(int min, int max);
}
