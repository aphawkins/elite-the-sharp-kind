// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;
using Elite.Engine.Views;

namespace Elite.Engine
{
    internal sealed class Threed
    {
        private const int LAND_X_MAX = 128;
        private const int LAND_Y_MAX = 128;
        private const int MAX_POLYS = 100;

        /// <summary>
        /// Colour map used to generate a SNES Elite style planet.
        /// </summary>
        private static readonly int[] s_snes_planet_colour = new int[]
        {
            // TODO: This is a quick hack and needs tidying up.
            102, 102,
            134, 134, 134, 134,
            167, 167, 167, 167,
            213, 213,
            255,
            83, 83, 83, 83,
            122,
            83, 83,
            249, 249, 249, 249,
            83,
            122,
            249, 249, 249, 249, 249, 249,
            83, 83,
            122,
            83, 83, 83, 83,
            255,
            213, 213,
            167, 167, 167, 167,
            134, 134, 134, 134,
            102, 102,
        };

        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly int[,] _landscape = new int[LAND_X_MAX + 1, LAND_Y_MAX + 1];
        private readonly Vector3[] _pointList = new Vector3[100];
        private readonly PolygonData[] _polyChain = new PolygonData[MAX_POLYS];

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
        /// <param name="ship"></param>
        internal void DrawObject(UniverseObject ship)
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
                DrawExplosion(ref ship);
                return;
            }

            // Only display ships in front of us.
            if (ship.Location.Z <= 0)
            {
                return;
            }

            if (ship.Type == ShipType.Planet)
            {
                DrawPlanet(ref ship);
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

            DrawShip(ref ship);
        }

