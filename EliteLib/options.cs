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
 *
 */

/*
 * Options.c
 */

//# include <stdlib.h>
//# include <string.h>

//# include "elite.h"
//# include "config.h"
//# include "gfx.h"
//# include "options.h"
//# include "main.h"
//# include "docked.h"
//# include "file.h" 

namespace Elite
{
	using Elite.Enums;

	internal static class options
	{

		static int hilite_item;

		const int NUM_OPTIONS = 4;
		const int NUM_SETTINGS = 6;

		const int OPTION_BAR_WIDTH = 400;
		const int OPTION_BAR_HEIGHT = 15;

		struct option
		{
			internal string text;
			internal bool docked_only;

			internal option(string text, bool docked_only)
			{
				this.text = text;
				this.docked_only = docked_only;
			}
		};

		static option[] option_list = new option[NUM_OPTIONS]
		{
			new("Save Commander",   true),
			new("Load Commander",   true),
			new("Game Settings",    false),
			new ("Quit",            false)
		};

		struct setting
		{
			internal string name;
			internal string[] value;

			internal setting(string name, string[] value)
			{
				this.name = name;
				this.value = value;
			}
		};

		static setting[] setting_list = new setting[NUM_SETTINGS]
		{
			new("Graphics:", new string[5] {"Solid", "Wireframe", "", "", ""}),
			new("Anti Alias:", new string[5] {"Off", "On", "", "", ""}),
			new("Planet Style:", new string[5] {"Wireframe", "Green", "SNES", "Fractal", ""}),
			new("Planet Desc.:", new string[5] {"BBC", "MSX", "", "", ""}),
			new("Instant Dock:", new string[5] {"Off", "On", "", "", ""}),
			new("Save Settings", new string[5] {"", "", "", "", ""})
		};

