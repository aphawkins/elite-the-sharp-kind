// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

////using System.Diagnostics;
using System.Numerics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful;
using Useful.Assets;
using Useful.Assets.Models;
using Useful.Assets.Palettes;
using Useful.Graphics;
using Useful.Graphics.Rendering;
using Useful.Maths;

namespace EliteSharpLib.Graphics;

internal sealed class EliteDraw : IEliteDraw
{
    // Upper bound on the points in any ship model, so the explosion
    // projection buffer never has to grow.
    private const int MaxModelPoints = 100;

    // Focal length as a multiple of the tier's screen height. 1.0 reproduces
    // the 16-bit render exactly (512 x 1.0 = the old 256 x Scale 2).
    private const float FocusFactor = 1.0f;

    private readonly uint _colorWhite;
    private readonly GameState _gameState;
    private readonly Vector4[] _pointList = new Vector4[MaxModelPoints];
    private readonly IPolygonRenderer _shipRenderer;
    private readonly RNG _rng;

    internal EliteDraw(GameState gameState, IGraphics graphics, IAssetLocator assetLocator, IPolygonRenderer shipRenderer, RNG rng)
    {
        _gameState = gameState;
        Graphics = graphics;
        _shipRenderer = shipRenderer;
        _rng = rng;
        Layout = new(
            graphics.ScreenWidth,
            graphics.ScreenHeight,
            graphics.ImageSize(nameof(ImageType.Scanner)),
            assetLocator.Tier == SystemTier.EightBit ? 1 : 2);
        Tier = assetLocator.Tier;
        Palette = PaletteReader.Read(assetLocator.PalettePath);
        _colorWhite = Palette["White"];
    }

    public ViewLayout Layout { get; }

    public SystemTier Tier { get; }

    // The original's projection is x * 256 / z against a 256-square view, i.e.
    // a focal length of one screen height. Deriving it from the tier's height
    // holds the vertical field of view constant, so a wider screen shows more
    // to the left and right rather than magnifying everything (decided
    // 2026-07-29; deriving it from the width instead narrows the vertical view
    // as the screen widens). It is deliberately not tied to Scale, which is
    // window/coordinate magnification, not zoom.
    public float Focus => Graphics.ScreenHeight * FocusFactor;

    public IGraphics Graphics { get; }

    public IPaletteCollection Palette { get; }

    public float Bottom => Layout.Bottom;

    public Vector2 Centre => Layout.Centre;

    public float Left => Layout.Left;

    public float Offset => Layout.Offset;

    public float Right => Layout.Right;

    public float Scale => Layout.Scale;

    public float ScannerLeft => Layout.ScannerLeft;

    public float ScannerRight => Layout.ScannerRight;

    public float ScannerTop => Layout.ScannerTop;

    public float Top => Layout.Top;

