// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

////using System.Diagnostics;
using System.Numerics;
using EliteSharp.Abstractions.Assets;
using EliteSharp.Abstractions.Renditions;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using SharpKind;
using SharpKind.Abstraction.Config;
using SharpKind.Assets;
using SharpKind.Assets.Models;
using SharpKind.Assets.Palettes;
using SharpKind.Graphics;
using SharpKind.Graphics.Rendering;
using SharpKind.Maths;

namespace EliteSharpLib.Graphics;

internal sealed class EliteDraw : IEliteDraw
{
    // Upper bound on the points in any ship model, so the explosion
    // projection buffer never has to grow.
    private const int MaxModelPoints = 100;

    // Focal length as a multiple of the rendition's screen height. 1.0 reproduces
    // the 16-bit render exactly (512 x 1.0 = the old 256 x Scale 2).
    private const float FocusFactor = 1.0f;

    private readonly FastColor _colorWhite;
    private readonly GameState _gameState;
    private readonly Vector4[] _pointList = new Vector4[MaxModelPoints];
    private readonly IPolygonRenderer _shipRenderer;
    private readonly RNG _rng;
    private readonly bool _shadesShips;

    // The pipeline's shading and output stages. Which of each is in use comes
    // from the live config, so the Settings view takes effect on the next
    // frame; the instances themselves never change.
    private readonly IShadingModel _unlit = new UnlitShading();
    private readonly IShadingModel _lambert = new LambertShading();
    private readonly IColourQuantiser _nearest;
    private readonly IColourQuantiser _dithered;

    internal EliteDraw(
        GameState gameState,
        IGraphics graphics,
        ScreenLayout screen,
        IAssetLocator assetLocator,
        IRendition rendition,
        IPolygonRenderer shipRenderer,
        RNG rng)
    {
        ArgumentNullException.ThrowIfNull(rendition);
        ArgumentNullException.ThrowIfNull(screen);

        _gameState = gameState;
        Graphics = graphics;
        _shipRenderer = shipRenderer;
        _rng = rng;
        Layout = new(
            screen.ScreenWidth,
            screen.ScreenHeight,
            graphics.ImageSize(nameof(ImageType.Scanner)),
            rendition.Scale);
        Palette = PaletteReader.Read(assetLocator.PalettePath);
        _shadesShips = rendition.ShadesShips;

        // Shading invents colours the assets never carried, so it answers to
        // the same limits the asset validator holds the assets to: an indexed
        // rendition can show only what its palette names, a direct-colour one
        // anything its DAC reaches.
        _nearest = assetLocator.Colours.PaletteNamesEveryColour
            ? new PaletteQuantiser(Palette.Values)
            : new ChannelGridQuantiser(assetLocator.Colours.ChannelBits);
        _dithered = new OrderedDitherQuantiser(_nearest);

        // Blending a shade across a face is only worth doing where the
        // rendition has shades to blend through. An indexed one can show no
        // colour its palette does not name, so a smooth gradient quantises
        // straight back to the same handful of steps a flat fill already
        // gives - the same reasoning as _shadesShips, one tier further on.
        BlendsShades = !assetLocator.Colours.PaletteNamesEveryColour;

        // After Palette: the rendition looks its colours up through this.
        Ships = rendition.CreateShipColours(this);
        _colorWhite = Palette["White"];
    }

    public ViewLayout Layout { get; }

    // The original's projection is x * 256 / z against a 256-square view, i.e.
    // a focal length of one screen height. Deriving it from the tier's height
    // holds the vertical field of view constant, so a wider screen shows more
    // to the left and right rather than magnifying everything (decided
    // 2026-07-29; deriving it from the width instead narrows the vertical view
    // as the screen widens). It is deliberately not tied to Scale, which is
    // window/coordinate magnification, not zoom.
    public float Focus => Layout.ScreenHeight * FocusFactor;

    public IGraphics Graphics { get; }

    public IPaletteCollection Palette { get; }

    public ShipColours Ships { get; }

    // Read from the live config for the same reason Shading is. Gouraud is
    // the one model whose colour varies within a face, so it is the one that
    // makes this true - and only where the rendition can show the difference.
    public bool ShadesPerVertex
        => BlendsShades
            && Shading != _unlit
            && _gameState.Config.Engine.Graphics.Shading == ShadingModelKind.Gouraud;

    // Read from the live config rather than cached, so the Settings view's
    // rows show on the next frame - the same reason ConfigPolygonRenderer
    // picks its strategy per frame. A wireframe world has no face to shade,
    // and a rendition with no colours to spare for shading stays unlit
    // whatever the commander asked for.
    // Whether the rendition has enough colours for a blend across a face to
    // survive being quantised. Set once: it is the rendition's, not the
    // commander's.
    private bool BlendsShades { get; }