		static void quit_screen()
		{
			elite.current_screen = SCR.SCR_QUIT;

			elite.alg_gfx.ClearDisplay();
			elite.alg_gfx.DrawTextCentre(20, "GAME OPTIONS", 140, GFX_COL.GFX_COL_GOLD);
			elite.alg_gfx.DrawLine(0, 36, 511, 36);

			elite.alg_gfx.DrawTextCentre(175, "QUIT GAME (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
		}

		static void display_setting_item(int item)
		{
			int x, y;
            if (item == (NUM_SETTINGS - 1))
			{
				y = ((NUM_SETTINGS + 1) / 2 * 30) + 96 + 32;
				elite.alg_gfx.DrawTextCentre(y, setting_list[item].name, 120, GFX_COL.GFX_COL_WHITE);
				return;
			}

            var v = item switch
            {
                0 => elite.config.UseWireframe ? 1 : 0,
                1 => elite.config.AntiAliasWireframe ? 1 : 0,
                2 => (int)elite.config.PlanetRenderStyle,
                3 => elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
                4 => elite.config.InstantDock ? 1 : 0,
                _ => 0,
            };
            x = ((item & 1) * 250) + 32;
			y = (item / 2 * 30) + 96;

			elite.alg_gfx.DrawTextLeft(x, y, setting_list[item].name, GFX_COL.GFX_COL_WHITE);
			elite.alg_gfx.DrawTextLeft(x + 120, y, setting_list[item].value[v], GFX_COL.GFX_COL_WHITE);
		}

		static void highlight_setting(int item)
		{
			int x, y;
			int width;

			if ((hilite_item != -1) && (hilite_item != item))
			{
				if (hilite_item == (NUM_SETTINGS - 1))
				{
					x = gfx.GFX_X_CENTRE - (OPTION_BAR_WIDTH / 2);
					y = ((NUM_SETTINGS + 1) / 2 * 30) + 96 + 32;
					width = OPTION_BAR_WIDTH;
				}
				else
				{
					x = ((hilite_item & 1) * 250) + 32 + 120;
					y = (hilite_item / 2 * 30) + 96;
					width = 100;
				}

				elite.alg_gfx.ClearArea(x, y, width, OPTION_BAR_HEIGHT);
				display_setting_item(hilite_item);
			}

			if (item == (NUM_SETTINGS - 1))
			{
				x = gfx.GFX_X_CENTRE - (OPTION_BAR_WIDTH / 2);
				y = ((NUM_SETTINGS + 1) / 2 * 30) + 96 + 32;
				width = OPTION_BAR_WIDTH;
			}
			else
			{
				x = ((item & 1) * 250) + 32 + 120;
				y = (item / 2 * 30) + 96;
				width = 100;
			}

			elite.alg_gfx.DrawRectangleFilled(x, y, width, OPTION_BAR_HEIGHT, GFX_COL.GFX_COL_DARK_RED);
			display_setting_item(item);
			hilite_item = item;
		}

		internal static void select_left_setting()
		{
			if ((hilite_item & 1) != 0)
				highlight_setting(hilite_item - 1);
		}

		internal static void select_right_setting()
		{
			if (((hilite_item & 1) == 0) && (hilite_item < (NUM_SETTINGS - 1)))
			{
				highlight_setting(hilite_item + 1);
			}
		}

		internal static void select_up_setting()
		{
			if (hilite_item == (NUM_SETTINGS - 1))
			{
				highlight_setting(NUM_SETTINGS - 2);
				return;
			}

			if (hilite_item > 1)
			{
				highlight_setting(hilite_item - 2);
			}
		}

		internal static void select_down_setting()
		{
			if (hilite_item == (NUM_SETTINGS - 2))
			{
				highlight_setting(NUM_SETTINGS - 1);
				return;
			}

			if (hilite_item < (NUM_SETTINGS - 2))
			{
				highlight_setting(hilite_item + 2);
			}
		}

		internal static void toggle_setting()
		{
			if (hilite_item == (NUM_SETTINGS - 1))
			{
				ConfigFile.WriteConfigAsync(elite.config);
				display_options();
				return;
			}

			switch (hilite_item)
			{
				case 0:
					elite.config.UseWireframe = !elite.config.UseWireframe;
					break;

				case 1:
					elite.config.AntiAliasWireframe = !elite.config.AntiAliasWireframe;
					break;

				case 2:
					elite.config.PlanetRenderStyle = (PlanetRenderStyle)((int)(elite.config.PlanetRenderStyle + 1) % 4);
					break;

				case 3:
					elite.config.PlanetDescriptions = (PlanetDescriptions)((int)(elite.config.PlanetDescriptions + 1) % 2);
                    break;

				case 4:
					elite.config.InstantDock = !elite.config.InstantDock;
					break;
			}

			highlight_setting(hilite_item);
		}


		static void game_settings_screen()
		{
			int i;

			elite.current_screen = SCR.SCR_SETTINGS;

			elite.alg_gfx.ClearDisplay();
			elite.alg_gfx.DrawTextCentre(20, "GAME SETTINGS", 140, GFX_COL.GFX_COL_GOLD);
			elite.alg_gfx.DrawLine(0, 36, 511, 36);

			for (i = 0; i < NUM_SETTINGS; i++)
			{
				display_setting_item(i);
			}

			hilite_item = -1;
			highlight_setting(0);
		}


		static void display_option_item(int i)
		{
			int y = (384 - (30 * NUM_OPTIONS)) / 2;
			y += i * 30;
			GFX_COL col = ((!elite.docked) && option_list[i].docked_only) ? GFX_COL.GFX_COL_GREY_1 : GFX_COL.GFX_COL_WHITE;

			elite.alg_gfx.DrawTextCentre(y, option_list[i].text, 120, col);
		}

		static void highlight_option(int i)
		{
			int y;
			int x;

			if ((hilite_item != -1) && (hilite_item != i))
			{
				x = gfx.GFX_X_CENTRE - (OPTION_BAR_WIDTH / 2);
				y = (384 - (30 * NUM_OPTIONS)) / 2;
				y += hilite_item * 30;
				elite.alg_gfx.ClearArea(x, y, OPTION_BAR_WIDTH, OPTION_BAR_HEIGHT);
				display_option_item(hilite_item);
			}

			x = gfx.GFX_X_CENTRE - (OPTION_BAR_WIDTH / 2);
			y = (384 - (30 * NUM_OPTIONS)) / 2;
			y += i * 30;

			elite.alg_gfx.DrawRectangleFilled(x, y, OPTION_BAR_WIDTH, OPTION_BAR_HEIGHT, GFX_COL.GFX_COL_DARK_RED);
			display_option_item(i);

			hilite_item = i;
		}

		internal static void select_previous_option()
		{
			if (hilite_item > 0)
				highlight_option(hilite_item - 1);
		}

		internal static void select_next_option()
		{
			if (hilite_item < (NUM_OPTIONS - 1))
			{
				highlight_option(hilite_item + 1);
			}
		}

		internal static void do_option()
		{
			if ((!elite.docked) && option_list[hilite_item].docked_only)
			{
				return;
			}

			switch (hilite_item)
			{
				case 0:
					alg_main.save_commander_screen();
					break;

				case 1:
					alg_main.load_commander_screen();
                    CommanderStatus.display_commander_status();
					break;

				case 2:
					game_settings_screen();
					break;

				case 3:
					quit_screen();
					break;
			}
		}

		internal static void display_options()
		{
			int i;

			elite.current_screen = SCR.SCR_OPTIONS;

			elite.alg_gfx.ClearDisplay();
			elite.alg_gfx.DrawTextCentre(20, "GAME OPTIONS", 140, GFX_COL.GFX_COL_GOLD);
			elite.alg_gfx.DrawLine(0, 36, 511, 36);
			elite.alg_gfx.DrawTextCentre(300, "Version: Release 1.0", 120, GFX_COL.GFX_COL_WHITE);
			elite.alg_gfx.DrawTextCentre(320, "www.newkind.co.uk", 120, GFX_COL.GFX_COL_WHITE);
			elite.alg_gfx.DrawTextCentre(340, "Written by Christian Pinder 1999-2001", 120, GFX_COL.GFX_COL_WHITE);
			elite.alg_gfx.DrawTextCentre(360, "Based on original code by Ian Bell & David Braben", 120, GFX_COL.GFX_COL_WHITE);

			for (i = 0; i < NUM_OPTIONS; i++)
			{
				display_option_item(i);
			}

			hilite_item = -1;
			highlight_option(0);
		}
	}
}