// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using SharpKind.Assets;
using SharpKind.Assets.Models;

namespace EliteSharpLib.Ships;

internal sealed class ShipFactory : IShipFactory
{
    private static readonly Dictionary<string, Func<IEliteDraw, IShip>> s_constructors = new()
    {
        { "Adder", draw => new Adder(draw) },
        { "Alloy", draw => new Alloy(draw) },
        { "Anaconda", draw => new Anaconda(draw) },
        { "AspMk2", draw => new AspMk2(draw) },
        { "Asteroid", draw => new Asteroid(draw) },
        { "Boa", draw => new Boa(draw) },
        { "Boulder", draw => new Boulder(draw) },
        { "CargoCannister", draw => new CargoCannister(draw) },
        { "CobraMk1", draw => new CobraMk1(draw) },
        { "CobraMk3", draw => new CobraMk3(draw) },
        { "CobraMk3Lone", draw => new CobraMk3Lone(draw) },
        { "Constrictor", draw => new Constrictor(draw) },
        { "Coriolis", draw => new Coriolis(draw) },
        { "Cougar", draw => new Cougar(draw) },
        { "DodecStation", draw => new DodecStation(draw) },
        { "EscapeCapsule", draw => new EscapeCapsule(draw) },
        { "FerDeLance", draw => new FerDeLance(draw) },
        { "Gecko", draw => new Gecko(draw) },
        { "Krait", draw => new Krait(draw) },
        { "Mamba", draw => new Mamba(draw) },
        { "Missile", draw => new Missile(draw) },
        { "Moray", draw => new Moray(draw) },
        { "Python", draw => new Python(draw) },
        { "PythonLone", draw => new PythonLone(draw) },
        { "RockHermit", draw => new RockHermit(draw) },
        { "RockSplinter", draw => new RockSplinter(draw) },
        { "Shuttle", draw => new Shuttle(draw) },
        { "Sidewinder", draw => new Sidewinder(draw) },
        { "Tharglet", draw => new Tharglet(draw) },
        { "Thargoid", draw => new Thargoid(draw) },
        { "Transporter", draw => new Transporter(draw) },
        { "Viper", draw => new Viper(draw) },
        { "Worm", draw => new Worm(draw) },
    };

    // Variants that share their parent's mesh, so the manifest lists only real model files.
    private static readonly Dictionary<string, string> s_modelNames = new()
    {
        { "CobraMk3Lone", "CobraMk3" },
        { "PythonLone", "Python" },
    };

    private readonly Dictionary<string, IShip> _ships;
    private readonly RNG _rng;

    private ShipFactory(Dictionary<string, IShip> ships, RNG rng)
    {
        _ships = ships;
        _rng = rng;
    }

    // rng is the game's: the factory's own rolls - whether an asteroid is
    // really a Rock Hermit, which trader shows up - are game decisions. The
    // entropy a ship draws with rides on the draw surface instead, so it
    // cannot be confused with this one. See RenderRandom.
    public static ShipFactory Create(IAssetLocator assetLocator, IEliteDraw draw, RNG rng)
    {
        ArgumentNullException.ThrowIfNull(assetLocator);

        string? unknown = assetLocator.ModelPaths.Keys.FirstOrDefault(x => !s_constructors.ContainsKey(x));
        if (unknown != null)
        {
            throw new EliteException($"Ship type '{unknown}' could not be found.");
        }

        // Every ship the manifest supplies a model for, including the variants
        // that borrow their parent's model.
        Dictionary<string, IShip> ships = [];
        foreach ((string name, Func<IEliteDraw, IShip> constructor) in s_constructors)
        {
            string modelName = s_modelNames.GetValueOrDefault(name, name);
            if (assetLocator.ModelPaths.TryGetValue(modelName, out string? modelPath))
            {
                IShip ship = constructor(draw);
                ship.Model = ModelReader.Read(modelPath, draw.Palette);
                ships[name] = ship;
            }
        }

        return new(ships, rng);
    }

    public IShip CreateShip(string shipName)
    => _ships.TryGetValue(shipName, out IShip? ship)
        ? (IShip)ship.Clone()
        : throw new EliteException($"Ship model '{shipName}' not found.");

    public IShip CreateAsteroid() => _rng.Random(256) > 253 ? CreateShip("RockHermit") : CreateShip("Asteroid");

    public IShip CreateLoneWolf()
    {
        int rnd = _rng.Random(256);
        int index = (rnd & 3) + (rnd > 127 ? 1 : 0);
        return index switch
        {
            0 => CreateShip("CobraMk3Lone"),
            1 => CreateShip("AspMk2"),
            2 => CreateShip("PythonLone"),
            3 => CreateShip("FerDeLance"),
            4 => CreateShip("Moray"),
            _ => throw new EliteException($"Unexpected lone wolf index '{index}' (rnd '{rnd}')."),
        };
    }

    public IShip CreatePackHunter()
    {
        // Original mt1: AND two random bytes together so each bit has a 25%
        // chance of being set, biasing the result toward smaller indices.
        int first = _rng.Random(256);
        int second = _rng.Random(256);
        int rnd = first & second & 7;
        return rnd switch
        {
            0 => CreateShip("Sidewinder"),
            1 => CreateShip("Mamba"),
            2 => CreateShip("Krait"),
            3 => CreateShip("Adder"),
            4 => CreateShip("Gecko"),
            5 => CreateShip("CobraMk1"),
            6 => CreateShip("Worm"),
            7 => CreateShip("CobraMk3"),
            _ => throw new EliteException($"Unexpected pack hunter roll '{rnd}'."),
        };
    }

    public IShip CreatePirate()
    {
        int rnd = _rng.Random(4);
        return rnd switch
        {
            0 => CreateShip("Sidewinder"),
            1 => CreateShip("Mamba"),
            2 => CreateShip("Krait"),
            3 => CreateShip("Adder"),
            _ => throw new EliteException($"Unexpected pirate roll '{rnd}'."),
        };
    }

    public IShip CreateTrader()
    {
        int rnd = _rng.Random(4);
        return rnd switch
        {
            0 => CreateShip("CobraMk3"),
            1 => CreateShip("Python"),
            2 => CreateShip("Boa"),
            3 => CreateShip("Anaconda"),
            _ => throw new EliteException($"Unexpected trader roll '{rnd}'."),
        };
    }

    public List<IShip> CreateParade() => new()
    {
        { CreateShip("Missile") },
        { CreateShip("Coriolis") },
        { CreateShip("EscapeCapsule") },
        { CreateShip("Alloy") },
        { CreateShip("CargoCannister") },
        { CreateShip("Boulder") },
        { CreateShip("Asteroid") },
        { CreateShip("RockSplinter") },
        { CreateShip("Shuttle") },
        { CreateShip("Transporter") },
        { CreateShip("CobraMk3") },
        { CreateShip("Python") },
        { CreateShip("Boa") },
        { CreateShip("Anaconda") },
        { CreateShip("RockHermit") },
        { CreateShip("Viper") },
        { CreateShip("Sidewinder") },
        { CreateShip("Mamba") },
        { CreateShip("Krait") },
        { CreateShip("Adder") },
        { CreateShip("Gecko") },
        { CreateShip("CobraMk1") },
        { CreateShip("Worm") },
        { CreateShip("AspMk2") },
        { CreateShip("FerDeLance") },
        { CreateShip("Moray") },
        { CreateShip("Thargoid") },
        { CreateShip("Tharglet") },
        { CreateShip("DodecStation") },
    };
}
