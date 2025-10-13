// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Graphics.Tests;

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
}
