// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Enums;
using EliteSharp.Ships;

namespace EliteSharp
{
    internal sealed class Stars
    {
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly PlayerShip _ship;
        private readonly Vector3[] _stars = new Vector3[20];

        internal Stars(GameState gameState, IGraphics graphics, PlayerShip ship)
        {
            _gameState = gameState;
            _graphics = graphics;
            _ship = ship;
        }

        internal bool WarpStars { get; set; }

        internal void CreateNewStars()
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i] = new()
                {
                    X = RNG.Random(-128, 127) | 8,
                    Y = RNG.Random(-128, 127) | 4,
                    Z = RNG.Random(255) | 0x90,
                };
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
                float y = _stars[i].Y;
                float x = _stars[i].X;
                _stars[i].X = y;
                _stars[i].Y = x;
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

                star.X += 128;
                star.Y += 96;

                star.X *= _graphics.Scale;
                star.Y *= _graphics.Scale;

                if ((!WarpStars) &&
                    (star.X >= _graphics.ViewT.X) && (star.X <= _graphics.ViewB.X) &&
                    (star.Y >= _graphics.ViewT.Y) && (star.Y <= _graphics.ViewB.Y))
                {
                    _graphics.DrawPixel(star, Colour.White);

                    if (zz < 0xC0)
                    {
                        _graphics.DrawPixel(new(star.X + 1, star.Y), Colour.White);
                    }

                    if (zz < 0x90)
                    {
                        _graphics.DrawPixel(new(star.X, star.Y + 1), Colour.White);
                        _graphics.DrawPixel(new(star.X + 1, star.Y + 1), Colour.White);
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

                //tx = yy * beta;
                //xx = xx + (tx * tx * 2);
                yy += beta;

                _stars[i].Y = yy;
                _stars[i].X = xx;

                if (WarpStars)
                {
                    _graphics.DrawLine(star, new((xx + 128) * _graphics.Scale, (yy + 96) * _graphics.Scale));
                }

                star.X = xx;
                star.Y = yy;

                if ((star.X > 120) || (star.X < -120) ||
                    (star.Y > 120) || (star.Y < -120) || (zz < 16))
                {
                    _stars[i] = new()
                    {
                        X = RNG.Random(-128, 127) | 8,
                        Y = RNG.Random(-128, 127) | 4,
                        Z = RNG.Random(255) | 144,
                    };
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

                star.X += 128;
                star.Y += 96;

                star.X *= _graphics.Scale;
                star.Y *= _graphics.Scale;

                if ((!WarpStars) &&
                    (star.X >= _graphics.ViewT.X) && (star.X <= _graphics.ViewB.X) &&
                    (star.Y >= _graphics.ViewT.Y) && (star.Y <= _graphics.ViewB.Y))
                {
                    _graphics.DrawPixel(star, Colour.White);

                    if (zz < 0xC0)
                    {
                        _graphics.DrawPixel(new(star.X + 1, star.Y), Colour.White);
                    }

                    if (zz < 0x90)
                    {
                        _graphics.DrawPixel(new(star.X, star.Y + 1), Colour.White);
                        _graphics.DrawPixel(new(star.X + 1, star.Y + 1), Colour.White);
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

                //tx = yy * beta;
                //xx = xx + (tx * tx * 2);
                yy += beta;

                if (WarpStars)
                {
                    float ey = yy;
                    float ex = xx;
                    ex = (ex + 128) * _graphics.Scale;
                    ey = (ey + 96) * _graphics.Scale;

                    if ((star.X >= _graphics.ViewT.X) && (star.X <= _graphics.ViewB.X) &&
                       (star.Y >= _graphics.ViewT.Y) && (star.Y <= _graphics.ViewB.Y) &&
                       (ex >= _graphics.ViewT.X) && (ex <= _graphics.ViewB.X) &&
                       (ey >= _graphics.ViewT.Y) && (ey <= _graphics.ViewB.Y))
                    {
                        _graphics.DrawLine(star, new((xx + 128) * _graphics.Scale, (yy + 96) * _graphics.Scale));
                    }
                }

                _stars[i].Y = yy;
                _stars[i].X = xx;

                if ((zz >= 300) || (MathF.Abs(yy) >= 110f))
                {
                    _stars[i].Z = RNG.Random(51, 178);

                    if (RNG.TrueOrFalse())
                    {
                        _stars[i].X = RNG.Random(-128, 127);
                        _stars[i].Y = RNG.TrueOrFalse() ? -115 : 115;
                    }
                    else
                    {
                        _stars[i].X = RNG.TrueOrFalse() ? -126 : 126;
                        _stars[i].Y = RNG.Random(-128, 127);
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

                star.X += 128;
                star.Y += 96;

                star.X *= _graphics.Scale;
                star.Y *= _graphics.Scale;

                if ((!WarpStars) &&
                    (star.X >= _graphics.ViewT.X) && (star.X <= _graphics.ViewB.X) &&
                    (star.Y >= _graphics.ViewT.Y) && (star.Y <= _graphics.ViewB.Y))
                {
                    _graphics.DrawPixel(star, Colour.White);

                    if (zz < 0xC0)
                    {
                        _graphics.DrawPixel(new(star.X + 1, star.Y), Colour.White);
                    }

                    if (zz < 0x90)
                    {
                        _graphics.DrawPixel(new(star.X, star.Y + 1), Colour.White);
                        _graphics.DrawPixel(new(star.X + 1, star.Y + 1), Colour.White);
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
                    _graphics.DrawLine(star, new((xx + 128) * _graphics.Scale, (yy + 96) * _graphics.Scale));
                }

                if (MathF.Abs(_stars[i].X) >= 116f)
                {
                    _stars[i].Y = RNG.Random(-128, 127);
                    _stars[i].X = (_gameState.CurrentScreen == Screen.LeftView) ? 115 : -115;
                    _stars[i].Z = RNG.Random(255) | 8;
                }
                else if (MathF.Abs(_stars[i].Y) >= 116f)
                {
                    _stars[i].X = RNG.Random(-128, 127);
                    _stars[i].Y = (alpha > 0) ? -110 : 110;
                    _stars[i].Z = RNG.Random(255) | 8;
                }
            }

            WarpStars = false;
        }
    }
}
