// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace SharpKind.App.Tests;

/// <summary>
/// What a player is told when the game will not start. The text is shared by
/// the console and the failure dialog, so it is worth pinning: the whole point
/// of the message is that it names the thing that went wrong.
/// </summary>
public class AppStartupTests
{
    [Fact]
    public void DescribeFailureNamesWhatWentWrong()
    {
        // Arrange
        InvalidOperationException ex = new("No rendition named '8-bot' is installed.");

        // Act
        string description = AppStartup.DescribeFailure(ex, @"C:\data");

        // Assert
        Assert.Contains("No rendition named '8-bot' is installed.", description, StringComparison.Ordinal);
    }

    [Fact]
    public void DescribeFailureNamesTheLogDirectory()
    {
        // Act
        string description = AppStartup.DescribeFailure(new InvalidOperationException("boom"), @"C:\data");

        // Assert
        Assert.Contains(Path.Combine(@"C:\data", "logs"), description, StringComparison.Ordinal);
    }

    // A missing native library is the one failure whose own message helps
    // nobody, so it gets told what it actually means instead.
    [Fact]
    public void DescribeFailureExplainsAMissingNativeLibrary()
    {
        // Act
        string description = AppStartup.DescribeFailure(new DllNotFoundException("SDL3"), @"C:\data");

        // Assert
        Assert.Contains("native library", description, StringComparison.Ordinal);
        Assert.Contains("platform/architecture", description, StringComparison.Ordinal);
    }

    [Fact]
    public void DescribeFailureWithNullExceptionThrows()
        => Assert.Throws<ArgumentNullException>(() => AppStartup.DescribeFailure(null!, @"C:\data"));
}
