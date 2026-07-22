// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Fakes.Controls;

namespace Useful.Controls.Tests;

public class KeyScriptPlayerTests
{
    [Fact]
    public void TapPressesForOneTickThenReleases()
    {
        FakeKeyboard keyboard = new();
        KeyScriptPlayer player = new(keyboard, [new KeyScriptEvent(0, ConsoleKey.S, KeyScriptAction.Tap)]);

        player.BeforeUpdate();
        Assert.True(keyboard.IsHeld(ConsoleKey.S));

        player.AfterUpdate();
        Assert.False(keyboard.IsHeld(ConsoleKey.S));
    }

    [Fact]
    public void HoldStaysDownUntilExplicitRelease()
    {
        FakeKeyboard keyboard = new();
        KeyScriptPlayer player = new(
            keyboard,
            [
                new KeyScriptEvent(0, ConsoleKey.UpArrow, KeyScriptAction.Hold),
                new KeyScriptEvent(2, ConsoleKey.UpArrow, KeyScriptAction.Release),
            ]);

        player.BeforeUpdate();
        player.AfterUpdate();
        Assert.True(keyboard.IsHeld(ConsoleKey.UpArrow));
        Assert.Equal(1, player.Tick);

        player.BeforeUpdate();
        player.AfterUpdate();
        Assert.True(keyboard.IsHeld(ConsoleKey.UpArrow));

        player.BeforeUpdate();
        Assert.False(keyboard.IsHeld(ConsoleKey.UpArrow));
        player.AfterUpdate();
    }

    [Fact]
    public void SaveFrameIsReportedOnlyOnItsTick()
    {
        FakeKeyboard keyboard = new();
        KeyScriptPlayer player = new(keyboard, [new KeyScriptEvent(1, ConsoleKey.None, KeyScriptAction.SaveFrame)]);

        player.BeforeUpdate();
        Assert.False(player.AfterUpdate());

        player.BeforeUpdate();
        Assert.True(player.AfterUpdate());

        player.BeforeUpdate();
        Assert.False(player.AfterUpdate());
    }
}
