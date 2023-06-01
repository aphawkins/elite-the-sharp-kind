// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;

namespace EliteSharp.WinForms
{
    public partial class GameWindow : Form
    {
        private readonly System.Windows.Forms.Timer _refreshTimer = new();
        private readonly IKeyboard _keyboard;

        //private readonly Task _game;
        public GameWindow(Bitmap bmp, IKeyboard keyboard)
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            _keyboard = keyboard;
            screen.Image = bmp;
        }

        private void DoThrow(Task t)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DoThrow(t)));
                return;
            }

            if (t.IsFaulted && t.Exception != null)
            {
                throw t.Exception;
            }
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e) =>

            //Debug.WriteLine("KeyDown KeyCode: " + e.KeyCode);
            //Debug.WriteLine("KeyDown KeyValue: " + e.KeyValue);
            _keyboard.KeyDown((CommandKey)e.KeyValue);

        private void GameWindow_KeyUp(object sender, KeyEventArgs e) => _keyboard.KeyUp((CommandKey)e.KeyValue);

        private void RefreshScreen() => screen.Refresh();
    }
}
