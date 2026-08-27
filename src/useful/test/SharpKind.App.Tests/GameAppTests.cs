// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.DependencyInjection;
using SharpKind.Abstraction;
using SharpKind.Abstraction.Config;
using Xunit;

namespace SharpKind.App.Tests;

/// <summary>
/// How a game exits when it cannot start. The case that matters is the
/// composition: it used to sit outside the try/catch, so a missing rendition or
/// asset unwound through <c>Main</c> as a raw stack trace and, from a shortcut,
/// as a window that never appeared.
/// <para>
/// <c>Run</c> writes a real log file under the user data directory, because
/// that path is resolved rather than injected. The tests use a log name of
/// their own and delete it afterwards.
/// </para>
/// </summary>
public sealed class GameAppTests : IDisposable
{
    private const string LogFileName = "sharpkind-app-tests-.log";
    private const string LogLevelVariable = "SHARPKIND_APP_TESTS_LOG_LEVEL";

    private readonly List<string> _reported = [];

    [Fact]
    public void AFailureToComposeIsReportedRatherThanUnwinding()
    {
        // Act
        int exit = Run((_, _, _) => throw new InvalidOperationException("No rendition named '8-bot' is installed."));

        // Assert
        Assert.Equal(-1, exit);
        Assert.Contains("8-bot", Assert.Single(_reported), StringComparison.Ordinal);
    }

    [Fact]
    public void AFailureWhileRunningIsStillReported()
    {
        // Act
        int exit = Run(Composing(new ThrowingGame()));

        // Assert
        Assert.Equal(-1, exit);
        Assert.Contains("the wheels came off", Assert.Single(_reported), StringComparison.Ordinal);
    }

    [Fact]
    public void AGameThatRunsToCompletionReportsNothing()
    {
        // Act
        int exit = Run(Composing(new QuietGame()));

        // Assert
        Assert.Equal(0, exit);
        Assert.Empty(_reported);
    }

    public void Dispose()
    {
        if (!AppStartup.TryResolveUserDataPath(out string userDataPath))
        {
            return;
        }

        string logs = Path.Combine(userDataPath, "logs");

        if (!Directory.Exists(logs))
        {
            return;
        }

        foreach (string file in Directory.EnumerateFiles(logs, "sharpkind-app-tests-*.log"))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Best effort: a log the test could not remove is litter, not a
                // failure of what is under test.
            }
        }
    }

    private static Func<string, Microsoft.Extensions.Logging.ILoggerFactory, EngineConfigSettings, ServiceCollection>
        Composing(IGameApp game)
        => (_, _, _) =>
        {
            ServiceCollection services = new();
            services.AddSingleton(game);
            return services;
        };

    private int Run(Func<string, Microsoft.Extensions.Logging.ILoggerFactory, EngineConfigSettings, ServiceCollection> buildServices)
        => GameApp.Run(
            "SharpKind.App tests",
            LogFileName,
            LogLevelVariable,
            (_, _) => new EngineConfigSettings(),
            buildServices,
            (_, message) => _reported.Add(message));

    private sealed class QuietGame : IGameApp
    {
        public void Run()
        {
        }
    }

    private sealed class ThrowingGame : IGameApp
    {
        public void Run() => throw new InvalidOperationException("the wheels came off");
    }
}
