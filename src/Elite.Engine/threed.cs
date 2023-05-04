/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 */

namespace Elite.Engine
{
    using System;
    using System.Numerics;
    using Elite.Engine.Enums;
    using Elite.Engine.Types;
    using Elite.Engine.Views;

    internal partial class Threed
	{
        private const int LAND_X_MAX = 128;
        private const int LAND_Y_MAX = 128;
        private static readonly int[,] landscape = new int[LAND_X_MAX + 1, LAND_Y_MAX + 1];
        private static readonly Vector3[] point_list = new Vector3[100];
		private readonly IGfx _gfx;
        private readonly Draw _draw;

        internal Threed(IGfx gfx, Draw draw)
		{
			_gfx = gfx;
			_draw = draw;
        }

        /// <summary>
        /// Hacked version of the draw ship routine to display ships...
        /// This needs a lot of tidying...
        /// caveat: it is a work in progress.
        /// A number of features(such as not showing detail at distance) have not yet been implemented.
        /// Check for hidden surface supplied by T.Harte.
        /// </summary>
        /// <param name="univ"></param>
        private static void DrawShip(ref univ_object univ)
		{
			Vector3[] trans_mat = new Vector3[3];
			int lasv;
            GFX_COL col;
			ShipData ship = EliteMain.ship_list[(int)univ.type];

			for (int i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

            Vector3 camera_vec = VectorMaths.mult_vector(univ.location, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);
			ship_face[] face_data = ship.face_data;

            /*
				for (i = 0; i < num_faces; i++)
				{
					vec.x = face_data[i].norm_x;
					vec.y = face_data[i].norm_y;
					vec.z = face_data[i].norm_z;

					vec = VectorMaths.unit_vector (&vec);
					cos_angle = VectorMaths.vector_dot_product (&vec, &camera_vec);

					visible[i] = (cos_angle < -0.13);
				}
			*/

            (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
            (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
            (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);

            for (int i = 0; i < ship.points.Length; i++)
			{
				Vector3 vec = VectorMaths.mult_vector(ship.points[i].point, trans_mat);
                vec += univ.location;

				if (vec.Z <= 0)
				{
                    vec.Z = 1;
				}

                vec.X = ((vec.X * 256 / vec.Z) + 128) * Graphics.GFX_SCALE;
				vec.Y = ((-vec.Y * 256 / vec.Z) + 96) * Graphics.GFX_SCALE;

				point_list[i] = vec;
			}

			for (int i = 0; i < ship.face_data.Length; i++)
			{
				int point0 = face_data[i].points[0];
                int point1 = face_data[i].points[1];
				int point2 = face_data[i].points.Length > 2 ? face_data[i].points[2] : 0;

                if ((((point_list[point0].X - point_list[point1].X) *
					 (point_list[point2].Y - point_list[point1].Y)) -
					 ((point_list[point0].Y - point_list[point1].Y) *
					 (point_list[point2].X - point_list[point1].X))) <= 0)
				{
					int num_points = face_data[i].points.Length;
                    Vector2[] poly_list = new Vector2[num_points];

 					float zavg = 0;

                    for (int j = 0; j < num_points; j++)
					{
                        poly_list[j].X = point_list[face_data[i].points[j]].X;
                        poly_list[j].Y = point_list[face_data[i].points[j]].Y;
                        zavg = MathF.Max(zavg, point_list[face_data[i].points[j]].Z);
                    }

                    DrawPolygonFilled(poly_list, face_data[i].colour, zavg);
				}
			}

			if (univ.flags.HasFlag(FLG.FLG_FIRING))
			{
				lasv = EliteMain.ship_list[(int)univ.type].front_laser;
				col = (univ.type == ShipType.Viper) ? GFX_COL.GFX_COL_CYAN : GFX_COL.GFX_COL_WHITE;

				Vector2[] pointList = new Vector2[]
				{
					new Vector2(point_list[lasv].X, point_list[lasv].Y),
					new(univ.location.X > 0 ? 0 : 511, RNG.Random(255) * 2)
				};

                DrawPolygonFilled(pointList, col, point_list[lasv].Z);
			}
		}

        /*
		 * Colour map used to generate a SNES Elite style planet.
		 * This is a quick hack and needs tidying up.
		 */
        private static readonly int[] snes_planet_colour = new int[]
		{
			102, 102,
			134, 134, 134, 134,
			167, 167, 167, 167,
			213, 213,
			255,
			83,83,83,83,
			122,
			83,83,
			249,249,249,249,
			83,
			122,
			249,249,249,249,249,249,
			83, 83,
			122,
			83,83, 83, 83,
			255,
			213, 213,
			167,167, 167, 167,
			134,134, 134, 134,
			102, 102
		};

		/// <summary>
		/// Generate a landscape map for a SNES Elite style planet.
		/// </summary>
        private static void GenerateSnesLandscape()
		{
			for (int y = 0; y <= LAND_Y_MAX; y++)
			{
				int colour = snes_planet_colour[y * (snes_planet_colour.Length - 1) / LAND_Y_MAX];
				for (int x = 0; x <= LAND_X_MAX; x++)
				{
					landscape[x, y] = colour;
				}
			}
		}

		/// <summary>
		/// Calculate the midpoint between two given points.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		/// <param name="ex"></param>
		/// <param name="ey"></param>
		/// <returns></returns>
        private static int CalcMidpoint(int sx, int sy, int ex, int ey) =>
			Math.Clamp(((landscape[sx, sy] + landscape[ex, ey]) / 2) + RNG.GaussianRandom(-7, 8), 0, 255);

        /// <summary>
        /// Calculate a square on the midpoint map.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="w"></param>
        private static void MidpointSquare(int tx, int ty, int w)
		{
			int d = w / 2;
			int mx = tx + d;
			int my = ty + d;
			int bx = tx + w;
			int by = ty + w;

			landscape[mx, ty] = CalcMidpoint(tx, ty, bx, ty);
			landscape[mx, by] = CalcMidpoint(tx, by, bx, by);
			landscape[tx, my] = CalcMidpoint(tx, ty, tx, by);
			landscape[bx, my] = CalcMidpoint(bx, ty, bx, by);
			landscape[mx, my] = CalcMidpoint(tx, my, bx, my);

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
		/// Generate a fractal landscape. Uses midpoint displacement method.
		/// </summary>
		/// <param name="seed">Initial seed for the generation.</param>
        private static void GenerateFractalLandscape(int seed)
		{
			int d = LAND_X_MAX / 8;
			Random random = new(seed);

			for (int y = 0; y <= LAND_Y_MAX; y += d)
			{
				for (int x = 0; x <= LAND_X_MAX; x += d)
				{
					landscape[x, y] = random.Next(255);
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
					int h = landscape[x, y];
					landscape[x, y] = h > 166
                        ? (int)(dark ? GFX_COL.GFX_COL_GREEN_1 : GFX_COL.GFX_COL_GREEN_2)
                        : (int)(dark ? GFX_COL.GFX_COL_BLUE_2 : GFX_COL.GFX_COL_BLUE_1);
                }
			}
		}

		internal static void GenerateLandscape(int rnd_seed)
		{
			switch (EliteMain.config.PlanetRenderStyle)
			{
				case PlanetRenderStyle.Wireframe:     /* Wireframe... do nothing for now... */
					break;

				case PlanetRenderStyle.Green:
					/* generate_green_landscape (); */
					break;

				case PlanetRenderStyle.SNES:
					GenerateSnesLandscape();
					break;

				case PlanetRenderStyle.Fractal:
					GenerateFractalLandscape(rnd_seed);
					break;
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
                Y = y + centre.Y
            };

            if (s.Y is < (Graphics.GFX_VIEW_TY + Graphics.GFX_Y_OFFSET) or
                > (Graphics.GFX_VIEW_BY + Graphics.GFX_Y_OFFSET))
			{
				return;
			}

			s.X = centre.X - x;
			float ex = centre.X + x;

			float rx = (-x * vx) - (y * vy);
			float ry = (-x * vy) + (y * vx);
			rx += radius * 65536;
			ry += radius * 65536;
			float div = radius * 1024;  /* radius * 2 * LAND_X_MAX >> 16 */

			for (; s.X <= ex; s.X++)
			{
				if (s.X is >= (Graphics.GFX_VIEW_TX + Graphics.GFX_X_OFFSET) and <= (Graphics.GFX_VIEW_BX + Graphics.GFX_X_OFFSET))
				{
					int lx = (int)Math.Clamp(MathF.Abs(rx / div), 0, LAND_X_MAX);
					int ly = (int)Math.Clamp(MathF.Abs(ry / div), 0, LAND_Y_MAX);
                    GFX_COL colour = (GFX_COL)landscape[lx, ly];
                    _gfx.DrawPixelFast(s, colour);
                }
				rx += vx;
				ry += vy;
			}
		}

		/// <summary>
		/// Draw a solid planet. Based on Doros circle drawing alogorithm.
		/// </summary>
		/// <param name="centre"></param>
		/// <param name="radius"></param>
		/// <param name="vec"></param>
        private void RenderPlanet(Vector2 centre, float radius, Vector3[] vec)
		{
            centre.X += Graphics.GFX_X_OFFSET;
            centre.Y += Graphics.GFX_Y_OFFSET;

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
		/// Draw a wireframe planet. 
		/// </summary>
		/// <param name="centre"></param>
		/// <param name="radius"></param>
        private void DrawWireframePlanet(Vector2 centre, float radius)
		{
            // TODO: At the moment we just draw a circle. Need to add in the two arcs that the original Elite had.
            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_WHITE);
		}

		/// <summary>
		/// Draw a planet.
		/// We can currently do three different types of planet: Wireframe, Fractal landscape or SNES Elite style
		/// </summary>
		/// <param name="planet"></param>
        private void DrawPlanet(ref univ_object planet)
		{
            Vector2 position = new()
            {
                X = planet.location.X * 256 / planet.location.Z,
                Y = planet.location.Y * 256 / planet.location.Z
            };

            position.Y = -position.Y;

            position.X += 128;
            position.Y += 96;

            position.X *= Graphics.GFX_SCALE;
            position.Y *= Graphics.GFX_SCALE;

			float radius = 6291456 / planet.location.Length();
			//	radius = 6291456 / ship_vec.z;   /* Planets are BIG! */

			radius *= Graphics.GFX_SCALE;

			if ((position.X + radius < 0) ||
				(position.X - radius > 511) ||
				(position.Y + radius < 0) ||
				(position.Y - radius > 383))
            {
                return;
            }

			switch (EliteMain.config.PlanetRenderStyle)
			{
				case PlanetRenderStyle.Wireframe:
					DrawWireframePlanet(position, radius);
					break;

				case PlanetRenderStyle.Green:
					_gfx.DrawCircleFilled(position, radius, GFX_COL.GFX_COL_GREEN_1);
					break;

				case PlanetRenderStyle.SNES:
				case PlanetRenderStyle.Fractal:
					RenderPlanet(position, radius, planet.rotmat);
					break;
			}
		}

        private void DrawExplosion(ref univ_object univ)
		{
			Vector3[] trans_mat = new Vector3[3];
			bool[] visible = new bool[32];

			if (univ.exp_delta > 251)
			{
				univ.flags |= FLG.FLG_REMOVE;
				return;
			}

			univ.exp_delta += 4;

			if (univ.location.Z <= 0)
            {
                return;
            }

            ShipData ship = EliteMain.ship_list[(int)univ.type];

			for (int i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

            Vector3 camera_vec = VectorMaths.mult_vector(univ.location, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);

            ship_face_normal[] ship_norm = ship.normals;

			for (int i = 0; i < ship.normals.Length; i++)
			{
				Vector3 vec = VectorMaths.unit_vector(ship_norm[i].direction);
				float cos_angle = VectorMaths.vector_dot_product(vec, camera_vec);
				visible[i] = cos_angle < -0.13;
			}

            (trans_mat[1].X, trans_mat[0].Y) = (trans_mat[0].Y, trans_mat[1].X);
            (trans_mat[2].X, trans_mat[0].Z) = (trans_mat[0].Z, trans_mat[2].X);
            (trans_mat[2].Y, trans_mat[1].Z) = (trans_mat[1].Z, trans_mat[2].Y);
			int np = 0;

			for (int i = 0; i < ship.points.Length; i++)
			{
				if (visible[ship.points[i].face1] || visible[ship.points[i].face2] ||
					visible[ship.points[i].face3] || visible[ship.points[i].face4])
				{
					Vector3 vec = VectorMaths.mult_vector(ship.points[i].point, trans_mat);
					Vector3 r = vec + univ.location;

					float sx = r.X * 256f / r.Z;
					float sy = r.Y * 256f / r.Z;

					sy = -sy;

					sx += 128;
					sy += 96;

					sx *= Graphics.GFX_SCALE;
					sy *= Graphics.GFX_SCALE;

					point_list[np].X = sx;
					point_list[np].Y = sy;
					np++;
				}
			}

			float z = univ.location.Z;
			float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
            float pr = univ.exp_delta * 256 / q;

			//	if (pr > 0x1C00)
			//		q = 254;
			//	else

			q = pr / 32;

			for (int cnt = 0; cnt < np; cnt++)
			{
				float sx = point_list[cnt].X;
				float sy = point_list[cnt].Y;

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

                            _gfx.DrawPixel(new(position.X + psx, position.Y + psy), GFX_COL.GFX_COL_WHITE);
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws an object in the universe. (Ship, Planet, Sun etc).
		/// </summary>
		/// <param name="ship"></param>
		internal void DrawObject(univ_object ship)
		{
			//Debug.Assert(elite._state.currentScreen is SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
			//	SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW or
			//	SCR.SCR_INTRO_ONE or SCR.SCR_INTRO_TWO or
			//	SCR.SCR_GAME_OVER or SCR.SCR_ESCAPE_POD or
			//	SCR.SCR_MISSION_1);

			if (ship.flags.HasFlag(FLG.FLG_DEAD) && !ship.flags.HasFlag(FLG.FLG_EXPLOSION))
			{
				ship.flags |= FLG.FLG_EXPLOSION;
				ship.exp_delta = 18;
			}

			if (ship.flags.HasFlag(FLG.FLG_EXPLOSION))
			{
				DrawExplosion(ref ship);
				return;
			}

			if (ship.location.Z <= 0)   /* Only display ships in front of us. */
            {
                return;
            }

            if (ship.type == ShipType.Planet)
			{
				DrawPlanet(ref ship);
				return;
			}

			if (ship.type == ShipType.Sun)
			{
				_draw.DrawSun(ship);
				return;
			}

			if ((MathF.Abs(ship.location.X) > ship.location.Z) ||    /* Check for field of vision. */
				(MathF.Abs(ship.location.Y) > ship.location.Z))
            {
                return;
            }

            DrawShip(ref ship);
		}

        private const int MAX_POLYS = 100;
        private static int total_polys;
        private static int start_poly;
        private static readonly PolygonData[] poly_chain = new PolygonData[MAX_POLYS];

        internal static void RenderStart()
        {
            start_poly = 0;
            total_polys = 0;
        }

        internal void RenderEnd()
        {
            if (total_polys == 0)
            {
                return;
            }

            for (int i = start_poly; i != -1; i = poly_chain[i].next)
            {
                GFX_COL colour = EliteMain.config.UseWireframe ? GFX_COL.GFX_COL_WHITE : poly_chain[i].face_colour;

                if (poly_chain[i].point_list.Length == 2)
                {
                    _gfx.DrawLine(poly_chain[i].point_list[0], poly_chain[i].point_list[1], colour);
                    continue;
                }

				if (EliteMain.config.UseWireframe)
				{
					_gfx.DrawPolygon(poly_chain[i].point_list, colour);
				}
				else
				{
                    _gfx.DrawPolygonFilled(poly_chain[i].point_list, colour);
                }
            };
        }

        private static void DrawPolygonFilled(Vector2[] point_list, GFX_COL face_colour, float zAvg)
		{
            int i;

			if (total_polys == MAX_POLYS)
			{
				return;
			}

			int x = total_polys;
			total_polys++;

			poly_chain[x].face_colour = face_colour;
			poly_chain[x].Z = zAvg;
			poly_chain[x].next = -1;
			poly_chain[x].point_list = new Vector2[point_list.Length];

			for (i = 0; i < point_list.Length; i++)
			{
				poly_chain[x].point_list[i].X = point_list[i].X;
				poly_chain[x].point_list[i].Y = point_list[i].Y;
			}

			if (x == 0)
			{
				return;
			}

			if (zAvg > poly_chain[start_poly].Z)
			{
				poly_chain[x].next = start_poly;
				start_poly = x;
				return;
			}

            for (i = start_poly; poly_chain[i].next != -1; i = poly_chain[i].next)
			{
				int nx = poly_chain[i].next;

				if (zAvg > poly_chain[nx].Z)
				{
					poly_chain[i].next = x;
					poly_chain[x].next = nx;
					return;
				}
			}

			poly_chain[i].next = x;
		}
	}
}