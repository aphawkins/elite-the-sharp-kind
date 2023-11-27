// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;

namespace EliteSharp.WinForms
{
    internal sealed partial class WinWindow : Form
    {
        private readonly IKeyboard _keyboard;

        public WinWindow(int width, int height, IKeyboard keyboard)
        {
            InitializeComponent();

            screen.Size = new Size(width, height);
            ClientSize = new Size(width, height);
            MaximumSize = new Size(width + 16, height + 39);
            MinimumSize = new Size(width + 16, height + 39);

            _keyboard = keyboard;
        }

        public void SetImage(Image image)
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