        internal void GenerateLandscape(int rnd_seed)
        {
            switch (_gameState.Config.PlanetRenderStyle)
            {
                // Wireframe... do nothing for now...
                case PlanetRenderStyle.Wireframe:
                    break;

                case PlanetRenderStyle.Green:
                    // generate_green_landscape ();
                    break;

                case PlanetRenderStyle.SNES:
                    GenerateSnesLandscape();
                    break;

                case PlanetRenderStyle.Fractal:
                    GenerateFractalLandscape(rnd_seed);
                    break;

                default:
                    break;
            }
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

        /// <summary>
        /// Calculate the midpoint between two given points.
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="ex"></param>
        /// <param name="ey"></param>
        /// <returns></returns>
        private int CalcMidpoint(int sx, int sy, int ex, int ey) =>
            Math.Clamp(((_landscape[sx, sy] + _landscape[ex, ey]) / 2) + RNG.GaussianRandom(-7, 8), 0, 255);

        private void DrawExplosion(ref UniverseObject univ)
        {
            Vector3[] trans_mat = new Vector3[3];
            bool[] visible = new bool[32];

            if (univ.ExpDelta > 251)
            {
                univ.Flags |= ShipFlags.Remove;
                return;
            }

            univ.ExpDelta += 4;

            if (univ.Location.Z <= 0)
            {
                return;
            }

            IShip ship = _gameState.ShipList[univ.Type];

            for (int i = 0; i < 3; i++)
            {
                trans_mat[i] = univ.Rotmat[i];
            }

            Vector3 camera_vec = VectorMaths.MultiplyVector(univ.Location, trans_mat);
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
                    Vector3 r = vec + univ.Location;

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

            float z = univ.Location.Z;
            float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
            float pr = univ.ExpDelta * 256 / q;

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
                    Vector2 position = new(RNG.Random(-128, 127), RNG.Random(-128, 127));

                    position.X = position.X * q / 256;
                    position.Y = position.Y * q / 256;

                    position.X = position.X + position.X + sx;
                    position.Y = position.Y + position.Y + sy;

                    int sizex = RNG.Random(1, 2);
                    int sizey = RNG.Random(1, 2);

                    for (int psy = 0; psy < sizey; psy++)
                    {
                        for (int psx = 0; psx < sizex; psx++)
                        {
                            //TODO: Bug - the X or Y could be negative
                            //Debug.Assert(position.X >= 0);
                            //Debug.Assert(position.Y >= 0);
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
        /// <param name="planet"></param>
        private void DrawPlanet(ref UniverseObject planet)
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

            switch (_gameState.Config.PlanetRenderStyle)
            {
                case PlanetRenderStyle.Wireframe:
                    DrawWireframePlanet(position, radius);
                    break;

                case PlanetRenderStyle.Green:
                    _graphics.DrawCircleFilled(position, radius, Colour.Green);
                    break;

                case PlanetRenderStyle.SNES:
                case PlanetRenderStyle.Fractal:
                    RenderPlanet(position, radius, planet.Rotmat);
                    break;

                default:
                    break;
            }
        }

        private void DrawPolygonFilled(Vector2[] point_list, Colour face_colour, float zAvg)
        {
            int i;

            if (_totalPolys == MAX_POLYS)
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
        /// <param name="univ"></param>
        private void DrawShip(ref UniverseObject univ)
        {
            Vector3[] trans_mat = new Vector3[3];
            int lasv;
            Colour col;
            IShip ship = _gameState.ShipList[univ.Type];

            for (int i = 0; i < 3; i++)
            {
                trans_mat[i] = univ.Rotmat[i];
            }

            Vector3 camera_vec = VectorMaths.MultiplyVector(univ.Location, trans_mat);
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
                vec += univ.Location;

                if (vec.Z <= 0)
                {
                    vec.Z = 1;
                }

                vec.X = ((vec.X * 256 / vec.Z) + 128) * _graphics.Scale;
                vec.Y = ((-vec.Y * 256 / vec.Z) + 96) * _graphics.Scale;

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

            if (univ.Flags.HasFlag(ShipFlags.Firing))
            {
                lasv = _gameState.ShipList[univ.Type].LaserFront;
                col = (univ.Type == ShipType.Viper) ? Colour.Cyan : Colour.White;

                Vector2[] pointList = new Vector2[]
                {
                    new Vector2(_pointList[lasv].X, _pointList[lasv].Y),
                    new(univ.Location.X > 0 ? 0 : 511, RNG.Random(255) * 2),
                };

                DrawPolygonFilled(pointList, col, _pointList[lasv].Z);
            }
        }

        /// <summary>
        /// Draw a wireframe planet.
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="radius"></param>
        private void DrawWireframePlanet(Vector2 centre, float radius) =>

            // TODO: At the moment we just draw a circle. Need to add in the two arcs that the original Elite had.
            _graphics.DrawCircle(centre, radius, Colour.White);

        /// <summary>
        /// Generate a fractal landscape. Uses midpoint displacement method.
        /// </summary>
        /// <param name="seed">Initial seed for the generation.</param>
        [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Randomness here requires seed.")]
        private void GenerateFractalLandscape(int seed)
        {
            const int d = LAND_X_MAX / 8;
            Random random = new(seed);

            for (int y = 0; y <= LAND_Y_MAX; y += d)
            {
                for (int x = 0; x <= LAND_X_MAX; x += d)
                {
                    _landscape[x, y] = random.Next(255);
                }
            }

            for (int y = 0; y < LAND_Y_MAX; y += d)
            {
                for (int x = 0; x < LAND_X_MAX; x += d)
                {
                    MidpointSquare(x, y, d);
                }
            }

            for (int y = 0; y <= LAND_Y_MAX; y++)
            {
                for (int x = 0; x <= LAND_X_MAX; x++)
                {
                    float dist = (x * x) + (y * y);
                    bool dark = dist > 10000;
                    int h = _landscape[x, y];
                    _landscape[x, y] = h > 166
                        ? (int)(dark ? Colour.Green : Colour.LightGreen)
                        : (int)(dark ? Colour.Blue : Colour.LightBlue);
                }
            }
        }

        /// <summary>
        /// Generate a landscape map for a SNES Elite style planet.
        /// </summary>
        private void GenerateSnesLandscape()
        {
            for (int y = 0; y <= LAND_Y_MAX; y++)
            {
                int colour = s_snes_planet_colour[y * (s_snes_planet_colour.Length - 1) / LAND_Y_MAX];
                for (int x = 0; x <= LAND_X_MAX; x++)
                {
                    _landscape[x, y] = colour;
                }
            }
        }

        /// <summary>
        /// Calculate a square on the midpoint map.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="w"></param>
        private void MidpointSquare(int tx, int ty, int w)
        {
            int d = w / 2;
            int mx = tx + d;
            int my = ty + d;
            int bx = tx + w;
            int by = ty + w;

            _landscape[mx, ty] = CalcMidpoint(tx, ty, bx, ty);
            _landscape[mx, by] = CalcMidpoint(tx, by, bx, by);
            _landscape[tx, my] = CalcMidpoint(tx, ty, tx, by);
            _landscape[bx, my] = CalcMidpoint(bx, ty, bx, by);
            _landscape[mx, my] = CalcMidpoint(tx, my, bx, my);

            if (d == 1)
            {
                return;
            }

            MidpointSquare(tx, ty, d);
            MidpointSquare(mx, ty, d);
            MidpointSquare(tx, my, d);
            MidpointSquare(mx, my, d);
        }

        /// <summary>
        /// Draw a solid planet. Based on Doros circle drawing alogorithm.
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="radius"></param>
        /// <param name="vec"></param>
        private void RenderPlanet(Vector2 centre, float radius, Vector3[] vec)
        {
            centre.X += _graphics.Offset.X;
            centre.Y += _graphics.Offset.Y;

            float vx = vec[1].X * 65536;
            float vy = vec[1].Y * 65536;

            float s = radius;
            float x = radius;
            float y = 0;

            s -= x + x;
            while (y <= x)
            {
                // Top of top half
                RenderPlanetLine(centre, y, -MathF.Floor(x), radius, vx, vy);

                // Bottom of top half
                RenderPlanetLine(centre, x, -y, radius, vx, vy);

                // Top of bottom half
                RenderPlanetLine(centre, x, y, radius, vx, vy);

                // Bottom of bottom half
                RenderPlanetLine(centre, y, MathF.Floor(x), radius, vx, vy);

                s += y + y + 1;
                y++;
                if (s >= 0)
                {
                    s -= x + x + 2;
                    x--;
                }
            }
        }

        /// <summary>
        /// Draw a line of the planet with appropriate rotation.
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        private void RenderPlanetLine(Vector2 centre, float x, float y, float radius, float vx, float vy)
        {
            Vector2 s = new()
            {
                Y = y + centre.Y,
            };

            if (s.Y < _graphics.ViewT.Y + _graphics.Offset.Y ||
                s.Y > _graphics.ViewB.Y + _graphics.Offset.Y)
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            float rx = (-x * vx) - (y * vy);
            float ry = (-x * vy) + (y * vx);
            rx += radius * 65536;
            ry += radius * 65536;

            // radius * 2 * LAND_X_MAX >> 16
            float div = radius * 1024;

            for (; s.X <= ex; s.X++)
            {
                if (s.X >= _graphics.ViewT.X + _graphics.Offset.X && s.X <= _graphics.ViewB.X + _graphics.Offset.X)
                {
                    int lx = (int)Math.Clamp(MathF.Abs(rx / div), 0, LAND_X_MAX);
                    int ly = (int)Math.Clamp(MathF.Abs(ry / div), 0, LAND_Y_MAX);
                    Colour colour = (Colour)_landscape[lx, ly];
                    _graphics.DrawPixelFast(s, colour);
                }

                rx += vx;
                ry += vy;
            }
        }
    }
}