    // z is one whole-face depth: the chain's sort key, and the flat depth
    // every pixel of the face tests with in RenderEnd. Flat rather than
    // per-vertex interpolated depth is deliberate: decal faces (cockpit
    // windows etc) sit exactly on the hull face beneath, and the
    // rasterizer's per-triangle interpolation cannot reproduce identical
    // depths for coplanar faces, which punches holes through the decals.
    // With one key per face a decal submitted with its base face's key
    // ties exactly, and the back-to-front chain order lets the
    // later-submitted decal win the tie, as the painter's draw order
    // always did.
    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor, float z)
        => _shipRenderer.Submit(points, faceColor, z);

    public void SetFullScreenClipRegion() => Graphics.SetClipRegion(new(0, 0), Graphics.ScreenWidth, Graphics.ScreenHeight);

    public void SetViewClipRegion() => Graphics.SetClipRegion(new(Layout.Left, Layout.Top), Layout.Width, Layout.Height);

    /// <summary>
    /// Draws an object in the universe. (Ship, Planet, Sun etc).
    /// </summary>
    public void DrawObject(IObject obj)
    {
        if (_gameState.CurrentScreen is not Screen.FrontView and not Screen.RearView and
            not Screen.LeftView and not Screen.RightView and
            not Screen.IntroOne and not Screen.IntroTwo and
            not Screen.GameOver and not Screen.EscapeCapsule and
            not Screen.MissionOne)
        {
            return;
        }

        if (obj.Flags.HasFlag(ShipProperties.Dead) && !obj.Flags.HasFlag(ShipProperties.Explosion))
        {
            obj.Flags |= ShipProperties.Explosion;
            ((IShip)obj).ExpDelta = 18;
        }

        if (obj.Flags.HasFlag(ShipProperties.Explosion))
        {
            DrawExplosion((IShip)obj);
            return;
        }

        // Only display ships in front of us.
        if (obj.Location.Z <= 0)
        {
            return;
        }

        if (obj.Type == ShipType.Planet)
        {
            obj.Draw();
            return;
        }

        if (obj.Type == ShipType.Sun)
        {
            obj.Draw();
            return;
        }

        // Check for field of vision.
        if (MathF.Abs(obj.Location.X) > obj.Location.Z ||
            MathF.Abs(obj.Location.Y) > obj.Location.Z)
        {
            return;
        }

        obj.Draw();
    }

    public void RenderEnd() => _shipRenderer.EndFrame();

    public void RenderStart() => _shipRenderer.StartFrame();

    private void DrawExplosion(IShip ship)
    {
        if (ship.ExpDelta > 251)
        {
            ship.Flags |= ShipProperties.Remove;
            return;
        }

        ship.ExpDelta += 4;

        if (ship.Location.Z <= 0)
        {
            return;
        }

        // The camera-vector / face-normal visibility check needs the rotation matrix's basis
        // vectors transposed relative to the direct point-transform below (see ShipBase.Draw).
        Matrix4x4 cameraMat = ship.Rotmat;
        (cameraMat.M12, cameraMat.M21) = (cameraMat.M21, cameraMat.M12);
        (cameraMat.M13, cameraMat.M31) = (cameraMat.M31, cameraMat.M13);
        (cameraMat.M23, cameraMat.M32) = (cameraMat.M32, cameraMat.M23);

        Vector4 camera_vec = Vector4.Transform(ship.Location, cameraMat);
        camera_vec = VectorMaths.UnitVector(camera_vec);

        foreach (FaceNormal faceNormal in ship.Model.FaceNormals)
        {
            Vector4 vec = VectorMaths.UnitVector(faceNormal.Direction);
            float cos_angle = VectorMaths.VectorDotProduct(vec, camera_vec);
            faceNormal.Visible = cos_angle < -0.13;
        }

        int np = ProjectExplosionPoints(ship);

        float z = ship.Location.Z;
        float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
        float pr = ship.ExpDelta * 256 / q;

        ////  if (pr > 0x1C00)
        ////      q = 254;
        ////  else
        q = pr / 32;

        DrawExplosionParticles(np, q);
    }

    // Project the ship's visible points into _pointList, returning how many
    // of them were written.
    private int ProjectExplosionPoints(IShip ship)
    {
        int np = 0;

        for (int i = 0; i < ship.Model.Points.Count; i++)
        {
            if (ship.Model.Points[i].FaceNormals.Any(x => x.Visible))
            {
                Vector4 vec = Vector4.Transform(ship.Model.Points[i].Coords, ship.Rotmat);
                Vector4 r = vec + ship.Location;
                Vector2 position = new(r.X, -r.Y);
                position *= Focus / r.Z;
                position += Layout.Centre;
                _pointList[np].X = position.X;
                _pointList[np].Y = position.Y;
                np++;
            }
        }

        return np;
    }

    // Scatter a cloud of debris blocks around each of the np projected points,
    // spread wider as the explosion grows (q).
    private void DrawExplosionParticles(int np, float q)
    {
        for (int cnt = 0; cnt < np; cnt++)
        {
            float sx = _pointList[cnt].X;
            float sy = _pointList[cnt].Y;

            for (int i = 0; i < 16; i++)
            {
                Vector2 position = new(_rng.Random(-128, 128), _rng.Random(-128, 128));

                position.X = position.X * q / 256;
                position.Y = position.Y * q / 256;

                position.X = position.X + position.X + sx;
                position.Y = position.Y + position.Y + sy;

                int sizex = _rng.Random(1, 3);
                int sizey = _rng.Random(1, 3);

                DrawExplosionBlock(position, sizex, sizey);
            }
        }
    }

    private void DrawExplosionBlock(Vector2 position, int sizex, int sizey)
    {
        for (int psy = 0; psy < sizey; psy++)
        {
            for (int psx = 0; psx < sizex; psx++)
            {
                Graphics.DrawPixel(new(position.X + psx, position.Y + psy), _colorWhite);
            }
        }
    }
}
