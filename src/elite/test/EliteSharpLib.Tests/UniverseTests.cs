// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Tests;

public class UniverseTests
{
    [Fact]
    public void UniverseAddShip()
    {
        // Arrange
        FakeEliteDraw draw = new();
        FakeShipFactory fakeShipFactory = new(draw, new(new Random(0)));
        Universe universe = new(fakeShipFactory, new(new Random(0)));
        IShip ship = fakeShipFactory.CreateShip("CobraMk3");

        // Act
        universe.AddNewShip(ship);

        // Assert
        Assert.Single(universe.GetAllObjects());
        Assert.Equal(ship, universe.GetAllObjects().First());
        Assert.Equal(ship, universe.FirstShip);
        Assert.Null(universe.Planet);
        Assert.Null(universe.StationOrSun);
        Assert.False(universe.IsStationPresent);
        Assert.Equal(0, universe.PoliceCount);
        Assert.Equal(1, universe.ShipCount(ShipType.CobraMk3));
        Assert.Equal(0, universe.ShipCount(ShipType.Planet));
    }

    [Fact]
    public void UniverseRemoveShip()
    {
        // Arrange
        FakeEliteDraw draw = new();
        FakeShipFactory fakeShipFactory = new(draw, new(new Random(0)));
        Universe universe = new(fakeShipFactory, new(new Random(0)));
        IShip ship = fakeShipFactory.CreateShip("CobraMk3");

        // Act
        universe.AddNewShip(ship);
        universe.RemoveShip(ship);

        // Assert
        Assert.False(universe.GetAllObjects().Any());
        Assert.Null(universe.Planet);
        Assert.Null(universe.StationOrSun);
        Assert.False(universe.IsStationPresent);
        Assert.Equal(0, universe.PoliceCount);
        Assert.Equal(0, universe.ShipCount(ShipType.CobraMk3));
    }

    [Fact]
    public void UniverseRemovePlanetClearsPlanet()
    {
        // Arrange
        FakeEliteDraw draw = new();
        RNG rng = new(new Random(0));
        FakeShipFactory fakeShipFactory = new(draw, rng);
        Universe universe = new(fakeShipFactory, rng);
        IShip planet = new FakeShip(draw, rng) { Type = ShipType.Planet };

        // Act
        universe.AddNewShip(planet, new(0, 0, 30000, 0), Matrix4x4.Identity, 0, 0);
        universe.RemoveShip(planet);

        // Assert
        Assert.Null(universe.Planet);
        Assert.False(universe.GetAllObjects().Any());
    }

    [Fact]
    public void UniverseRemoveStationClearsStationOrSun()
    {
        // Arrange
        FakeEliteDraw draw = new();
        RNG rng = new(new Random(0));
        FakeShipFactory fakeShipFactory = new(draw, rng);
        Universe universe = new(fakeShipFactory, rng);

        IShip station = new FakeShip(draw, rng)
        {
            Type = ShipType.Coriolis,
            Flags = ShipProperties.Station,
        };

        // Act
        universe.AddNewShip(station, new(0, 0, 30000, 0), Matrix4x4.Identity, 0, -127);
        universe.RemoveShip(station);

        // Assert
        Assert.Null(universe.StationOrSun);
        Assert.False(universe.IsStationPresent);
        Assert.False(universe.GetAllObjects().Any());
    }
}
