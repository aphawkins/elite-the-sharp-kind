// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Types;
using EliteSharpLib.Views;
using Useful.Controls;

namespace EliteSharpLib.Tests;

// The jumps set the state each mission screen's own Reset tests for and let
// that Reset run, so a jump landing on the wrong screen - or on the right
// screen with the wrong briefing - means the entry conditions have drifted
// from the controllers. That is exactly what these check.
public class MissionJumpTests
{
    // The first two stages belong to the Constrictor mission and the rest to
    // the Thargoid one, and the mission number is which briefing the screen
    // then shows.
    [Theory]
    [InlineData(0, (int)MissionStage.ConstrictorBriefed)]
    [InlineData(1, (int)MissionStage.ConstrictorRewarded)]
    [InlineData(2, (int)MissionStage.ThargoidSummoned)]
    [InlineData(3, (int)MissionStage.ThargoidCarryingPlans)]
    [InlineData(4, (int)MissionStage.ThargoidRewarded)]
    public void EachStageReachesItsBriefing(int stage, int expectedMission)
    {
        using HeadlessGameHarness harness = new();
        harness.Run(3, [new(1, ConsoleKey.N, KeyScriptAction.Tap), new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap)]);

        MissionJump.To(harness.Game.State, stage);

        Assert.Equal(stage < 2 ? Screen.MissionOne : Screen.MissionTwo, harness.Game.State.CurrentScreen);
        Assert.Equal((MissionStage)expectedMission, harness.Game.State.Cmdr.Mission);
    }

    [Fact]
    public void CountCoversEveryStage()
    {
        using HeadlessGameHarness harness = new();
        harness.Run(3, [new(1, ConsoleKey.N, KeyScriptAction.Tap), new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap)]);

        // Cycling Count times from any starting point must come back round
        // rather than falling off the end into the default branch.
        for (int stage = 0; stage < MissionJump.Count; stage++)
        {
            MissionJump.To(harness.Game.State, stage);
            Assert.True(harness.Game.State.CurrentScreen is Screen.MissionOne or Screen.MissionTwo);
        }
    }

    [Fact]
    public void IsDisabledUnlessTheEnvironmentVariableIsSet()
        => Assert.Equal(
            Environment.GetEnvironmentVariable(MissionJump.EnvVar) is not null,
            MissionJump.IsEnabled);
}
