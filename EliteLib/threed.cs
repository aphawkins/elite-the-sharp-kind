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

namespace Elite
{
	using System.Diagnostics;
	using System.Numerics;
	using Elite.Enums;
	using Elite.Structs;

	internal static class threed
	{
		const int LAND_X_MAX = 128;
		const int LAND_Y_MAX = 128;

		static int[,] landscape = new int[LAND_X_MAX + 1, LAND_Y_MAX + 1];

		static point[] point_list = new point[100];

		/*
		 * The following routine is used to draw a wireframe represtation of a ship.
		 *
		 * caveat: it is a work in progress.
		 * A number of features (such as not showing detail at distance) have not yet been implemented.
		 *
		 */
		static void draw_wireframe_ship(ref univ_object univ)
		{
			Vector3[] trans_mat = new Vector3[3];
			int i;
			int sx, sy, ex, ey;
			float rx, ry, rz;
			bool[] visible = new bool[32];
			Vector3 vec;
			Vector3 camera_vec;
			float cos_angle;
			float tmp;
			ship_face_normal[] ship_norm;
			int num_faces;
			ship_data ship;
			int lasv;

			ship = elite.ship_list[(int)univ.type];

			for (i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

			camera_vec = univ.location;
			VectorMaths.mult_vector(ref camera_vec, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);

			num_faces = ship.num_faces;

			for (i = 0; i < num_faces; i++)
			{
				ship_norm = ship.normals;

				vec.X = ship_norm[i].x;
				vec.Y = ship_norm[i].y;
				vec.Z = ship_norm[i].z;

				if ((vec.X == 0) && (vec.Y == 0) && (vec.Z == 0))
				{
					visible[i] = true;
				}
				else
				{
					vec = VectorMaths.unit_vector(vec);
					cos_angle = VectorMaths.vector_dot_product(vec, camera_vec);
					visible[i] = cos_angle < -0.2;
				}
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

			for (i = 0; i < ship.num_points; i++)
			{
				vec.X = ship.points[i].x;
				vec.Y = ship.points[i].y;
				vec.Z = ship.points[i].z;

				VectorMaths.mult_vector(ref vec, trans_mat);

				rx = vec.X + univ.location.X;
				ry = vec.Y + univ.location.Y;
				rz = vec.Z + univ.location.Z;

				sx = (int)(rx * 256 / rz);
				sy = (int)(ry * 256 / rz);

				sy = -sy;

				sx += 128;
				sy += 96;

				sx *= gfx.GFX_SCALE;
				sy *= gfx.GFX_SCALE;

				point_list[i].X = sx;
				point_list[i].Y = sy;

			}

			for (i = 0; i < ship.num_lines; i++)
			{
				if (visible[ship.lines[i].face1] ||
					visible[ship.lines[i].face2])
				{
					sx = point_list[ship.lines[i].start_point].X;
					sy = point_list[ship.lines[i].start_point].Y;

					ex = point_list[ship.lines[i].end_point].X;
					ey = point_list[ship.lines[i].end_point].Y;

                    elite.alg_gfx.DrawLine(sx, sy, ex, ey);
				}
			}


			if (univ.flags.HasFlag(FLG.FLG_FIRING))
			{
				lasv = elite.ship_list[(int)univ.type].front_laser;
                elite.alg_gfx.DrawLine(point_list[lasv].X, point_list[lasv].Y, univ.location.X > 0 ? 0 : 511, random.rand255() * 2);
			}
		}

		/*
		 * Hacked version of the draw ship routine to display solid ships...
		 * This needs a lot of tidying...
		 *
		 * Check for hidden surface supplied by T.Harte.
		 */
		static void draw_solid_ship(ref univ_object univ)
		{
			int sx, sy;
			float rx, ry, rz;
			Vector3 vec;
			Vector3 camera_vec;
			int num_points;
			int zavg;
			Vector3[] trans_mat = new Vector3[3];
			int lasv;
            GFX_COL col;

			ship_solid solid_data = shipface.ship_solids[(int)univ.type];
			ship_data ship = elite.ship_list[(int)univ.type];

			for (int i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

			camera_vec = univ.location;
			VectorMaths.mult_vector(ref camera_vec, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);

			int num_faces = solid_data.num_faces;
			ship_face[] face_data = solid_data.face_data;

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

			float tmp = trans_mat[0].Y;
			trans_mat[0].Y = trans_mat[1].X;
			trans_mat[1].X = tmp;

			tmp = trans_mat[0].Z;
			trans_mat[0].Z = trans_mat[2].X;
			trans_mat[2].X = tmp;

			tmp = trans_mat[1].Z;
			trans_mat[1].Z = trans_mat[2].Y;
			trans_mat[2].Y = tmp;


			for (int i = 0; i < ship.num_points; i++)
			{
				vec.X = ship.points[i].x;
				vec.Y = ship.points[i].y;
				vec.Z = ship.points[i].z;

				VectorMaths.mult_vector(ref vec, trans_mat);

				rx = vec.X + univ.location.X;
				ry = vec.Y + univ.location.Y;
				rz = vec.Z + univ.location.Z;

				if (rz <= 0)
				{
					rz = 1;
				}

				sx = (int)(rx * 256 / rz);
				sy = (int)(ry * 256 / rz);

				sy = -sy;

				sx += 128;
				sy += 96;

				sx *= gfx.GFX_SCALE;
				sy *= gfx.GFX_SCALE;

				point_list[i].X = sx;
				point_list[i].Y = sy;
				point_list[i].Z = (int)rz;
			}

			for (int i = 0; i < num_faces; i++)
			{
				if ((((point_list[face_data[i].p1].X - point_list[face_data[i].p2].X) *
					 (point_list[face_data[i].p3].Y - point_list[face_data[i].p2].Y)) -
					 ((point_list[face_data[i].p1].Y - point_list[face_data[i].p2].Y) *
					 (point_list[face_data[i].p3].X - point_list[face_data[i].p2].X))) <= 0)
				{
					num_points = face_data[i].points;
                    System.Numerics.Vector2[] poly_list = new System.Numerics.Vector2[num_points];

                    poly_list[0].X = point_list[face_data[i].p1].X;
					poly_list[0].Y = point_list[face_data[i].p1].Y;
					zavg = point_list[face_data[i].p1].Z;

					poly_list[1].X = point_list[face_data[i].p2].X;
					poly_list[1].Y = point_list[face_data[i].p2].Y;
					zavg = Math.Max(zavg, point_list[face_data[i].p2].Z);

					if (num_points > 2)
					{
						poly_list[2].X = point_list[face_data[i].p3].X;
						poly_list[2].Y = point_list[face_data[i].p3].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p3].Z);
					}

					if (num_points > 3)
					{
						poly_list[3].X = point_list[face_data[i].p4].X;
						poly_list[3].Y = point_list[face_data[i].p4].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p4].Z);
					}

					if (num_points > 4)
					{
						poly_list[4].X = point_list[face_data[i].p5].X;
						poly_list[4].Y = point_list[face_data[i].p5].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p5].Z);
					}

