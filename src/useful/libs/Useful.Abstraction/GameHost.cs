// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Controls;
using Useful.Timing;

namespace Useful.Abstraction;

/// <summary>
/// Runs an <see cref="IGame"/> on the fixed-timestep <see cref="GameLoop"/>:
/// the keyboard is polled and the game updated at <c>updatesPerSecond</c>,
/// and the game draws at up to <c>maxFramesPerSecond</c>.
/// </summary>
/// <remarks>
/// Two opt-in, environment-variable-gated debug facilities for the rare
/// live-SDL-window check that a headless <c>HeadlessGameHarness</c> run
/// can't cover, so default behaviour (no env vars set) is unchanged:
/// <list type="bullet">
/// <item><description><c>GAME_KEY_SCRIPT</c>: a file path (or, if the
/// value isn't an existing file, the script text itself) parsed by
/// <see cref="KeyScriptParser"/> and replayed into the real keyboard sink
/// tick-by-tick via <see cref="KeyScriptPlayer"/> - reproducible scripted
/// input instead of OS-level key injection.</description></item>
/// <item><description><c>GAME_FRAME_DUMP_DIR</c>: a directory that
/// enables the F12 debug key and the script's <c>SaveFrame</c> command,
/// both of which dump the current native-resolution framebuffer there as
/// a BMP.</description></item>
/// </list>
/// </remarks>
public static class GameHost
{
    private const string KeyScriptEnvVar = "GAME_KEY_SCRIPT";
    private const string FrameDumpDirEnvVar = "GAME_FRAME_DUMP_DIR";

    public static void Run(IAbstraction abstraction, IGame game, double updatesPerSecond, double maxFramesPerSecond)
    {
        Guard.ArgumentNull(abstraction);
        Guard.ArgumentNull(game);

        KeyScriptPlayer? scriptPlayer = CreateScriptPlayer(abstraction.Keyboard);
        string? frameDumpDirectory = Environment.GetEnvironmentVariable(FrameDumpDirEnvVar);

        GameLoop loop = new(
            updatesPerSecond,
            () =>
            {
                scriptPlayer?.BeforeUpdate();
                abstraction.Keyboard.Poll();
                game.Update();
                bool scriptSaveRequested = scriptPlayer?.AfterUpdate() ?? false;

                if (frameDumpDirectory is not null
                    && (scriptSaveRequested || abstraction.Keyboard.IsPressed(ConsoleKey.F12)))
                {
                    SaveFrameDump(abstraction, frameDumpDirectory);
                }
            },
            game.Draw,
            () => game.IsRunning && !abstraction.Keyboard.Close,
            maxFramesPerSecond);
        loop.Run();
    }

    private static KeyScriptPlayer? CreateScriptPlayer(IKeyboard keyboard)
    {
        string? scriptSource = Environment.GetEnvironmentVariable(KeyScriptEnvVar);
        if (scriptSource is null || keyboard is not IKeyboardSink sink)
        {
            return null;
        }

        string scriptText = File.Exists(scriptSource) ? File.ReadAllText(scriptSource) : scriptSource;
        return new KeyScriptPlayer(sink, KeyScriptParser.Parse(scriptText));
    }

    private static void SaveFrameDump(IAbstraction abstraction, string frameDumpDirectory)
    {
        Directory.CreateDirectory(frameDumpDirectory);
        string path = Path.Combine(frameDumpDirectory, $"frame-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmssfff}.bmp");
        abstraction.Graphics.SaveScreen(path);
    }
}
