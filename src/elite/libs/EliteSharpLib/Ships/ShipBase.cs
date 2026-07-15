// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;
using Useful.Assets.Models;
using Useful.Maths;

namespace EliteSharpLib.Ships;

internal class ShipBase : IShip
{
    private readonly IEliteDraw _draw;
    private readonly uint _colorCyan;
    private readonly uint _colorWhite;
    private int[]? _faceRoot;

    internal ShipBase(IEliteDraw draw)
    {
        _draw = draw;
        Model = ModelReader.None;

        _colorCyan = draw.Palette["Cyan"];
        _colorWhite = draw.Palette["White"];
    }

    private ShipBase(ShipBase other)
    {
        _draw = other._draw;
        Model = other.Model;
    }

    public int Acceleration { get; set; }

    public float Bounty { get; set; }

    public int Bravery { get; set; }

    public int Energy { get; set; }

    public int EnergyMax { get; set; }

    public int ExpDelta { get; set; }

    public ShipProperties Flags { get; set; } = ShipProperties.None;

    public int LaserFront { get; set; }

    public int LaserStrength { get; set; }

    public Vector4 Location { get; set; }

    public int LootMax { get; set; }

    public float MinDistance { get; set; }

    public int Missiles { get; set; }

    public int MissilesMax { get; set; }

    public string Name { get; set; } = string.Empty;

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public StockType ScoopedType { get; set; }

    public float Size { get; set; }

    public IObject? Target { get; set; }

    public ShipType Type { get; set; }

    public int VanishPoint { get; set; }

    public float Velocity { get; set; }

    public float VelocityMax { get; set; }

    public ThreeDModel Model { get; set; }

    public IObject Clone()
    {
        ShipBase ship = new(this);
        this.CopyTo(ship);
        return ship;
    }

    /// <summary>
    /// Hacked version of the draw ship routine to display ships...
    /// This needs a lot of tidying...
    /// caveat: it is a work in progress.
    /// A number of features(such as not showing detail at distance) have not yet been implemented.
    /// Check for hidden surface supplied by T.Harte.
    /// </summary>
    public virtual void Draw()
    {
        Vector4[] pointList = new Vector4[100];

        // Camera vector (unit) - keep previous call to UnitVector
        _ = VectorMaths.UnitVector(Vector4.Transform(Location, Rotmat));

        // Transform model points
        TransformModelPoints(Rotmat, pointList);

        // Draw faces
        DrawModelFaces(pointList);

        // Draw firing lasers if needed
        DrawLasers(pointList);
    }

    private void TransformModelPoints(Matrix4x4 transform, Vector4[] pointList)
    {
        for (int i = 0; i < Model.Points.Count; i++)
        {
            Vector4 vec = Vector4.Transform(Model.Points[i].Coords, transform);
            vec += Location;

            if (vec.Z <= 0)
            {
                vec.Z = 1;
            }

            vec.X = ((vec.X * 256 / vec.Z) + (_draw.Centre.X / 2)) * _draw.Graphics.Scale;
            vec.Y = ((-vec.Y * 256 / vec.Z) + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale;

            pointList[i] = vec;
        }
    }

    private void DrawModelFaces(Vector4[] pointList)
    {
        _faceRoot ??= FindFaceRoots();

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            int point0 = GetPointIndex(Model.Faces[i].Points[0]);
            int point1 = GetPointIndex(Model.Faces[i].Points[1]);
            int point2 = Model.Faces[i].Points.Count > 2
                ? GetPointIndex(Model.Faces[i].Points[2])
                : GetPointIndex(Model.Faces[i].Points[0]);

            if (((pointList[point0].X - pointList[point1].X) * (pointList[point2].Y - pointList[point1].Y)) <=
                ((pointList[point0].Y - pointList[point1].Y) * (pointList[point2].X - pointList[point1].X)))
            {
                int num_points = Model.Faces[i].Points.Count;
                Vector2[] poly_list = new Vector2[num_points];

                for (int j = 0; j < num_points; j++)
                {
                    int index = GetPointIndex(Model.Faces[i].Points[j]);
                    poly_list[j].X = pointList[index].X;
                    poly_list[j].Y = pointList[index].Y;
                }

                _draw.DrawPolygonFilled(poly_list, Model.Faces[i].Color, FaceMeanZ(_faceRoot[i], pointList));
            }
        }
    }

    // The whole-face depth key: the mean Z of the face's transformed
    // points. Decals and detail lines use their root face's key so they
    // tie exactly with the surface they sit on and draw over it.
    private float FaceMeanZ(int faceIndex, Vector4[] pointList)
    {
        Face face = Model.Faces[faceIndex];
        float z = 0;
        for (int j = 0; j < face.Points.Count; j++)
        {
            z += pointList[GetPointIndex(face.Points[j])].Z;
        }

        return z / face.Points.Count;
    }

    // For each face, the face it sits on: decal faces (cockpit windows,
    // engine plates) and 2-point detail lines lie exactly in the plane of
    // an earlier, larger face. They must render over that base face, so
    // they share its depth key. Faces on no earlier plane root to
    // themselves. Computed once per instance from the model geometry.
    private int[] FindFaceRoots()
    {
        int[] roots = new int[Model.Faces.Count];
        List<(Vector4 Normal, float Offset, int Index)> planes = [];

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            roots[i] = i;

            Face face = Model.Faces[i];
            foreach ((Vector4 normal, float offset, int index) in planes)
            {
                bool onPlane = true;
                for (int j = 0; j < face.Points.Count && onPlane; j++)
                {
                    onPlane = MathF.Abs(VectorMaths.VectorDotProduct(normal, face.Points[j].Coords) - offset) < 0.1f;
                }

                if (onPlane)
                {
                    roots[i] = roots[index];
                    break;
                }
            }

            if (face.Points.Count >= 3)
            {
                Vector4 edge1 = face.Points[1].Coords - face.Points[0].Coords;
                Vector4 edge2 = face.Points[2].Coords - face.Points[0].Coords;
                Vector3 cross = Vector3.Cross(new(edge1.X, edge1.Y, edge1.Z), new(edge2.X, edge2.Y, edge2.Z));
                if (cross.LengthSquared() > 0)
                {
                    cross = Vector3.Normalize(cross);
                    Vector4 normal = new(cross, 0);
                    planes.Add((normal, VectorMaths.VectorDotProduct(normal, face.Points[0].Coords), i));
                }
            }
        }

        return roots;
    }

    private void DrawLasers(Vector4[] pointList)
    {
        if (Flags.HasFlag(ShipProperties.Firing))
        {
            int lasv = LaserFront;
            uint color = (Type == ShipType.Viper) ? _colorCyan : _colorWhite;

            Vector2[] laserPoints =
            [
                new(pointList[lasv].X, pointList[lasv].Y),
                new(Location.X > 0 ? 0 : 511, RNG.Random(256) * 2),
            ];

            _draw.DrawPolygonFilled(laserPoints, color, pointList[lasv].Z);
        }
    }

    private int GetPointIndex(Point point)
    {
        foreach (Point p in Model.Points)
        {
            if (p == point)
            {
                return Model.Points.IndexOf(p);
            }
        }

        throw new InvalidOperationException("Point not found in model points.");
    }
}
