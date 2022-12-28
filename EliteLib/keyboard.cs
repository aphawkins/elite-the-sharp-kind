/*
 * Elite - The New Kind.
 *
 * Allegro version of the keyboard routines.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999-2001.
 * email: <christian@newkind.co.uk>
 *
 */

/*
 * keyboard.c
 *
 * Code to handle keyboard input.
 */

//# include <stdlib.h>
//# include <string.h>

//# include "allegro.h"

namespace Elite
{
	using System.Diagnostics;

	internal static class keyboard
	{
        internal static bool kbd_F1_pressed;
        internal static bool kbd_F2_pressed;
        internal static bool kbd_F3_pressed;
        internal static bool kbd_F4_pressed;
        internal static bool kbd_F5_pressed;
        internal static bool kbd_F6_pressed;
        internal static bool kbd_F7_pressed;
        internal static bool kbd_F8_pressed;
        internal static bool kbd_F9_pressed;
        internal static bool kbd_F10_pressed;
        internal static bool kbd_F11_pressed;
        internal static bool kbd_F12_pressed;
        internal static bool kbd_y_pressed;
        internal static bool kbd_n_pressed;
        internal static bool kbd_fire_pressed;
        internal static bool kbd_ecm_pressed;
        internal static bool kbd_energy_bomb_pressed;
        internal static bool kbd_hyperspace_pressed;
        internal static bool kbd_ctrl_pressed;
        internal static bool kbd_jump_pressed;
        internal static bool kbd_escape_pressed;
        internal static bool kbd_dock_pressed;
        internal static bool kbd_d_pressed;
        internal static bool kbd_origin_pressed;
        internal static bool kbd_find_pressed;
        internal static bool kbd_fire_missile_pressed;
        internal static bool kbd_target_missile_pressed;
        internal static bool kbd_unarm_missile_pressed;
        internal static bool kbd_pause_pressed;
        internal static bool kbd_resume_pressed;
        internal static bool kbd_inc_speed_pressed;
        internal static bool kbd_dec_speed_pressed;
        internal static bool kbd_up_pressed;
        internal static bool kbd_down_pressed;
        internal static bool kbd_left_pressed;
        internal static bool kbd_right_pressed;
        internal static bool kbd_enter_pressed;
        internal static bool kbd_backspace_pressed;
		internal static bool kbd_space_pressed;

        internal static int kbd_keyboard_startup()
		{
			//	set_keyboard_rate(2000, 2000);
			return 0;
		}

        static int kbd_keyboard_shutdown()
		{
			return 0;
		}

		internal static void kbd_poll_keyboard()
		{
			Debug.WriteLine("kbd_poll_keyboard");

			//poll_keyboard();

			//kbd_F1_pressed = key[KEY_F1];
			//kbd_F2_pressed = key[KEY_F2];
			//kbd_F3_pressed = key[KEY_F3];
			//kbd_F4_pressed = key[KEY_F4];
			//kbd_F5_pressed = key[KEY_F5];
			//kbd_F6_pressed = key[KEY_F6];
			//kbd_F7_pressed = key[KEY_F7];
			//kbd_F8_pressed = key[KEY_F8];
			//kbd_F9_pressed = key[KEY_F9];
			//kbd_F10_pressed = key[KEY_F10];
			//kbd_F11_pressed = key[KEY_F11];
			//kbd_F12_pressed = key[KEY_F12];

			//kbd_y_pressed = key[KEY_Y];
			//kbd_n_pressed = key[KEY_N];

			//kbd_fire_pressed = key[KEY_A];
			//kbd_ecm_pressed = key[KEY_E];
			//kbd_energy_bomb_pressed = key[KEY_TAB];
			//kbd_hyperspace_pressed = key[KEY_H];
			//kbd_ctrl_pressed = (key[KEY_LCONTROL]) || (key[KEY_RCONTROL]);
			//kbd_jump_pressed = key[KEY_J];
			//kbd_escape_pressed = key[KEY_ESC];

			//kbd_dock_pressed = key[KEY_C];
			//kbd_d_pressed = key[KEY_D];
			//kbd_origin_pressed = key[KEY_O];
			//kbd_find_pressed = key[KEY_F];

			//kbd_fire_missile_pressed = key[KEY_M];
			//kbd_target_missile_pressed = key[KEY_T];
			//kbd_unarm_missile_pressed = key[KEY_U];

			//kbd_pause_pressed = key[KEY_P];
			//kbd_resume_pressed = key[KEY_R];

			//kbd_inc_speed_pressed = key[KEY_SPACE];
			//kbd_dec_speed_pressed = key[KEY_SLASH];

			//kbd_up_pressed = key[KEY_S] || key[KEY_UP];
			//kbd_down_pressed = key[KEY_X] || key[KEY_DOWN];
			//kbd_left_pressed = key[KEY_COMMA] || key[KEY_LEFT];
			//kbd_right_pressed = key[KEY_STOP] || key[KEY_RIGHT];

			//kbd_enter_pressed = key[KEY_ENTER];
			//kbd_backspace_pressed = key[KEY_BACKSPACE];
			//kbd_space_pressed = key[KEY_SPACE];

			//while (keypressed())
			//{
			//	readkey();
			//}
		}

		internal static char kbd_read_key()
		{
            Debug.WriteLine("kbd_read_key");

			//int keynum;
			//int keycode;
			//char keyasc;

			//kbd_enter_pressed = false;
			//kbd_backspace_pressed = false;

			//keynum = readkey();
			//keycode = keynum >> 8;
			//keyasc = keynum & 255;

			//if (keycode == KEY_ENTER)
			//{
			//	kbd_enter_pressed = true;
			//	return (char)0;
			//}

			//if (keycode == KEY_BACKSPACE)
			//{
			//	kbd_backspace_pressed = true;
			//	return (char)0;
			//}

			//return keyasc;
			return '\0';
		}


		static void kbd_clear_key_buffer()
		{
			Debug.WriteLine("kbd_clear_key_buffer");

			//while (keypressed())
			//{
			//	readkey();
			//}
		}
	}
}