// 'Useful Libraries' - Andy Hawkins 2023-2026.

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
    public void FromUInt32RoundTrips()
    {
        FastColor color = FastColor.FromUInt32(0xFF102030);

        Assert.Equal((byte)0xFF, color.A);
        Assert.Equal((byte)0x10, color.R);
        Assert.Equal((byte)0x20, color.G);
        Assert.Equal((byte)0x30, color.B);
    }

    [Fact]
    public void ToUInt32RoundTrips()
    {
        FastColor color = new(0xFF102030);

        Assert.Equal(0xFF102030u, FastColor.ToUInt32(color));
        Assert.Equal(0xFF102030u, color.Argb);
    }
}
