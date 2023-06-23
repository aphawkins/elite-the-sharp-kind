// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Ships;
using EliteSharp.Views;

namespace EliteSharp
{
    internal sealed class Threed
    {
        private const int MAXPOLYS = 100;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly Vector3[] _pointList = new Vector3[100];
        private readonly PolygonData[] _polyChain = new PolygonData[MAXPOLYS];
        private int _startPoly;
        private int _totalPolys;

        internal Threed(GameState gameState, IGraphics graphics, Draw draw)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
        }

        /// <summary>
        /// Draws an object in the universe. (Ship, Planet, Sun etc).
        /// </summary>
        internal void DrawObject(IShip ship, IPlanetRenderer? planetRenderer = null)
        {
            //Debug.Assert(elite._state.currentScreen is SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
            //  SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW or
            //  SCR.SCR_INTRO_ONE or SCR.SCR_INTRO_TWO or
            //  SCR.SCR_GAME_OVER or SCR.SCR_ESCAPE_CAPSULE or
            //  SCR.SCR_MISSION_1);
            if (ship.Flags.HasFlag(ShipFlags.Dead) && !ship.Flags.HasFlag(ShipFlags.Explosion))
            {
                ship.Flags |= ShipFlags.Explosion;
                ship.ExpDelta = 18;
            }

            if (ship.Flags.HasFlag(ShipFlags.Explosion))
            {
                DrawExplosion(ship);
                return;
            }

            // Only display ships in front of us.
            if (ship.Location.Z <= 0)
            {
                return;
            }

            if (ship.Type == ShipType.Planet && planetRenderer != null)
            {
                DrawPlanet(ship, planetRenderer);
                return;
            }

            if (ship.Type == ShipType.Sun)
            {
                _draw.DrawSun(ship);
                return;
            }

            // Check for field of vision.
            if ((MathF.Abs(ship.Location.X) > ship.Location.Z) ||
                (MathF.Abs(ship.Location.Y) > ship.Location.Z))
            {
                return;
            }

            DrawShip(ship);
        }

        internal void RenderEnd()
        {
            if (_totalPolys == 0)
            {
                return;
            }

            for (int i = _startPoly; i != -1; i = _polyChain[i].Next)
            {
                Colour colour = _gameState.Config.UseWireframe ? Colour.White : _polyChain[i].FaceColour;

                if (_polyChain[i].PointList.Length == 2)
                {
                    _graphics.DrawLine(_polyChain[i].PointList[0], _polyChain[i].PointList[1], colour);
                    continue;
                }

                if (_gameState.Config.UseWireframe)
                {
                    _graphics.DrawPolygon(_polyChain[i].PointList, colour);
                }
                else
                {
                    _graphics.DrawPolygonFilled(_polyChain[i].PointList, colour);
                }
            }
        }

        internal void RenderStart()
        {
            _startPoly = 0;
            _totalPolys = 0;
        }

        private void DrawExplosion(IShip ship)
        {
            Vector3[] trans_mat = new Vector3[3];
            bool[] visible = new bool[32];

            if (ship.ExpDelta > 251)
            {
                ship.Flags |= ShipFlags.Remove;
                return;
            }

            ship.ExpDelta += 4;

            if (ship.Location.Z <= 0)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                trans_mat[i] = ship.Rotmat[i];
            }

            Vector3 camera_vec = VectorMaths.MultiplyVector(ship.Location, trans_mat);
            camera_vec = VectorMaths.UnitVector(camera_vec);

            ShipFaceNormal[] ship_norm = ship.FaceNormals;

            for (int i = 0; i < ship.FaceNormals.Length; i++)
            {
                Vector3 vec = VectorMaths.UnitVector(ship_norm[i].Direction);
                float cos_angle = VectorMaths.VectorDotProduct(vec, camera_vec);
                visible[i] = cos_angle < -0.13;
            }

