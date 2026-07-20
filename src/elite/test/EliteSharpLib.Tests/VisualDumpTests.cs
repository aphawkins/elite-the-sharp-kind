// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Graphics;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests;

// Renders ships through the real software rasterizer and saves BMP files
// for visual inspection (found under EliteFrames in the temp dir).
// The overlap/interpenetration scenes exercise the hidden-surface handling
// that the painter's algorithm gets wrong per face.
public class VisualDumpTests
{
    [Fact]
    public void DumpShipFrames()
    {
        string outDir = Path.Combine(Path.GetTempPath(), "EliteFrames");
        Directory.CreateDirectory(outDir);

        FastBitmap? lastFrame = null;
        using SoftwareGraphics graphics = SoftwareGraphics.Create(512, 512, b => lastFrame = b, AssetLocator.Create());
        GameState gameState = new(new ScreenManager<Screen, IView>(new FakeKeyboard()));
        ZBufferRenderer shipRenderer = new(graphics);
        EliteDraw draw = new(gameState, graphics, AssetLocator.Create(), shipRenderer);
        ShipFactory factory = ShipFactory.Create(AssetLocator.Create(), draw);

        void RenderAndSave(string name, params IShip[] ships)
        {
            graphics.Clear();
            draw.RenderStart();
            foreach (IShip ship in ships)
            {
                ship.Draw();
            }

            draw.RenderEnd();
            graphics.ScreenUpdate();
            Assert.NotNull(lastFrame);
            SaveBmp(lastFrame, Path.Combine(outDir, name));
        }

        // a lone ship at several orientations (self-occlusion of its own
        // rear faces is where "bits of hidden surfaces show through")
        IShip cobra = factory.CreateShip("CobraMk3");
        cobra.Location = new(0, 0, 420, 0);
        for (int i = 0; i < 4; i++)
        {
            float angle = 0.4f + (i * 0.8f);
            cobra.Rotmat = Matrix4x4.CreateRotationY(angle) * Matrix4x4.CreateRotationX(0.3f + (i * 0.2f));
            RenderAndSave($"frame_cobra_{i}.bmp", cobra);
        }

        // two ships overlapping on screen with interleaved depth ranges
        IShip viper = factory.CreateShip("Viper");
        cobra.Location = new(-15, 0, 420, 0);
        cobra.Rotmat = Matrix4x4.CreateRotationY(0.9f) * Matrix4x4.CreateRotationX(0.4f);
        viper.Location = new(25, 10, 460, 0);
        viper.Rotmat = Matrix4x4.CreateRotationY(-0.7f) * Matrix4x4.CreateRotationX(-0.2f);
        RenderAndSave("frame_overlap.bmp", cobra, viper);

        // ships whose models carry 2-point line faces (hull window detail)
        // over large flat hull faces
        IShip transporter = factory.CreateShip("Transporter");
        transporter.Location = new(0, 0, 110, 0);
        for (int i = 0; i < 4; i++)
        {
            transporter.Rotmat =
                Matrix4x4.CreateRotationY(-0.6f + (i * 0.7f)) * Matrix4x4.CreateRotationX(-0.35f + (i * 0.25f));
            RenderAndSave($"frame_transporter_{i}.bmp", transporter);
        }

        // a full spin, where wrap-around hull faces meet far-side decals
        for (int i = 0; i < 12; i++)
        {
            transporter.Rotmat =
                Matrix4x4.CreateRotationY(i * MathF.PI / 6) * Matrix4x4.CreateRotationX(0.25f);
            RenderAndSave($"frame_transporter_spin_{i:00}.bmp", transporter);
        }

        // the Cobra seen from behind (the red engine plates)
        IShip cobraRear = factory.CreateShip("CobraMk3");
        cobraRear.Location = new(0, 0, 260, 0);
        cobraRear.Rotmat = Matrix4x4.CreateRotationY(MathF.PI - 0.25f) * Matrix4x4.CreateRotationX(0.15f);
        RenderAndSave("frame_cobra_rear.bmp", cobraRear);

        // two hulls actually intersecting - unsortable for a per-face
        // painter's algorithm, only per-pixel depth gets this right
        IShip krait = factory.CreateShip("Krait");
        cobra.Location = new(0, 0, 430, 0);
        cobra.Rotmat = Matrix4x4.CreateRotationY(0.5f);
        krait.Location = new(10, 5, 430, 0);
        krait.Rotmat = Matrix4x4.CreateRotationY(-1.1f) * Matrix4x4.CreateRotationX(0.6f);
        RenderAndSave("frame_interpenetrate.bmp", cobra, krait);
    }

