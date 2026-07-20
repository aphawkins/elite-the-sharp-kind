// 'Useful Libraries' - Andy Hawkins 2025.

using Xunit;

namespace Useful.Tests;

public sealed class FastColorTests
{
    [Fact]
    public void FastColorEquals()
    {
        // Arrange

        // Act

        // Assert
        Assert.True(BaseColors.TransparentWhite.Equals(BaseColors.TransparentWhite));
        Assert.Equal(BaseColors.TransparentWhite, BaseColors.TransparentWhite);
        Assert.Equal(BaseColors.TransparentWhite, new FastColor(0x00FFFFFF));
    }

    [Fact]
    public void ImplicitConversionFromUintRoundTrips()
    {
        FastColor color = 0xFF102030;

        Assert.Equal((byte)0xFF, color.A);
        Assert.Equal((byte)0x10, color.R);
        Assert.Equal((byte)0x20, color.G);
        Assert.Equal((byte)0x30, color.B);
    }

    [Fact]
    public void ImplicitConversionToUintRoundTrips()
    {
        FastColor color = new(0xFF102030);

        Assert.Equal(0xFF102030u, (uint)color);
    }
}
