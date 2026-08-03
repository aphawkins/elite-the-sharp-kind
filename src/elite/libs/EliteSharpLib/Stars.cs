// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views.Stars;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;

namespace EliteSharpLib;

internal sealed class Stars
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly PlayerShip _ship;
    private readonly Vector4[] _stars = new Vector4[20];
    private readonly IStarfieldRenderer _renderer;
    private readonly RNG _rng;

    // What this frame is showing, refilled by each starfield pass and handed
    // to the rendition to draw. Which stars go in is the game's decision; how
    // one looks is not.
    private readonly List<StarMark> _marks = [];

    internal Stars(GameState gameState, IEliteDraw draw, PlayerShip ship, IStarfieldRenderer renderer, RNG rng)
    {
        _gameState = gameState;
        _ship = ship;
        _draw = draw;
        _renderer = renderer;
        _rng = rng;
    }

    internal bool WarpStars { get; set; }

    // Star coordinates are held in the original's 256-wide space, centred on
    // the view, so they map to the screen by the same factor as the projected
    // planet and sun radii. The star-space half-extents below are the screen's
    // own half-extents divided back through it, which keeps the starfield
    // filling exactly the view at any tier.
    private float StarScale => _draw.Focus / 256;

    private float StarHalfWidth => _draw.Layout.ViewportCentre.X / StarScale;

    private float StarHalfHeight => _draw.Layout.ViewportCentre.Y / StarScale;

    internal void CreateNewStars()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i] = CreateNewStar();
        }

        WarpStars = false;
    }

    /// <summary>
    /// When we change view, flip the stars over so they look like other stars.
    /// </summary>
    internal void FlipStars()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].X = -_stars[i].X;
            _stars[i].Y = -_stars[i].Y;
        }
    }

    internal void FrontStarfield()
    {
        _marks.Clear();
        float delta = WarpStars ? 50 : _ship.Speed;
        float alpha = _ship.Roll;
        float beta = _ship.Climb;

        alpha /= 256;
        delta /= 2;

        for (int i = 0; i < _stars.Length; i++)
        {
            Vector2 star = PlotStar(i);

            // Move the stars to their new locations...
            float q = delta / _stars[i].Z;

            _stars[i].Z -= delta;
            float yy = _stars[i].Y + (_stars[i].Y * q);
            float xx = _stars[i].X + (_stars[i].X * q);
            float zz = _stars[i].Z;

            yy += xx * alpha;
            xx -= yy * alpha;

            ////tx = yy * beta;
            ////xx = xx + (tx * tx * 2);
            yy += beta;

            _stars[i].Y = yy;
            _stars[i].X = xx;

            if (WarpStars)
            {
                // The forward streak is not checked against the view: it
                // runs from wherever the star was to wherever it now is, and
                // the renderer clips what falls outside.
                _marks.Add(new(star, ToScreen(xx, yy), true, zz));
            }

            star.X = xx;
            star.Y = yy;

            if ((star.X > StarHalfWidth)
                || (star.X < -StarHalfWidth) ||
                (star.Y > (_draw.Layout.ViewportBottom - _draw.Layout.ViewportCentre.Y) / StarScale)
                || (star.Y < -StarHalfHeight) ||
                (zz < 16))
            {
                _stars[i] = CreateNewStar();
            }
        }

        WarpStars = false;
        _renderer.Draw(_marks);
    }

    internal void LeftStarfield()
    {
        float delta = WarpStars ? 50 : _ship.Speed;
        SideStarfield(-_ship.Roll, -_ship.Climb, -delta);
    }

    internal void RearStarfield()
    {
        _marks.Clear();
        float delta = WarpStars ? 50 : _ship.Speed;
        float alpha = -_ship.Roll;
        float beta = -_ship.Climb;

        alpha /= 256;
        delta /= 2;

        for (int i = 0; i < _stars.Length; i++)
        {
            Vector2 star = PlotStar(i);

            // Move the stars to their new locations...
            float q = delta / _stars[i].Z;

            _stars[i].Z += delta;
            float yy = _stars[i].Y - (_stars[i].Y * q);
            float xx = _stars[i].X - (_stars[i].X * q);
            float zz = _stars[i].Z;

            yy += xx * alpha;
            xx -= yy * alpha;

            ////tx = yy * beta;
            ////xx = xx + (tx * tx * 2);
            yy += beta;

            if (WarpStars)
            {
                DrawStarStreak(star, xx, yy);
            }

            _stars[i].Y = yy;
            _stars[i].X = xx;

            if ((zz >= 300) || (MathF.Abs(yy) >= 110))
            {
                RecycleStarAtEdge(i);
            }
        }

        WarpStars = false;
        _renderer.Draw(_marks);
    }

    internal void RightStarfield()
    {
        float delta = WarpStars ? 50 : _ship.Speed;
        SideStarfield(_ship.Roll, _ship.Climb, delta);
    }

    // Star space (centred on the view) to screen pixels.
    private Vector2 ToScreen(float xx, float yy)
        => _draw.Layout.ViewportCentre + (new Vector2(xx, yy) * StarScale);

    // Draw the motion streak from a star's old screen position to where it has
    // just moved to, when both ends are inside the view.
    private void DrawStarStreak(Vector2 star, float xx, float yy)
    {
        Vector2 end = ToScreen(xx, yy);
        float ex = end.X;
        float ey = end.Y;

        if ((star.X >= _draw.Layout.ViewportLeft)
            && (star.X <= _draw.Layout.ViewportRight) &&
            (star.Y >= _draw.Layout.ViewportTop)
            && (star.Y <= _draw.Layout.ViewportBottom) &&
            (ex >= _draw.Layout.ViewportLeft)
            && (ex <= _draw.Layout.ViewportRight) &&
            (ey >= _draw.Layout.ViewportTop)
            && (ey <= _draw.Layout.ViewportBottom))
        {
            _marks.Add(new(star, new(ex, ey), true, 0));
        }
    }

    // A star that has passed the camera or run off the top or bottom comes
    // back in at a random point on one of the view's edges.
    private void RecycleStarAtEdge(int i)
    {
        _stars[i].Z = _rng.Random(51, 179);

        if (_rng.TrueOrFalse())
        {
            _stars[i].X = _rng.Random(-(int)StarHalfWidth, (int)StarHalfWidth);
            _stars[i].Y = _rng.TrueOrFalse() ? -(int)StarHalfHeight : (int)StarHalfHeight;
        }
        else
        {
            _stars[i].X = _rng.TrueOrFalse() ? -(int)StarHalfWidth : (int)StarHalfWidth;
            _stars[i].Y = _rng.Random(-(int)StarHalfHeight, (int)StarHalfHeight);
        }
    }

    private Vector4 CreateNewStar() => new()
    {
        X = _rng.Random(-(int)StarHalfWidth, (int)StarHalfWidth) | 8,
        Y = _rng.Random(-(int)StarHalfHeight, (int)StarHalfHeight) | 4,
        Z = _rng.Random(256) | 144,
    };

    // Draws star i in its current screen location (a bright pixel that grows
    // as it approaches the camera), then returns that screen position so the
    // caller can draw a motion streak from it once the star has moved.
    private Vector2 PlotStar(int i)
    {
        Vector2 star = new()
        {
            Y = _stars[i].Y,
            X = _stars[i].X,
        };
        float zz = _stars[i].Z;

        star = ToScreen(star.X, star.Y);

        if ((!WarpStars) &&
            (star.X >= _draw.Layout.ViewportLeft)
            && (star.X <= _draw.Layout.ViewportRight) &&
            (star.Y >= _draw.Layout.ViewportTop)
            && (star.Y <= _draw.Layout.ViewportBottom))
        {
            _marks.Add(new(star, star, false, zz));
        }

        return star;
    }

    private void SideStarfield(float alpha, float beta, float delta)
    {
        _marks.Clear();
        for (int i = 0; i < _stars.Length; i++)
        {
            Vector2 star = PlotStar(i);

            float yy = _stars[i].Y;
            float xx = _stars[i].X;
            float zz = _stars[i].Z;

            float delt8 = delta / (zz / 32);
            xx += delt8;

            xx += yy * (beta / 256);
            yy -= xx * (beta / 256);

            xx += yy / 256 * (alpha / 256) * (-xx);
            yy += yy / 256 * (alpha / 256) * yy;

            yy += alpha;

            _stars[i].Y = yy;
            _stars[i].X = xx;

            if (WarpStars)
            {
                _marks.Add(new(star, ToScreen(xx, yy), true, zz));
            }

            if (MathF.Abs(_stars[i].X) >= StarHalfWidth)
            {
                _stars[i].X = (_gameState.CurrentScreen == Screen.LeftView) ? StarHalfWidth : -StarHalfWidth;
                _stars[i].Y = _rng.Random(-(int)StarHalfHeight, (int)StarHalfHeight);
                _stars[i].Z = _rng.Random(256) | 8;
            }
            else if (MathF.Abs(_stars[i].Y) >= 116)
            {
                _stars[i].X = _rng.Random(-(int)StarHalfWidth, (int)StarHalfWidth);
                _stars[i].Y = (alpha > 0) ? -StarHalfHeight : StarHalfHeight;
                _stars[i].Z = _rng.Random(256) | 8;
            }
        }

        WarpStars = false;
        _renderer.Draw(_marks);
    }
}
