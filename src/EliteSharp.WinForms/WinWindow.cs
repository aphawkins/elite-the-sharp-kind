// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;

namespace EliteSharp.WinForms
{
    internal sealed partial class WinWindow : Form
    {
        private readonly System.Windows.Forms.Timer _refreshTimer = new();
        private readonly IKeyboard _keyboard;
        ////private readonly object _locker;

        public WinWindow(int width, int height, IKeyboard keyboard, object locker, Image screenImage)
        {
            ////_locker = locker;
            InitializeComponent();

            screen.Size = new Size(width, height);
            ClientSize = new Size(width, height);
            MaximumSize = new Size(width + 16, height + 39);
            MinimumSize = new Size(width + 16, height + 39);

            _refreshTimer.Interval = 100;
            //// _refreshTimer.Tick += (sender, e) => RefreshScreen();
            _refreshTimer.Start();

            _keyboard = keyboard;
            //// ScreenBitmap = new Bitmap(screen.Width, screen.Height, PixelFormat.Format32bppArgb);
            ////_locker = locker;
            ScreenImage = screenImage;
            _ = locker;
        }

        //// <summary>
        //// internal Bitmap ScreenBitmap { get; }
        //// </summary>

        internal Image ScreenImage
        {
            get =>
                    ////lock (_locker)
                    ////{
                    screen.Image; ////}

            set =>
                ////lock (_locker)
                ////{
                SetImage(value); ////screen.Refresh(); ////}
        }

        private void SetImage(Image image)
        {
            if (screen.InvokeRequired)
            {
                screen.Invoke(SetImage, image);
                return;
            }

            screen.Image = image;
            screen.Refresh();
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

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
            => _keyboard.KeyDown((CommandKey)e.KeyValue);

        private void GameWindow_KeyUp(object sender, KeyEventArgs e) => _keyboard.KeyUp((CommandKey)e.KeyValue);
    }
}
