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

    // Position the camera in the driver's seat (original inside view). The
    // y value is limited so the view never dips below or too close to the
    // road surface (the original ran LimitViewpointY between the car
    // physics and CalcGameViewpoint every frame).
    public void FollowCar(CarPhysics car)
    {
        Useful.Guard.ArgumentNull(car);

        X = car.PlayerX >> Track.LogPrecision;
        Y = (car.LimitViewpointY() >> (Track.LogPrecision - 2)) + HeightAboveRoad;
        Z = car.PlayerZ >> Track.LogPrecision;

        XAngle = car.PlayerXAngle;
        YAngle = car.PlayerYAngle;
        ZAngle = car.PlayerZAngle;
    }

    // Position the camera at a viewpoint looking at a target (original
    // LockViewpointToTarget). Positions are in the original's render scale:
    // x/z in physics units, y negative upwards.
    public void LookAt(long viewX, long viewY, long viewZ, long targetX, long targetY, long targetZ)
    {
        X = (int)(viewX >> Track.LogPrecision);
        Y = (int)(-viewY >> Track.LogPrecision);
        Z = (int)(viewZ >> Track.LogPrecision);

        // y angle from the x/z direction to the target
        int lockedY = LockAngle(targetX - viewX, targetZ - viewZ);

        // x angle from the y difference and the horizontal distance
        double a = (targetX - viewX) >> Track.LogPrecision;
        double b = (targetZ - viewZ) >> Track.LogPrecision;
        long adjacent = (long)(Math.Sqrt((a * a) + (b * b)) * AmigaTrig.Precision);
        int lockedX = LockAngle(targetY - viewY, adjacent);

        // the locked angles use the drawing convention; the camera holds the
        // raw physics convention (x and z reversed)
        XAngle = -lockedX & (Track.MaxAngle - 1);
        YAngle = lockedY & (Track.MaxAngle - 1);
        ZAngle = 0;
    }

    // Calculate the anti-clockwise angle for the given opposite/adjacent
    // (original LockAngle in 3D Engine.cpp).
    private static int LockAngle(long opposite, long adjacent)
    {
        // use inverse tan to calculate the basic angle in radians
        // (90 degrees when the adjacent is zero, preventing division by zero)
        double radians = adjacent == 0 ? Math.PI / 2 : Math.Atan((double)opposite / adjacent);

        // convert radians to the internal angle format
        double angle = radians * Track.MaxAngle / (2 * Math.PI);

        // convert the angle from the first quadrant to the full range
        return opposite >= 0
            ? (adjacent >= 0 ? (int)angle : (int)angle + AmigaTrig.Degrees180)
            : (adjacent <= 0 ? (int)angle + AmigaTrig.Degrees180 : (int)angle + AmigaTrig.Degrees360);
    }
}
