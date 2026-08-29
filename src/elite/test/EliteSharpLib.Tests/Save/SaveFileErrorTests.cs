// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Text.Json.Nodes;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;

namespace EliteSharpLib.Tests.Save;

/// <summary>
/// A save is turned away for one of a dozen reasons, and which one is the
/// difference between a file a commander can repair and a file they cannot.
/// The screen used to say "Error Loading Commander!" whichever it was.
/// </summary>
public class SaveFileErrorTests
{
    [Fact]
    public void AMissingFileSaysThereIsNoSuchCommander()
    {
        SaveFile saveFile = CreateSaveFile(out _);

        Assert.False(saveFile.LoadCommander("NoSuchCommander"));
        Assert.Equal("No Such Commander", saveFile.LastLoadError);
    }

    [Fact]
    public void AFileThatIsNotJsonSaysItCouldNotBeRead()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        File.WriteAllText(Path.Combine(directory, "Corrupt.cmdr"), "{ not valid json");

        Assert.False(saveFile.LoadCommander("Corrupt"));
        Assert.Equal("Unreadable Commander File", saveFile.LastLoadError);
    }

    [Theory]
    [InlineData("fuel", 99, "Bad Fuel")]
    [InlineData("missiles", 9, "Bad Missiles")]
    [InlineData("cargoCapacity", 21, "Bad Cargo Bay")]
    [InlineData("score", -1, "Bad Score")]
    [InlineData("galaxyNumber", 8, "Bad Galaxy")]
    [InlineData("marketRandomiser", 256, "Bad Market")]
    [InlineData("version", 99, "Bad Version")]
    public void AFieldOutOfRangeIsNamed(string field, int value, string expected)
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Edited");
        Edit(directory, "Edited", save => save[field] = value);

        Assert.False(saveFile.LoadCommander("Edited"));
        Assert.Equal(expected, saveFile.LastLoadError);
    }

    [Fact]
    public void AnUnknownLaserIsNamedAsALaser()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Laser");
        Edit(directory, "Laser", save => save["lasers"]!["rear"] = "Disintegrator");

        Assert.False(saveFile.LoadCommander("Laser"));
        Assert.Equal("Bad Lasers", saveFile.LastLoadError);
    }

    [Fact]
    public void AGoodTheSetDoesNotHaveIsNamedAsCargo()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Typo");
        Edit(
            directory,
            "Typo",
            save =>
            {
                JsonObject cargo = save["cargo"]!.AsObject();
                cargo.Remove("Furs");
                cargo["Pelts"] = 0;
            });

        Assert.False(saveFile.LoadCommander("Typo"));
        Assert.Equal("Bad Cargo", saveFile.LastLoadError);
    }

    [Fact]
    public void StationStockIsNamedSeparatelyFromCargo()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Shelf");
        Edit(directory, "Shelf", save => save["stationStock"]!["Food"] = 64);

        Assert.False(saveFile.LoadCommander("Shelf"));
        Assert.Equal("Bad Station Stock", saveFile.LastLoadError);
    }

    [Fact]
    public void AMissionNothingProvidesIsNamedAsAMission()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Unknown");
        Edit(directory, "Unknown", save => save["missions"]!["Smuggling"] = new JsonObject { ["stage"] = "Briefed" });

        Assert.False(saveFile.LoadCommander("Unknown"));
        Assert.Equal("Bad Missions", saveFile.LastLoadError);
    }

    // A hold that overflows is the cargo's fault, not the bay's: the bay size
    // itself is one of the two the game fits.
    [Fact]
    public void MoreCargoThanTheHoldTakesIsNamedAsCargo()
    {
        SaveFile saveFile = CreateSaveFile(out string directory);
        saveFile.SaveCommander("Overloaded");
        Edit(directory, "Overloaded", save => save["cargo"]!["Food"] = 21);

        Assert.False(saveFile.LoadCommander("Overloaded"));
        Assert.Equal("Bad Cargo", saveFile.LastLoadError);
    }

    [Fact]
    public void ALoadThatWorksLeavesNoError()
    {
        SaveFile saveFile = CreateSaveFile(out _);
        saveFile.SaveCommander("Fine");

        Assert.True(saveFile.LoadCommander("Fine"));
        Assert.Equal(string.Empty, saveFile.LastLoadError);
    }

    // The screen shows whatever the save file reported, so a commander reads
    // the reason rather than a fixed sentence.
    [Fact]
    public void TheLoadScreenShowsTheReason()
    {
        FakeKeyboard keyboard = new();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        GameState gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new(gameState);
        Trade trade = TestGoods.Trade(gameState, ship);
        string directory = Path.Combine(Path.GetTempPath(), "SaveFileErrorTests_" + Guid.NewGuid().ToString("N"));
        SaveFile saveFile = new(gameState, ship, trade, new PlanetController(gameState), TestMissions.Registry(), directory);
        LoadCommanderController controller = new(gameState, keyboard, saveFile, new FakeLoadView());

        saveFile.SaveCommander("Broken");
        Edit(directory, "Broken", save => save["fuel"] = 99);

        controller.Reset();
        foreach (char letter in "BROKEN")
        {
            keyboard.KeyDown((ConsoleKey)letter, default);
            controller.HandleInput();
            keyboard.KeyUp((ConsoleKey)letter, default);
        }

        keyboard.KeyDown(ConsoleKey.Enter, default);
        controller.HandleInput();

        Assert.Equal("Bad Fuel", controller.BuildModel().ErrorMessage);
    }

    private static void Edit(string directory, string name, Action<JsonObject> edit)
    {
        string path = Path.Combine(directory, name + ".cmdr");
        JsonObject save = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
        edit(save);
        File.WriteAllText(path, save.ToJsonString());
    }

    private static SaveFile CreateSaveFile(out string directory)
    {
        Environment.SetEnvironmentVariable(SaveFile.DebugCommanderEnvVar, null);

        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new(gameState);
        Trade trade = TestGoods.Trade(gameState, ship);
        directory = Path.Combine(Path.GetTempPath(), "SaveFileErrorTests_" + Guid.NewGuid().ToString("N"));

        SaveFile saveFile = new(
            gameState,
            ship,
            trade,
            new PlanetController(gameState),
            TestMissions.Registry(),
            directory);

        saveFile.GetLastSave();

        return saveFile;
    }

    private sealed class FakeLoadView : EliteSharp.Abstractions.Views.IView<EliteSharp.Abstractions.Views.LoadCommanderModel>
    {
        public void Draw(EliteSharp.Abstractions.Views.LoadCommanderModel model)
        {
        }
    }
}
