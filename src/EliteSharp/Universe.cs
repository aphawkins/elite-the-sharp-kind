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

        private readonly List<IObject> _objects = new();

        private readonly Dictionary<ShipType, int> _shipCount = new();

        internal Universe() => ClearUniverse();

        internal bool IsStationPresent => _shipCount[ShipType.Coriolis] != 0 || _shipCount[ShipType.Dodec] != 0;

        internal IObject? Planet { get; private set; }

        internal IObject? FirstShip => _objects.Count > 0 ? _objects[0] : StationOrSun;

        internal int PoliceCount => _shipCount[ShipType.Viper];

        internal IObject? StationOrSun { get; private set; }

        internal bool AddNewShip(IObject newShip, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null, "Rotation matrix should not be null.");

            if (_objects.Count >= MaxUniverseObjects)
            {
                return false;
            }

            newShip.Location = location;
            newShip.Rotmat = rotmat;
            newShip.RotX = rotx;
            newShip.RotZ = rotz;
            newShip.Energy = newShip.EnergyMax;
            newShip.Missiles = newShip.MissilesMax;
            _shipCount[newShip.Type]++;

            if (newShip.Class is ShipClass.Station || newShip.Type == ShipType.Sun)
            {
                StationOrSun = newShip;
            }
            else if (newShip.Type is ShipType.Planet)
            {
                Planet = newShip;
            }
            else
            {
                _objects.Add(newShip);
            }

            return true;
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
            ShipType station = planetTechLevel >= 10 ? ShipType.Dodec : ShipType.Coriolis;
            AddNewShip(ShipFactory.Create(station), position, rotmat, 0, -127);
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
}
