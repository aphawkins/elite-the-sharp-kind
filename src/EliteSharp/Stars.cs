// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Views;

namespace EliteSharp;

internal sealed class Stars
{
    private readonly IDraw _draw;
    private readonly GameState _gameState;
    private readonly PlayerShip _ship;
    private readonly Vector3[] _stars = new Vector3[20];

    internal Stars(GameState gameState, IDraw draw, PlayerShip ship)
    {
        _gameState = gameState;
        _ship = ship;
        _draw = draw;
    }

    internal bool WarpStars { get; set; }

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
        float delta = WarpStars ? 50 : _ship.Speed;
        float alpha = _ship.Roll;
        float beta = _ship.Climb;

        alpha /= 256;
        delta /= 2;

        for (int i = 0; i < _stars.Length; i++)
        {
            // Plot the stars in their current locations...
            Vector2 star = new()
            {
                Y = _stars[i].Y,
                X = _stars[i].X,
            };

            float zz = _stars[i].Z;
            star += _draw.Centre / 2;
            star *= _draw.Graphics.Scale;

            if ((!WarpStars) &&
                (star.X >= _draw.Left)
                && (star.X <= _draw.Right) &&
                (star.Y >= _draw.Top)
                && (star.Y <= _draw.Bottom))
            {
                _draw.Graphics.DrawPixel(star, EliteColors.White);

                if (zz < 192)
                {
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y), EliteColors.White);
                }

