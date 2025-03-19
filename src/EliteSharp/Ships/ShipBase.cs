// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Trader;

namespace EliteSharp.Ships;

internal class ShipBase : IShip
{
    private readonly IDraw _draw;

    internal ShipBase(IDraw draw) => _draw = draw;

    private ShipBase(ShipBase other) => _draw = other._draw;

    public int Acceleration { get; set; }

    public float Bounty { get; set; }

    public int Bravery { get; set; }

    public int Energy { get; set; }

    public int EnergyMax { get; set; }

    public int ExpDelta { get; set; }

    public ShipFaceNormal[] FaceNormals { get; set; } = [];

    public ShipFace[] Faces { get; set; } = [];

    public ShipProperties Flags { get; set; } = ShipProperties.None;

    public int LaserFront { get; set; }

    public int LaserStrength { get; set; }

    public ShipLine[] Lines { get; set; } = [];

    public Vector3 Location { get; set; }

    public int LootMax { get; set; }

    public float MinDistance { get; set; }

    public int Missiles { get; set; }

    public int MissilesMax { get; set; }

    public string Name { get; set; } = string.Empty;

    public ShipPoint[] Points { get; set; } = [];

    public Vector3[] Rotmat { get; set; } = new Vector3[3];

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public StockType ScoopedType { get; set; }

    public float Size { get; set; }

    public IObject? Target { get; set; }

    public ShipType Type { get; set; }

    public int VanishPoint { get; set; }

    public float Velocity { get; set; }

    public float VelocityMax { get; set; }

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
        Vector3[] pointList = new Vector3[100];
        Vector3[] trans_mat = new Vector3[3];
        int lasv;

        for (int i = 0; i < 3; i++)
        {
            trans_mat[i] = Rotmat[i];
        }

        Vector3 camera_vec = VectorMaths.MultiplyVector(Location, trans_mat);
        _ = VectorMaths.UnitVector(camera_vec);
        ShipFace[] face_data = Faces;

        ////for (i = 0; i < num_faces; i++)
        ////{
        ////  vec.x = face_data[i].norm_x;
        ////  vec.y = face_data[i].norm_y;
        ////  vec.z = face_data[i].norm_z;

        ////  vec = VectorMaths.unit_vector (&vec);
        ////  cos_angle = VectorMaths.vector_dot_product (&vec, &camera_vec);

        ////  visible[i] = (cos_angle < -0.13);
        ////}
        (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
        (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
        (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);

        for (int i = 0; i < Points.Length; i++)
        {
            Vector3 vec = VectorMaths.MultiplyVector(Points[i].Point, trans_mat);
            vec += Location;

            if (vec.Z <= 0)
            {
                vec.Z = 1;
            }

            vec.X = ((vec.X * 256 / vec.Z) + (_draw.Centre.X / 2)) * _draw.Graphics.Scale;
            vec.Y = ((-vec.Y * 256 / vec.Z) + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale;

            pointList[i] = vec;
        }

        for (int i = 0; i < Faces.Length; i++)
        {
            int point0 = face_data[i].Points[0];
            int point1 = face_data[i].Points[1];
            int point2 = face_data[i].Points.Length > 2 ? face_data[i].Points[2] : 0;

            if (((pointList[point0].X - pointList[point1].X) *
                (pointList[point2].Y - pointList[point1].Y)) <= ((pointList[point0].Y - pointList[point1].Y) *
                    (pointList[point2].X - pointList[point1].X)))
            {
                int num_points = face_data[i].Points.Length;
                Vector2[] poly_list = new Vector2[num_points];

                float zavg = 0;

                for (int j = 0; j < num_points; j++)
                {
                    poly_list[j].X = pointList[face_data[i].Points[j]].X;
                    poly_list[j].Y = pointList[face_data[i].Points[j]].Y;
                    zavg = MathF.Max(zavg, pointList[face_data[i].Points[j]].Z);
                }

                _draw.DrawPolygonFilled(poly_list, face_data[i].Color, zavg);
            }
        }

        if (Flags.HasFlag(ShipProperties.Firing))
        {
            lasv = LaserFront;
            FastColor color = (Type == ShipType.Viper) ? EliteColors.Cyan : EliteColors.White;

            Vector2[] laserPoints =
            [
                new(pointList[lasv].X, pointList[lasv].Y),
                new(Location.X > 0 ? 0 : 511, RNG.Random(256) * 2),
            ];

            _draw.DrawPolygonFilled(laserPoints, color, pointList[lasv].Z);
        }
    }
}
