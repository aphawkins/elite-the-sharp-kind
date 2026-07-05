// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Abstraction;
using Useful.Fakes.Controls;
using Xunit;

namespace Useful.Tests;

public class ScreenManagerTests
{
    private enum TestScreenId
    {
        None = 0,
        First = 1,
        Second = 2,
    }

    [Fact]
    public void ConstructWithNullKeyboardThrows()
        => Assert.Throws<ArgumentNullException>(() => new ScreenManager<TestScreenId, FakeScreen>(null!));

    [Fact]
    public void AddNullScreenThrows()
    {
        // Arrange
        ScreenManager<TestScreenId, FakeScreen> manager = new(new FakeKeyboard());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => manager.Add(TestScreenId.First, null!));
    }

    [Fact]
    public void NoScreenIsCurrentBeforeTheFirstSet()
    {
        // Arrange & Act
        ScreenManager<TestScreenId, FakeScreen> manager = new(new FakeKeyboard());

        // Assert
        Assert.Equal(TestScreenId.None, manager.CurrentId);
        Assert.Null(manager.Current);
    }

    [Fact]
    public void SetMakesTheScreenCurrentAndResetsIt()
    {
        // Arrange
        ScreenManager<TestScreenId, FakeScreen> manager = new(new FakeKeyboard());
        FakeScreen screen = new();
        manager.Add(TestScreenId.First, screen);

        // Act
        manager.Set(TestScreenId.First);

        // Assert
        Assert.Equal(TestScreenId.First, manager.CurrentId);
        Assert.Same(screen, manager.Current);
        Assert.Equal(1, screen.ResetCount);
    }

    [Fact]
    public void SetClearsPendingKeyPresses()
    {
        // Arrange
        FakeKeyboard keyboard = new();
        ScreenManager<TestScreenId, FakeScreen> manager = new(keyboard);
        manager.Add(TestScreenId.First, new());
        keyboard.KeyDown(ConsoleKey.S, ConsoleModifiers.None);

        // Act
        manager.Set(TestScreenId.First);

        // Assert
        Assert.False(keyboard.IsPressed(ConsoleKey.S));
    }

    [Fact]
    public void SetSwitchesBetweenScreens()
    {
        // Arrange
        ScreenManager<TestScreenId, FakeScreen> manager = new(new FakeKeyboard());
        FakeScreen first = new();
        FakeScreen second = new();
        manager.Add(TestScreenId.First, first);
        manager.Add(TestScreenId.Second, second);
        manager.Set(TestScreenId.First);

        // Act
        manager.Set(TestScreenId.Second);

        // Assert
        Assert.Equal(TestScreenId.Second, manager.CurrentId);
        Assert.Same(second, manager.Current);
        Assert.Equal(1, first.ResetCount);
        Assert.Equal(1, second.ResetCount);
    }

    private sealed class FakeScreen : IGameScreen
    {
        public int ResetCount { get; private set; }

        public void Reset() => ResetCount++;

        public void Update()
        {
        }

        public void Draw()
        {
        }
    }
}
