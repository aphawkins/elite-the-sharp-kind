/**
 *
 * Elite - The New Kind.
 *
 * Allegro version of Graphics routines.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999-2001.
 * email: <christian@newkind.co.uk>
 *
 * Routines for drawing anti-aliased lines and circles by T.Harte.
 *
 **/

namespace Elite
{
	using System.Diagnostics;
	using System.Drawing;
	using Elite.Enums;
	using Elite.Structs;

	public interface IGfx
	{
		int gfx_graphics_startup();
		void gfx_graphics_shutdown();

		/// <summary>
		/// Blit the back buffer to the screen.
		/// </summary>
		void gfx_update_screen();

		void gfx_acquire_screen();

		void gfx_release_screen();

		void gfx_fast_plot_pixel(int x, int y, GFX_COL col);

		void gfx_plot_pixel(int x, int y, GFX_COL col);

		void gfx_draw_filled_circle(int cx, int cy, int radius, GFX_COL circle_colour);

		void gfx_draw_circle(int cx, int cy, int radius, GFX_COL circle_colour);

		void gfx_draw_line(int x1, int y1, int x2, int y2);

		void gfx_draw_colour_line(int x1, int y1, int x2, int y2, GFX_COL line_colour);

		void gfx_draw_colour_line_xor(int x1, int y1, int x2, int y2, GFX_COL line_colour);

		void gfx_draw_triangle(int x1, int y1, int x2, int y2, int x3, int y3, GFX_COL col);

		void gfx_display_text(int x, int y, string txt);

		void gfx_display_colour_text(int x, int y, string txt, GFX_COL col);

		void gfx_display_centre_text(int y, string str, int psize, GFX_COL col);

		void gfx_clear_display();

		void gfx_clear_text_area();

		void gfx_clear_area(int tx, int ty, int bx, int by);

		void gfx_draw_rectangle(int tx, int ty, int bx, int by, GFX_COL col);

		void gfx_display_pretty_text(int tx, int ty, int bx, int by, string txt);

		void gfx_draw_scanner();

		void gfx_set_clip_region(int tx, int ty, int bx, int by);

		void gfx_start_render();

		void gfx_render_polygon(point[] point_list, GFX_COL face_colour, int zavg);

		void gfx_render_line(int x1, int y1, int x2, int y2, int dist, GFX_COL col);

		void gfx_finish_render();

		void gfx_draw_sprite(IMG sprite_no, int x, int y);

		bool gfx_request_file(string title, string path, string ext);
	}
}