// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Views;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Assets;

namespace EliteSharpLib.Tests.Views;

public sealed class ViewLoaderTests : IDisposable
{
    private const string EightBitAssembly = "EliteSharp.Views.EightBit.dll";
    private const string SixteenBitAssembly = "EliteSharp.Views.SixteenBit.dll";

    private readonly string _baseDirectory;
    private bool _isDisposed;

    public ViewLoaderTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), "EliteSharpLib.Tests.Views", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_baseDirectory);
    }

    [Fact]
    public void FindsThePackForTheConfiguredTier()
    {
        // Arrange: both packs are copied in off disk, which is the whole point
        // - neither the game nor this test hands the loader anything.
        GivenPacks();

        // Act
        IViewPack pack = ViewLoader.LoadFrom(_baseDirectory, SystemTier.EightBit, NullLogger.Instance);

        // Assert
        Assert.Equal(SystemTier.EightBit, pack.Tier);
    }

    [Fact]
    public void FindsEachTierSeparately()
    {
        // Arrange
        GivenPacks();

        // Act
        IViewPack pack = ViewLoader.LoadFrom(_baseDirectory, SystemTier.SixteenBit, NullLogger.Instance);

        // Assert
        Assert.Equal(SystemTier.SixteenBit, pack.Tier);
    }

    [Fact]
    public void RefusesToStartWithNoPluginFolder()
    {
        // Act & Assert: unlike a missing mission, this is fatal - there would
        // be nothing to draw the game with.
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => ViewLoader.LoadFrom(_baseDirectory, SystemTier.EightBit, NullLogger.Instance));

        Assert.Contains("EightBit", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesToStartWhenOnlyTheOtherTierIsInstalled()
    {
        // Arrange: a commander who deleted one pack and configured that tier.
        GivenPacks(SixteenBitAssembly);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => ViewLoader.LoadFrom(_baseDirectory, SystemTier.EightBit, NullLogger.Instance));
    }

    [Fact]
    public void SkipsAFileThatIsNotAnAssemblyAndCarriesOn()
    {
        // Arrange: one unreadable file must not cost the commander a tier that
        // is installed and readable.
        GivenPacks();
        File.WriteAllText(Path.Combine(PluginFolder(), "rubbish.dll"), "not an assembly");

        // Act
        IViewPack pack = ViewLoader.LoadFrom(_baseDirectory, SystemTier.EightBit, NullLogger.Instance);

        // Assert
        Assert.Equal(SystemTier.EightBit, pack.Tier);
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

    private string PluginFolder() => Path.Combine(_baseDirectory, ViewLoader.FolderName);

    private void GivenPacks(params string[] assemblies)
    {
        string folder = Directory.CreateDirectory(PluginFolder()).FullName;

        foreach (string assembly in assemblies.Length > 0 ? assemblies : [EightBitAssembly, SixteenBitAssembly])
        {
            File.Copy(Path.Combine(AppContext.BaseDirectory, assembly), Path.Combine(folder, assembly));
        }
    }
}
