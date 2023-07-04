// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Ships
{
    internal static class ShipFactory
    {
        internal static IShip CreateAsteroid() => RNG.Random(256) > 253 ? new RockHermit() : new Asteroid();

        internal static IShip CreateLoneWolf()
        {
            int rnd = RNG.Random(256);
            return ((rnd & 3) + (rnd > 127 ? 1 : 0)) switch
            {
                0 => new CobraMk3Lone(),
                1 => new AspMk2(),
                2 => new PythonLone(),
                3 => new FerDeLance(),
                4 => new Moray(),
                _ => throw new NotImplementedException(),
            };
        }

        internal static IShip CreatePackHunter() => RNG.Random(7) switch
        {
            0 => new Sidewinder(),
            1 => new Mamba(),
            2 => new Krait(),
            3 => new Adder(),
            4 => new Gecko(),
            5 => new CobraMk1(),
            6 => new Worm(),
            _ => throw new NotImplementedException(),
        };

        internal static IShip CreatePirate() => RNG.Random(4) switch
        {
            0 => new Sidewinder(),
            1 => new Mamba(),
            2 => new Krait(),
            3 => new Adder(),
            _ => throw new NotImplementedException(),
        };

        internal static IShip CreateTrader() => RNG.Random(4) switch
        {
            0 => new CobraMk3(),
            1 => new Python(),
            2 => new Boa(),
            3 => new Anaconda(),
            _ => throw new NotImplementedException(),
        };
    }
}
