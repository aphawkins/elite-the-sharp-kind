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
    using System.Windows.Forms;

    public class Keyboard : IKeyboard
    {
        public bool kbd_F1_pressed { get; private set; }
        public bool kbd_F2_pressed { get; private set; }
        public bool kbd_F3_pressed { get; private set; }
        public bool kbd_F4_pressed { get; private set; }
        public bool kbd_F5_pressed { get; private set; }
        public bool kbd_F6_pressed { get; private set; }
        public bool kbd_F7_pressed { get; private set; }
        public bool kbd_F8_pressed { get; private set; }
        public bool kbd_F9_pressed { get; private set; }
        public bool kbd_F10_pressed { get; private set; }
        public bool kbd_F11_pressed { get; private set; }
        public bool kbd_F12_pressed { get; private set; }
        public bool kbd_y_pressed { get; private set; }
        public bool kbd_n_pressed { get; private set; }
        public bool kbd_fire_pressed { get; private set; }
        public bool kbd_ecm_pressed { get; private set; }
        public bool kbd_energy_bomb_pressed { get; private set; }
        public bool kbd_hyperspace_pressed { get; private set; }
        public bool kbd_ctrl_pressed { get; private set; }
        public bool kbd_jump_pressed { get; private set; }
        public bool kbd_escape_pressed { get; private set; }
        public bool kbd_dock_pressed { get; private set; }
        public bool kbd_d_pressed { get; private set; }
        public bool kbd_origin_pressed { get; private set; }
        public bool kbd_find_pressed { get; private set; }
        public bool kbd_fire_missile_pressed { get; private set; }
        public bool kbd_target_missile_pressed { get; private set; }
        public bool kbd_unarm_missile_pressed { get; private set; }
        public bool kbd_pause_pressed { get; private set; }
        public bool kbd_resume_pressed { get; private set; }
        public bool kbd_inc_speed_pressed { get; private set; }
        public bool kbd_dec_speed_pressed { get; private set; }
        public bool kbd_up_pressed { get; private set; }
        public bool kbd_down_pressed { get; private set; }
        public bool kbd_left_pressed { get; private set; }
        public bool kbd_right_pressed { get; private set; }
        public bool kbd_enter_pressed { get; private set; }
        public bool kbd_backspace_pressed { get; private set; }
        public bool kbd_space_pressed { get; private set; }

        public int kbd_keyboard_startup()
        {
            Debug.WriteLine(nameof(kbd_keyboard_startup));

            //	set_keyboard_rate(2000, 2000);
            return 0;
        }

        int kbd_keyboard_shutdown()
        {
            return 0;
        }

        public void kbd_poll_keyboard()
        {
            Debug.WriteLine(nameof(kbd_poll_keyboard));

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
            //    readkey();
            //}
        }

        public char kbd_read_key()
        {
            Debug.WriteLine(nameof(kbd_read_key));

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

        void kbd_clear_key_buffer()
        {
            Debug.WriteLine(nameof(kbd_clear_key_buffer));

            //while (keypressed())
            //{
            //	readkey();
            //}
        }

        public void KeyPressed(int keyValue)
        {
            kbd_F1_pressed = keyValue == 112;
            kbd_F2_pressed = keyValue == 113;
            kbd_F3_pressed = keyValue == 114;
            kbd_F4_pressed = keyValue == 115;
            kbd_F5_pressed = keyValue == 116;
            kbd_F6_pressed = keyValue == 117;
            kbd_F7_pressed = keyValue == 118;
            kbd_F8_pressed = keyValue == 119;
            kbd_F9_pressed = keyValue == 120;
            kbd_F10_pressed = keyValue == 121;
            kbd_F11_pressed = keyValue == 122;
            kbd_F12_pressed = keyValue == 123;

            kbd_y_pressed = keyValue == 89; // Y
            kbd_n_pressed = keyValue == 78; // N

            kbd_fire_pressed = keyValue == 65; // A
            kbd_ecm_pressed = keyValue == 69; // E
            // TODO: Fix unhandled TAB
            kbd_energy_bomb_pressed = keyValue == 81; // Q  (Should be TAB)
            kbd_hyperspace_pressed = keyValue == 72; // H
            kbd_ctrl_pressed = keyValue == 17; // CTRL
            kbd_jump_pressed = keyValue == 74; // J
            kbd_escape_pressed = keyValue == 27; // ESC

            kbd_dock_pressed = keyValue == 67; // C
            kbd_d_pressed = keyValue == 68; // D
            kbd_origin_pressed = keyValue == 79; // O
            kbd_find_pressed = keyValue == 70; // F

            kbd_fire_missile_pressed = keyValue == 77; // M
            kbd_target_missile_pressed = keyValue == 84; // T
            kbd_unarm_missile_pressed = keyValue == 85; // U

            kbd_pause_pressed = keyValue == 80; // P
            kbd_resume_pressed = keyValue == 82; // R

            kbd_inc_speed_pressed = keyValue == 32; // SPACE
            kbd_dec_speed_pressed = keyValue == 220; // BACKSLASH

            //TODO: fix these for arrow keys
            kbd_up_pressed = keyValue == 83; // S (or UP)
            kbd_down_pressed = keyValue == 88; // X (|| DOWN)
            kbd_left_pressed = keyValue == 188; // < (|| LEFT)
            kbd_right_pressed = keyValue == 190; // > (|| RIGHT)

            //TODO: ENTER needs properly handling
            kbd_enter_pressed = keyValue == -1; // ENTER
            kbd_backspace_pressed = keyValue == 8; // BACKSPACE
            kbd_space_pressed = keyValue == 32; // SPACE
        }
    }
}