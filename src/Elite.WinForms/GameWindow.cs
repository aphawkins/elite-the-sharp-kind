// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine;
using Elite.Engine.Enums;

namespace Elite.WinForms
{
    public partial class GameWindow : Form
    {
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly System.Windows.Forms.Timer _refreshTimer = new();
        private readonly ISound _sound;

        public GameWindow()
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            Bitmap bmp = new(512, 512);
            screen.Image = bmp;

            _graphics = new GdiGraphics(ref bmp);
            _sound = new Sound();
            _keyboard = new Keyboard();
            Task.Run(() => new EliteMain(_graphics, _sound, _keyboard));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) =>

            //Debug.WriteLine("KeyDown KeyCode: " + e.KeyCode);
            //Debug.WriteLine("KeyDown KeyValue: " + e.KeyValue);

            _keyboard.KeyDown((CommandKey)e.KeyValue);

        private void Form1_KeyUp(object sender, KeyEventArgs e) => _keyboard.KeyUp((CommandKey)e.KeyValue);

        private void RefreshScreen() => screen.Refresh();
    }
}
