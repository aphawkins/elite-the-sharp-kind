// 'Useful Libraries' - Andy Hawkins 2025.

using Xunit;

namespace Useful.Tests;

public class GuardTests
{
    [Fact]
    public void ArgumentNullThrowsWhenArgumentIsNullAndParamNameIsCaller()
    {
        // Arrange
        const string obj = null!;

        // Act
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNull(obj));

        // Assert
        Assert.Equal("obj", ex.ParamName);
    }

    [Fact]
    public void ArgumentNullDoesNotThrowWhenArgumentIsNotNull()
    {
        // Arrange
        object obj = new();

        // Act & Assert
        Guard.ArgumentNull(obj);
    }
}
