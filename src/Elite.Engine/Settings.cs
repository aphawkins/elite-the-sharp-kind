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

namespace Elite.Engine
{
    using Elite.Engine.Enums;

    internal static partial class Settings
	{
        private static int hilite_item;

        private static readonly Setting[] setting_list = 
		{
			new("Graphics:", new string[5] {"Solid", "Wireframe", "", "", ""}),
			new("Anti Alias:", new string[5] {"Off", "On", "", "", ""}),
			new("Planet Style:", new string[5] {"Wireframe", "Green", "SNES", "Fractal", ""}),
			new("Planet Desc.:", new string[5] {"BBC", "MSX", "", "", ""}),
			new("Instant Dock:", new string[5] {"Off", "On", "", "", ""}),
			new("Save Settings", new string[5] {"", "", "", "", ""})
		};

        internal static void quit_screen()
		{
            elite.SetView(SCR.SCR_QUIT);

			elite.draw.DrawQuit();

            for (; ; )
            {
                if (elite.keyboard.IsKeyPressed(CommandKey.Y))
                {
					Environment.Exit(0);
                    break;
                }

                if (elite.keyboard.IsKeyPressed(CommandKey.N))
                {
                    break;
                }
            }
        }

		internal static void select_left_setting()
		{
			if (hilite_item.IsOdd())
            {
                hilite_item--;
            }

            elite.draw.DrawSettings(setting_list, hilite_item);
        }

		internal static void select_right_setting()
		{
			if (!hilite_item.IsOdd() && (hilite_item < (setting_list.Length - 1)))
			{
				hilite_item++;
			}

			elite.draw.DrawSettings(setting_list, hilite_item);
		}

		internal static void select_up_setting()
		{
			if (hilite_item == (setting_list.Length - 1))
			{
                hilite_item = setting_list.Length - 2;
			}

			if (hilite_item > 1)
			{
				hilite_item -= 2;
			}

            elite.draw.DrawSettings(setting_list, hilite_item);
        }

		internal static void select_down_setting()
		{
			if (hilite_item == (setting_list.Length - 2))
			{
                hilite_item = setting_list.Length - 1;
			}

			if (hilite_item < (setting_list.Length - 2))
			{
				hilite_item += 2;
			}

            elite.draw.DrawSettings(setting_list, hilite_item);
        }

		internal static void toggle_setting()
		{
			if (hilite_item == (setting_list.Length - 1))
			{
				ConfigFile.WriteConfigAsync(elite.config);
				Options.display_options();
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

			elite.draw.DrawSettings(setting_list, hilite_item);
		}

        internal static void game_settings_screen()
		{
			elite.SetView(SCR.SCR_SETTINGS);

			hilite_item = 0;
            elite.draw.DrawSettings(setting_list, hilite_item);
        }
	}
}