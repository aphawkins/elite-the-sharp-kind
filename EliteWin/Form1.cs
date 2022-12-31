namespace EliteWin
{
    using Elite;

    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer _refreshTimer = new();
        IGfx _gfx;

        public Form1()
        {
            InitializeComponent();

            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();
        }

        private void RefreshScreen()
        {
            screen.Refresh();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
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

            alg_main.main(ref _gfx);
        }
    }
}