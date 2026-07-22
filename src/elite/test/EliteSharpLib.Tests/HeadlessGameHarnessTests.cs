// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Views;
using Useful.Controls;

namespace EliteSharpLib.Tests;

public class HeadlessGameHarnessTests
{
    [Fact]
    public void InitialStateIsNoScreenBeforeFirstUpdate()
    {
        using HeadlessGameHarness harness = new();

        GameStateSummary state = harness.State;

        Assert.Equal(Screen.None, state.Screen);
    }

    [Fact]
    public void ScriptedKeysDriveIntroThroughToCommanderStatus()
    {
        using HeadlessGameHarness harness = new();

        // Tick 0 runs InitialiseGame(), which sets the view to IntroOne and
        // clears any pressed keys on the way in - so the script's first key
        // lands on tick 1. N (tap) at tick 1 answers "Load New Commander?"
        // with No: IntroOne -> IntroTwo. Space (tap) at tick 2 leaves the
        // ship parade: IntroTwo -> CommanderStatus.
        KeyScriptEvent[] script =
        [
            new(1, ConsoleKey.N, KeyScriptAction.Tap),
            new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
        ];

        GameStateSummary state = harness.Run(3, script);

        Assert.Equal(Screen.CommanderStatus, state.Screen);
        Assert.True(state.IsDocked);
        Assert.False(state.IsGameOver);
    }

    [Fact]
    public void SaveFrameWritesABmpOfTheWholeGame()
    {
        using HeadlessGameHarness harness = new();
        harness.Run(2, []);

        string path = Path.Combine(Path.GetTempPath(), $"elite_harness_frame_{Guid.NewGuid():N}.bmp");
        try
        {
            harness.SaveFrame(path);

            FileInfo info = new(path);
            Assert.True(info.Exists);
            Assert.True(info.Length > 0);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
