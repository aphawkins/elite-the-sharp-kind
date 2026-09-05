// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Views;
using SharpKind.Fakes.Input;
using SharpKind.Input;

namespace EliteSharpLib.Tests.Views;

public class GamepadControlsTests
{
    [Fact]
    public void AnIdlePadAsksForNothing()
    {
        FakeGamepad pad = new();

        Assert.Equal(0, GamepadControls.Roll(pad));
        Assert.Equal(0, GamepadControls.Pitch(pad));
        Assert.Equal(0, GamepadControls.Yaw(pad));
        Assert.False(GamepadControls.IsFiring(pad));
        Assert.False(GamepadControls.IsAccelerating(pad));
        Assert.False(GamepadControls.IsDecelerating(pad));
    }

    // A stick has to travel a good way before it counts, so a resting
    // stick's drift cannot roll the ship. A digital stick reports the ends of
    // the range, so it always passes.
    [Theory]
    [InlineData(-1f, -1)]
    [InlineData(-0.6f, -1)]
    [InlineData(-0.4f, 0)]
    [InlineData(0f, 0)]
    [InlineData(0.4f, 0)]
    [InlineData(0.6f, 1)]
    [InlineData(1f, 1)]
    public void TheStickRollsOnlyPastTheThreshold(float x, int expected)
    {
        FakeGamepad pad = new();
        pad.AxisMoved(GamepadAxis.LeftX, x);

        Assert.Equal(expected, GamepadControls.Roll(pad));
    }

    // Pulling back climbs: SDL reports that as negative, the same sense
    // Elite's own "up" control has.
    [Theory]
    [InlineData(-1f, -1)]
    [InlineData(0f, 0)]
    [InlineData(1f, 1)]
    public void TheStickPitches(float y, int expected)
    {
        FakeGamepad pad = new();
        pad.AxisMoved(GamepadAxis.LeftY, y);

        Assert.Equal(expected, GamepadControls.Pitch(pad));
    }

    // The twist axis. SDL numbers a raw joystick's axes and says nothing
    // about what they are, so a SideWinder's Z Rotation arrives as index 2,
    // which the port names RightX.
    [Theory]
    [InlineData(-1f, -1)]
    [InlineData(0f, 0)]
    [InlineData(1f, 1)]
    public void TheStickYawsOnItsTwist(float twist, int expected)
    {
        FakeGamepad pad = new();
        pad.AxisMoved(GamepadAxis.RightX, twist);

        Assert.Equal(expected, GamepadControls.Yaw(pad));
    }

    [Theory]
    [InlineData(GamepadButton.A)]
    [InlineData(GamepadButton.X)]
    public void EitherFireButtonFires(GamepadButton button)
    {
        FakeGamepad pad = new();
        pad.ButtonDown(button);

        Assert.True(GamepadControls.IsFiring(pad));
    }

    // Firing is polled every frame, so a held button must survive repeated
    // reads - IsPressed's one-shot consumption would not.
    [Fact]
    public void AHeldFireButtonKeepsFiring()
    {
        FakeGamepad pad = new();
        pad.ButtonDown(GamepadButton.A);

        Assert.True(GamepadControls.IsFiring(pad));
        Assert.True(GamepadControls.IsFiring(pad));
        Assert.True(GamepadControls.IsFiring(pad));
    }

    [Fact]
    public void SpeedIsOnTheRemainingButtonsAndOnTheTriggers()
    {
        FakeGamepad buttons = new();
        buttons.ButtonDown(GamepadButton.B);
        Assert.True(GamepadControls.IsAccelerating(buttons));

        buttons.ButtonUp(GamepadButton.B);
        buttons.ButtonDown(GamepadButton.Y);
        Assert.True(GamepadControls.IsDecelerating(buttons));

        FakeGamepad triggers = new();
        triggers.AxisMoved(GamepadAxis.RightTrigger, 1f);
        Assert.True(GamepadControls.IsAccelerating(triggers));

        triggers.AxisMoved(GamepadAxis.LeftTrigger, 1f);
        Assert.True(GamepadControls.IsDecelerating(triggers));
    }

    // Fire is on buttons 1 and 3, speed on 2 and 4, so pressing one never
    // triggers another.
    [Fact]
    public void TheFireAndSpeedButtonsDoNotOverlap()
    {
        FakeGamepad pad = new();
        pad.ButtonDown(GamepadButton.A);

        Assert.False(GamepadControls.IsAccelerating(pad));
        Assert.False(GamepadControls.IsDecelerating(pad));
    }
}
