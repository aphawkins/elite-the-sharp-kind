// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful;

/// <summary>
/// Shared console-app startup helpers: resolving the per-user data directory and reporting
/// startup/crash failures in a way that's actionable rather than a raw stack dump.
/// </summary>
public static class AppStartup
{
    private const string AppDataDirName = "TheSharpKind";

    /// <summary>
    /// Resolves the shared per-user data directory (e.g. <c>%AppData%\TheSharpKind</c> on Windows,
    /// <c>~/.config/TheSharpKind</c> on Linux). Returns <see langword="false"/>, prints a diagnostic
    /// to stderr, and appends a fallback startup log if it cannot be resolved to an absolute path -
    /// most commonly because the HOME environment variable is unset on Linux.
    /// </summary>
    public static bool TryResolveUserDataPath(out string userDataPath)
    {
        userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataDirName);

        if (Path.IsPathRooted(userDataPath))
        {
            return true;
        }

        const string message = "Could not determine the user data directory (resolved to a relative path); "
            + "the HOME environment variable is likely not set.";

        Console.Error.WriteLine(message);
        Console.Error.WriteLine("Set HOME and try again.");
        WriteFallbackStartupLog(message);
        return false;
    }

    /// <summary>
    /// Prints an actionable hint for an unhandled startup/runtime failure. Full exception details
    /// (including the stack trace) are expected to already have been logged via the app's normal
    /// logger before calling this; it only improves what the user sees on the console.
    /// </summary>
    public static void WriteFailureHint(Exception ex, string userDataPath)
    {
        if (ex is DllNotFoundException)
        {
            Console.Error.WriteLine("A required native library could not be loaded.");
            Console.Error.WriteLine("On Linux this usually means the SDL2 runtime packages aren't installed:");
            Console.Error.WriteLine("  sudo apt-get install libsdl2-2.0-0 libsdl2-ttf-2.0-0 libsdl2-mixer-2.0-0");
        }
        else
        {
            Console.Error.WriteLine("Application terminated unexpectedly.");
        }

        Console.Error.WriteLine($"See the log file under {Path.Combine(userDataPath, "logs")} for full details.");
    }

    private static void WriteFallbackStartupLog(string message)
    {
        try
        {
            File.AppendAllText(
                Path.Combine(Path.GetTempPath(), $"{AppDataDirName}-startup-error.log"),
                $"{DateTime.UtcNow:O} {message}{Environment.NewLine}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best-effort fallback logging only; nothing more useful to do if this fails too.
        }
    }
}
