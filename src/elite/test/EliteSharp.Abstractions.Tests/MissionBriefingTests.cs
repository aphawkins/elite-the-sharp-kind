// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Missions;
using Xunit;

namespace EliteSharp.Abstractions.Tests;

public class MissionBriefingTests
{
    [Fact]
    public void RefusesABriefingWithNothingToSay()
        => Assert.Throws<ArgumentException>(() => new MissionBriefing { Paragraphs = [] });

    [Fact]
    public void RefusesParagraphsThatWereNeverSet()
        => Assert.Throws<ArgumentException>(() => new MissionBriefing { Paragraphs = default });

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RefusesABlankParagraph(string blank)
        => Assert.Throws<ArgumentException>(() => new MissionBriefing { Paragraphs = ["Something.", blank] });

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RefusesAHeadlineThatIsBlankRatherThanAbsent(string blank)
        => Assert.Throws<ArgumentException>(() => new MissionBriefing { Paragraphs = ["Something."], Headline = blank });

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RefusesAShipNameThatIsBlankRatherThanAbsent(string blank)
        => Assert.Throws<ArgumentException>(() => new MissionBriefing { Paragraphs = ["Something."], ShipName = blank });

    [Fact]
    public void ChecksAgainWhenOneBriefingIsBuiltFromAnother()
    {
        // Arrange: `with` is the way round a constructor, so the checks have to
        // sit on the setters rather than in one.
        MissionBriefing briefing = new() { Paragraphs = ["Something."] };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => briefing with { Paragraphs = [] });
        Assert.Throws<ArgumentException>(() => briefing with { Headline = " " });
        Assert.Throws<ArgumentException>(() => briefing with { ShipName = " " });
    }

    [Fact]
    public void KeepsTheWordsItWasHandedWhenTheMissionGoesOnEditing()
    {
        // Arrange: a mission builds its paragraphs from a list, and must not be
        // able to rewrite a briefing the game is already showing.
        List<string> paragraphs = ["Something."];
        MissionBriefing briefing = new() { Paragraphs = [.. paragraphs] };

        // Act
        paragraphs[0] = "Something else.";

        // Assert
        Assert.Equal("Something.", briefing.Paragraphs[0]);
    }

    [Fact]
    public void CountsTwoBriefingsWithTheSameWordsAsOne()
    {
        // Arrange: the compiler's own comparison would compare the paragraphs
        // by reference and make these unequal.
        MissionBriefing first = new() { Paragraphs = ["One.", "Two."], Headline = "Well done", Portrait = MissionPortrait.Blake };
        MissionBriefing second = new() { Paragraphs = ["One.", "Two."], Headline = "Well done", Portrait = MissionPortrait.Blake };

        // Act & Assert
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void TellsTwoBriefingsApartByTheirWords()
    {
        // Arrange
        MissionBriefing first = new() { Paragraphs = ["One."] };
        MissionBriefing second = new() { Paragraphs = ["Two."] };

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void KeepsTheRestOfTheBriefingWhenOnePartIsChanged()
    {
        // Arrange
        MissionBriefing briefing = new()
        {
            Paragraphs = ["One."],
            Headline = "Well done",
            Portrait = MissionPortrait.Blake,
            ShipName = "Constrictor",
        };

        // Act
        MissionBriefing amended = briefing with { Headline = null };

        // Assert
        Assert.Null(amended.Headline);
        Assert.Equal(MissionPortrait.Blake, amended.Portrait);
        Assert.Equal("Constrictor", amended.ShipName);
        Assert.Equal(["One."], amended.Paragraphs);
    }
}
