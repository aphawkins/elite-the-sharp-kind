// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp
{
    internal sealed class Universe
    {
        private const int MaxUniverseObjects = 20;
        private readonly IDraw _draw;
        private readonly List<IShip> _objects = new();
        private readonly Dictionary<ShipType, int> _shipCount = new();

        internal Universe(IDraw draw)
        {
            _draw = draw;
            ClearUniverse();
        }

        internal bool IsStationPresent => _shipCount[ShipType.Coriolis] != 0 || _shipCount[ShipType.Dodec] != 0;

        internal IShip? Planet { get; private set; }

        internal IShip? FirstShip => _objects.Count > 0 ? _objects[0] : StationOrSun;

        internal int PoliceCount => _shipCount[ShipType.Viper];

        internal IShip? StationOrSun { get; private set; }

        internal bool AddNewShip(IShip newShip, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null, "Rotation matrix should not be null.");

            if (_objects.Count >= MaxUniverseObjects)
            {
                return false;
            }

            newShip.Location = location;
            newShip.Rotmat = rotmat;
            if (newShip is IShipEx newShipEx)
            {
                newShipEx.RotX = rotx;
                newShipEx.RotZ = rotz;
                newShipEx.Energy = newShipEx.EnergyMax;
                newShipEx.Missiles = newShipEx.MissilesMax;
            }

            _shipCount[newShip.Type]++;

            if (newShip.Flags.HasFlag(ShipFlags.Station) || newShip.Type == ShipType.Sun)
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

        internal bool AddNewShip(IShipEx ship)
        {
            Vector3 position = new()
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

            return AddNewShip(ship, position, VectorMaths.GetInitialMatrix(), 0, 0);
        }

        internal void AddNewStation(int planetTechLevel, Vector3 position, Vector3[] rotmat)
        {
            IShipEx station = planetTechLevel >= 10 ? new DodecStation(_draw) : new Coriolis(_draw);
            AddNewShip(station, position, rotmat, 0, -127);
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

        internal IEnumerable<IShip> GetAllObjects()
        {
            if (Planet != null)
            {
                yield return Planet;
            }

            if (StationOrSun != null)
            {
                yield return StationOrSun;
            }

            foreach (IShip obj in _objects.ToList())
            {
                yield return obj;
            }
        }

        internal void RemoveShip(IShip ship)
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