                if (zz < 144)
                {
                    _draw.Graphics.DrawPixel(new(star.X, star.Y + 1), EliteColors.White);
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y + 1), EliteColors.White);
                }
            }

            // Move the stars to their new locations...
            float q = delta / _stars[i].Z;

            _stars[i].Z -= delta;
            float yy = _stars[i].Y + (_stars[i].Y * q);
            float xx = _stars[i].X + (_stars[i].X * q);
            zz = _stars[i].Z;

            yy += xx * alpha;
            xx -= yy * alpha;

            ////tx = yy * beta;
            ////xx = xx + (tx * tx * 2);
            yy += beta;

            _stars[i].Y = yy;
            _stars[i].X = xx;

            if (WarpStars)
            {
                _draw.Graphics.DrawLine(
                    star,
                    new((xx + (_draw.Centre.X / 2)) * _draw.Graphics.Scale, (yy + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale),
                    EliteColors.White);
            }

            star.X = xx;
            star.Y = yy;

            if ((star.X > _draw.Centre.X / 2)
                || (star.X < -_draw.Centre.X / 2) ||
                (star.Y > (_draw.Bottom - _draw.Centre.Y) / 2)
                || (star.Y < -_draw.Centre.Y / 2) ||
                (zz < 16))
            {
                _stars[i] = CreateNewStar();
            }
        }

        WarpStars = false;
    }

    internal void LeftStarfield()
    {
        float delta = WarpStars ? 50 : _ship.Speed;
        SideStarfield(-_ship.Roll, -_ship.Climb, -delta);
    }

    internal void RearStarfield()
    {
        float delta = WarpStars ? 50 : _ship.Speed;
        float alpha = -_ship.Roll;
        float beta = -_ship.Climb;

        alpha /= 256;
        delta /= 2;

        for (int i = 0; i < _stars.Length; i++)
        {
            // Plot the stars in their current locations...
            Vector2 star = new()
            {
                Y = _stars[i].Y,
                X = _stars[i].X,
            };
            float zz = _stars[i].Z;

            star += _draw.Centre / 2;
            star *= _draw.Graphics.Scale;

            if ((!WarpStars) &&
                (star.X >= _draw.Left)
                && (star.X <= _draw.Right) &&
                (star.Y >= _draw.Top)
                && (star.Y <= _draw.Bottom))
            {
                _draw.Graphics.DrawPixel(star, EliteColors.White);

                if (zz < 192)
                {
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y), EliteColors.White);
                }

                if (zz < 144)
                {
                    _draw.Graphics.DrawPixel(new(star.X, star.Y + 1), EliteColors.White);
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y + 1), EliteColors.White);
                }
            }

            // Move the stars to their new locations...
            float q = delta / _stars[i].Z;

            _stars[i].Z += delta;
            float yy = _stars[i].Y - (_stars[i].Y * q);
            float xx = _stars[i].X - (_stars[i].X * q);
            zz = _stars[i].Z;

            yy += xx * alpha;
            xx -= yy * alpha;

            ////tx = yy * beta;
            ////xx = xx + (tx * tx * 2);
            yy += beta;

            if (WarpStars)
            {
                float ey = yy;
                float ex = xx;
                ex = (ex + (_draw.Centre.X / 2)) * _draw.Graphics.Scale;
                ey = (ey + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale;

                if ((star.X >= _draw.Left)
                    && (star.X <= _draw.Right) &&
                    (star.Y >= _draw.Top)
                    && (star.Y <= _draw.Bottom) &&
                    (ex >= _draw.Left)
                    && (ex <= _draw.Right) &&
                    (ey >= _draw.Top)
                    && (ey <= _draw.Bottom))
                {
                    _draw.Graphics.DrawLine(
                        star,
                        new((xx + (_draw.Centre.X / 2)) * _draw.Graphics.Scale, (yy + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale),
                        EliteColors.White);
                }
            }

            _stars[i].Y = yy;
            _stars[i].X = xx;

            if ((zz >= 300) || (MathF.Abs(yy) >= 110))
            {
                _stars[i].Z = RNG.Random(51, 179);

                if (RNG.TrueOrFalse())
                {
                    _stars[i].X = RNG.Random(-(int)_draw.Centre.X / 2, (int)_draw.Centre.X / 2);
                    _stars[i].Y = RNG.TrueOrFalse() ? -(int)_draw.Centre.Y / 2 : (int)_draw.Centre.Y / 2;
                }
                else
                {
                    _stars[i].X = RNG.TrueOrFalse() ? -(int)_draw.Centre.X / 2 : (int)_draw.Centre.X / 2;
                    _stars[i].Y = RNG.Random(-(int)_draw.Centre.Y / 2, (int)_draw.Centre.Y / 2);
                }
            }
        }

        WarpStars = false;
    }

    internal void RightStarfield()
    {
        float delta = WarpStars ? 50 : _ship.Speed;
        SideStarfield(_ship.Roll, _ship.Climb, delta);
    }

    private Vector3 CreateNewStar() => new()
    {
        X = RNG.Random(-(int)_draw.Centre.X / 2, (int)_draw.Centre.X / 2) | 8,
        Y = RNG.Random(-(int)_draw.Centre.Y / 2, (int)_draw.Centre.Y / 2) | 4,
        Z = RNG.Random(256) | 144,
    };

    private void SideStarfield(float alpha, float beta, float delta)
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            Vector2 star = new()
            {
                Y = _stars[i].Y,
                X = _stars[i].X,
            };
            float zz = _stars[i].Z;

            star += _draw.Centre / 2;
            star *= _draw.Graphics.Scale;

            if ((!WarpStars) &&
                (star.X >= _draw.Left)
                && (star.X <= _draw.Right) &&
                (star.Y >= _draw.Top)
                && (star.Y <= _draw.Bottom))
            {
                _draw.Graphics.DrawPixel(star, EliteColors.White);

                if (zz < 192)
                {
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y), EliteColors.White);
                }

                if (zz < 144)
                {
                    _draw.Graphics.DrawPixel(new(star.X, star.Y + 1), EliteColors.White);
                    _draw.Graphics.DrawPixel(new(star.X + 1, star.Y + 1), EliteColors.White);
                }
            }

            float yy = _stars[i].Y;
            float xx = _stars[i].X;
            zz = _stars[i].Z;

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
                _draw.Graphics.DrawLine(
                    star,
                    new((xx + (_draw.Centre.X / 2)) * _draw.Graphics.Scale, (yy + (_draw.Centre.Y / 2)) * _draw.Graphics.Scale),
                    EliteColors.White);
            }

            if (MathF.Abs(_stars[i].X) >= _draw.Centre.X / 2)
            {
                _stars[i].X = (_gameState.CurrentScreen == Screen.LeftView) ? _draw.Centre.X / 2 : -_draw.Centre.X / 2;
                _stars[i].Y = RNG.Random(-(int)_draw.Centre.Y / 2, (int)_draw.Centre.Y / 2);
                _stars[i].Z = RNG.Random(256) | 8;
            }
            else if (MathF.Abs(_stars[i].Y) >= 116)
            {
                _stars[i].X = RNG.Random(-(int)_draw.Centre.X / 2, (int)_draw.Centre.X / 2);
                _stars[i].Y = (alpha > 0) ? -_draw.Centre.Y / 2 : _draw.Centre.Y / 2;
                _stars[i].Z = RNG.Random(256) | 8;
            }
        }

        WarpStars = false;
    }
}
