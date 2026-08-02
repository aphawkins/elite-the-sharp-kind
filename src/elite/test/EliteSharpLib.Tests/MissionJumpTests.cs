// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Missions;
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
    // the Thargoid one, and the pair of stages is which briefing the screen
    // then shows - the Thargoid jumps also want the Constrictor paid for,
    // which is the one dependency between the two missions.
    [Theory]
    [InlineData(0, ConstrictorMission.Briefed, ThargoidMission.None)]
    [InlineData(1, ConstrictorMission.Rewarded, ThargoidMission.None)]
    [InlineData(2, ConstrictorMission.Rewarded, ThargoidMission.Summoned)]
    [InlineData(3, ConstrictorMission.Rewarded, ThargoidMission.CarryingPlans)]
    [InlineData(4, ConstrictorMission.Rewarded, ThargoidMission.Rewarded)]
    public void EachStageReachesItsBriefing(int stage, string expectedConstrictor, string expectedThargoid)
    {
        using HeadlessGameHarness harness = new();
        harness.Run(3, [new(1, ConsoleKey.N, KeyScriptAction.Tap), new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap)]);

        MissionJump.To(harness.Game.State, stage);

        Assert.Equal(stage < 2 ? Screen.MissionOne : Screen.MissionTwo, harness.Game.State.CurrentScreen);
        Assert.Equal(expectedConstrictor, harness.Game.State.Cmdr.Missions.StageOf(ConstrictorMission.Id));
        Assert.Equal(expectedThargoid, harness.Game.State.Cmdr.Missions.StageOf(ThargoidMission.Id));
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
