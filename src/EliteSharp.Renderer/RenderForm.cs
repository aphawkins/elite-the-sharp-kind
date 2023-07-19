// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Imaging;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Ships;

namespace EliteSharp.Renderer
{
    public partial class RenderForm : Form
    {
        private readonly Bitmap _bmp = new(512, 512, PixelFormat.Format32bppRgb);
        private readonly int[,] _buffer = new int[512, 512];
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Views.Screen, Views.IView> _views;
        private IObject _planet;

        public RenderForm()
        {
            InitializeComponent();
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Views.Screen, Views.IView>();
            _gameState = new GameState(_keyboard, _views);
            _graphics = new SoftwareGraphics(_buffer);
            _draw = new Draw(_gameState, _graphics);
            _planet = new WireframePlanet(_draw);

            comboPlanets.Items.AddRange(new[] { "Wireframe", "Solid", "Striped", "Fractal" });
            comboPlanets.SelectedIndex = 0;
        }

        private void BtnRender_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => BtnRender_Click(sender, e)));
                return;
            }

            _draw.ClearDisplay();

            _planet.Draw();

            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    _bmp.SetPixel(x, y, Color.FromArgb(_buffer[x, y]));
                }
            }

            renderBox.Image = _bmp;
        }

        private void ComboPlanets_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboPlanets.SelectedIndex)
            {
                case 0: _planet = new WireframePlanet(_draw); break;
                case 1: _planet = new SolidPlanet(_draw, Colour.Green); break;
                case 2: _planet = new StripedPlanet(_draw); break;
                case 3: _planet = new FractalPlanet(_draw, 14229); break;
                default:
                    break;
            }

            _planet.Rotmat = VectorMaths.GetInitialMatrix();
        }
    }
}
