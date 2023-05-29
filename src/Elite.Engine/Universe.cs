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
        internal Dictionary<ShipType, int> ShipCount { get; } = new();

        internal IShip[] Objects { get; } = new IShip[EliteMain.MaxUniverseObjects];

        internal int AddNewShip(ShipType shipType, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null, "Rotation matrix should not be null.");
            for (int i = 0; i < EliteMain.MaxUniverseObjects; i++)
            {
                if (Objects[i].Type == ShipType.None)
                {
                    IShip ship = ShipFactory.ConstructShip(shipType);
                    ship.Location = location;
                    ship.Rotmat = rotmat;
                    ship.RotX = rotx;
                    ship.RotZ = rotz;
                    ship.Energy = ship.EnergyMax;
                    ship.Missiles = ship.MissilesMax;
                    Objects[i] = ship;
                    ShipCount[shipType]++;
                    return i;
                }
            }

            return -1;
        }
    }
}
