// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;
using Useful;

namespace StuntCarRacerLib.Rendering;

// Software view transform, near-plane clipping and perspective projection,
// following the original software pipeline in 3D Engine.cpp (the parts the
// Direct3D remake had disabled). Works in 'track units' - see SceneCamera.
public sealed class Scene3D
{
    // Original Z_CLIP_BOUNDARY - don't take this value any lower,
    // otherwise calculations overflow.
    internal const int ZClipBoundary = 128;

    // Original FOCUS was 512 for a screen width of 640.
    internal const float FocusFactor = 512f / 640f;

    private readonly TrigCoefficients _trig = new();

    private int _cameraX;
    private int _cameraY;
    private int _cameraZ;
    private float _focus;
    private float _halfWidth;
    private float _halfHeight;

    // Clip a camera-space polygon against the near plane z = ZClipBoundary.
    // Returns the number of resulting points written to the output span
    // (up to one more point than the input).
    public static int ClipPolygonToNearPlane(in ReadOnlySpan<Coord3D> input, in Span<Coord3D> output)
    {
        int count = 0;

        for (int i = 0; i < input.Length; i++)
        {
            Coord3D current = input[i];
            Coord3D next = input[(i + 1) % input.Length];

            bool currentInside = current.Z >= ZClipBoundary;
            bool nextInside = next.Z >= ZClipBoundary;

            if (currentInside)
            {
                output[count++] = current;
            }

            if (currentInside != nextInside)
            {
                output[count++] = ClipEdge(current, next);
            }
        }

        return count;
    }

    public void SetView(SceneCamera camera, float screenWidth, float screenHeight)
    {
        Guard.ArgumentNull(camera);

        _cameraX = camera.X;
        _cameraY = camera.Y;
        _cameraZ = camera.Z;

        _trig.CalculateYXZ(camera.XAngle, camera.YAngle, camera.ZAngle);

        _focus = screenWidth * FocusFactor;
        _halfWidth = screenWidth / 2;
        _halfHeight = screenHeight / 2;
    }

    // Transform a world point (track units, y up) into camera space.
    public Coord3D TransformPoint(int worldX, int worldY, int worldZ)
    {
        long dx = worldX - _cameraX;
        long dy = worldY - _cameraY;
        long dz = worldZ - _cameraZ;

        int x = (int)(((dx * _trig.XX) + (dy * _trig.XY) + (dz * _trig.XZ)) >> Track.LogPrecision);
        int y = (int)(((dx * _trig.YX) + (dy * _trig.YY) + (dz * _trig.YZ)) >> Track.LogPrecision);
        int z = (int)(((dx * _trig.ZX) + (dy * _trig.ZY) + (dz * _trig.ZZ)) >> Track.LogPrecision);

        return new(x, y, z);
    }

    // Perspective projection of a camera-space point (z must be positive).
    public Vector2 ProjectPoint(Coord3D cameraPoint)
    {
        float z = cameraPoint.Z;
        return new(
            _halfWidth + (_focus * cameraPoint.X / z),
            _halfHeight - (_focus * cameraPoint.Y / z));
    }

    // Intersect an edge with the near plane z = ZClipBoundary.
    private static Coord3D ClipEdge(Coord3D start, Coord3D end)
    {
        long dz = (long)end.Z - start.Z;

        // both points are on the same side when dz is zero, so this is only
        // defensive (matching the original ZClip divide-by-zero guard)
        if (dz == 0)
        {
            return start with { Z = ZClipBoundary };
        }

        long t = ZClipBoundary - start.Z;
        int x = (int)(start.X + (((long)end.X - start.X) * t / dz));
        int y = (int)(start.Y + (((long)end.Y - start.Y) * t / dz));

        return new(x, y, ZClipBoundary);
    }
}
