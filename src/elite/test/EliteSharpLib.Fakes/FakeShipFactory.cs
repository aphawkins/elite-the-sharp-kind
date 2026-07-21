// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Fakes;

internal sealed class FakeShipFactory(IEliteDraw draw, RNG rng) : IShipFactory
{
    public IShip CreateAsteroid() => new FakeShip(draw, rng);

    public IShip CreateLoneWolf() => new FakeShip(draw, rng);

    public IShip CreatePackHunter() => new FakeShip(draw, rng);

    public List<IShip> CreateParade() => [new FakeShip(draw, rng)];

    public IShip CreatePirate() => new FakeShip(draw, rng);

    public IShip CreateShip(string shipName) => new FakeShip(draw, rng)
    {
        Type = ShipType.CobraMk3,
    };

    public IShip CreateTrader() => new FakeShip(draw, rng);
}
