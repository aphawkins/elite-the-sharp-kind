// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;

namespace StuntCarRacerLib.Rendering;

// Viewpoint for drawing the world, following the original CalcGameViewpoint.
// Positions are in 'track units' - the fixed-point world with PRECISION
// removed, matching the units used by the original GetPieceVertex
// (x/z = physics position >> 14; y = piece coord y / 4, positive upwards).
public sealed class SceneCamera
{
    // Cockpit viewpoint height above the car position (original HEIGHT_ABOVE_ROAD).
    private const int HeightAboveRoad = 100;

    public int X { get; private set; }

    public int Y { get; private set; }

    public int Z { get; private set; }

    // Raw physics angle format (unsigned, 65536 = 360 degrees).
    public int XAngle { get; private set; }

    public int YAngle { get; private set; }

    public int ZAngle { get; private set; }

    // Position the camera in the driver's seat (original inside view).
    public void FollowCar(CarPhysics car)
    {
        Useful.Guard.ArgumentNull(car);

        X = car.PlayerX >> Track.LogPrecision;
        Y = (car.PlayerY >> (Track.LogPrecision - 2)) + HeightAboveRoad;
        Z = car.PlayerZ >> Track.LogPrecision;

        XAngle = car.PlayerXAngle;
        YAngle = car.PlayerYAngle;
        ZAngle = car.PlayerZAngle;
    }
}
