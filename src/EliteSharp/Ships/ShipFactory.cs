// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal class ShipFactory
    {
        private readonly IDraw _draw;

        internal ShipFactory(IDraw draw) => _draw = draw;

        internal IShip CreateAsteroid() => RNG.Random(256) > 253 ? new RockHermit(_draw) : new Asteroid(_draw);

        internal IShip CreateLoneWolf()
        {
            int rnd = RNG.Random(256);
            return ((rnd & 3) + (rnd > 127 ? 1 : 0)) switch
            {
                0 => new CobraMk3Lone(_draw),
                1 => new AspMk2(_draw),
                2 => new PythonLone(_draw),
                3 => new FerDeLance(_draw),
                4 => new Moray(_draw),
                _ => throw new EliteException(),
            };
        }

        internal IShip CreatePackHunter() => RNG.Random(7) switch
        {
            0 => new Sidewinder(_draw),
            1 => new Mamba(_draw),
            2 => new Krait(_draw),
            3 => new Adder(_draw),
            4 => new Gecko(_draw),
            5 => new CobraMk1(_draw),
            6 => new Worm(_draw),
            _ => throw new EliteException(),
        };

        internal IShip CreatePirate() => RNG.Random(4) switch
        {
            0 => new Sidewinder(_draw),
            1 => new Mamba(_draw),
            2 => new Krait(_draw),
            3 => new Adder(_draw),
            _ => throw new EliteException(),
        };

        internal IShip CreateTrader() => RNG.Random(4) switch
        {
            0 => new CobraMk3(_draw),
            1 => new Python(_draw),
            2 => new Boa(_draw),
            3 => new Anaconda(_draw),
            _ => throw new EliteException(),
        };

        internal List<IShip> CreateParade() => new()
        {
            { new Missile(_draw) },
            { new Coriolis(_draw) },
            { new EscapeCapsule(_draw) },
            { new Alloy(_draw) },
            { new CargoCannister(_draw) },
            { new Boulder(_draw) },
            { new Asteroid(_draw) },
            { new RockSplinter(_draw) },
            { new Shuttle(_draw) },
            { new Transporter(_draw) },
            { new CobraMk3(_draw) },
            { new Python(_draw) },
            { new Boa(_draw) },
            { new Anaconda(_draw) },
            { new RockHermit(_draw) },
            { new Viper(_draw) },
            { new Sidewinder(_draw) },
            { new Mamba(_draw) },
            { new Krait(_draw) },
            { new Adder(_draw) },
            { new Gecko(_draw) },
            { new CobraMk1(_draw) },
            { new Worm(_draw) },
            { new AspMk2(_draw) },
            { new FerDeLance(_draw) },
            { new Moray(_draw) },
            { new Thargoid(_draw) },
            { new Tharglet(_draw) },
            { new DodecStation(_draw) },
        };
    }
}
