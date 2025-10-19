// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.SDL.Tests;

public class SDLKeyboardTests
{
    [Fact]
    public void InitialStateDefaults()
    {
        SDLKeyboard kb = new();

        Assert.False(kb.IsPressed(ConsoleKey.A));
        Assert.False(kb.IsPressed(ConsoleModifiers.Shift));
        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(ConsoleKey.None, key);
        Assert.Equal(ConsoleModifiers.None, modifiers);
    }

    [Fact]
    public void KeyDownLastPressedReturnsAndClears()
    {
        SDLKeyboard kb = new();

        kb.KeyDown(ConsoleKey.A, ConsoleModifiers.Shift);

        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(ConsoleKey.A, key);
        Assert.Equal(ConsoleModifiers.Shift, modifiers);

        // subsequent LastPressed is cleared
        (ConsoleKey key1, ConsoleModifiers modifiers1) = kb.LastPressed();
        Assert.Equal(ConsoleKey.None, key1);
        Assert.Equal(ConsoleModifiers.None, modifiers1);
    }

    [Fact]
    public void KeyDownIsPressedKeyConsumedThenNotPressed()
    {
        SDLKeyboard kb = new();

        kb.KeyDown(ConsoleKey.B, ConsoleModifiers.None);

        // first call consumes the last-key and returns true
        Assert.True(kb.IsPressed(ConsoleKey.B));

        // second call returns false (consumed / cleared)
        Assert.False(kb.IsPressed(ConsoleKey.B));
    }

    [Fact]
    public void KeyDownIsPressedModifierConsumedThenNotPressed()
    {
        SDLKeyboard kb = new();

        kb.KeyDown(ConsoleKey.C, ConsoleModifiers.Control);

        // first call consumes the last-modifier and returns true
        Assert.True(kb.IsPressed(ConsoleModifiers.Control));

        // second call returns false (consumed / cleared)
        Assert.False(kb.IsPressed(ConsoleModifiers.Control));
    }

    [Fact]
    public void KeyUpRemovesPressed()
    {
        SDLKeyboard kb = new();

        kb.KeyDown(ConsoleKey.D, ConsoleModifiers.Alt);

        // remove it
        kb.KeyUp(ConsoleKey.D, ConsoleModifiers.Alt);

        // Should not be reported pressed (last-key was set by KeyDown, but IsPressed will check pressed map too)
        Assert.False(kb.IsPressed(ConsoleKey.D));
        Assert.False(kb.IsPressed(ConsoleModifiers.Alt));
    }

    [Fact]
    public void ClearPressedResetsAllState()
    {
        SDLKeyboard kb = new();

        kb.KeyDown(ConsoleKey.E, ConsoleModifiers.Shift | ConsoleModifiers.Control);
        Assert.True(kb.IsPressed(ConsoleKey.E) || kb.IsPressed(ConsoleModifiers.Shift) || kb.IsPressed(ConsoleModifiers.Control));

        kb.ClearPressed();

        Assert.False(kb.IsPressed(ConsoleKey.E));
        Assert.False(kb.IsPressed(ConsoleModifiers.Shift));
        Assert.False(kb.IsPressed(ConsoleModifiers.Control));
        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(ConsoleKey.None, key);
        Assert.Equal(ConsoleModifiers.None, modifiers);
    }
}