    private IShadingModel Shading
    {
        get
        {
            GraphicsConfigSettings graphics = _gameState.Config.Engine.Graphics;

            return _shadesShips
                && graphics.FillMode == FillMode.Solid
                && graphics.Shading is ShadingModelKind.Lambert or ShadingModelKind.Gouraud
                    ? _lambert
                    : _unlit;
        }
    }

    // Dithering an unshaded world would only dither the model's own colours,
    // which are already displayable, so it follows the shading model.
    private IColourQuantiser Quantiser
        => _gameState.Config.Engine.Graphics.Quantisation == Quantisation.Ordered && Shading != _unlit
            ? _dithered
            : _nearest;

    // depths is the camera-space depth at each point, which the z-buffered
    // strategy interpolates per pixel; z is one whole-face key, which the
    // painter's strategy sorts the face by. Decal faces (cockpit windows
    // etc) sit exactly on the hull face beneath, so the caller gives them
    // their base face's z key to tie in the painter's order, and a small
    // near bias in depths to win outright under the per-pixel test.
    public void DrawPolygonFilled(Vector2[] points, float[] depths, FastColor faceColor, float z)
    {
        // A dither travels with the polygon because only the fill can apply
        // it, one answer per pixel; anything else already resolved in
        // ShadeFace and nothing needs to go down.
        IColourQuantiser quantiser = Quantiser;

        _shipRenderer.Submit(points, depths, faceColor, z, quantiser.IsPositionDependent ? quantiser : null);
    }

    // As above with a colour per point. The quantiser always travels with the
    // polygon here, dithering or not: a blended colour is a different colour
    // at every pixel, so there is nothing ShadeVertex could have resolved for
    // the whole face.
    public void DrawPolygonFilled(Vector2[] points, float[] depths, FastColor[] cornerColors, float z)
        => _shipRenderer.Submit(points, depths, cornerColors, z, Quantiser);

    // Read from the live config rather than cached, so the Settings view's
    // toggle shows on the next frame - the same reason ConfigPolygonRenderer
    // picks its strategy per frame. Wireframe has no faces to light.
    public FastColor ShadeFace(FastColor faceColour, Vector3 cameraNormal, byte fullyLit)
    {
        FastColor shaded = Shading.Shade(faceColour, cameraNormal, fullyLit);

        // A dither has to be asked per pixel, so it is left to the fill and
        // the face colour travels unquantised; anything else resolves the
        // whole face once, here.
        IColourQuantiser quantiser = Quantiser;

        return quantiser.IsPositionDependent ? shaded : quantiser.Quantise(shaded, 0, 0);
    }

    // Deliberately unquantised, unlike ShadeFace: the fill blends between the
    // corners and quantises what it arrives at, per pixel. Quantising here
    // would reduce the ends of the gradient and then blend between the
    // reduced values, which is a worse approximation of the same curve.
    public FastColor ShadeVertex(FastColor faceColour, Vector3 cameraNormal, byte fullyLit)
        => Shading.Shade(faceColour, cameraNormal, fullyLit);

    public void SetFullScreenClipRegion() => Graphics.SetClipRegion(new(0, 0), Layout.ScreenWidth, Layout.ScreenHeight);

    public void SetViewClipRegion() => Graphics.SetClipRegion(
        new(Layout.ViewportLeft, Layout.ViewportTop),
        Layout.ViewportWidth,
        Layout.ViewportHeight);

    /// <summary>
    /// Draws an object in the universe. (Ship, Planet, Sun etc).
    /// </summary>
    public void DrawObject(IObject obj)
    {
        if (_gameState.CurrentScreen is not Screen.FrontView and not Screen.RearView and
            not Screen.LeftView and not Screen.RightView and
            not Screen.IntroOne and not Screen.IntroTwo and
            not Screen.GameOver and not Screen.EscapeCapsule and
            not Screen.MissionBriefing)
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

        // Through the interface: Projector is a default implementation, so it
        // is not in scope on the implementing class.
        PerspectiveProjector projector = ((IEliteDraw)this).Projector;

        for (int i = 0; i < ship.Model.Points.Count; i++)
        {
            if (ship.Model.Points[i].FaceNormals.Any(x => x.Visible))
            {
                Vector4 vec = Vector4.Transform(ship.Model.Points[i].Coords, ship.Rotmat);
                Vector4 r = vec + ship.Location;
                Vector2 position = projector.Project(r.X, r.Y, r.Z);
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