    // Painter's and z-buffer only disagree on decal/base-face ties (the
    // open decal-seam defect, see CHANGELOG); a decal-free model rendered
    // alone, at an angle with no self-occlusion ambiguity, should render
    // identically either way.
    [Fact]
    public void PainterAndZBufferRenderIdenticallyForNonDecalGeometry()
    {
        (uint[] Pixels, int Width, int Height) RenderAsteroid(Func<IGraphics, IPolygonRenderer> createRenderer)
        {
            FastBitmap? lastFrame = null;
            using SoftwareGraphics graphics = SoftwareGraphics.Create(512, 512, b => lastFrame = b, AssetLocator.Create());
            IPolygonRenderer shipRenderer = createRenderer(graphics);
            GameState gameState = new(new ScreenManager<Screen, IView>(new FakeKeyboard()));
            EliteDraw draw = new(gameState, graphics, AssetLocator.Create(), shipRenderer);
            ShipFactory factory = ShipFactory.Create(AssetLocator.Create(), draw);

            IShip asteroid = factory.CreateShip("Asteroid");
            asteroid.Location = new(0, 0, 300, 0);
            asteroid.Rotmat = Matrix4x4.CreateRotationY(0.9f) * Matrix4x4.CreateRotationX(0.4f);

            graphics.Clear();
            draw.RenderStart();
            asteroid.Draw();
            draw.RenderEnd();
            graphics.ScreenUpdate();

            Assert.NotNull(lastFrame);
            int width = lastFrame.Width;
            int height = lastFrame.Height;
            uint[] pixels = new uint[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[(y * width) + x] = lastFrame.GetPixel(x, y);
                }
            }

            return (pixels, width, height);
        }

        (uint[] painterPixels, int width, int height) = RenderAsteroid(g => new PainterRenderer(g));
        (uint[] depthBufferPixels, _, _) = RenderAsteroid(g => new ZBufferRenderer(g));

        Assert.Equal(width * height, painterPixels.Length);
        Assert.Equal(painterPixels, depthBufferPixels);
    }

    private static void SaveBmp(FastBitmap bitmap, string path)
    {
        int w = bitmap.Width;
        int h = bitmap.Height;
        int rowSize = ((w * 3) + 3) & ~3;
        int dataSize = rowSize * h;

        using BinaryWriter writer = new(File.Create(path));
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write(54 + dataSize);
        writer.Write(0);
        writer.Write(54);
        writer.Write(40);
        writer.Write(w);
        writer.Write(h);
        writer.Write((short)1);
        writer.Write((short)24);
        writer.Write(0);
        writer.Write(dataSize);
        writer.Write(2835);
        writer.Write(2835);
        writer.Write(0);
        writer.Write(0);

        byte[] row = new byte[rowSize];
        for (int y = h - 1; y >= 0; y--)
        {
            for (int x = 0; x < w; x++)
            {
                uint argb = bitmap.GetPixel(x, y);
                row[x * 3] = (byte)(argb & 0xff);
                row[(x * 3) + 1] = (byte)((argb >> 8) & 0xff);
                row[(x * 3) + 2] = (byte)((argb >> 16) & 0xff);
            }

            writer.Write(row);
        }
    }
}
