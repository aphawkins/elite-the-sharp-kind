// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets
{
    internal sealed class SolidPlanet : IObject
    {
        private readonly Colour _colour;
        private readonly IDraw _draw;
        private readonly PlanetRenderer _planetRenderer;

        internal SolidPlanet(IDraw draw, Colour colour)
        {
            _draw = draw;
            _colour = colour;
            _planetRenderer = new(draw);
        }

        private SolidPlanet(SolidPlanet other)
        {
            _draw = other._draw;
            _planetRenderer = other._planetRenderer;
        }

        public ShipFlags Flags { get; set; }

        public Vector3 Location { get; set; } = new(0, 0, 123456);

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public ShipType Type { get; set; } = ShipType.Planet;

        public IObject Clone()
        {
            SolidPlanet planet = new(this);
            this.CopyTo(planet);
            return planet;
        }

        public void Draw()
        {
            (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
            if (v != null)
            {
                _draw.Graphics.DrawCircleFilled(v.Value.Position, v.Value.Radius, _colour);
            }
        }
    }
}
