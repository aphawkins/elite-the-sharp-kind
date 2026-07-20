// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

////using System.Diagnostics;
using System.Numerics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful.Assets;
using Useful.Assets.Models;
using Useful.Assets.Palettes;
using Useful.Graphics;
using Useful.Maths;

namespace EliteSharpLib.Graphics;

internal sealed class EliteDraw : IEliteDraw
{
    private readonly uint _colorGold;
    private readonly uint _colorWhite;
    private readonly uint _colorYellow;
    private readonly GameState _gameState;
    private readonly Vector4[] _pointList = new Vector4[100];
    private readonly IShipRenderer _shipRenderer;

    internal EliteDraw(GameState gameState, IGraphics graphics, IAssetLocator assetLocator, IShipRenderer shipRenderer)
    {
        _gameState = gameState;
        Graphics = graphics;
        _shipRenderer = shipRenderer;
        Palette = PaletteReader.Read(assetLocator.PalettePath);
        _colorGold = Palette["Gold"];
        _colorWhite = Palette["White"];
        _colorYellow = Palette["Yellow"];
    }

    public float Bottom
        => _gameState.Config.IsViewFullFrame ? Graphics.ScreenHeight - BorderWidth : Graphics.ScreenHeight - ScannerHeight;

    public Vector2 Centre => new(Graphics.ScreenWidth / 2, (ScannerTop / 2) + BorderWidth);

    public IGraphics Graphics { get; }

    public bool IsWidescreen { get; }

    public float Left => BorderWidth;

    public float Offset => ScannerLeft;

    public IPaletteCollection Palette { get; }

    public float Right => Graphics.ScreenWidth - BorderWidth;

    public float ScannerLeft => Centre.X - (ScannerWidth / 2);

    public float ScannerRight => ScannerLeft + ScannerWidth - 1;

    public float ScannerTop => Graphics.ScreenHeight - ScannerHeight;

    public float Top => BorderWidth;

    internal float Height => Bottom - BorderWidth;

    internal float Width => Graphics.ScreenWidth - (2 * BorderWidth);

    private static float BorderWidth => 1;

    private static float ScannerHeight => 129;

    private static float ScannerWidth => 512;

    public void DrawBorder()
    {
        for (int i = 0; i < BorderWidth; i++)
        {
            Graphics.DrawRectangle(new(i, i), Graphics.ScreenWidth - 1 - (2 * i), Bottom - (2 * i), _colorWhite);
        }
    }

    public void DrawHyperspaceCountdown(int countdown)
        => Graphics.DrawTextRight(new(Left + 21, Top + 4), $"{countdown}", nameof(FontType.Small), _colorWhite);

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
    public void DrawPolygonFilled(Vector2[] points, uint faceColor, float z)
        => _shipRenderer.SubmitFace(points, faceColor, z);

    public void DrawTextPretty(Vector2 position, float width, string text)
    {
        int i = 0;
        float maxlen = width / 8;
        int previous = i;

        while (i < text.Length)
        {
            i += (int)maxlen;
            i = Math.Clamp(i, 0, text.Length - 1);

            while (text[i] is not ' ' and not ',' and not '.')
            {
                i--;
            }

            i++;
            Graphics.DrawTextLeft(position, text[previous..i], nameof(FontType.Small), _colorWhite);
            previous = i;
            position.Y += 8 * Graphics.Scale;
        }
    }

    public void DrawViewHeader(string title)
    {
        Graphics.DrawTextCentre(Top + 6, title, nameof(FontType.Large), _colorGold);
        Graphics.DrawLine(new(Left, 36), new(Right, 36), _colorWhite);

        // Vertical lines
        Graphics.DrawLine(new(ScannerLeft, Top + 37), new(ScannerLeft, ScannerTop), _colorYellow);
        Graphics.DrawLine(new(ScannerRight, Top + 37), new(ScannerRight, ScannerTop), _colorYellow);
    }

    public void SetFullScreenClipRegion() => Graphics.SetClipRegion(new(0, 0), Graphics.ScreenWidth, Graphics.ScreenHeight);

    public void SetViewClipRegion() => Graphics.SetClipRegion(new(Left, Top), Width, Height);

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

        int np = 0;

        for (int i = 0; i < ship.Model.Points.Count; i++)
        {
            if (ship.Model.Points[i].FaceNormals.Any(x => x.Visible))
            {
                Vector4 vec = Vector4.Transform(ship.Model.Points[i].Coords, ship.Rotmat);
                Vector4 r = vec + ship.Location;
                Vector2 position = new(r.X, -r.Y);
                position *= 256 / r.Z;
                position += Centre / 2;
                position *= Graphics.Scale;
                _pointList[np].X = position.X;
                _pointList[np].Y = position.Y;
                np++;
            }
        }

        float z = ship.Location.Z;
        float q = z >= 0x2000 ? 254 : (int)(z / 32) | 1;
        float pr = ship.ExpDelta * 256 / q;

        ////  if (pr > 0x1C00)
        ////      q = 254;
        ////  else
        q = pr / 32;

        for (int cnt = 0; cnt < np; cnt++)
        {
            float sx = _pointList[cnt].X;
            float sy = _pointList[cnt].Y;

            for (int i = 0; i < 16; i++)
            {
                Vector2 position = new(RNG.Random(-128, 128), RNG.Random(-128, 128));

                position.X = position.X * q / 256;
                position.Y = position.Y * q / 256;

                position.X = position.X + position.X + sx;
                position.Y = position.Y + position.Y + sy;

                int sizex = RNG.Random(1, 3);
                int sizey = RNG.Random(1, 3);

                for (int psy = 0; psy < sizey; psy++)
                {
                    for (int psx = 0; psx < sizex; psx++)
                    {
                        Graphics.DrawPixel(new(position.X + psx, position.Y + psy), _colorWhite);
                    }
                }
            }
        }
    }
}
