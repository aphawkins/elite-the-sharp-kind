// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Fakes;

internal sealed class FakeShipFactory(IEliteDraw draw) : IShipFactory
{
    public IShip CreateAsteroid() => new FakeShip(draw);

    public IShip CreateLoneWolf() => new FakeShip(draw);

    public IShip CreatePackHunter() => new FakeShip(draw);

    public List<IShip> CreateParade() => [new FakeShip(draw)];

    public IShip CreatePirate() => new FakeShip(draw);

    public IShip CreateShip(string shipName) => new FakeShip(draw)
    {
        Type = ShipType.CobraMk3,
    };

    public IShip CreateTrader() => new FakeShip(draw);
}
