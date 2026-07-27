// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using Useful.Assets;
using Useful.Fakes;
using Useful.Fakes.Assets;

namespace EliteSharpLib.Tests;

public class ShipFactoryTests
{
    [Theory]
    [InlineData(254, "RockHermit")]
    [InlineData(0, "Asteroid")]
    public void CreateAsteroidPicksShipByRoll(int roll, string expectedName)
    {
        // Arrange: the 1-in-256 RockHermit roll (RNG.Random(256) > 253) is
        // forced deterministically instead of hunting for a seed that hits it.
        FakeAssetLocator locator = new();
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource { RandomValue = roll });
        ShipFactory factory = ShipFactory.Create(locator, draw, rng);

        Dictionary<string, IShip> dict = new()
        {
            { "RockHermit", new FakeShip(draw, rng) { Name = "RockHermit" } },
            { "Asteroid", new FakeShip(draw, rng) { Name = "Asteroid" } },
        };
        SetShipsField(factory, dict);

        // Act
        IShip created = factory.CreateAsteroid();

        // Assert
        Assert.Equal(expectedName, created.Name);
    }

    [Fact]
    public void CreateShipReturnsCloneAndPrototypeIsUnchanged()
    {
        // Arrange
        FakeAssetLocator locator = new();
        FakeEliteDraw draw = new();
        ShipFactory factory = ShipFactory.Create(locator, draw, new(new Random(0)));

        FakeShip prototype = new(draw, new(new Random(0)))
        {
            Name = "Prototype",
            Energy = 42,
        };

        Dictionary<string, IShip> dict = new()
        {
            { "Prototype", prototype },
        };

        SetShipsField(factory, dict);

        // Act
        IShip created = factory.CreateShip("Prototype");

        // Assert clone is not same reference
        Assert.NotSame(prototype, created);
        Assert.Equal(prototype.Name, created.Name);
        Assert.Equal(prototype.Energy, created.Energy);

        // Mutate created instance and verify prototype unchanged
        created.Energy = 7;
        created.Name = "Changed";
        Assert.Equal(42, prototype.Energy);
        Assert.Equal("Prototype", prototype.Name);
    }

    [Fact]
    public void CreateShipMissingNameThrowsEliteException()
    {
        // Arrange
        FakeAssetLocator locator = new();
        FakeEliteDraw draw = new();
        ShipFactory factory = ShipFactory.Create(locator, draw, new(new Random(0)));

        // Act & Assert
        Assert.Throws<EliteException>(() => factory.CreateShip("DoesNotExist"));
    }

    [Theory]
    [InlineData(0, "CobraMk3Lone")]
    [InlineData(1, "AspMk2")]
    [InlineData(2, "PythonLone")]
    [InlineData(3, "FerDeLance")]
    [InlineData(131, "Moray")]
    public void CreateLoneWolfPicksShipByRoll(int roll, string expectedType)
    {
        // Arrange: the real manifest, so every lone wolf the roll can pick
        // has to have a model entry.
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource { RandomValue = roll });
        ShipFactory factory = ShipFactory.Create(AssetLocator.Create(), draw, rng);

        // Act
        IShip created = factory.CreateLoneWolf();

        // Assert
        Assert.Equal(expectedType, created.Type.ToString());
    }

    [Fact]
    public void CreateUnknownModelNameThrowsEliteException()
    {
        // Arrange
        FakeAssetLocator locator = new()
        {
            ModelPaths = new Dictionary<string, string>()
            {
                { "NotAShip", "Assets/Models/adder.obj" },
            },
        };

        FakeEliteDraw draw = new();

        // Act & Assert
        Assert.Throws<EliteException>(() => ShipFactory.Create(locator, draw, new(new Random(0))));
    }

    [Fact]
    public void CreateParadeReturnsListAndPrototypesRemainUnchanged()
    {
        // Arrange
        FakeAssetLocator locator = new()
        {
            ModelPaths = new Dictionary<string, string>()
            {
                { "Adder", "Assets/Models/adder.obj" },
            },
        };

        FakeEliteDraw draw = new();
        ShipFactory factory = ShipFactory.Create(locator, draw, new(new Random(0)));

        string[] names =
        [
            "Missile",
            "Coriolis",
            "EscapeCapsule",
            "Alloy",
            "CargoCannister",
            "Boulder",
            "Asteroid",
            "RockSplinter",
            "Shuttle",
            "Transporter",
            "CobraMk3",
            "Python",
            "Boa",
            "Anaconda",
            "RockHermit",
            "Viper",
            "Sidewinder",
            "Mamba",
            "Krait",
            "Adder",
            "Gecko",
            "CobraMk1",
            "Worm",
            "AspMk2",
            "FerDeLance",
            "Moray",
            "Thargoid",
            "Tharglet",
            "DodecStation",
        ];

        Dictionary<string, IShip> dict = [];
        foreach (string name in names)
        {
            dict[name] = new FakeShip(draw, new(new Random(0)))
            {
                Name = name,
                Energy = 100,
            };
        }

        SetShipsField(factory, dict);

        // Act
        List<IShip> parade = factory.CreateParade();

        // Assert correct count
        Assert.Equal(29, parade.Count);

        // Mutate each created ship and assert the prototype remains unchanged
        for (int i = 0; i < parade.Count; i++)
        {
            IShip clonedShip = (IShip)parade[i].Clone();
            clonedShip.Energy = -1;
            Assert.Equal(100, dict[parade[i].Name].Energy);
            Assert.Equal(-1, clonedShip.Energy);
        }
    }

    private static void SetShipsField(object factory, IDictionary<string, IShip> ships)
    {
        FieldInfo field = factory.GetType().GetField("_ships", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("_ships field not found");
        field.SetValue(factory, ships);
    }
}
