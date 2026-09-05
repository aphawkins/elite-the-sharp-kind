// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using SharpKind.Input;

namespace EliteSharpLib.Tests;

// Yaw is not Elite's. It exists behind ELITE_DEBUG_YAW, so what these check
// is the switch as much as the axis: with the variable unset the yaw keys do
// nothing at all, which is what keeps the shipped game the original's two
// axes and the golden traces unchanged.
[Trait("Level", "Integration")]

// Serialised against the other classes that read or write ELITE_DEBUG_YAW.
// EnvironmentVariableScope puts a variable back, but it cannot stop another
// class reading it in the meantime: an environment variable belongs to the
// process, and xUnit runs test classes in parallel.
[Collection("EnvironmentVariables")]
public class DebugYawTests
{
    // Into flight: N past the title, Space past the parade, F1 to launch.
    // Launch zeroes the yaw, so the key has to be held after it.
    private static readonly KeyScriptEvent[] s_launchThenYawLeft =
    [
        new(1, ConsoleKey.N, KeyScriptAction.Tap),
        new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
        new(4, ConsoleKey.F1, KeyScriptAction.Tap),
        new(40, ConsoleKey.Q, KeyScriptAction.Hold),
    ];

    [Fact]
    public void TheYawKeysTurnTheShipWhenTheVariableIsSet()
    {
        using EnvironmentVariableScope yaw = EnvironmentVariableScope.Set(DebugYaw.EnvVar, "1");
        using HeadlessGameHarness harness = new(randomSeed: 1);

        harness.Run(60, s_launchThenYawLeft);

        Assert.True(harness.Resolve<PlayerShip>().Yaw < 0);
    }

    [Fact]
    public void TheYawKeysDoNothingWhenTheVariableIsUnset()
    {
        using EnvironmentVariableScope yaw = EnvironmentVariableScope.Set(DebugYaw.EnvVar, null);
        using HeadlessGameHarness harness = new(randomSeed: 1);

        harness.Run(60, s_launchThenYawLeft);

        Assert.Equal(0, harness.Resolve<PlayerShip>().Yaw);
    }

    // The stick centres itself the way the roll does, so a released yaw
    // decays to nothing rather than leaving the ship turning for ever.
    [Fact]
    public void AReleasedYawLevelsOut()
    {
        using EnvironmentVariableScope yaw = EnvironmentVariableScope.Set(DebugYaw.EnvVar, "1");
        using HeadlessGameHarness harness = new(randomSeed: 1);

        harness.Run(
            120,
            [
                new(1, ConsoleKey.N, KeyScriptAction.Tap),
                new(2, ConsoleKey.Spacebar, KeyScriptAction.Tap),
                new(4, ConsoleKey.F1, KeyScriptAction.Tap),
                new(40, ConsoleKey.Q, KeyScriptAction.Hold),
                new(60, ConsoleKey.Q, KeyScriptAction.Release),
            ]);

        Assert.Equal(0, harness.Resolve<PlayerShip>().Yaw);
    }
}
