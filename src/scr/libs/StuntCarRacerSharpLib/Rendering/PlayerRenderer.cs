// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Cars;
using StuntCarRacerSharpLib.Tracks;

namespace StuntCarRacerSharpLib.Rendering;

// Draws the player's own car, for the outside view only - in the cockpit
// view the camera sits inside it. The original had no player mesh in world
// space either: the remake's SetCarWorldTransform builds one from the
// player's position and angles, which is what this mirrors, orienting the
// shared CarMesh on four corners rotated out of the car's own frame rather
// than on measured wheel positions the way the opponent does.
public sealed class PlayerRenderer
{
    // The remake raises the car mesh by VCAR_HEIGHT/3 "so wheels are fully
    // visible" (`StuntCarRacer.cpp:935-936`), and the mesh's own wheels
    // reach VCAR_HEIGHT/4 below its origin - so the wheels end up this far
    // above the car's centre. CarMesh puts the mesh bottom on the corner
    // frame, so the same offset applied to the corners gives the same
    // result.
    private const int RideHeight = (CarMesh.CarHeight / 3) - (CarMesh.CarHeight / 4);

    private readonly CarPhysics _car;
    private readonly CarMesh _carMesh;

    public PlayerRenderer(CarPhysics car, CarMesh carMesh)
    {
        ArgumentNullException.ThrowIfNull(car);
        ArgumentNullException.ThrowIfNull(carMesh);
        _car = car;
        _carMesh = carMesh;
    }

    // Appends the player's polygons (world track units) for depth-sorted
    // drawing with the track.
    public void AppendWorldPolygons(ICollection<WorldPolygon> polygons)
    {
        ArgumentNullException.ThrowIfNull(polygons);

        int x = _car.PlayerX >> Track.LogPrecision;
        int y = _car.PlayerY >> (Track.LogPrecision - 2);
        int z = _car.PlayerZ >> Track.LogPrecision;

        // the mesh's own footprint, not the physics car's: CarMesh only
        // takes orientation and centre from the frame, so corners that
        // match the visible wheels are what the road clearance below has
        // to be measured at
        const int halfWidth = CarMesh.CarWidth / 2;
        const int halfLength = CarMesh.CarLength / 2;

        Coord3D rearLeft = Corner(x, y, z, -halfWidth, -halfLength);
        Coord3D rearRight = Corner(x, y, z, halfWidth, -halfLength);
        Coord3D frontLeft = Corner(x, y, z, -halfWidth, halfLength);
        Coord3D frontRight = Corner(x, y, z, halfWidth, halfLength);

        // The car's centre is not its wheel contact line: under a hard
        // landing the suspension bottoms out and the body drops below the
        // road, which drew the car half-buried in it. Lift the whole frame
        // clear rather than clamping each corner, so the car stays rigid
        // and keeps the roll and pitch the angles give it. This is the
        // player's version of the max(road, actual) heights the opponent
        // draws itself on.
        int lift = Math.Max(
            Math.Max(_car.FrontLeftRoadY - frontLeft.Y, _car.FrontRightRoadY - frontRight.Y),
            Math.Max(_car.RearRoadY - rearLeft.Y, _car.RearRoadY - rearRight.Y));

        if (lift > 0)
        {
            rearLeft.Y += lift;
            rearRight.Y += lift;
            frontLeft.Y += lift;
            frontRight.Y += lift;
        }

        _carMesh.Append(polygons, rearLeft, rearRight, frontLeft, frontRight);
    }

    // A wheel corner in world track units: the car-local offset rotated by
    // the car's orientation, added to its centre.
    private Coord3D Corner(int x, int y, int z, int localX, int localZ)
    {
        Coord3D offset = _car.RotateToWorld(localX, RideHeight, localZ);

        return new(x + offset.X, y + offset.Y, z + offset.Z);
    }
}
