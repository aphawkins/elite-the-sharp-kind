// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using Useful.Maths;

namespace EliteSharp;

internal sealed class Universe
{
    private const int MaxUniverseObjects = 20;
    private readonly IEliteDraw _draw;
    private readonly List<IObject> _objects = [];
    private readonly Dictionary<ShipType, int> _shipCount = [];

    internal Universe(IEliteDraw draw)
    {
        _draw = draw;
        ClearUniverse();
    }

    internal bool IsStationPresent => _shipCount[ShipType.Coriolis] != 0 || _shipCount[ShipType.Dodec] != 0;

    internal IObject? Planet { get; private set; }

    internal IObject? FirstShip => _objects.Count > 0 ? _objects[0] : StationOrSun;

    internal int PoliceCount => _shipCount[ShipType.Viper];

    internal IObject? StationOrSun { get; private set; }

    internal bool AddNewShip(IObject newObj, Vector4 location, Matrix4x4 rotmat, float rotx, float rotz)
    {
        if (_objects.Count >= MaxUniverseObjects)
        {
            return false;
        }

        newObj.Location = location;
        newObj.Rotmat = rotmat.ToVector4Array();
        if (newObj is IShip newShip)
        {
            newShip.RotX = rotx;
            newShip.RotZ = rotz;
            newShip.Energy = newShip.EnergyMax;
            newShip.Missiles = newShip.MissilesMax;
        }

        _shipCount[newObj.Type]++;

        if (newObj.Flags.HasFlag(ShipProperties.Station) || newObj.Type == ShipType.Sun)
        {
            StationOrSun = newObj;
        }
        else if (newObj.Type is ShipType.Planet)
        {
            Planet = newObj;
        }
        else
        {
            _objects.Add(newObj);
        }

        return true;
    }

    internal bool AddNewShip(IShip ship)
    {
        Vector4 position = new()
        {
            X = 1000 + RNG.Random(8192),
            Y = 1000 + RNG.Random(8192),
            Z = 12000,
        };

        if (RNG.Random(256) > 127)
        {
            position.X = -position.X;
        }

        if (RNG.Random(256) > 127)
        {
            position.Y = -position.Y;
        }

        return AddNewShip(ship, position, VectorMaths.GetLeftHandedBasisMatrix, 0, 0);
    }

    internal void AddNewStation(int planetTechLevel, Vector4 position, Vector4[] rotmat)
    {
        IShip station = planetTechLevel >= 10 ? new DodecStation(_draw) : new Coriolis(_draw);
        AddNewShip(station, position, rotmat.ToMatrix4x4(), 0, -127);
    }

    internal void ClearUniverse()
    {
        Planet = null;
        StationOrSun = null;
        _objects.Clear();

        foreach (ShipType shipType in Enum.GetValues<ShipType>())
        {
            _shipCount[shipType] = 0;
        }
    }

    internal IEnumerable<IObject> GetAllObjects()
    {
        if (Planet != null)
        {
            yield return Planet;
        }

        if (StationOrSun != null)
        {
            yield return StationOrSun;
        }

        foreach (IObject obj in _objects.ToList())
        {
            yield return obj;
        }
    }

    internal void RemoveShip(IObject ship)
    {
        if (ship.Type > ShipType.None)
        {
            _shipCount[ship.Type]--;
        }

        _objects.Remove(ship);
    }

    internal int ShipCount(ShipType shipType) => _shipCount[shipType];
}
