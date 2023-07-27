// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Ships;
using EliteSharp.WinForms;

namespace EliteSharp.Renderer
{
    public partial class RenderForm : Form
    {
        private readonly Bitmap _bmp = new(512, 512, PixelFormat.Format32bppArgb);
        private readonly EBitmap _buffer = new(512, 512);
        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Views.Screen, Views.IView> _views;
        private IDraw _draw;
        private IGraphics _graphics;
        private IObject _obj;

        public RenderForm()
        {
            InitializeComponent();
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Views.Screen, Views.IView>();
            _gameState = new GameState(_keyboard, _views);
            comboRenderer.Items.AddRange(new[] { "GDI", "Software" });
            comboRenderer.SelectedIndex = 0;

            _graphics = new GdiGraphics(_bmp);
            _draw = new Draw(_gameState, _graphics);
            _obj = new WireframePlanet(_draw);

            numLocationZ.Minimum = 0;
            numLocationZ.Maximum = 9999999;
            numLocationZ.Increment = 10000;
            numLocationZ.Value = (decimal)_obj.Location.Z;

            comboObjects.Items.AddRange(new[] { "Wireframe", "Solid", "Striped", "Fractal" });
            comboObjects.SelectedIndex = 0;
        }

        private void BtnRender_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => BtnRender_Click(sender, e)));
                return;
            }

            _graphics.Clear();
            _draw.SetFullScreenClipRegion();
            _obj.Location = new(_obj.Location.X, _obj.Location.Y, (float)numLocationZ.Value);
            _obj.Draw();
            UpdateScreen();
        }

        private void ComboPlanets_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboObjects.SelectedIndex)
            {
                case 0: _obj = new WireframePlanet(_draw); break;
                case 1: _obj = new SolidPlanet(_draw, EColor.Green); break;
                case 2: _obj = new StripedPlanet(_draw); break;
                case 3: _obj = new FractalPlanet(_draw, 14229); break;
                default:
                    break;
            }

            _obj.Rotmat = VectorMaths.GetInitialMatrix().ToVectors();
        }

        private void ComboRenderer_SelectedIndexChanged(object sender, EventArgs e) => SelectRenderer();

        private void SelectRenderer()
        {
            _graphics = comboRenderer.SelectedIndex switch
            {
                1 => new SoftwareGraphics(_buffer),
                _ => new GdiGraphics(_bmp),
            };

            _draw = new Draw(_gameState, _graphics);
            _obj = new WireframePlanet(_draw);
        }

        private void UpdateScreen()
        {
            _graphics.ScreenUpdate();

            switch (comboRenderer.SelectedIndex)
            {
                case 1:
                    for (int y = 0; y < 512; y++)
                    {
                        for (int x = 0; x < 512; x++)
                        {
                            _bmp.SetPixel(x, y, Color.FromArgb(_buffer.GetPixel(x, y).ToArgb()));
                        }
                    }

                    break;

                default:
                    break;
            }

            renderBox.Image = _bmp;
        }
    }
}
