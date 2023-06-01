// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Ships
{
    internal static class ShipFactory
    {
        internal static IObject ConstructShip(ShipType type) => type switch
        {
            ShipType.Adder => new Adder(),
            ShipType.Alloy => new Alloy(),
            ShipType.Anaconda => new Anaconda(),
            ShipType.AspMk2 => new AspMk2(),
            ShipType.None => new NullObject(),
            ShipType.Missile => new Missile(),
            ShipType.Coriolis => new Coriolis(),
            ShipType.EscapeCapsule => new EscapeCapsule(),
            ShipType.Cargo => new CargoCannister(),
            ShipType.Boulder => new Boulder(),
            ShipType.Asteroid => new Asteroid(),
            ShipType.Rock => new RockSplinter(),
            ShipType.Shuttle => new Shuttle(),
            ShipType.Transporter => new Transporter(),
            ShipType.CobraMk3 => new CobraMk3(),
            ShipType.Python => new Python(),
            ShipType.Boa => new Boa(),
            ShipType.Hermit => new RockHermit(),
            ShipType.Viper => new Viper(),
            ShipType.Sidewinder => new Sidewinder(),
            ShipType.Mamba => new Mamba(),
            ShipType.Krait => new Krait(),
            ShipType.Gecko => new Gecko(),
            ShipType.CobraMk1 => new CobraMk1(),
            ShipType.Worm => new Worm(),
            ShipType.CobraMk3Lone => new CobraMk3Lone(),
            ShipType.PythonLone => new PythonLone(),
            ShipType.FerDeLance => new FerDeLance(),
            ShipType.Moray => new Moray(),
            ShipType.Thargoid => new Thargoid(),
            ShipType.Tharglet => new Tharglet(),
            ShipType.Constrictor => new Constrictor(),
            ShipType.Cougar => new Cougar(),
            ShipType.Dodec => new DodecStation(),
            ShipType.Sun => new Sun(),
            ShipType.Planet => new Planet(),
            _ => throw new NotImplementedException(),
        };
    }
}