            (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
            (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
            (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);
            int np = 0;

            for (int i = 0; i < ship.Points.Length; i++)
            {
                if (visible[ship.Points[i].Face1] || visible[ship.Points[i].Face2] ||
                    visible[ship.Points[i].Face3] || visible[ship.Points[i].Face4])
                {
                    Vector3 vec = VectorMaths.MultiplyVector(ship.Points[i].Point, trans_mat);
                    Vector3 r = vec + ship.Location;

                    float sx = r.X * 256f / r.Z;
                    float sy = r.Y * 256f / r.Z;

                    sy = -sy;

                    sx += 128;
                    sy += 96;

                    sx *= _graphics.Scale;
                    sy *= _graphics.Scale;

                    _pointList[np].X = sx;
                    _pointList[np].Y = sy;
                    np++;
                }
            }

            float z = ship.Location.Z;
            float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
            float pr = ship.ExpDelta * 256 / q;

            //  if (pr > 0x1C00)
            //      q = 254;
            //  else
            q = pr / 32;

            for (int cnt = 0; cnt < np; cnt++)
            {
                float sx = _pointList[cnt].X;
                float sy = _pointList[cnt].Y;

                for (int i = 0; i < 16; i++)
                {
                    Vector2 position = new(RNG.Random(-128, 128), RNG.Random(-128, 128));

                    position.X = position.X * q / 256;
                    position.Y = position.Y * q / 256;

                    position.X = position.X + position.X + sx;
                    position.Y = position.Y + position.Y + sy;

                    int sizex = RNG.Random(1, 3);
                    int sizey = RNG.Random(1, 3);

                    for (int psy = 0; psy < sizey; psy++)
                    {
                        for (int psx = 0; psx < sizex; psx++)
                        {
                            _graphics.DrawPixel(new(position.X + psx, position.Y + psy), Colour.White);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw a planet.
        /// We can currently do three different types of planet: Wireframe, Fractal landscape or SNES Elite style.
        /// </summary>
        private void DrawPlanet(IShip planet, IPlanetRenderer planetRenderer)
        {
            Vector2 position = new()
            {
                X = planet.Location.X * 256 / planet.Location.Z,
                Y = planet.Location.Y * 256 / planet.Location.Z,
            };

            position.Y = -position.Y;

            position.X += 128;
            position.Y += 96;

            position.X *= _graphics.Scale;
            position.Y *= _graphics.Scale;

            float radius = 6291456 / planet.Location.Length();

            // Planets are BIG!
            //  radius = 6291456 / ship_vec.z;
            radius *= _graphics.Scale;

            if ((position.X + radius < 0) ||
                (position.X - radius > 511) ||
                (position.Y + radius < 0) ||
                (position.Y - radius > 383))
            {
                return;
            }

            planetRenderer.Draw(position, radius, planet.Rotmat);
        }

        private void DrawPolygonFilled(Vector2[] point_list, Colour face_colour, float zAvg)
        {
            int i;

            if (_totalPolys == MAXPOLYS)
            {
                return;
            }

            int x = _totalPolys;
            _totalPolys++;

            _polyChain[x].FaceColour = face_colour;
            _polyChain[x].Z = zAvg;
            _polyChain[x].Next = -1;
            _polyChain[x].PointList = new Vector2[point_list.Length];

            for (i = 0; i < point_list.Length; i++)
            {
                _polyChain[x].PointList[i].X = point_list[i].X;
                _polyChain[x].PointList[i].Y = point_list[i].Y;
            }

            if (x == 0)
            {
                return;
            }

            if (zAvg > _polyChain[_startPoly].Z)
            {
                _polyChain[x].Next = _startPoly;
                _startPoly = x;
                return;
            }

            for (i = _startPoly; _polyChain[i].Next != -1; i = _polyChain[i].Next)
            {
                int nx = _polyChain[i].Next;

                if (zAvg > _polyChain[nx].Z)
                {
                    _polyChain[i].Next = x;
                    _polyChain[x].Next = nx;
                    return;
                }
            }

            _polyChain[i].Next = x;
        }

        /// <summary>
        /// Hacked version of the draw ship routine to display ships...
        /// This needs a lot of tidying...
        /// caveat: it is a work in progress.
        /// A number of features(such as not showing detail at distance) have not yet been implemented.
        /// Check for hidden surface supplied by T.Harte.
        /// </summary>
        private void DrawShip(IShip ship)
        {
            Vector3[] trans_mat = new Vector3[3];
            int lasv;
            Colour col;

            for (int i = 0; i < 3; i++)
            {
                trans_mat[i] = ship.Rotmat[i];
            }

            Vector3 camera_vec = VectorMaths.MultiplyVector(ship.Location, trans_mat);
            _ = VectorMaths.UnitVector(camera_vec);
            ShipFace[] face_data = ship.Faces;

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

            for (int i = 0; i < ship.Points.Length; i++)
            {
                Vector3 vec = VectorMaths.MultiplyVector(ship.Points[i].Point, trans_mat);
                vec += ship.Location;

                if (vec.Z <= 0)
                {
                    vec.Z = 1;
                }

                vec.X = ((vec.X * 256 / vec.Z) + (_draw.Centre.X / 2)) * _graphics.Scale;
                vec.Y = ((-vec.Y * 256 / vec.Z) + (_draw.Centre.Y / 2)) * _graphics.Scale;

                _pointList[i] = vec;
            }

            for (int i = 0; i < ship.Faces.Length; i++)
            {
                int point0 = face_data[i].Points[0];
                int point1 = face_data[i].Points[1];
                int point2 = face_data[i].Points.Length > 2 ? face_data[i].Points[2] : 0;

                if ((((_pointList[point0].X - _pointList[point1].X) *
                     (_pointList[point2].Y - _pointList[point1].Y)) -
                     ((_pointList[point0].Y - _pointList[point1].Y) *
                     (_pointList[point2].X - _pointList[point1].X))) <= 0)
                {
                    int num_points = face_data[i].Points.Length;
                    Vector2[] poly_list = new Vector2[num_points];

                    float zavg = 0;

                    for (int j = 0; j < num_points; j++)
                    {
                        poly_list[j].X = _pointList[face_data[i].Points[j]].X;
                        poly_list[j].Y = _pointList[face_data[i].Points[j]].Y;
                        zavg = MathF.Max(zavg, _pointList[face_data[i].Points[j]].Z);
                    }

                    DrawPolygonFilled(poly_list, face_data[i].Colour, zavg);
                }
            }

            if (ship.Flags.HasFlag(ShipFlags.Firing))
            {
                lasv = ship.LaserFront;
                col = (ship.Type == ShipType.Viper) ? Colour.Cyan : Colour.White;

                Vector2[] pointList = new Vector2[]
                {
                    new Vector2(_pointList[lasv].X, _pointList[lasv].Y),
                    new(ship.Location.X > 0 ? 0 : 511, RNG.Random(256) * 2),
                };

                DrawPolygonFilled(pointList, col, _pointList[lasv].Z);
            }
        }
    }
}
