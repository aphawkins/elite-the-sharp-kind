// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Xunit;

namespace EliteSharp.Missions.Abstractions.Tests;

public class MissionRewardTests
{
    [Fact]
    public void RefusesAnAwardWorthNothing()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new MissionAward(0, 0));

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void RefusesAnAwardThatTakesRatherThanGives(int combatScore, int credits)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new MissionAward(combatScore, credits));

    [Fact]
    public void AllowsKitAloneToBeTheWholeAward()
    {
        // Act: the energy unit is worth having whatever the numbers say.
        MissionAward award = new(0, 0, MissionEquipment.NavalEnergyUnit);

        // Assert
        Assert.Equal(MissionEquipment.NavalEnergyUnit, award.Equipment);
    }

    [Fact]
    public void PaysNoKitWhenTheAwardIsRankAndCashAlone()
        => Assert.Null(new MissionAward(256, 5000).Equipment);

    [Fact]
    public void RefusesAnAmbushNobodyWouldEverMeet()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new AmbushEncounter("Thargoid", 0));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RefusesAnAmbushByAShipWithNoName(string blank)
        => Assert.Throws<ArgumentException>(() => new AmbushEncounter(blank, 10));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RefusesALoneWolfWithNoName(string blank)
        => Assert.Throws<ArgumentException>(() => new LoneWolfEncounter(blank, true));

    [Fact]
    public void KeepsWhatTheEncountersWereBuiltWith()
    {
        // Act
        AmbushEncounter ambush = new("Thargoid", 10);
        LoneWolfEncounter loneWolf = new("Constrictor", true);

        // Assert
        Assert.Equal("Thargoid", ambush.ShipName);
        Assert.Equal(10, ambush.ChanceInTwoFiftySix);
        Assert.Equal("Constrictor", loneWolf.ShipName);
        Assert.True(loneWolf.Unique);
    }
}
