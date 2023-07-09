// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Trader;
using EliteSharp.Views;

namespace EliteSharp.Ships
{
    internal class ShipBase : IShip
    {
        private readonly IDraw _draw;

        internal ShipBase(IDraw draw) => _draw = draw;

        public int Acceleration { get; set; }

        public float Bounty { get; protected set; }

        public int Bravery { get; set; }

        public ShipClass Class { get; protected set; }

        public int Energy { get; set; }

        public int EnergyMax { get; protected set; }

        public int ExpDelta { get; set; }

        public ShipFaceNormal[] FaceNormals { get; protected set; } = Array.Empty<ShipFaceNormal>();

        public ShipFace[] Faces { get; protected set; } = Array.Empty<ShipFace>();

        public ShipFlags Flags { get; set; } = ShipFlags.None;

        public int LaserFront { get; protected set; }

        public int LaserStrength { get; protected set; }

        public ShipLine[] Lines { get; protected set; } = Array.Empty<ShipLine>();

        public Vector3 Location { get; set; }

        public int LootMax { get; protected set; }

        public float MinDistance { get; protected set; }

        public int Missiles { get; set; }

        public int MissilesMax { get; protected set; }

        public string Name { get; protected set; } = string.Empty;

        public ShipPoint[] Points { get; protected set; } = Array.Empty<ShipPoint>();

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public StockType ScoopedType { get; protected set; }

        public float Size { get; protected set; }

        public IShip? Target { get; set; }

        public ShipType Type { get; protected set; }

        public int VanishPoint { get; protected set; }

        public float Velocity { get; set; }

        public float VelocityMax { get; protected set; }

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

            //for (i = 0; i < num_faces; i++)
            //{
            //  vec.x = face_data[i].norm_x;
            //  vec.y = face_data[i].norm_y;
            //  vec.z = face_data[i].norm_z;

            //  vec = VectorMaths.unit_vector (&vec);
            //  cos_angle = VectorMaths.vector_dot_product (&vec, &camera_vec);

            //  visible[i] = (cos_angle < -0.13);
            //}
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

                if ((((pointList[point0].X - pointList[point1].X) *
                     (pointList[point2].Y - pointList[point1].Y)) -
                     ((pointList[point0].Y - pointList[point1].Y) *
                     (pointList[point2].X - pointList[point1].X))) <= 0)
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

                    _draw.DrawPolygonFilled(poly_list, face_data[i].Colour, zavg);
                }
            }

            if (Flags.HasFlag(ShipFlags.Firing))
            {
                lasv = LaserFront;
                Colour colour = (Type == ShipType.Viper) ? Colour.Cyan : Colour.White;

                Vector2[] laserPoints = new Vector2[]
                {
                    new Vector2(pointList[lasv].X, pointList[lasv].Y),
                    new(Location.X > 0 ? 0 : 511, RNG.Random(256) * 2),
                };

                _draw.DrawPolygonFilled(laserPoints, colour, pointList[lasv].Z);
            }
        }

        public virtual IShip Clone() => new ShipBase(_draw)
        {
            Bounty = Bounty,
            EnergyMax = EnergyMax,
            FaceNormals = FaceNormals,
            Faces = Faces,
            LaserFront = LaserFront,
            LaserStrength = LaserStrength,
            Lines = Lines,
            LootMax = LootMax,
            MissilesMax = MissilesMax,
            Name = Name,
            Points = Points,
            ScoopedType = ScoopedType,
            Size = Size,
            Class = Class,
            VanishPoint = VanishPoint,
            VelocityMax = VelocityMax,
            ExpDelta = ExpDelta,
            Flags = Flags,
            Type = Type,
            Location = Location.Cloner(),
            Rotmat = Rotmat.Cloner(),
            RotX = RotX,
            RotZ = RotZ,
            Energy = Energy,
            Velocity = Velocity,
            Acceleration = Acceleration,
            Missiles = Missiles,
            Target = Target,
            Bravery = Bravery,
            MinDistance = MinDistance,
        };
    }
}
