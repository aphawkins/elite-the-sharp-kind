// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Ships;

namespace EliteSharp
{
    internal sealed class Universe
    {
        private const int MaxUniverseObjects = 20;

        private readonly IObject[] _objects = new IObject[MaxUniverseObjects];

        private readonly Dictionary<ShipType, int> _shipCount = new();

        internal bool IsStationPresent => _shipCount[ShipType.Coriolis] != 0 || _shipCount[ShipType.Dodec] != 0;

        internal IObject Planet => _objects[0];

        internal int PoliceCount => _shipCount[ShipType.Viper];

        internal IObject StationOrSun => _objects[1];

        internal int ShipCount(ShipType shipType) => _shipCount[shipType];

        internal bool AddNewShip(IObject ship, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null, "Rotation matrix should not be null.");
            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                if (_objects[i].Type == ShipType.None)
                {
                    ship.Location = location;
                    ship.Rotmat = rotmat;
                    ship.RotX = rotx;
                    ship.RotZ = rotz;
                    ship.Energy = ship.EnergyMax;
                    ship.Missiles = ship.MissilesMax;
                    _objects[i] = ship;
                    _shipCount[ship.Type]++;
                    return true;
                }
            }

            return false;
        }

        internal bool AddNewShip(IObject ship)
        {
            Vector3 position = new()
            {
                X = 1000 + RNG.Random(8191),
                Y = 1000 + RNG.Random(8191),
                Z = 12000,
            };

            if (RNG.Random(255) > 127)
            {
                position.X = -position.X;
            }

            if (RNG.Random(255) > 127)
            {
                position.Y = -position.Y;
            }

            return AddNewShip(ship, position, VectorMaths.GetInitialMatrix(), 0, 0);
        }

        internal void AddNewStation(int planetTechLevel, Vector3 position, Vector3[] rotmat)
        {
            _objects[1] = new NullObject();
            ShipType station = planetTechLevel >= 10 ? ShipType.Dodec : ShipType.Coriolis;
            AddNewShip(ShipFactory.Create(station), position, rotmat, 0, -127);
        }

        internal void ClearUniverse()
        {
            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                _objects[i] = new NullObject();
            }

            foreach (ShipType shipType in Enum.GetValues<ShipType>())
            {
                _shipCount[shipType] = 0;
            }
        }

        internal IEnumerable<IObject> GetAllObjects()
        {
            foreach (IObject obj in _objects)
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

            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                if (_objects[i] == ship)
                {
                    _objects[i] = new NullObject();
                }
            }
        }
    }
}
