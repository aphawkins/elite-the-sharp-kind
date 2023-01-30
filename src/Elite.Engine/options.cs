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
	using Elite.Engine.Views;

    internal class option
    {
        internal string text;
        internal bool docked_only;

        internal option(string text, bool docked_only)
        {
            this.text = text;
            this.docked_only = docked_only;
        }
    };

    internal static class Options
	{
        private static int hilite_item;

        private static readonly option[] option_list =
		{
			new("Save Commander",   true),
			new("Load Commander",   true),
			new("Game Settings",    false),
			new ("Quit",            false)
		};

		internal static void select_previous_option()
		{
			if (hilite_item > 0)
            {
                hilite_item--;
            }

            elite.draw.DrawOptions(option_list, hilite_item);
        }

		internal static void select_next_option()
		{
			if (hilite_item < (option_list.Length - 1))
			{
				hilite_item++;
			}

			elite.draw.DrawOptions(option_list, hilite_item);
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
					elite.save_commander_screen();
					break;

				case 1:
                    elite.load_commander_screen();
                    CommanderStatus.display_commander_status();
					break;

				case 2:
					Settings.game_settings_screen();
					break;

				case 3:
                    Settings.quit_screen();
					break;
			}
		}

		internal static void display_options()
		{
			elite.current_screen = SCR.SCR_OPTIONS;

			hilite_item = 0;
			elite.draw.DrawOptions(option_list, hilite_item);
		}
	}
}