// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;

namespace EliteSharpLib.Tests.Ships;

/// <summary>
/// The ships are a file now, so the file itself is something that can be
/// wrong. A table the game cannot read must say which file and why: there is
/// nothing to fly without one, and an empty universe is not a diagnosis.
/// </summary>
[Trait("Level", "Integration")]
public sealed class ShipTableTests : IDisposable
{
    // The fixtures are real files, read from beside the assembly, rather than
    // JSON spelled out in C#: the subject here is a file, and a string literal
    // that happens to be JSON is not one.
    private static readonly string s_oneShip = Fixture("one-ship.json");

    private readonly string _directory;
    private bool _isDisposed;

    public ShipTableTests()
    {
        _directory = Path.Combine(Path.GetTempPath(), "ShipTableTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void ReadsAShipAndAllOfItsNumbers()
    {
        ShipDefinition ship = Assert.Single(ShipTable.LoadFrom(Given(s_oneShip)));

        Assert.Equal("Adder", ship.Id);
        Assert.Equal("Adder", ship.Model);
        Assert.Equal(ShipType.Adder, ship.Type);
        Assert.Equal(
            ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry,
            ship.Flags);
        Assert.Equal("Adder", ship.Name);
        Assert.Null(ship.ScoopedType);
        Assert.Equal(4, ship.Bounty);
        Assert.Equal(85, ship.EnergyMax);
        Assert.Equal(8, ship.LaserStrength);
        Assert.Equal(200, ship.MinDistance);
        Assert.Equal(2500, ship.Size);
        Assert.Equal(20, ship.VanishPoint);
        Assert.Equal(24, ship.VelocityMax);
    }

    // A variant states only what it changes, which is what the two lone
    // wolves were expressing by deriving from their parent's class.
    [Fact]
    public void AVariantKeepsWhatItDoesNotChange()
    {
        IReadOnlyList<ShipDefinition> ships = ShipTable.LoadFrom(Given(Fixture("variant.json")));
        ShipDefinition lone = ships[1];

        // Its own.
        Assert.Equal(ShipType.PythonLone, lone.Type);
        Assert.Equal(ShipProperties.LoneWolf | ShipProperties.Bold | ShipProperties.Angry, lone.Flags);
        Assert.Equal(20, lone.Bounty);
        Assert.Equal(2, lone.LootMax);

        // Its parent's, including the mesh - which is what keeps the manifest
        // listing only real model files.
        Assert.Equal("Python", lone.Model);
        Assert.Equal("Python", lone.Name);
        Assert.Equal(250, lone.EnergyMax);
        Assert.Equal(6400, lone.Size);
        Assert.Equal(20, lone.VelocityMax);
    }

    [Fact]
    public void NamesTheFileThatIsNotThere()
    {
        string path = Path.Combine(_directory, "absent.json");

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains(path, ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NamesTheFileThatWillNotParse()
    {
        string path = Given("{ this is not json");

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains(path, ex.Message, StringComparison.Ordinal);
        Assert.IsType<System.Text.Json.JsonException>(ex.InnerException);
    }

    [Fact]
    public void RejectsAFileThatLeavesTheShipsOut()
    {
        EliteException ex = Assert.Throws<EliteException>(
            () => ShipTable.LoadFrom(Given(Fixture("no-ships.json"))));

        Assert.Contains("no ships", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAShipWithNoId()
    {
        string path = Given(s_oneShip.Replace("\"id\": \"Adder\"", "\"id\": \"\"", StringComparison.Ordinal));

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains("no id", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAKindTheGameDoesNotHave()
    {
        string path = Given(s_oneShip.Replace("\"type\": \"Adder\"", "\"type\": \"Frigate\"", StringComparison.Ordinal));

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains("Frigate", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsABehaviourTheGameDoesNotHave()
    {
        string path = Given(s_oneShip.Replace("\"Bold\"", "\"Reckless\"", StringComparison.Ordinal));

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains("Reckless", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAShipBasedOnOneTheFileDoesNotDescribe()
    {
        string path = Given(Fixture("variant.json").Replace(
            "\"basedOn\": \"Python\"",
            "\"basedOn\": \"Boa\"",
            StringComparison.Ordinal));

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains("Boa", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsTheSameShipTwice()
    {
        string path = Given(s_oneShip.Replace(
            "\"ships\": [",
            "\"ships\": [ { \"id\": \"Adder\", \"type\": \"Adder\", \"name\": \"Adder\" },",
            StringComparison.Ordinal));

        EliteException ex = Assert.Throws<EliteException>(() => ShipTable.LoadFrom(path));

        Assert.Contains("twice", ex.Message, StringComparison.Ordinal);
    }

    // The table travels with the assembly, so the shipped one has to be found
    // and read without anybody saying where it is - and it has to still hold
    // every ship the game knows how to ask for.
    [Fact]
    public void TheShippedTableIsFoundBesideTheAssembly()
    {
        IReadOnlyList<ShipDefinition> ships = ShipTable.Load();

        Assert.Equal(33, ships.Count);
        Assert.Equal(33, ships.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());

        // The four the goods set is checked against still name their good.
        Assert.Equal(
            ScoopableGoods.Required.Order(StringComparer.Ordinal),
            ships.Where(x => x.ScoopedType != null)
                .Select(x => x.ScoopedType!)
                .Order(StringComparer.Ordinal));
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            try
            {
                Directory.Delete(_directory, recursive: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // A leftover temp folder must not fail a test.
            }

            _isDisposed = true;
        }
    }

    private static string Fixture(string name)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Ships", "Fixtures", name));

    private string Given(string json)
    {
        string path = Path.Combine(_directory, $"{Guid.NewGuid():N}.json");
        File.WriteAllText(path, json);

        return path;
    }
}
