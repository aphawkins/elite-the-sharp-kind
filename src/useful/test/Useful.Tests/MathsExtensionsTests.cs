// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Maths;
using Xunit;

namespace Useful.Tests;

public class MathsExtensionsTests
{
    [Theory]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(-3, true)]
    [InlineData(0, false)]
    public void IsOddIntReturnsExpectedResult(int value, bool expected) => Assert.Equal(expected, value.IsOdd());

    [Theory]
    [InlineData(3.9f, true)] // truncates to 3, which is odd
    [InlineData(4.9f, false)] // truncates to 4, which is even
    [InlineData(-3.9f, true)] // truncates to -3, which is odd
    [InlineData(0f, false)]
    public void IsOddFloatTruncatesBeforeCheckingParity(float value, bool expected) => Assert.Equal(expected, value.IsOdd());
}
