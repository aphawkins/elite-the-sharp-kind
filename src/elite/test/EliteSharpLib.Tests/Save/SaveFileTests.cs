// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Save;

public class SaveFileTests
{
    [Fact]
    public void LoadCommanderWithNoSaveFileReturnsFalse()
    {
        // Arrange
        SaveFile saveFile = CreateSaveFile(out _);

        // Act
        bool result = saveFile.LoadCommander("NoSuchCommander");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void LoadCommanderWithCorruptJsonReturnsFalseInsteadOfThrowing()
    {
        // Arrange
        SaveFile saveFile = CreateSaveFile(out string directory);
        File.WriteAllText(Path.Combine(directory, "Corrupt.cmdr"), "{ not valid json");

        // Act
        bool result = saveFile.LoadCommander("Corrupt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void LoadCommanderWithTruncatedArraysReturnsFalseInsteadOfThrowing()
    {
        // Arrange: a hand-edited file with a short galaxySeed array - SaveStateToGameState
        // indexes GalaxySeed[0..5], so without validation this used to throw.
        SaveFile saveFile = CreateSaveFile(out string directory);
        File.WriteAllText(
            Path.Combine(directory, "Truncated.cmdr"),
            /*lang=json,strict*/ "{\"galaxySeed\": [1, 2, 3]}");

        // Act
        bool result = saveFile.LoadCommander("Truncated");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SaveCommanderThenLoadCommanderRoundTrips()
    {
        // Arrange
        SaveFile saveFile = CreateSaveFile(out _);

        // Act
        bool saved = saveFile.SaveCommander("RoundTrip");
        bool loaded = saveFile.LoadCommander("RoundTrip");

        // Assert
        Assert.True(saved);
        Assert.True(loaded);
    }

    private static SaveFile CreateSaveFile(out string directory)
    {
        ScreenManager<Screen, IView> views = new(new FakeKeyboard());
        GameState gameState = new(views);
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);
        PlanetController planet = new(gameState);
        directory = Path.Combine(Path.GetTempPath(), "SaveFileTests_" + Guid.NewGuid().ToString("N"));
        return new SaveFile(gameState, ship, trade, planet, directory);
    }
}
