namespace Elite.WinForms
{
    using Elite.Engine;

    public partial class GameWindow : Form
    {
        private readonly System.Windows.Forms.Timer _refreshTimer = new();
        private IGfx _gfx;
        private ISound _sound;
        private IKeyboard _keyboard;

        public GameWindow()
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            Bitmap bmp = new(512, 512);
            screen.Image = bmp;

            _gfx = new GdiGraphics(ref bmp);
            _sound = new Sound();
            _keyboard = new Keyboard();
            Task.Run(() => new elite(_gfx, _sound, _keyboard));
        }

        private void RefreshScreen()
        {
            screen.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("KeyDown KeyCode: " + e.KeyCode);
            //Debug.WriteLine("KeyDown KeyValue: " + e.KeyValue);

            _keyboard.KeyDown(e.KeyValue);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _keyboard.KeyUp(e.KeyValue);
        }
    }
}