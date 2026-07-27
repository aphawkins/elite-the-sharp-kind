// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using Useful.Assets;
using Useful.Assets.Models;

namespace EliteSharpLib.Ships;

internal sealed class ShipFactory : IShipFactory
{
    private static readonly Dictionary<string, Func<IEliteDraw, RNG, IShip>> s_constructors = new()
    {
        { "Adder", (draw, rng) => new Adder(draw, rng) },
        { "Alloy", (draw, rng) => new Alloy(draw, rng) },
        { "Anaconda", (draw, rng) => new Anaconda(draw, rng) },
        { "AspMk2", (draw, rng) => new AspMk2(draw, rng) },
        { "Asteroid", (draw, rng) => new Asteroid(draw, rng) },
        { "Boa", (draw, rng) => new Boa(draw, rng) },
        { "Boulder", (draw, rng) => new Boulder(draw, rng) },
        { "CargoCannister", (draw, rng) => new CargoCannister(draw, rng) },
        { "CobraMk1", (draw, rng) => new CobraMk1(draw, rng) },
        { "CobraMk3", (draw, rng) => new CobraMk3(draw, rng) },
        { "CobraMk3Lone", (draw, rng) => new CobraMk3Lone(draw, rng) },
        { "Constrictor", (draw, rng) => new Constrictor(draw, rng) },
        { "Coriolis", (draw, rng) => new Coriolis(draw, rng) },
        { "Cougar", (draw, rng) => new Cougar(draw, rng) },
        { "DodecStation", (draw, rng) => new DodecStation(draw, rng) },
        { "EscapeCapsule", (draw, rng) => new EscapeCapsule(draw, rng) },
        { "FerDeLance", (draw, rng) => new FerDeLance(draw, rng) },
        { "Gecko", (draw, rng) => new Gecko(draw, rng) },
        { "Krait", (draw, rng) => new Krait(draw, rng) },
        { "Mamba", (draw, rng) => new Mamba(draw, rng) },
        { "Missile", (draw, rng) => new Missile(draw, rng) },
        { "Moray", (draw, rng) => new Moray(draw, rng) },
        { "Python", (draw, rng) => new Python(draw, rng) },
        { "PythonLone", (draw, rng) => new PythonLone(draw, rng) },
        { "RockHermit", (draw, rng) => new RockHermit(draw, rng) },
        { "RockSplinter", (draw, rng) => new RockSplinter(draw, rng) },
        { "Shuttle", (draw, rng) => new Shuttle(draw, rng) },
        { "Sidewinder", (draw, rng) => new Sidewinder(draw, rng) },
        { "Tharglet", (draw, rng) => new Tharglet(draw, rng) },
        { "Thargoid", (draw, rng) => new Thargoid(draw, rng) },
        { "Transporter", (draw, rng) => new Transporter(draw, rng) },
        { "Viper", (draw, rng) => new Viper(draw, rng) },
        { "Worm", (draw, rng) => new Worm(draw, rng) },
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
        foreach ((string name, Func<IEliteDraw, RNG, IShip> constructor) in s_constructors)
        {
            string modelName = s_modelNames.GetValueOrDefault(name, name);
            if (assetLocator.ModelPaths.TryGetValue(modelName, out string? modelPath))
            {
                IShip ship = constructor(draw, rng);
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
        int rnd = _rng.Random(7);
        return rnd switch
        {
            0 => CreateShip("Sidewinder"),
            1 => CreateShip("Mamba"),
            2 => CreateShip("Krait"),
            3 => CreateShip("Adder"),
            4 => CreateShip("Gecko"),
            5 => CreateShip("CobraMk1"),
            6 => CreateShip("Worm"),
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
