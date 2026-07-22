// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerSharpLib.Cars;
using StuntCarRacerSharpLib.Tracks;
using Useful;

namespace StuntCarRacerSharpLib.Rendering;

// Software view transform, near-plane clipping and perspective projection,
// following the original software pipeline in 3D Engine.cpp (the parts the
// Direct3D remake had disabled). Works in 'track units' - see SceneCamera.
public sealed class Scene3D
{
    // The remake's projection near plane (D3DXMatrixPerspectiveFovLH zn).
    // The original software engine clipped at Z_CLIP_BOUNDARY = 128 to
    // avoid fixed-point overflow, which amputated the nearest 128 units of
    // road - visible along the bottom of the screen whenever the car
    // pitches. Clipping is done in floats here, so the boundary can sit as
    // close as the remake's.
    internal const float NearPlane = 0.5f;

    // Original FOCUS was 512 for a screen width of 640.
    internal const float FocusFactor = 512f / 640f;

    private readonly TrigCoefficients _trig = new();

    private int _cameraX;
    private int _cameraY;
    private int _cameraZ;
    private float _focus;
    private float _halfWidth;
    private float _halfHeight;

    // Clip a camera-space polygon against the near plane z = NearPlane.
    // Returns the number of resulting points written to the output span
    // (up to one more point than the input).
    public static int ClipPolygonToNearPlane(in ReadOnlySpan<Vector3> input, in Span<Vector3> output)
    {
        Span<Vector2> textureCoords = stackalloc Vector2[input.Length];
        Span<Vector2> clippedTextureCoords = stackalloc Vector2[input.Length + 1];
        return ClipPolygonToNearPlane(input, textureCoords, output, clippedTextureCoords);
    }

    // As above, interpolating a texture coordinate per point through the
    // clip (textureCoords pairs with input, outputTextureCoords with output).
    public static int ClipPolygonToNearPlane(
        in ReadOnlySpan<Vector3> input,
        in ReadOnlySpan<Vector2> textureCoords,
        in Span<Vector3> output,
        in Span<Vector2> outputTextureCoords)
    {
        int count = 0;

        for (int i = 0; i < input.Length; i++)
        {
            int nextIndex = (i + 1) % input.Length;
            Vector3 current = input[i];
            Vector3 next = input[nextIndex];

            bool currentInside = current.Z >= NearPlane;
            bool nextInside = next.Z >= NearPlane;

            if (currentInside)
            {
                outputTextureCoords[count] = textureCoords[i];
                output[count++] = current;
            }

            if (currentInside != nextInside)
            {
                float t = (NearPlane - current.Z) / (next.Z - current.Z);
                outputTextureCoords[count] = Vector2.Lerp(textureCoords[i], textureCoords[nextIndex], t);
                output[count++] = new(
                    current.X + ((next.X - current.X) * t),
                    current.Y + ((next.Y - current.Y) * t),
                    NearPlane);
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
        => ProjectPoint(new Vector3(cameraPoint.X, cameraPoint.Y, cameraPoint.Z));

    public Vector2 ProjectPoint(Vector3 cameraPoint)
    {
        float z = cameraPoint.Z;
        return new(
            _halfWidth + (_focus * cameraPoint.X / z),
            _halfHeight - (_focus * cameraPoint.Y / z));
    }
}
