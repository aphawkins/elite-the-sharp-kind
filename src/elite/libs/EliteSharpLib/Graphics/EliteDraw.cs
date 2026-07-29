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

    // Focal length as a multiple of the tier's screen width. 1.0 reproduces
    // the 16-bit render exactly (512 x 1.0 = the old 256 x Scale 2).
    private const float FocusFactor = 1.0f;

    private readonly uint _colorGold;
    private readonly uint _colorWhite;
    private readonly uint _colorYellow;
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
        Scale = assetLocator.Tier == SystemTier.EightBit ? 1 : 2;
        Palette = PaletteReader.Read(assetLocator.PalettePath);
        _colorGold = Palette["Gold"];
        _colorWhite = Palette["White"];
        _colorYellow = Palette["Yellow"];
    }

    public float Bottom
        => _gameState.Config.IsViewFullFrame ? Graphics.ScreenHeight - BorderWidth : Graphics.ScreenHeight - ScannerHeight;

    public Vector2 Centre => new(Graphics.ScreenWidth / 2, (ScannerTop / 2) + BorderWidth);

    // The original's projection is x * 256 / z against a 256-wide view, i.e.
    // a focal length of one screen width. Deriving it from the tier's width
    // keeps the field of view identical at every tier; it is deliberately not
    // tied to Scale, which is window/coordinate magnification, not zoom.
    public float Focus => Graphics.ScreenWidth * FocusFactor;

    public IGraphics Graphics { get; }

    public bool IsWidescreen { get; }

    public float Left => BorderWidth;

    public float Offset => ScannerLeft;

    public IPaletteCollection Palette { get; }

    public float Right => Graphics.ScreenWidth - BorderWidth;

    // Elite's drawing maths works in the original's 256-unit coordinate
    // space, and this maps that onto the tier's screen. Kept whole, per the
    // pixel-doubling rule in docs/decisions.md: a fractional value would put
    // HUD text and ship vertices on half-pixels.
    public float Scale { get; }

    public float ScannerLeft => Centre.X - (ScannerWidth / 2);

    public float ScannerRight => ScannerLeft + ScannerWidth - 1;

    public float ScannerTop => Graphics.ScreenHeight - ScannerHeight;

    public float Top => BorderWidth;

    // DrawBorder's rectangle draws its far edge at position+size-1 (last
    // inclusive pixel), one short of Right/Bottom, so the view clip must
    // stop one pixel earlier still or content lands on top of the border
    // line itself instead of stopping short of it.
    internal float Height => Bottom - BorderWidth - 1;

    internal float Width => Graphics.ScreenWidth - (2 * BorderWidth) - 1;

    private static float BorderWidth => 1;

    // Taken from the scanner bitmap itself rather than hardcoded, so each
    // tier's scanner art defines its own HUD height and width (the 8-bit
    // scanner is 320x56 against the 16-bit 512x129).
    private float ScannerHeight => Graphics.ImageSize(nameof(ImageType.Scanner)).Y;

    private float ScannerWidth => Graphics.ImageSize(nameof(ImageType.Scanner)).X;

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
    public void DrawPolygonFilled(Vector2[] points, FastColor faceColor, float z)
        => _shipRenderer.Submit(points, faceColor, z);

    public void DrawTextPretty(Vector2 position, float width, string text)
    {
        int i = 0;
        float maxlen = width / 8;
        int previous = i;

        while (i < text.Length)
        {
            i += (int)maxlen;
            i = Math.Clamp(i, 0, text.Length - 1);
            int breakPoint = i;

            while (i > previous && text[i] is not ' ' and not ',' and not '.')
            {
                i--;
            }

            // No space/comma/period found within the line width: hard-break the word.
            i = i > previous ? i + 1 : breakPoint + 1;

            Graphics.DrawTextLeft(position, text[previous..i], nameof(FontType.Small), _colorWhite);
            previous = i;
            position.Y += 8 * Scale;
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
                position += Centre;
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
