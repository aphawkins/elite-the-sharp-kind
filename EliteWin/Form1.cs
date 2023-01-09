namespace EliteWin
{
    using Elite;
    using Elite.Enums;

    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer _refreshTimer = new();
        IGfx _gfx;
        ISound _sound;
        IKeyboard _keyboard;

        public Form1()
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            Bitmap bmp = new(512, 512);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.Black);
                }
            }

            screen.Image = bmp;

            _gfx = new alg_gfx(ref bmp);
            _sound = new Sound();
            _keyboard = new Keyboard();
            Task.Run(() => alg_main.main(ref _gfx, ref _sound, ref _keyboard));
        }

        private void RefreshScreen()
        {
            screen.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("KeyDown KeyCode: " + e.KeyCode);
            //Debug.WriteLine("KeyDown KeyValue: " + e.KeyValue);

            _keyboard.KeyPressed(e.KeyValue);
        }
    }
}