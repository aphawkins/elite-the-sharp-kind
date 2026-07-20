// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Assets.Models;

namespace StuntCarRacerLib.Rendering;

// The visible car shape from the original Car.cpp (CreateCarInVB): four flat
// wheel quads and a wedge-shaped body, loaded from car.obj and coloured via
// CarPalette. The mesh is oriented onto a frame derived from the car's wheel
// positions.
public sealed class CarMesh
{
    private readonly ThreeDModel _model;

    // How far the mesh's lowest point sits below its own origin, so Append
    // can raise the mesh to sit exactly on the wheel-corner frame.
    private readonly float _bottomOffset;

    public CarMesh()
        : this(Path.Combine(AppContext.BaseDirectory, "Assets", "Models", "car.obj"))
    {
    }

    internal CarMesh(string modelPath)
    {
        _model = ModelReader.Read(modelPath, CarPalette.Colours());
        _bottomOffset = -_model.Points.Min(p => p.Coords.Y);
    }

    // Appends the car mesh polygons, oriented on the given wheel-corner
    // positions (world track units); the mesh bottom sits at corner level.
    public void Append(
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
        // sits at corner level
        double originX = ((frontX + rearX) / 2) + (vertX * _bottomOffset);
        double originY = ((frontY + rearY) / 2) + (vertY * _bottomOffset);
        double originZ = ((frontZ + rearZ) / 2) + (vertZ * _bottomOffset);

        Span<Coord3D> world = stackalloc Coord3D[_model.Points.Count];
        for (int i = 0; i < _model.Points.Count; i++)
        {
            Vector4 v = _model.Points[i].Coords;
            world[i] = new(
                (int)(originX + (rightX * v.X) + (vertX * v.Y) + (forwardX * v.Z)),
                (int)(originY + (rightY * v.X) + (vertY * v.Y) + (forwardY * v.Z)),
                (int)(originZ + (rightZ * v.X) + (vertZ * v.Y) + (forwardZ * v.Z)));
        }

        foreach (Face face in _model.Faces)
        {
            Coord3D[] facePoints = new Coord3D[face.PointIndices.Count];
            for (int j = 0; j < facePoints.Length; j++)
            {
                facePoints[j] = world[face.PointIndices[j]];
            }

            polygons.Add(new(facePoints, face.Color));
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
