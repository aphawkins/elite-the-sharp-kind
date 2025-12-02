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

    public Vector4[] Rotmat { get; set; } = new Vector4[4];

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
        Vector4[] trans_mat = new Vector4[4];

        for (int i = 0; i < Rotmat.Length; i++)
        {
            trans_mat[i] = Rotmat[i];
        }

        // Build transform matrix from trans_mat array
        Matrix4x4 transform = trans_mat.ToMatrix4x4();

        // Camera vector (unit) - keep previous call to UnitVector
        Vector4 camera_vec = Vector4.Transform(Location, transform);
        _ = VectorMaths.UnitVector(camera_vec);

        // The following three swaps mirror original behavior
        (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
        (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
        (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);

        // Transform model points
        TransformModelPoints(transform, pointList);

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

                float zavg = 0;

                for (int j = 0; j < num_points; j++)
                {
                    poly_list[j].X = pointList[GetPointIndex(Model.Faces[i].Points[j])].X;
                    poly_list[j].Y = pointList[GetPointIndex(Model.Faces[i].Points[j])].Y;
                    zavg = MathF.Max(zavg, pointList[GetPointIndex(Model.Faces[i].Points[j])].Z);
                }

                _draw.DrawPolygonFilled(poly_list, Model.Faces[i].Color, zavg);
            }
        }
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
