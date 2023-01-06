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
			Vector[] trans_mat = new Vector[3];
			int i;
			int sx, sy, ex, ey;
			float rx, ry, rz;
			bool[] visible = new bool[32];
			Vector vec;
			Vector camera_vec;
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

				sx = (int)((rx * 256) / rz);
				sy = (int)((ry * 256) / rz);

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
			Vector vec;
			Vector camera_vec;
			int num_points;
			int zavg;
			Vector[] trans_mat = new Vector[3];
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

				sx = (int)((rx * 256) / rz);
				sy = (int)((ry * 256) / rz);

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
				if (((point_list[face_data[i].p1].X - point_list[face_data[i].p2].X) *
					 (point_list[face_data[i].p3].Y - point_list[face_data[i].p2].Y) -
					 (point_list[face_data[i].p1].Y - point_list[face_data[i].p2].Y) *
					 (point_list[face_data[i].p3].X - point_list[face_data[i].p2].X)) <= 0)
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
            int colour;

			for (int y = 0; y <= LAND_Y_MAX; y++)
			{
				colour = snes_planet_colour[y * snes_planet_colour.Length / LAND_Y_MAX];
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

			int old_seed = random.get_rand_seed();
			random.set_rand_seed(rnd_seed);

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
					dist = x * x + y * y;
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

			random.set_rand_seed(old_seed);
		}

		internal static void generate_landscape(int rnd_seed)
		{
			switch (elite.planet_render_style)
			{
				case 0:     /* Wireframe... do nothing for now... */
					break;

				case 1:
					/* generate_green_landscape (); */
					break;

				case 2:
					generate_snes_landscape();
					break;

				case 3:
					generate_fractal_landscape(rnd_seed);
					break;
			}
		}

		/*
		 * Draw a line of the planet with appropriate rotation.
		 */
		static void render_planet_line(int xo, int yo, int x, int y, int radius, int vx, int vy)
		{
			int lx, ly;
			int rx, ry;
            GFX_COL colour;
			int sx, sy;
			int ex;
			int div;

			sy = y + yo;

			if ((sy < gfx.GFX_VIEW_TY + gfx.GFX_Y_OFFSET) ||
				(sy > gfx.GFX_VIEW_BY + gfx.GFX_Y_OFFSET))
			{
				return;
			}

			sx = xo - x;
			ex = xo + x;

			rx = -x * vx - y * vy;
			ry = -x * vy + y * vx;
			rx += radius << 16;
			ry += radius << 16;
			div = radius << 10;  /* radius * 2 * LAND_X_MAX >> 16 */


			for (; sx <= ex; sx++)
			{
				if ((sx >= (gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET)) && (sx <= (gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET)))
				{
					lx = rx / div;
					ly = ry / div;
					colour = (GFX_COL)landscape[lx, ly];

                    elite.alg_gfx.PlotPixelFast(sx, sy, colour);
				}
				rx += vx;
				ry += vy;
			}
		}


		/*
		 * Draw a solid planet.  Based on Doros circle drawing alogorithm.
		 */
		static void render_planet(int xo, int yo, int radius, Vector[] vec)
		{
			int x, y;
			int s;
			int vx, vy;

			xo += gfx.GFX_X_OFFSET;
			yo += gfx.GFX_Y_OFFSET;

			vx = (int)(vec[1].X * 65536);
			vy = (int)(vec[1].Y * 65536);

			s = radius;
			x = radius;
			y = 0;

			s -= x + x;
			while (y <= x)
			{
				render_planet_line(xo, yo, x, y, radius, vx, vy);
				render_planet_line(xo, yo, x, -y, radius, vx, vy);
				render_planet_line(xo, yo, y, x, radius, vx, vy);
				render_planet_line(xo, yo, y, -x, radius, vx, vy);

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
		static void draw_wireframe_planet(int xo, int yo, int radius, Vector[] vec)
		{
            elite.alg_gfx.DrawCircle(xo, yo, radius, GFX_COL.GFX_COL_WHITE);
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
			int x, y;
			int radius;

			x = (int)((planet.location.X * 256) / planet.location.Z);
			y = (int)((planet.location.Y * 256) / planet.location.Z);

			y = -y;

			x += 128;
			y += 96;

			x *= gfx.GFX_SCALE;
			y *= gfx.GFX_SCALE;

			radius = 6291456 / planet.distance;
			//	radius = 6291456 / ship_vec.z;   /* Planets are BIG! */

			radius *= gfx.GFX_SCALE;

			if ((x + radius < 0) ||
				(x - radius > 511) ||
				(y + radius < 0) ||
				(y - radius > 383))
				return;

			switch (elite.planet_render_style)
			{
				case 0:
					draw_wireframe_planet(x, y, radius, planet.rotmat);
					break;

				case 1:
                    elite.alg_gfx.DrawCircleFilled(x, y, radius, GFX_COL.GFX_COL_GREEN_1);
					break;

				case 2:
				case 3:
					render_planet(x, y, radius, planet.rotmat);
					break;
			}
		}

		static void render_sun_line(int xo, int yo, int x, int y, int radius)
		{
			int sy = yo + y;
			int sx, ex;
            GFX_COL colour;
			int dx, dy;
			int distance;
			int inner, outer;
			int inner2;
			bool mix;

			if ((sy < gfx.GFX_VIEW_TY + gfx.GFX_Y_OFFSET) ||
				(sy > gfx.GFX_VIEW_BY + gfx.GFX_Y_OFFSET))
			{
				return;
			}

			sx = xo - x;
			ex = xo + x;

			sx -= (radius * (2 + (random.randint() & 7))) >> 8;
			ex += (radius * (2 + (random.randint() & 7))) >> 8;

			if ((sx > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET) ||
				(ex < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET))
			{
				return;
			}

			if (sx < gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET)
			{
				sx = gfx.GFX_VIEW_TX + gfx.GFX_X_OFFSET;
			}

			if (ex > gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET)
			{
				ex = gfx.GFX_VIEW_BX + gfx.GFX_X_OFFSET;
			}

			inner = (radius * (200 + (random.randint() & 7))) >> 8;
			inner *= inner;

			inner2 = (radius * (220 + (random.randint() & 7))) >> 8;
			inner2 *= inner2;

			outer = (radius * (239 + (random.randint() & 7))) >> 8;
			outer *= outer;

			dy = y * y;
			dx = sx - xo;

			for (; sx <= ex; sx++, dx++)
			{
				mix = ((sx ^ y) & 1) == 1;
				distance = dx * dx + dy;

				if (distance < inner)
				{
					colour = GFX_COL.GFX_COL_WHITE;
				}
				else if (distance < inner2)
				{
					colour = GFX_COL.GFX_COL_YELLOW_4;
				}
				else if (distance < outer)
				{
					colour = GFX_COL.GFX_COL_ORANGE_3;
				}
				else
				{
					colour = mix ? GFX_COL.GFX_COL_ORANGE_1 : GFX_COL.GFX_COL_ORANGE_2;
				}

                elite.alg_gfx.PlotPixelFast(sx, sy, colour);
			}
		}

		static void render_sun(int xo, int yo, int radius)
		{
			int x, y;
			int s;

			xo += gfx.GFX_X_OFFSET;
			yo += gfx.GFX_Y_OFFSET;

			s = -radius;
			x = radius;
			y = 0;

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
			int x, y;
			int radius;

			x = (int)((planet.location.X * 256) / planet.location.Z);
			y = (int)((planet.location.Y * 256) / planet.location.Z);

			y = -y;

			x += 128;
			y += 96;

			x *= gfx.GFX_SCALE;
			y *= gfx.GFX_SCALE;

			radius = 6291456 / planet.distance;

			radius *= gfx.GFX_SCALE;

			if ((x + radius < 0) ||
				(x - radius > 511) ||
				(y + radius < 0) ||
				(y - radius > 383))
				return;

			render_sun(x, y, radius);
		}

		static void draw_explosion(ref univ_object univ)
		{
			int i;
			int z;
			int q;
			int pr;
			int px, py;
			int cnt;
			int sizex, sizey, psx, psy;
			Vector[] trans_mat = new Vector[3];
			int sx, sy;
			float rx, ry, rz;
			bool[] visible = new bool[32];
			Vector vec;
			Vector camera_vec;
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

				visible[i] = (cos_angle < -0.13);
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

					sx = (int)((rx * 256) / rz);
					sy = (int)((ry * 256) / rz);

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

			pr = (univ.exp_delta * 256) / q;

			//	if (pr > 0x1C00)
			//		q = 254;
			//	else

			q = pr / 32;

			old_seed = random.get_rand_seed();
			random.set_rand_seed(univ.exp_seed);

			for (cnt = 0; cnt < np; cnt++)
			{
				sx = point_list[cnt].X;
				sy = point_list[cnt].Y;

				for (i = 0; i < 16; i++)
				{
					px = random.rand255() - 128;
					py = random.rand255() - 128;

					px = (px * q) / 256;
					py = (py * q) / 256;

					px = px + px + sx;
					py = py + py + sy;

					sizex = (random.randint() & 1) + 1;
					sizey = (random.randint() & 1) + 1;

					for (psy = 0; psy < sizey; psy++)
					{
						for (psx = 0; psx < sizex; psx++)
						{
                            elite.alg_gfx.PlotPixel(px + psx, py + psy, GFX_COL.GFX_COL_WHITE);
						}
					}
				}
			}

			random.set_rand_seed(old_seed);
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
				return;

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
				return;

			if (elite.wireframe)
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