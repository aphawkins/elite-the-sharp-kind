// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine
{
    internal sealed class Stars
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly PlayerShip _ship;
        internal bool WarpStars { get; set; }
        private readonly Vector3[] _stars = new Vector3[20];

        internal Stars(GameState gameState, IGfx gfx, PlayerShip ship)
        {
            _gameState = gameState;
            _gfx = gfx;
            _ship = ship;
        }

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
                    X = _stars[i].X
                };
                float zz = _stars[i].Z;

                star.X += 128;
                star.Y += 96;

                star.X *= Graphics.GFX_SCALE;
                star.Y *= Graphics.GFX_SCALE;

                if ((!WarpStars) &&
                    (star.X >= Graphics.GFX_VIEW_TX) && (star.X <= Graphics.GFX_VIEW_BX) &&
                    (star.Y >= Graphics.GFX_VIEW_TY) && (star.Y <= Graphics.GFX_VIEW_BY))
                {
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

                    if (zz < 0xC0)
                    {
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
                    }

                    if (zz < 0x90)
                    {
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                    }
                }


                // Move the stars to their new locations...
                float Q = delta / _stars[i].Z;

                _stars[i].Z -= delta;
                float yy = _stars[i].Y + (_stars[i].Y * Q);
                float xx = _stars[i].X + (_stars[i].X * Q);
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
                    _gfx.DrawLine(star, new((xx + 128) * Graphics.GFX_SCALE, (yy + 96) * Graphics.GFX_SCALE));
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
                    continue;
                }

            }

            WarpStars = false;
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
                    X = _stars[i].X
                };
                float zz = _stars[i].Z;

                star.X += 128;
                star.Y += 96;

                star.X *= Graphics.GFX_SCALE;
                star.Y *= Graphics.GFX_SCALE;

                if ((!WarpStars) &&
                    (star.X >= Graphics.GFX_VIEW_TX) && (star.X <= Graphics.GFX_VIEW_BX) &&
                    (star.Y >= Graphics.GFX_VIEW_TY) && (star.Y <= Graphics.GFX_VIEW_BY))
                {
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

                    if (zz < 0xC0)
                    {
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
                    }

                    if (zz < 0x90)
                    {
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                    }
                }

                // Move the stars to their new locations...

                float Q = delta / _stars[i].Z;

                _stars[i].Z += delta;
                float yy = _stars[i].Y - (_stars[i].Y * Q);
                float xx = _stars[i].X - (_stars[i].X * Q);
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
                    ex = (ex + 128) * Graphics.GFX_SCALE;
                    ey = (ey + 96) * Graphics.GFX_SCALE;

                    if ((star.X >= Graphics.GFX_VIEW_TX) && (star.X <= Graphics.GFX_VIEW_BX) &&
                       (star.Y >= Graphics.GFX_VIEW_TY) && (star.Y <= Graphics.GFX_VIEW_BY) &&
                       (ex >= Graphics.GFX_VIEW_TX) && (ex <= Graphics.GFX_VIEW_BX) &&
                       (ey >= Graphics.GFX_VIEW_TY) && (ey <= Graphics.GFX_VIEW_BY))
                    {
                        _gfx.DrawLine(star, new((xx + 128) * Graphics.GFX_SCALE, (yy + 96) * Graphics.GFX_SCALE));
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

        internal void LeftStarfield()
        {
            float delta = WarpStars ? 50 : _ship.Speed;
            SideStarfield(-_ship.Roll, -_ship.Climb, -delta);
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
                    X = _stars[i].X
                };
                float zz = _stars[i].Z;

                star.X += 128;
                star.Y += 96;

                star.X *= Graphics.GFX_SCALE;
                star.Y *= Graphics.GFX_SCALE;

                if ((!WarpStars) &&
                    (star.X >= Graphics.GFX_VIEW_TX) && (star.X <= Graphics.GFX_VIEW_BX) &&
                    (star.Y >= Graphics.GFX_VIEW_TY) && (star.Y <= Graphics.GFX_VIEW_BY))
                {
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

                    if (zz < 0xC0)
                    {
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
                    }

                    if (zz < 0x90)
                    {
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
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
                    _gfx.DrawLine(star, new((xx + 128) * Graphics.GFX_SCALE, (yy + 96) * Graphics.GFX_SCALE));
                }

                if (MathF.Abs(_stars[i].X) >= 116f)
                {
                    _stars[i].Y = RNG.Random(-128, 127);
                    _stars[i].X = (_gameState.CurrentScreen == SCR.SCR_LEFT_VIEW) ? 115 : -115;
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
    }
}
