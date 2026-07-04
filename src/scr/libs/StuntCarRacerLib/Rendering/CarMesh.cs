// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Tracks;
using Useful;

namespace StuntCarRacerLib.Rendering;

// The visible car shape from the original Car.cpp (CreateCarInVB): four flat
// wheel quads and a wedge-shaped body, coloured from the track palette.
// The mesh is oriented onto a frame derived from the car's wheel positions.
public static class CarMesh
{
    // VCAR dimensions from the original Car.h.
    private const int Width = 162;

    private const int Length = 256;

    private const int Height = 162;

    // Model-space vertices (x right, y up, z forward), from CreateCarInVB.
    private static readonly Coord3D[] s_vertices =
    [
        new(-Width / 2, -Height / 4, -Length / 2), // rear left wheel
        new(-Width / 2, 0, -Length / 2),
        new(-Width / 4, 0, -Length / 2),
        new(-Width / 4, -Height / 4, -Length / 2),

        new(Width / 4, -Height / 4, -Length / 2), // rear right wheel
        new(Width / 4, 0, -Length / 2),
        new(Width / 2, 0, -Length / 2),
        new(Width / 2, -Height / 4, -Length / 2),

        new(-Width / 2, -Height / 4, Length / 2), // front left wheel
        new(-Width / 2, 0, Length / 2),
        new(-Width / 4, 0, Length / 2),
        new(-Width / 4, -Height / 4, Length / 2),

        new(Width / 4, -Height / 4, Length / 2), // front right wheel
        new(Width / 4, 0, Length / 2),
        new(Width / 2, 0, Length / 2),
        new(Width / 2, -Height / 4, Length / 2),

        new(-Width / 4, -Height / 8, -Length / 2), // car rear points
        new(-3 * Width / 16, Height / 4, -Length / 2),
        new(3 * Width / 16, Height / 4, -Length / 2),
        new(Width / 4, -Height / 8, -Length / 2),

        new(-Width / 4, -Height / 8, Length / 2), // car front points
        new(-Width / 4, 0, Length / 2),
        new(Width / 4, 0, Length / 2),
        new(Width / 4, -Height / 8, Length / 2),
    ];

    // Quads as vertex indices plus a track palette colour offset
    // (each pair of triangles in the original forms one quad).
    private static readonly (int A, int B, int C, int D, int ColourOffset)[] s_quads =
    [
        (0, 1, 2, 3, 0), // rear left wheel (black)
        (4, 5, 6, 7, 0), // rear right wheel
        (8, 9, 10, 11, 0), // front left wheel
        (12, 13, 14, 15, 0), // front right wheel
        (20, 21, 17, 16, 12), // car left side
        (19, 18, 22, 23, 12), // car right side
        (16, 17, 18, 19, 10), // car back
        (23, 22, 21, 20, 10), // car front
        (17, 21, 22, 18, 15), // car top (white)
        (19, 23, 20, 16, 9), // car bottom
    ];

    // Appends the car mesh polygons, oriented on the given wheel-corner
    // positions (world track units); the mesh bottom sits at corner level.
    public static void Append(
        ICollection<WorldPolygon> polygons,
        Coord3D rearLeft,
        Coord3D rearRight,
        Coord3D frontLeft,
        Coord3D frontRight)
    {
        Guard.ArgumentNull(polygons);

        // orientation frame from the wheel corners
        (double rightX, double rightY, double rightZ) = Normalize(
            rearRight.X - rearLeft.X,
            rearRight.Y - rearLeft.Y,
            rearRight.Z - rearLeft.Z);

        double frontX = (frontLeft.X + frontRight.X) / 2.0;
        double frontY = (frontLeft.Y + frontRight.Y) / 2.0;
        double frontZ = (frontLeft.Z + frontRight.Z) / 2.0;
        double rearX = (rearLeft.X + rearRight.X) / 2.0;
        double rearY = (rearLeft.Y + rearRight.Y) / 2.0;
        double rearZ = (rearLeft.Z + rearRight.Z) / 2.0;

        (double forwardX, double forwardY, double forwardZ) = Normalize(frontX - rearX, frontY - rearY, frontZ - rearZ);

        // up = forward x right (positive y when the car is level)
        (double vertX, double vertY, double vertZ) = Normalize(
            (forwardY * rightZ) - (forwardZ * rightY),
            (forwardZ * rightX) - (forwardX * rightZ),
            (forwardX * rightY) - (forwardY * rightX));

        // origin at the centre of the corners, raised so the mesh bottom
        // (-Height/4) sits at corner level
        double originX = ((frontX + rearX) / 2) + (vertX * Height / 4);
        double originY = ((frontY + rearY) / 2) + (vertY * Height / 4);
        double originZ = ((frontZ + rearZ) / 2) + (vertZ * Height / 4);

        Span<Coord3D> world = stackalloc Coord3D[s_vertices.Length];
        for (int i = 0; i < s_vertices.Length; i++)
        {
            Coord3D v = s_vertices[i];
            world[i] = new(
                (int)(originX + (rightX * v.X) + (vertX * v.Y) + (forwardX * v.Z)),
                (int)(originY + (rightY * v.X) + (vertY * v.Y) + (forwardY * v.Z)),
                (int)(originZ + (rightZ * v.X) + (vertZ * v.Y) + (forwardZ * v.Z)));
        }

        foreach ((int a, int b, int c, int d, int colourOffset) in s_quads)
        {
            polygons.Add(new(
                [world[a], world[b], world[c], world[d]],
                ScrPalette.Colour(Track.ScrBaseColour + colourOffset)));
        }
    }

    private static (double X, double Y, double Z) Normalize(double x, double y, double z)
    {
        double length = Math.Sqrt((x * x) + (y * y) + (z * z));
        if (length < 1e-9)
        {
            return (0, 0, 0);
        }

        return (x / length, y / length, z / length);
    }
}
