// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Immutable;
using EliteSharp.Abstractions.Missions;
using Xunit;

namespace EliteSharp.Abstractions.Tests;

public class MissionStagesTests
{
    private static readonly MissionStages s_stages = new(["NotStarted", "Briefed", "Done"]);

    [Fact]
    public void RefusesAMissionWithNoStages()
        => Assert.Throws<ArgumentException>(() => new MissionStages([]));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void RefusesABlankStageName(string blank)
        => Assert.Throws<ArgumentException>(() => new MissionStages(["NotStarted", blank]));

    [Fact]
    public void RefusesTwoStagesWithOneName()
        => Assert.Throws<ArgumentException>(() => new MissionStages(["NotStarted", "NotStarted"]));

    [Fact]
    public void TellsStagesApartByTheirBytes()
    {
        // Arrange: casing is not prose here, it is a save-file key.
        MissionStages stages = new(["NotStarted", "notstarted"]);

        // Act & Assert
        Assert.Equal(0, stages.IndexOf("NotStarted"));
        Assert.Equal(1, stages.IndexOf("notstarted"));
    }

    [Fact]
    public void StartsInTheFirstStageDeclared()
        => Assert.Equal("NotStarted", s_stages.NotStarted);

    [Fact]
    public void DoesNotPlaceAStageItNeverDeclared()
        => Assert.Equal(-1, s_stages.IndexOf("Rewarded"));

    [Fact]
    public void BuildsAStepThatMovesForwards()
    {
        // Act
        MissionStep step = s_stages.Step("NotStarted", "Briefed");

        // Assert
        Assert.Equal("Briefed", step.Stage);
        Assert.Null(step.Briefing);
        Assert.Null(step.Award);
    }

    [Fact]
    public void RefusesAStepToTheStageAlreadyIn()
        => Assert.Throws<ArgumentException>(() => s_stages.Step("Briefed", "Briefed"));

    [Fact]
    public void RefusesAStepBackwards()
        => Assert.Throws<ArgumentException>(() => s_stages.Step("Done", "Briefed"));

    [Fact]
    public void RefusesAStepFromAStageItNeverDeclared()
        => Assert.Throws<ArgumentException>(() => s_stages.Step("Rewarded", "Done"));

    [Fact]
    public void RefusesAStepToAStageItNeverDeclared()
        => Assert.Throws<ArgumentException>(() => s_stages.Step("NotStarted", "Rewarded"));

    [Fact]
    public void CarriesTheBriefingAndAwardOnTheStep()
    {
        // Arrange
        MissionBriefing briefing = new() { Paragraphs = ["Well done, Commander."] };
        MissionAward award = new(256, 5000);

        // Act
        MissionStep step = s_stages.Step("Briefed", "Done", briefing, award);

        // Assert
        Assert.Same(briefing, step.Briefing);
        Assert.Same(award, step.Award);
    }

    [Fact]
    public void HandsOutStageNamesNobodyCanEdit()
    {
        // Arrange: the names are handed out for the save file to be checked
        // against, so a caller must not be able to rewrite the mission's stages
        // through them.
        ImmutableArray<string> names = s_stages.Names;

        // Act
        _ = names.Add("Smuggled");

        // Assert
        Assert.Equal(3, s_stages.Names.Length);
    }

    [Fact]
    public void KeepsTheStagesItWasGivenWhenTheCallerGoesOnEditingTheList()
    {
        // Arrange
        List<string> names = ["NotStarted", "Briefed"];
        MissionStages stages = new(names);

        // Act
        names.Add("Smuggled");

        // Assert
        Assert.Equal(2, stages.Names.Length);
    }
}
