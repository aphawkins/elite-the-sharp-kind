// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Ships;

namespace EliteSharp.Tests
{
    public class UniverseTests
    {
        [Fact]
        public void UniverseAddShip()
        {
            // Arrange
            Universe universe = new();
            IObject ship = new CobraMk3();

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
            Universe universe = new();
            IObject ship = new CobraMk3();

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
    }
}
