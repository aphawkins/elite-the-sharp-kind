// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Planets;
using EliteSharp.Views;

namespace EliteSharp.Ships
{
    internal sealed class Planet : ShipBase
    {
        private readonly IDraw _draw;
        private readonly IPlanetRenderer _renderer;

        internal Planet(IDraw draw, IPlanetRenderer renderer)
            : base(draw)
        {
            _draw = draw;
            _renderer = renderer;
            Type = ShipType.Planet;
        }

        /// <summary>
        /// Draw a planet.
        /// We can currently do three different types of planet: Wireframe, Fractal landscape or SNES Elite style.
        /// </summary>
        public override void Draw()
        {
            Vector2 position = new(Location.X, -Location.Y);
            position *= 256 / Location.Z;
            position += _draw.Centre / 2;
            position *= _draw.Graphics.Scale;

            float radius = 6291456 / Location.Length();

            // Planets are BIG!
            //  radius = 6291456 / ship_vec.z;
            radius *= _draw.Graphics.Scale;

            if ((position.X + radius < _draw.Left) ||
                (position.X - radius > _draw.Right) ||
                (position.Y + radius < _draw.Top) ||
                (position.Y - radius > _draw.Bottom))
            {
                return;
            }

            _renderer.Draw(position, radius, Rotmat);
        }

        public override IShip Clone() => new Planet(_draw, _renderer)
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
    }
}
