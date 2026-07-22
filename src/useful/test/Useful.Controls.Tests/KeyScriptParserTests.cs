// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls.Tests;

public class KeyScriptParserTests
{
    [Fact]
    public void ParsesTapHoldAndRelease()
    {
        IReadOnlyList<KeyScriptEvent> events = KeyScriptParser.Parse(
            """
            0 Tap S
            1 Hold UpArrow
            5 Release UpArrow
            """);

        KeyScriptEvent[] expected =
        [
            new(0, ConsoleKey.S, KeyScriptAction.Tap),
            new(1, ConsoleKey.UpArrow, KeyScriptAction.Hold),
            new(5, ConsoleKey.UpArrow, KeyScriptAction.Release),
        ];
        Assert.Equal(expected, events);
    }

    [Fact]
    public void ParsesSaveFrameWithNoKey()
    {
        IReadOnlyList<KeyScriptEvent> events = KeyScriptParser.Parse("3 SaveFrame");

        Assert.Equal([new KeyScriptEvent(3, ConsoleKey.None, KeyScriptAction.SaveFrame)], events);
    }

    [Fact]
    public void ParsesCombinedModifiers()
    {
        IReadOnlyList<KeyScriptEvent> events = KeyScriptParser.Parse("2 Tap A Shift,Control");

        Assert.Equal(
            [new KeyScriptEvent(2, ConsoleKey.A, KeyScriptAction.Tap, ConsoleModifiers.Shift | ConsoleModifiers.Control)],
            events);
    }

    [Fact]
    public void SkipsBlankLinesAndComments()
    {
        IReadOnlyList<KeyScriptEvent> events = KeyScriptParser.Parse(
            """

            # a comment
            0 Tap A

            """);

        Assert.Equal([new KeyScriptEvent(0, ConsoleKey.A, KeyScriptAction.Tap)], events);
    }

    [Fact]
    public void ThrowsOnMissingKey()
        => Assert.Throws<FormatException>(() => KeyScriptParser.Parse("0 Tap"));

    [Fact]
    public void ThrowsOnTooFewFields()
        => Assert.Throws<FormatException>(() => KeyScriptParser.Parse("0"));
}
