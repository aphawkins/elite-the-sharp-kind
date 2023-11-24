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
        private const int ScreenWidth = 512;
        private const int ScreenHeight = 512;

        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Views.Screen, Views.IView> _views;
        private Draw _draw;
        private IGraphics _graphics;
        private IObject _obj;

        public RenderForm()
        {
            InitializeComponent();
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Views.Screen, Views.IView>();
            _gameState = new GameState(_keyboard, _views);
            comboRenderer.Items.AddRange(["GDI", "Software"]);
            comboRenderer.SelectedIndex = 0;

            _graphics = new GDIGraphics(ScreenWidth, ScreenHeight, GDIScreenUpdate);
            _draw = new Draw(_gameState, _graphics);
            _obj = new WireframePlanet(_draw);

            numLocationZ.Minimum = 0;
            numLocationZ.Maximum = 9999999;
            numLocationZ.Increment = 10000;
            numLocationZ.Value = (decimal)_obj.Location.Z;

            comboObjects.Items.AddRange(["Wireframe", "Solid", "Striped", "Fractal"]);
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
                case 0:
                    _obj = new WireframePlanet(_draw);
                    break;

                case 1:
                    _obj = new SolidPlanet(_draw, EliteColors.Green);
                    break;

                case 2:
                    _obj = new StripedPlanet(_draw);
                    break;

                case 3:
                    _obj = new FractalPlanet(_draw, 14229);
                    break;
            }

            _obj.Rotmat = VectorMaths.GetInitialMatrix();
        }

        private void ComboRenderer_SelectedIndexChanged(object sender, EventArgs e) => SelectRenderer();

        private void SelectRenderer()
        {
            _graphics = comboRenderer.SelectedIndex switch
            {
                1 => new SoftwareGraphics(ScreenWidth, ScreenHeight, SoftwareScreenUpdate),
                _ => new GDIGraphics(ScreenWidth, ScreenHeight, GDIScreenUpdate),
            };

            _draw = new Draw(_gameState, _graphics);
            _obj = new WireframePlanet(_draw);
        }

        private void UpdateScreen() => _graphics.ScreenUpdate();

        private void GDIScreenUpdate(Bitmap bitmap) => renderBox.Image = bitmap;

        private void SoftwareScreenUpdate(FastBitmap fastBitmap)
            => renderBox.Image = (Bitmap)new(
                ScreenWidth,
                ScreenHeight,
                ScreenWidth * 4,
                PixelFormat.Format32bppArgb,
                fastBitmap.BitmapHandle);
    }
}
