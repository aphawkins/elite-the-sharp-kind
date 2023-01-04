namespace Elite
{
    public interface IKeyboard
    {
        bool kbd_backspace_pressed { get; }
        bool kbd_ctrl_pressed { get; }
        bool kbd_d_pressed { get; }
        bool kbd_dec_speed_pressed { get; }
        bool kbd_dock_pressed { get; }
        bool kbd_down_pressed { get; }
        bool kbd_ecm_pressed { get; }
        bool kbd_energy_bomb_pressed { get; }
        bool kbd_enter_pressed { get; }
        bool kbd_escape_pressed { get; }
        bool kbd_F1_pressed { get; }
        bool kbd_F10_pressed { get; }
        bool kbd_F11_pressed { get; }
        bool kbd_F12_pressed { get; }
        bool kbd_F2_pressed { get; }
        bool kbd_F3_pressed { get; }
        bool kbd_F4_pressed { get; }
        bool kbd_F5_pressed { get; }
        bool kbd_F6_pressed { get; }
        bool kbd_F7_pressed { get; }
        bool kbd_F8_pressed { get; }
        bool kbd_F9_pressed { get; }
        bool kbd_find_pressed { get; }
        bool kbd_fire_missile_pressed { get; }
        bool kbd_fire_pressed { get; }
        bool kbd_hyperspace_pressed { get; }
        bool kbd_inc_speed_pressed { get; }
        bool kbd_jump_pressed { get; }
        bool kbd_left_pressed { get; }
        bool kbd_n_pressed { get; }
        bool kbd_origin_pressed { get; }
        bool kbd_pause_pressed { get; }
        bool kbd_resume_pressed { get; }
        bool kbd_right_pressed { get; }
        bool kbd_space_pressed { get; }
        bool kbd_target_missile_pressed { get; }
        bool kbd_unarm_missile_pressed { get; }
        bool kbd_up_pressed { get; }
        bool kbd_y_pressed { get; }

        int kbd_keyboard_startup();
        void kbd_poll_keyboard();
        char kbd_read_key();

        void KeyPressed(int keyValue);
    }
}