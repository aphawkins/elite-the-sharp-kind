// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Missions;
using EliteSharpLib.Missions;
using Microsoft.Extensions.Logging.Abstractions;

namespace EliteSharpLib.Tests.Missions;

public sealed class MissionLoaderTests : IDisposable
{
    private const string PluginAssembly = "EliteSharp.Missions.TestPlugin.dll";

    private readonly string _baseDirectory;
    private bool _isDisposed;

    public MissionLoaderTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), "EliteSharpLib.Tests.Missions", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_baseDirectory);
    }

    [Fact]
    public void FindsNothingAndSaysSoWhenThereIsNoPluginFolder()
    {
        // Act: having no plugins is the normal case, so it must not throw.
        IReadOnlyList<IMission> missions = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance);

        // Assert
        Assert.Empty(missions);
    }

    [Fact]
    public void FindsNothingWhenThePluginFolderIsEmpty()
    {
        // Arrange
        _ = Directory.CreateDirectory(PluginFolder());

        // Act
        IReadOnlyList<IMission> missions = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance);

        // Assert
        Assert.Empty(missions);
    }

    [Fact]
    public void FindsAMissionInAnAssemblyItWasNeverBuiltAgainst()
    {
        // Arrange: the plugin is copied in off disk, which is the whole point -
        // the tests do not reference it, and neither does the game.
        GivenPlugin();

        // Act
        IReadOnlyList<IMission> missions = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance);

        // Assert
        IMission mission = Assert.Single(missions);
        Assert.Equal("Errand", mission.Name);
    }

    [Fact]
    public void HandsBackAMissionThatWorks()
    {
        // Arrange
        GivenPlugin();
        IMission mission = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance)[0];

        // Act
        MissionStep? step = mission.Advance(new DockedContext(), mission.Stages.NotStarted);

        // Assert
        Assert.NotNull(step);
        Assert.Equal("Briefed", step.Stage);
        Assert.Equal("Run this errand, Commander.", step.Briefing?.Paragraphs[0]);
    }

    [Fact]
    public void SkipsAFileThatIsNotAnAssemblyAndCarriesOn()
    {
        // Arrange: one unreadable file is one plugin the commander does not
        // get, not a game that will not start.
        GivenPlugin();
        File.WriteAllText(Path.Combine(PluginFolder(), "rubbish.dll"), "not an assembly");

        // Act
        IReadOnlyList<IMission> missions = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance);

        // Assert
        Assert.Single(missions);
    }

    [Fact]
    public void IgnoresFilesThatAreNotAssembliesAtAll()
    {
        // Arrange
        _ = Directory.CreateDirectory(PluginFolder());
        File.WriteAllText(Path.Combine(PluginFolder(), "readme.txt"), "Drop mission plugins here.");

        // Act
        IReadOnlyList<IMission> missions = MissionLoader.LoadFrom(_baseDirectory, NullLogger.Instance);

        // Assert
        Assert.Empty(missions);
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            try
            {
                Directory.Delete(_baseDirectory, recursive: true);
            }
            catch (IOException)
            {
                // The loaded assembly keeps its file open, so best-effort
                // cleanup: a leftover temp folder must not fail a test.
            }
            catch (UnauthorizedAccessException)
            {
                // As above.
            }

            _isDisposed = true;
        }
    }

    private string PluginFolder() => Path.Combine(_baseDirectory, MissionLoader.FolderName);

    private void GivenPlugin()
    {
        string folder = Directory.CreateDirectory(PluginFolder()).FullName;
        File.Copy(Path.Combine(AppContext.BaseDirectory, PluginAssembly), Path.Combine(folder, PluginAssembly));
    }

    private sealed class DockedContext : IMissionContext
    {
        public int CombatScore => 0;

        public int GalaxyNumber => 0;

        public int CurrentPlanetNumber => 0;

        public bool IsDocked => true;

        public string? StageOf(string missionName) => null;
    }
}
