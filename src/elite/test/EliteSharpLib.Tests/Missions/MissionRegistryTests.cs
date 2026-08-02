// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Missions.Abstractions;
using EliteSharpLib.Missions;
using Microsoft.Extensions.Logging.Abstractions;

namespace EliteSharpLib.Tests.Missions;

public class MissionRegistryTests
{
    [Fact]
    public void HoldsNothingWhenNoMissionWasFound()
        => Assert.Empty(Registry().All);

    [Fact]
    public void FindsAMissionByTheNameTheSaveFileRecords()
    {
        // Arrange
        StubMission errand = new("Errand");
        MissionRegistry registry = Registry(errand, new StubMission("Constrictor"));

        // Act & Assert
        Assert.Same(errand, registry.Find("Errand"));
        Assert.Equal(2, registry.All.Count);
    }

    [Fact]
    public void FindsNoMissionWhoseNameNoPluginProvides()
    {
        // Arrange: this is what a save file naming a removed plugin comes back
        // as, and it has to be an answer rather than a crash.
        MissionRegistry registry = Registry(new StubMission("Errand"));

        // Act & Assert
        Assert.Null(registry.Find("Thargoid"));
    }

    [Fact]
    public void TellsMissionNamesApartByTheirBytes()
    {
        // Arrange: a mission name is a save-file key, not prose.
        MissionRegistry registry = Registry(new StubMission("Errand"));

        // Act & Assert
        Assert.Null(registry.Find("errand"));
    }

    [Fact]
    public void RefusesToStartWhenTwoMissionsAnswerToOneName()
    {
        // Arrange: a save file naming it could mean either, so there is nothing
        // safe to do but stop.
        StubMission[] missions = [new("Errand"), new("Errand")];

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => Registry(missions));
        Assert.Contains("Errand", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void IsBuiltByTheGamesOwnComposition()
    {
        // Arrange: nothing asks for the registry yet, so without this the
        // registration would go untried until the missions are wired in.
        using HeadlessGameHarness harness = new();

        // Act: the game ships no plugin folder, so this finds nothing and must
        // still come back.
        MissionRegistry registry = harness.Resolve<MissionRegistry>();

        // Assert
        Assert.Empty(registry.All);
    }

    private static MissionRegistry Registry(params IMission[] missions)
        => new(missions, NullLogger<MissionRegistry>.Instance);

    private sealed class StubMission(string name) : IMission
    {
        public string Name { get; } = name;

        public MissionStages Stages { get; } = new(["NotStarted"]);

        public MissionStep? Advance(IMissionContext context, string stage) => null;
    }
}
