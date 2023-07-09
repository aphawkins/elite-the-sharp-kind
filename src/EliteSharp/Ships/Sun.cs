// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.Ships
{
    internal sealed class Sun : ShipBase
    {
        private readonly IDraw _draw;

        internal Sun(IDraw draw)
            : base(draw)
        {
            _draw = draw;
            Type = ShipType.Sun;
        }

        public override IShip Clone() => new Sun(_draw)
        {
            Bounty = Bounty,
            EnergyMax = EnergyMax,
            FaceNormals = FaceNormals,
            Faces = Faces,
            LaserFront = LaserFront,
            LaserStrength = LaserStrength,
            Lines = Lines,
            LootMax = LootMax,
            MissilesMax = MissilesMax,
            Name = Name,
            Points = Points,
            ScoopedType = ScoopedType,
            Size = Size,
            Class = Class,
            VanishPoint = VanishPoint,
            VelocityMax = VelocityMax,
            ExpDelta = ExpDelta,
            Flags = Flags,
            Type = Type,
            Location = Location.Cloner(),
            Rotmat = Rotmat.Cloner(),
            RotX = RotX,
            RotZ = RotZ,
            Energy = Energy,
            Velocity = Velocity,
            Acceleration = Acceleration,
            Missiles = Missiles,
            Target = Target,
            Bravery = Bravery,
            MinDistance = MinDistance,
        };

        public override void Draw()
        {
            Vector2 centre = new(Location.X, -Location.Y);

            centre *= 256 / Location.Z;
            centre += _draw.Centre / 2;
            centre *= _draw.Graphics.Scale;

            float radius = 6291456 / Location.Length() * _draw.Graphics.Scale;

            if ((centre.X + radius < _draw.Left) ||
                (centre.X - radius > _draw.Right) ||
                (centre.Y + radius < _draw.Top) ||
                (centre.Y - radius > _draw.Bottom))
            {
                return;
            }

            float s = -radius;
            float x = radius;
            float y = 0;

            // s -= x + x;
            while (y <= x)
            {
                // Top of top half
                RenderSunLine(centre, y, -MathF.Floor(x), radius);

                // Top of top half
                RenderSunLine(centre, x, -y, radius);

                // Top of bottom half
                RenderSunLine(centre, x, y, radius);

                // Bottom of bottom half
                RenderSunLine(centre, y, MathF.Floor(x), radius);

                s += y + y + 1;
                y++;
                if (s >= 0)
                {
                    s -= x + x + 2;
                    x--;
                }
            }
        }

        private void RenderSunLine(Vector2 centre, float x, float y, float radius)
        {
            Vector2 s = new()
            {
                Y = centre.Y + y,
            };

            if (s.Y < _draw.Top || s.Y > _draw.Bottom)
            {
                return;
            }

            s.X = centre.X - x;
            float ex = centre.X + x;

            s.X -= radius * RNG.Random(2, 10) / 256f;
            ex += radius * RNG.Random(2, 10) / 256f;

            if (ex < _draw.Left || s.X > _draw.Right)
            {
                return;
            }

            if (s.X < _draw.Left)
            {
                s.X = _draw.Left;
            }

            if (ex > _draw.Right)
            {
                ex = _draw.Right;
            }

            float inner = radius * (200 + RNG.Random(8)) / 256;
            inner *= inner;

            float inner2 = radius * (220 + RNG.Random(8)) / 256;
            inner2 *= inner2;

            float outer = radius * (239 + RNG.Random(8)) / 256;
            outer *= outer;

            float dy = y * y;
            float dx = s.X - centre.X;

            for (; s.X <= ex; s.X++, dx++)
            {
                float distance = (dx * dx) + dy;

                Colour colour = distance < inner
                    ? Colour.White
                    : distance < inner2
                        ? Colour.LightYellow
                        : distance < outer
                            ? Colour.LightOrange
                            : ((int)s.X ^ (int)y).IsOdd() ? Colour.Orange : Colour.DarkOrange;

                _draw.Graphics.DrawPixelFast(s, colour);
            }
        }
    }
}
