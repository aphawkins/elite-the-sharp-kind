// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using Elite.Engine.Ships;

namespace Elite.Engine
{
    internal sealed class Universe
    {
        private const int MaxUniverseObjects = 20;

        internal Dictionary<ShipType, int> ShipCount { get; } = new();

        internal IObject Planet => Objects[0];

        internal IObject StationOrSun => Objects[1];

        private IObject[] Objects { get; } = new IObject[MaxUniverseObjects];

        internal IObject AddNewShip(ShipType shipType, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null, "Rotation matrix should not be null.");
            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                if (Objects[i].Type == ShipType.None)
                {
                    IObject ship = ShipFactory.ConstructShip(shipType);
                    ship.Location = location;
                    ship.Rotmat = rotmat;
                    ship.RotX = rotx;
                    ship.RotZ = rotz;
                    ship.Energy = ship.EnergyMax;
                    ship.Missiles = ship.MissilesMax;
                    Objects[i] = ship;
                    ShipCount[shipType]++;
                    return ship;
                }
            }

            return new NullObject();
        }

        internal IObject AddNewShip(ShipType type)
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

            return AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
        }

        internal void AddNewStation(int planetTechLevel, Vector3 position, Vector3[] rotmat)
        {
            ShipType station = planetTechLevel >= 10 ? ShipType.Dodec : ShipType.Coriolis;
            Objects[1] = new NullObject();
            AddNewShip(station, position, rotmat, 0, -127);
        }

        internal void ClearUniverse()
        {
            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                Objects[i] = new NullObject();
            }

            foreach (ShipType shipType in Enum.GetValues<ShipType>())
            {
                ShipCount[shipType] = 0;
            }
        }

        internal IEnumerable<IObject> GetAllObjects()
        {
            foreach (IObject obj in Objects)
            {
                yield return obj;
            }
        }

        internal void RemoveShip(IObject ship)
        {
            if (ship.Type > ShipType.None)
            {
                ShipCount[ship.Type]--;
            }

            for (int i = 0; i < MaxUniverseObjects; i++)
            {
                if (Objects[i] == ship)
                {
                    Objects[i] = new NullObject();
                }
            }
        }
    }
}
