// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Ships;

internal interface IShipFactory
{
    public IShip CreateAsteroid();

    public IShip CreateLoneWolf();

    public IShip CreatePackHunter();

    public List<IShip> CreateParade();

    public IShip CreatePirate();

    public IShip CreateShip(string shipName);

    public IShip CreateTrader();
}
