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
    using System.Numerics;
	using Elite.Engine.Enums;
	using Elite.Engine.Types;

	internal static class threed
	{
        private const int LAND_X_MAX = 128;
        private const int LAND_Y_MAX = 128;
        private static int[,] landscape = new int[LAND_X_MAX + 1, LAND_Y_MAX + 1];
        private static Vector3[] point_list = new Vector3[100];

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
			ship_data ship = elite.ship_list[(int)univ.type];

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

                vec.X = ((vec.X * 256 / vec.Z) + 128) * gfx.GFX_SCALE;
				vec.Y = ((-vec.Y * 256 / vec.Z) + 96) * gfx.GFX_SCALE;

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
				lasv = elite.ship_list[(int)univ.type].front_laser;
				col = (univ.type == SHIP.SHIP_VIPER) ? GFX_COL.GFX_COL_CYAN : GFX_COL.GFX_COL_WHITE;

				Vector2[] pointList = new Vector2[]
				{
					new Vector2(point_list[lasv].X, point_list[lasv].Y),
					new(univ.location.X > 0 ? 0 : 511, random.rand255() * 2)
				};

                DrawPolygonFilled(pointList, col, point_list[lasv].Z);
			}
		}

        /*
		 * Colour map used to generate a SNES Elite style planet.
		 * This is a quick hack and needs tidying up.
		 */
        private static int[] snes_planet_colour = new int[]
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

        /*
		 * Generate a landscape map for a SNES Elite style planet.
		 */
        private static void generate_snes_landscape()
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

        /*
		 * Guassian random number generator.
		 * Returns a number between -7 and +8 with Gaussian distribution.
		 */
        private static int grand()
		{
			int i;
			int r;

			r = 0;
			for (i = 0; i < 12; i++)
			{
				r += random.randint() & 15;
			}

			r /= 12;
			r -= 7;

			return r;
		}

        /*
		 * Calculate the midpoint between two given points.
		 */
        private static int calc_midpoint(int sx, int sy, int ex, int ey)
		{
			int a = landscape[sx, sy];
			int b = landscape[ex, ey];

			int n = ((a + b) / 2) + grand();
			if (n < 0)
			{
				n = 0;
			}

			if (n > 255)
			{
				n = 255;
			}

			return n;
		}

        /*
		 * Calculate a square on the midpoint map.
		 */
        private static void midpoint_square(int tx, int ty, int w)
		{
			int d = w / 2;
			int mx = tx + d;
			int my = ty + d;
			int bx = tx + w;
			int by = ty + w;

			landscape[mx, ty] = calc_midpoint(tx, ty, bx, ty);
			landscape[mx, by] = calc_midpoint(tx, by, bx, by);
			landscape[tx, my] = calc_midpoint(tx, ty, tx, by);
			landscape[bx, my] = calc_midpoint(bx, ty, bx, by);
			landscape[mx, my] = calc_midpoint(tx, my, bx, my);

			if (d == 1)
			{
				return;
			}

			midpoint_square(tx, ty, d);
			midpoint_square(mx, ty, d);
			midpoint_square(tx, my, d);
			midpoint_square(mx, my, d);
		}

        /*
		 * Generate a fractal landscape.
		 * Uses midpoint displacement method.
		 */
        private static void generate_fractal_landscape(int rnd_seed)
		{
			int h;
			float dist;
			bool dark;

			int old_seed = random.rand_seed;
			random.rand_seed = rnd_seed;

			int d = LAND_X_MAX / 8;

			for (int y = 0; y <= LAND_Y_MAX; y += d)
			{
				for (int x = 0; x <= LAND_X_MAX; x += d)
				{
					landscape[x, y] = random.randint() & 255;
				}
			}

			for (int y = 0; y < LAND_Y_MAX; y += d)
			{
				for (int x = 0; x < LAND_X_MAX; x += d)
				{
					midpoint_square(x, y, d);
				}
			}

			for (int y = 0; y <= LAND_Y_MAX; y++)
			{
				for (int x = 0; x <= LAND_X_MAX; x++)
				{
					dist = (x * x) + (y * y);
					dark = dist > 10000;
					h = landscape[x, y];
					landscape[x, y] = h > 166
                        ? (int)(dark ? GFX_COL.GFX_COL_GREEN_1 : GFX_COL.GFX_COL_GREEN_2)
                        : (int)(dark ? GFX_COL.GFX_COL_BLUE_2 : GFX_COL.GFX_COL_BLUE_1);
                }
			}

			random.rand_seed = old_seed;
		}

		internal static void generate_landscape(int rnd_seed)
		{
			switch (elite.config.PlanetRenderStyle)
			{
				case PlanetRenderStyle.Wireframe:     /* Wireframe... do nothing for now... */
					break;

				case PlanetRenderStyle.Green:
					/* generate_green_landscape (); */
					break;

				case PlanetRenderStyle.SNES:
					generate_snes_landscape();
					break;

				case PlanetRenderStyle.Fractal:
					generate_fractal_landscape(rnd_seed);
					break;
			}
		}

        /*
		 * Draw a line of the planet with appropriate rotation.
		 */
        private static void render_planet_line(float xo, float yo, float x, float y, float radius, float vx, float vy)
		{
            Vector2 s = new()
            {
                Y = y + yo
            };

            if (s.Y is < (gfx.GFX_VIEW_TY + gfx.GFX_Y_OFFSET) or
                > (gfx.GFX_VIEW_BY + gfx.GFX_Y_OFFSET))
			{
				return;
			}

			s.X = xo - x;
			float ex = xo + x;

			float rx = (-x * vx) - (y * vy);
			float ry = (-x * vy) + (y * vx);
			rx += radius * 65536f;
			ry += radius * 65536f;
			float div = radius * 1024f;  /* radius * 2 * LAND_X_MAX >> 16 */

			//Debug.Assert(rx > 0);
			//Debug.Assert(ry > 0);

			for (; s.X <= ex; s.X++)
			{
				if (s.X is >= (gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET) and <= (gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET))
				{
					int lx = (int)(rx / div);
                    int ly = (int)(ry / div);
                    //TODO: fix colours
                    //GFX_COL colour = (GFX_COL)landscape[lx, ly];
                    GFX_COL colour = lx < 0 || lx > 128 || ly < 0 || ly > 128 ? GFX_COL.GFX_COL_PINK_1 : (GFX_COL)landscape[lx, ly];
                    elite.alg_gfx.DrawPixelFast(s, colour);
                }
				rx += vx;
				ry += vy;
			}
		}


        /*
		 * Draw a solid planet.  Based on Doros circle drawing alogorithm.
		 */
        private static void render_planet(Vector2 centre, float radius, Vector3[] vec)
		{
            centre.X += gfx.GFX_X_OFFSET;
            centre.Y += gfx.GFX_Y_OFFSET;

            float vx = vec[1].X * 65536;
            float vy = vec[1].Y * 65536;

			float s = radius;
            float x = radius;
            float y = 0;

			s -= x + x;
			while (y <= x)
			{
				render_planet_line(centre.X, centre.Y, x, y, radius, vx, vy);
				render_planet_line(centre.X, centre.Y, x, -y, radius, vx, vy);
				render_planet_line(centre.X, centre.Y, y, x, radius, vx, vy);
				render_planet_line(centre.X, centre.Y, y, -x, radius, vx, vy);

				s += y + y + 1;
				y++;
				if (s >= 0)
				{
					s -= x + x + 2;
					x--;
				}
			}
		}

        /*
		 * Draw a wireframe planet.
		 * At the moment we just draw a circle.
		 * Need to add in the two arcs that the original Elite had.
		 */
        private static void draw_wireframe_planet(Vector2 centre, float radius, Vector3[] vec)
		{
            elite.alg_gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_WHITE);
		}

        /*
		 * Draw a planet.
		 * We can currently do three different types of planet...
		 * - Wireframe.
		 * - Fractal landscape.
		 * - SNES Elite style.
		 */
        private static void draw_planet(ref univ_object planet)
		{
            Vector2 position = new()
            {
                X = planet.location.X * 256 / planet.location.Z,
                Y = planet.location.Y * 256 / planet.location.Z
            };

            position.Y = -position.Y;

            position.X += 128;
            position.Y += 96;

            position.X *= gfx.GFX_SCALE;
            position.Y *= gfx.GFX_SCALE;

			float radius = 6291456f / planet.location.Length();
			//	radius = 6291456 / ship_vec.z;   /* Planets are BIG! */

			radius *= gfx.GFX_SCALE;

			if ((position.X + radius < 0) ||
				(position.X - radius > 511) ||
				(position.Y + radius < 0) ||
				(position.Y - radius > 383))
            {
                return;
            }

			switch (elite.config.PlanetRenderStyle)
			{
				case PlanetRenderStyle.Wireframe:
					draw_wireframe_planet(position, radius, planet.rotmat);
					break;

				case PlanetRenderStyle.Green:
					elite.alg_gfx.DrawCircleFilled(position, radius, GFX_COL.GFX_COL_GREEN_1);
					break;

				case PlanetRenderStyle.SNES:
				case PlanetRenderStyle.Fractal:
					render_planet(position, radius, planet.rotmat);
					break;
			}
		}

        private static void render_sun_line(float xo, float yo, float x, float y, float radius)
		{
            Vector2 s = new()
            {
                Y = yo + y
            };

			if (s.Y is < (gfx.GFX_VIEW_TY + gfx.GFX_Y_OFFSET) or
                > (gfx.GFX_VIEW_BY + gfx.GFX_Y_OFFSET))
			{
				return;
			}

			s.X = xo - x;
			float ex = xo + x;

			s.X -= radius * (2f + (random.randint() & 7)) / 256f;
			ex += radius * (2f + (random.randint() & 7)) / 256f;

			if ((s.X > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET) ||
				(ex < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET))
			{
				return;
			}

			if (s.X < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET)
			{
				s.X = gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET;
			}

			if (ex > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET)
			{
				ex = gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET;
			}

			float inner = radius * (200 + (random.randint() & 7)) / 256f;
			inner *= inner;

			float inner2 = radius * (220 + (random.randint() & 7)) / 256f;
            inner2 *= inner2;

			float outer = radius * (239 + (random.randint() & 7)) / 256f;
            outer *= outer;

			float dy = y * y;
			float dx = s.X - xo;

			for (; s.X <= ex; s.X++, dx++)
			{
				float distance = (dx * dx) + dy;

				GFX_COL colour = distance < inner
                    ? GFX_COL.GFX_COL_WHITE
                    : distance < inner2
						? GFX_COL.GFX_COL_YELLOW_4
						: distance < outer
							? GFX_COL.GFX_COL_ORANGE_3
							: MathF.Pow(s.X, y).IsOdd() ? GFX_COL.GFX_COL_ORANGE_1 : GFX_COL.GFX_COL_ORANGE_2;

                elite.alg_gfx.DrawPixelFast(s, colour);
			}
		}

        private static void render_sun(float xo, float yo, float radius)
		{
			xo += gfx.GFX_X_OFFSET;
			yo += gfx.GFX_Y_OFFSET;

			float s = -radius;
            float x = radius;
            float y = 0f;

			// s -= x + x;
			while (y <= x)
			{
				render_sun_line(xo, yo, x, y, radius);
				render_sun_line(xo, yo, x, -y, radius);
				render_sun_line(xo, yo, y, x, radius);
				render_sun_line(xo, yo, y, -x, radius);

				s += y + y + 1;
				y++;
				if (s >= 0)
				{
					s -= x + x + 2;
					x--;
				}
			}
		}

        private static void draw_sun(ref univ_object planet)
		{
			float x = planet.location.X * 256f / planet.location.Z;
            float y = planet.location.Y * 256f / planet.location.Z;

			y = -y;

			x += 128;
			y += 96;

			x *= gfx.GFX_SCALE;
			y *= gfx.GFX_SCALE;

			float radius = 6291456f / planet.location.Length();

			radius *= gfx.GFX_SCALE;

			if ((x + radius < 0) ||
				(x - radius > 511) ||
				(y + radius < 0) ||
				(y - radius > 383))
            {
                return;
            }

            render_sun(x, y, radius);
		}

        private static void draw_explosion(ref univ_object univ)
		{
			int i;
			int q;
			int pr;
			int cnt;
			Vector3[] trans_mat = new Vector3[3];
			float rx, ry, rz;
			bool[] visible = new bool[32];
			Vector3 camera_vec;
			float cos_angle;
			float tmp;
			ship_face_normal[] ship_norm;
			ship_point[] sp;
			ship_data ship;
			int np;
			int old_seed;

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

            ship = elite.ship_list[(int)univ.type];

			for (i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

            camera_vec = VectorMaths.mult_vector(univ.location, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);

			ship_norm = ship.normals;

			for (i = 0; i < ship.normals.Length; i++)
			{
				Vector3 vec = VectorMaths.unit_vector(ship_norm[i].direction);
				cos_angle = VectorMaths.vector_dot_product(vec, camera_vec);

				visible[i] = cos_angle < -0.13;
			}

			tmp = trans_mat[0].Y;
			trans_mat[0].Y = trans_mat[1].X;
			trans_mat[1].X = tmp;

			tmp = trans_mat[0].Z;
			trans_mat[0].Z = trans_mat[2].X;
			trans_mat[2].X = tmp;

			tmp = trans_mat[1].Z;
			trans_mat[1].Z = trans_mat[2].Y;
			trans_mat[2].Y = tmp;

			sp = ship.points;
			np = 0;

			for (i = 0; i < ship.points.Length; i++)
			{
				if (visible[sp[i].face1] || visible[sp[i].face2] ||
					visible[sp[i].face3] || visible[sp[i].face4])
				{
					Vector3 vec = VectorMaths.mult_vector(sp[i].point, trans_mat);

					rx = vec.X + univ.location.X;
					ry = vec.Y + univ.location.Y;
					rz = vec.Z + univ.location.Z;

					float sx = rx * 256f / rz;
					float sy = ry * 256f / rz;

					sy = -sy;

					sx += 128f;
					sy += 96f;

					sx *= gfx.GFX_SCALE;
					sy *= gfx.GFX_SCALE;

					point_list[np].X = sx;
					point_list[np].Y = sy;
					np++;
				}
			}

			float z = univ.location.Z;

			q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;

            pr = univ.exp_delta * 256 / q;

			//	if (pr > 0x1C00)
			//		q = 254;
			//	else

			q = pr / 32;

			old_seed = random.rand_seed;
			random.rand_seed = univ.exp_seed;

			for (cnt = 0; cnt < np; cnt++)
			{
				float sx = point_list[cnt].X;
				float sy = point_list[cnt].Y;

				for (i = 0; i < 16; i++)
				{
					Vector2 position = new(random.rand255() - 128, random.rand255() - 128);

                    position.X = position.X * q / 256;
                    position.Y = position.Y * q / 256;

                    position.X = position.X + position.X + sx;
                    position.Y = position.Y + position.Y + sy;

					int sizex = (random.randint() & 1) + 1;
					int sizey = (random.randint() & 1) + 1;

					for (int psy = 0; psy < sizey; psy++)
					{
						for (int psx = 0; psx < sizex; psx++)
						{
							//TODO: Bug - the X or Y could be negative
                            //Debug.Assert(position.X >= 0);
                            //Debug.Assert(position.Y >= 0);

                            elite.alg_gfx.DrawPixel(new(position.X + psx, position.Y + psy), GFX_COL.GFX_COL_WHITE);
						}
					}
				}
			}

			random.rand_seed = old_seed;
		}

		/// <summary>
		/// Draws an object in the universe. (Ship, Planet, Sun etc).
		/// </summary>
		/// <param name="ship"></param>
		internal static void DrawObject(ref univ_object ship)
		{
			if (elite.current_screen is not SCR.SCR_FRONT_VIEW and not SCR.SCR_REAR_VIEW and
                not SCR.SCR_LEFT_VIEW and not SCR.SCR_RIGHT_VIEW and
                not SCR.SCR_INTRO_ONE and not SCR.SCR_INTRO_TWO and
                not SCR.SCR_GAME_OVER and not SCR.SCR_ESCAPE_POD)
			{
				return;
			}

			if (ship.flags.HasFlag(FLG.FLG_DEAD) && !ship.flags.HasFlag(FLG.FLG_EXPLOSION))
			{
				ship.flags |= FLG.FLG_EXPLOSION;
				ship.exp_seed = random.randint();
				ship.exp_delta = 18;
			}

			if (ship.flags.HasFlag(FLG.FLG_EXPLOSION))
			{
				draw_explosion(ref ship);
				return;
			}

			if (ship.location.Z <= 0)   /* Only display ships in front of us. */
            {
                return;
            }

            if (ship.type == SHIP.SHIP_PLANET)
			{
				draw_planet(ref ship);
				return;
			}

			if (ship.type == SHIP.SHIP_SUN)
			{
				draw_sun(ref ship);
				return;
			}

			if ((MathF.Abs(ship.location.X) > ship.location.Z) ||    /* Check for field of vision. */
				(MathF.Abs(ship.location.Y) > ship.location.Z))
            {
                return;
            }

            DrawShip(ref ship);
		}

        private struct poly_data
        {
            internal float Z;
            internal GFX_COL face_colour;
            internal Vector2[] point_list;
            internal int next;
        };

        private const int MAX_POLYS = 100;
        private static int total_polys;
        private static int start_poly;
        private static readonly poly_data[] poly_chain = new poly_data[MAX_POLYS];

        internal static void RenderStart()
        {
            start_poly = 0;
            total_polys = 0;
        }

        internal static void RenderFinish()
        {
            if (total_polys == 0)
            {
                return;
            }

            for (int i = start_poly; i != -1; i = poly_chain[i].next)
            {
                GFX_COL colour = elite.config.UseWireframe ? GFX_COL.GFX_COL_WHITE : poly_chain[i].face_colour;

                if (poly_chain[i].point_list.Length == 2)
                {
                    elite.alg_gfx.DrawLine(poly_chain[i].point_list[0], poly_chain[i].point_list[1], colour);
                    continue;
                }

				if (elite.config.UseWireframe)
				{
					elite.alg_gfx.DrawPolygon(poly_chain[i].point_list, colour);
				}
				else
				{
                    elite.alg_gfx.DrawPolygonFilled(poly_chain[i].point_list, colour);
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