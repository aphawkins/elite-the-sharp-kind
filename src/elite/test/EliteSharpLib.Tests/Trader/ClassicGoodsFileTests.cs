// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Goods.Classic;

namespace EliteSharpLib.Tests.Trader;

/// <summary>
/// The goods table is a file now, so the file itself is something that can be
/// wrong. A set that cannot say what it trades must say which file and why:
/// the game will not start without one, and "no market" is not a diagnosis.
/// </summary>
public sealed class ClassicGoodsFileTests : IDisposable
{
    private const string OneGood = """
        {
          "name": "Test",
          "goods": [
            {
              "id": "Food", "name": "Food", "basePrice": 1.9, "economyAdjust": -2,
              "baseQuantity": 6, "mask": 1, "units": "t", "fillsHold": true,
              "openingStationStock": 16, "isSoldByStations": true,
              "contrabandWeight": 0, "isDroppedByShips": true
            }
          ]
        }
        """;

    private readonly string _directory;
    private bool _isDisposed;

    public ClassicGoodsFileTests()
    {
        _directory = Path.Combine(Path.GetTempPath(), "ClassicGoodsFileTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void ReadsTheSetAndItsGoods()
    {
        ClassicGoodsSet set = new(Given(OneGood));

        Assert.Equal("Test", set.Name);
        EliteSharp.Abstractions.Trading.Good good = Assert.Single(set.Goods);
        Assert.Equal("Food", good.Id);
        Assert.Equal(1.9f, good.BasePrice);
        Assert.Equal(-2, good.EconomyAdjust);
        Assert.Equal(16, good.OpeningStationStock);
        Assert.True(good.FillsHold);
        Assert.True(good.IsDroppedByShips);
    }

    [Fact]
    public void NamesTheFileThatIsNotThere()
    {
        string path = Path.Combine(_directory, "absent.json");

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains(path, ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NamesTheFileThatWillNotParse()
    {
        string path = Given("{ this is not json");

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains(path, ex.Message, StringComparison.Ordinal);
        Assert.IsType<System.Text.Json.JsonException>(ex.InnerException);
    }

    [Fact]
    public void RejectsAFileWithNoGoods()
    {
        string path = Given("""{ "name": "Empty", "goods": [] }""");

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains("no goods", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAFileThatLeavesTheGoodsOut()
    {
        string path = Given("""{ "name": "Nothing" }""");

        Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));
    }

    [Fact]
    public void RejectsAFileThatDoesNotNameItsSet()
    {
        string path = Given(OneGood.Replace("\"name\": \"Test\"", "\"name\": \" \"", StringComparison.Ordinal));

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains("does not name", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAGoodWithNoId()
    {
        string path = Given(OneGood.Replace("\"id\": \"Food\"", "\"id\": \"\"", StringComparison.Ordinal));

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains("no id", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsAGoodWithNoUnits()
    {
        string path = Given(OneGood.Replace("\"units\": \"t\"", "\"units\": \"\"", StringComparison.Ordinal));

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => new ClassicGoodsSet(path));

        Assert.Contains("Food", ex.Message, StringComparison.Ordinal);
        Assert.Contains("units", ex.Message, StringComparison.Ordinal);
    }

    // The table travels with the assembly, so the shipped one has to be found
    // and read without anybody saying where it is.
    [Fact]
    public void TheShippedFileIsFoundBesideTheAssembly()
    {
        ClassicGoodsSet set = new();

        Assert.Equal("Classic", set.Name);
        Assert.Equal(17, set.Goods.Count);
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

    private string Given(string json)
    {
        string path = Path.Combine(_directory, $"{Guid.NewGuid():N}.json");
        File.WriteAllText(path, json);

        return path;
    }
}
