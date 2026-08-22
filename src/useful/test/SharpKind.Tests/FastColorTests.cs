// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace SharpKind.Tests;

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
    public void BlendPassesOpaqueSourceThrough()
        => Assert.Equal(new FastColor(0xFF102030), FastColor.Blend(new(0xFF102030), new(0xFF807060)));

    [Fact]
    public void BlendKeepsDestinationForFullyTransparentSource()
        => Assert.Equal(new FastColor(0xFF807060), FastColor.Blend(new(0x00102030), new(0xFF807060)));

    [Fact]
    public void BlendMixesHalfwayAndStaysOpaque()
    {
        FastColor result = FastColor.Blend(new(0x80FFFFFF), new(0xFF000000));

        Assert.Equal((byte)255, result.A);
        Assert.Equal((byte)128, result.R);
        Assert.Equal((byte)128, result.G);
        Assert.Equal((byte)128, result.B);
    }

    [Theory]
    [InlineData(0x00u)]
    [InlineData(0x40u)]
    [InlineData(0x80u)]
    [InlineData(0xC0u)]
    [InlineData(0xFFu)]
    public void BlendOfEqualColoursIsThatColour(uint alpha)
        => Assert.Equal(
            new FastColor(0xFF204060),
            FastColor.Blend(new((alpha << 24) | 0x204060), new(0xFF204060)));

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
