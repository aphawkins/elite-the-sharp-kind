// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;

namespace EliteSharpLib.Fakes;

internal sealed class FakeShipFactory : IShipFactory
{
    public IShip CreateAsteroid() => new FakeShip();

    public IShip CreateLoneWolf() => new FakeShip();

    public IShip CreatePackHunter() => new FakeShip();

    public List<IShip> CreateParade() => [new FakeShip()];

    public IShip CreatePirate() => new FakeShip();

    public IShip CreateShip(string shipName) => new FakeShip()
    {
        Type = ShipType.CobraMk3,
    };

    public IShip CreateTrader() => new FakeShip();
}