					if (num_points > 5)
					{
						poly_list[5].X = point_list[face_data[i].p6].X;
						poly_list[5].Y = point_list[face_data[i].p6].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p6].Z);
					}

					if (num_points > 6)
					{
						poly_list[6].X = point_list[face_data[i].p7].X;
						poly_list[6].Y = point_list[face_data[i].p7].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p7].Z);
					}

					if (num_points > 7)
					{
						poly_list[7].X = point_list[face_data[i].p8].X;
						poly_list[7].Y = point_list[face_data[i].p8].Y;
						zavg = Math.Max(zavg, point_list[face_data[i].p8].Z);
					}

                    elite.alg_gfx.DrawPolygon(poly_list, face_data[i].colour, zavg);
				}
			}

			if (univ.flags.HasFlag(FLG.FLG_FIRING))
			{
				lasv = elite.ship_list[(int)univ.type].front_laser;
				col = (univ.type == SHIP.SHIP_VIPER) ? GFX_COL.GFX_COL_CYAN : GFX_COL.GFX_COL_WHITE;

                elite.alg_gfx.DrawLine(point_list[lasv].X, point_list[lasv].Y,
								 univ.location.X > 0 ? 0 : 511, random.rand255() * 2,
								 point_list[lasv].Z, col);
			}
		}

		/*
		 * Colour map used to generate a SNES Elite style planet.
		 * This is a quick hack and needs tidying up.
		 */
		static int[] snes_planet_colour = new int[]
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
		static void generate_snes_landscape()
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
		static int grand()
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
		static int calc_midpoint(int sx, int sy, int ex, int ey)
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
		static void midpoint_square(int tx, int ty, int w)
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
		static void generate_fractal_landscape(int rnd_seed)
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
					if (h > 166)
					{
						landscape[x, y] = (int)(dark ? GFX_COL.GFX_COL_GREEN_1 : GFX_COL.GFX_COL_GREEN_2);
					}
					else
					{
						landscape[x, y] = (int)(dark ? GFX_COL.GFX_COL_BLUE_2 : GFX_COL.GFX_COL_BLUE_1);
					}
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
		static void render_planet_line(float xo, float yo, float x, float y, float radius, float vx, float vy)
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
                    elite.alg_gfx.PlotPixelFast(s, colour);
                }
				rx += vx;
				ry += vy;
			}
		}


		/*
		 * Draw a solid planet.  Based on Doros circle drawing alogorithm.
		 */
		static void render_planet(Vector2 centre, float radius, Vector3[] vec)
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
		static void draw_wireframe_planet(Vector2 centre, float radius, Vector3[] vec)
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
		static void draw_planet(ref univ_object planet)
		{
			Vector2 position = new();
            position.X = planet.location.X * 256 / planet.location.Z;
            position.Y = planet.location.Y * 256 / planet.location.Z;

            position.Y = -position.Y;

            position.X += 128;
            position.Y += 96;

            position.X *= gfx.GFX_SCALE;
            position.Y *= gfx.GFX_SCALE;

			float radius = 6291456f / planet.distance;
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

		static void render_sun_line(float xo, float yo, float x, float y, float radius)
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
							: ((int)MathF.Pow(s.X, y)).IsOdd() ? GFX_COL.GFX_COL_ORANGE_1 : GFX_COL.GFX_COL_ORANGE_2;

                elite.alg_gfx.PlotPixelFast(s, colour);
			}
		}

		static void render_sun(float xo, float yo, float radius)
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

		static void draw_sun(ref univ_object planet)
		{
			float x = planet.location.X * 256f / planet.location.Z;
            float y = planet.location.Y * 256f / planet.location.Z;

			y = -y;

			x += 128;
			y += 96;

			x *= gfx.GFX_SCALE;
			y *= gfx.GFX_SCALE;

			float radius = 6291456f / planet.distance;

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

		static void draw_explosion(ref univ_object univ)
		{
			int i;
			int z;
			int q;
			int pr;
			int cnt;
			Vector3[] trans_mat = new Vector3[3];
			int sx, sy;
			float rx, ry, rz;
			bool[] visible = new bool[32];
			Vector3 vec;
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
				return;

			ship = elite.ship_list[(int)univ.type];

			for (i = 0; i < 3; i++)
			{
				trans_mat[i] = univ.rotmat[i];
			}

			camera_vec = univ.location;
			VectorMaths.mult_vector(ref camera_vec, trans_mat);
			camera_vec = VectorMaths.unit_vector(camera_vec);

			ship_norm = ship.normals;

			for (i = 0; i < ship.num_faces; i++)
			{
				vec.X = ship_norm[i].x;
				vec.Y = ship_norm[i].y;
				vec.Z = ship_norm[i].z;

				vec = VectorMaths.unit_vector(vec);
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

			for (i = 0; i < ship.num_points; i++)
			{
				if (visible[sp[i].face1] || visible[sp[i].face2] ||
					visible[sp[i].face3] || visible[sp[i].face4])
				{
					vec.X = sp[i].x;
					vec.Y = sp[i].y;
					vec.Z = sp[i].z;

					VectorMaths.mult_vector(ref vec, trans_mat);

					rx = vec.X + univ.location.X;
					ry = vec.Y + univ.location.Y;
					rz = vec.Z + univ.location.Z;

					sx = (int)(rx * 256 / rz);
					sy = (int)(ry * 256 / rz);

					sy = -sy;

					sx += 128;
					sy += 96;

					sx *= gfx.GFX_SCALE;
					sy *= gfx.GFX_SCALE;

					point_list[np].X = sx;
					point_list[np].Y = sy;
					np++;
				}
			}


			z = (int)univ.location.Z;

			if (z >= 0x2000)
				q = 254;
			else
				q = (z / 32) | 1;

			pr = univ.exp_delta * 256 / q;

			//	if (pr > 0x1C00)
			//		q = 254;
			//	else

			q = pr / 32;

			old_seed = random.rand_seed;
			random.rand_seed = univ.exp_seed;

			for (cnt = 0; cnt < np; cnt++)
			{
				sx = point_list[cnt].X;
				sy = point_list[cnt].Y;

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

                            elite.alg_gfx.PlotPixel(new(position.X + psx, position.Y + psy), GFX_COL.GFX_COL_WHITE);
						}
					}
				}
			}

			random.rand_seed = old_seed;
		}

		/*
		 * Draws an object in the universe.
		 * (Ship, Planet, Sun etc).
		 */
		internal static void draw_ship(ref univ_object ship)
		{
			if ((elite.current_screen != SCR.SCR_FRONT_VIEW) && (elite.current_screen != SCR.SCR_REAR_VIEW) &&
				(elite.current_screen != SCR.SCR_LEFT_VIEW) && (elite.current_screen != SCR.SCR_RIGHT_VIEW) &&
				(elite.current_screen != SCR.SCR_INTRO_ONE) && (elite.current_screen != SCR.SCR_INTRO_TWO) &&
				(elite.current_screen != SCR.SCR_GAME_OVER) && (elite.current_screen != SCR.SCR_ESCAPE_POD))
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

			if ((Math.Abs(ship.location.X) > ship.location.Z) ||    /* Check for field of vision. */
				(Math.Abs(ship.location.Y) > ship.location.Z))
            {
                return;
            }

            if (elite.config.UseWireframe)
			{
				draw_wireframe_ship(ref ship);
			}
			else
			{
				draw_solid_ship(ref ship);
			}
		}
	}
}