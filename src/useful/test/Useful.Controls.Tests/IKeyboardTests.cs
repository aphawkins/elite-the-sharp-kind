// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Fakes.Controls;

namespace Useful.Controls.Tests;

public class IKeyboardTests
{
    [Fact]
    public void InitialStateIsNotPressedCloseFalseLastPressedDefault()
    {
        FakeKeyboard kb = new();

        Assert.False(kb.Close);
        Assert.False(kb.IsPressed(ConsoleKey.A));
        Assert.False(kb.IsPressed(ConsoleModifiers.Shift));
        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(default, key);
        Assert.Equal(default, modifiers);
    }

    [Fact]
    public void KeyDownSetsPressedLastPressedAndKeyUpRemovesPressed()
    {
        FakeKeyboard kb = new();

        kb.KeyDown(ConsoleKey.A, ConsoleModifiers.Shift);
        Assert.True(kb.IsPressed(ConsoleKey.A));
        Assert.True(kb.IsPressed(ConsoleModifiers.Shift));

        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(ConsoleKey.A, key);
        Assert.Equal(ConsoleModifiers.Shift, modifiers);

        kb.KeyUp(ConsoleKey.A, ConsoleModifiers.Shift);
        Assert.False(kb.IsPressed(ConsoleKey.A));
        Assert.False(kb.IsPressed(ConsoleModifiers.Shift));
    }

    [Fact]
    public void ClearPressedRemovesAllPressed()
    {
        FakeKeyboard kb = new();

        kb.KeyDown(ConsoleKey.A, ConsoleModifiers.None);
        kb.KeyDown(ConsoleKey.B, ConsoleModifiers.Control);
        Assert.True(kb.IsPressed(ConsoleKey.A));
        Assert.True(kb.IsPressed(ConsoleKey.B));

        kb.ClearPressed();
        Assert.False(kb.IsPressed(ConsoleKey.A));
        Assert.False(kb.IsPressed(ConsoleKey.B));
    }

    [Fact]
    public void PollDoesNotThrowAndCloseCanBeSetByImplementation()
    {
        FakeKeyboard kb = new();

        // Poll is a no-op in the fake, but should not throw in real usage
        kb.Poll();

        // demonstrate Close is readable; implementation may control it
        kb.SetClose(true);
        Assert.True(kb.Close);
    }
}
