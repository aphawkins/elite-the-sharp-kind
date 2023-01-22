namespace EliteWin
{
    using Elite;

    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer _refreshTimer = new();
        private IGfx _gfx;
        private ISound _sound;
        private IKeyboard _keyboard;

        public Form1()
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            Bitmap bmp = new(512, 512);
            screen.Image = bmp;

            _gfx = new alg_gfx(ref bmp);
            _sound = new Sound();
            _keyboard = new Keyboard();
            Task.Run(() => elite.main(ref _gfx, ref _sound, ref _keyboard));
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