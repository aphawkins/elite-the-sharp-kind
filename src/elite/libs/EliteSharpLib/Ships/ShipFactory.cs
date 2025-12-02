// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Reflection;
using EliteSharpLib.Graphics;
using Useful;
using Useful.Assets;
using Useful.Assets.Models;

namespace EliteSharpLib.Ships;

internal class ShipFactory : IShipFactory
{
    private Dictionary<string, IShip> _ships = [];

    public static ShipFactory Create(IAssetLocator assetLocator, IEliteDraw draw)
    {
        Guard.ArgumentNull(assetLocator);

        return new()
        {
            _ships = assetLocator.ModelPaths.ToDictionary(
                x => x.Key,
                x => CreateShipFromName(x.Key, x.Value, draw)),
        };
    }

    public IShip CreateShip(string shipName)
    => _ships.TryGetValue(shipName, out IShip? ship)
        ? (IShip)ship.Clone()
        : throw new EliteException($"Ship model '{shipName}' not found.");

    public IShip CreateAsteroid() => RNG.Random(256) > 253 ? CreateShip("RockHermit") : CreateShip("Asteroid");

    public IShip CreateLoneWolf()
    {
        int rnd = RNG.Random(256);
        return ((rnd & 3) + (rnd > 127 ? 1 : 0)) switch
        {
            0 => CreateShip("CobraMk3Lone"),
            1 => CreateShip("AspMk2"),
            2 => CreateShip("PythonLone"),
            3 => CreateShip("FerDeLance"),
            4 => CreateShip("Moray"),
            _ => throw new EliteException(),
        };
    }

    public IShip CreatePackHunter() => RNG.Random(7) switch
    {
        0 => CreateShip("Sidewinder"),
        1 => CreateShip("Mamba"),
        2 => CreateShip("Krait"),
        3 => CreateShip("Adder"),
        4 => CreateShip("Gecko"),
        5 => CreateShip("CobraMk1"),
        6 => CreateShip("Worm"),
        _ => throw new EliteException(),
    };

    public IShip CreatePirate() => RNG.Random(4) switch
    {
        0 => CreateShip("Sidewinder"),
        1 => CreateShip("Mamba"),
        2 => CreateShip("Krait"),
        3 => CreateShip("Adder"),
        _ => throw new EliteException(),
    };

    public IShip CreateTrader() => RNG.Random(4) switch
    {
        0 => CreateShip("CobraMk3"),
        1 => CreateShip("Python"),
        2 => CreateShip("Boa"),
        3 => CreateShip("Anaconda"),
        _ => throw new EliteException(),
    };

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

    // TODO: create ships purely from metadata
    private static IShip CreateShipFromName(string name, string modelPath, IEliteDraw draw)
    {
        Type? type = (Type.GetType(name) ??
            Assembly.GetCallingAssembly().GetType("EliteSharpLib.Ships." + name))
            ?? throw new EliteException($"Type '{name}' could not be found.");

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        object? instance = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, [draw], null);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        if (instance is IShip ship)
        {
            ship.Model = ModelReader.Read(modelPath, draw.Palette);
            return ship;
        }

        throw new EliteException($"Type '{name}' is not an IShip.");
    }
}
