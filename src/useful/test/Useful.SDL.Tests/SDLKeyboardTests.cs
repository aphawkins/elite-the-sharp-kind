// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Controls;
using Useful.Fakes.Controls;

namespace Useful.SDL.Tests;

public class SDLKeyboardTests
{
    [Fact]
    public void InitialStateDefaults()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

        Assert.False(kb.IsPressed(ConsoleKey.A));
        Assert.False(kb.IsPressed(ConsoleModifiers.Shift));
        (ConsoleKey key, ConsoleModifiers modifiers) = kb.LastPressed();
        Assert.Equal(ConsoleKey.None, key);
        Assert.Equal(ConsoleModifiers.None, modifiers);
    }

    [Fact]
    public void KeyDownLastPressedReturnsAndClears()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

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
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

        kb.KeyDown(ConsoleKey.B, ConsoleModifiers.None);

        // first call consumes the last-key and returns true
        Assert.True(kb.IsPressed(ConsoleKey.B));

        // second call returns false (consumed / cleared)
        Assert.False(kb.IsPressed(ConsoleKey.B));
    }

    [Fact]
    public void KeyDownIsPressedModifierConsumedThenNotPressed()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

        kb.KeyDown(ConsoleKey.C, ConsoleModifiers.Control);

        // first call consumes the last-modifier and returns true
        Assert.True(kb.IsPressed(ConsoleModifiers.Control));

        // second call returns false (consumed / cleared)
        Assert.False(kb.IsPressed(ConsoleModifiers.Control));
    }

    // Regression test: driving/movement-style controls poll every tick and
    // need the key's continuous physical state, not IsPressed's one-shot
    // consumption (which would make a held key look "let go" the very next
    // tick unless a fresh SDL key-repeat event happened to arrive first).
    [Fact]
    public void IsHeldStaysTrueAcrossRepeatedPollsWhileKeyRemainsDown()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

        kb.KeyDown(ConsoleKey.UpArrow, ConsoleModifiers.None);

        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));
        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));
        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));

        kb.KeyUp(ConsoleKey.UpArrow, ConsoleModifiers.None);

        Assert.False(kb.IsHeld(ConsoleKey.UpArrow));
    }

    // Regression test: holding a second key must not affect an
    // already-held key's IsHeld state (this is the actual "multiple keys
    // held" bug — IsPressed's consumption combined with the OS only
    // re-sending key-repeat events for the most recently pressed key made
    // an earlier held key go silently unresponsive once a second key was
    // pressed).
    [Fact]
    public void IsHeldTracksMultipleKeysIndependently()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

        kb.KeyDown(ConsoleKey.UpArrow, ConsoleModifiers.None);
        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));

        // pressing (and polling) a second key must not clear the first
        kb.KeyDown(ConsoleKey.LeftArrow, ConsoleModifiers.None);
        Assert.True(kb.IsHeld(ConsoleKey.LeftArrow));
        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));

        kb.KeyUp(ConsoleKey.LeftArrow, ConsoleModifiers.None);
        Assert.False(kb.IsHeld(ConsoleKey.LeftArrow));
        Assert.True(kb.IsHeld(ConsoleKey.UpArrow));
    }

    [Fact]
    public void KeyUpRemovesPressed()
    {
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

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
        FakeInput fakeInput = new();
        SoftwareKeyboard kb = new(fakeInput);

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
