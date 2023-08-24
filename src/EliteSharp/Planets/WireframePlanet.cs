// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets
{
    internal sealed class WireframePlanet : IObject
    {
        private readonly IDraw _draw;
        private readonly PlanetRenderer _planetRenderer;

        internal WireframePlanet(IDraw draw)
        {
            _draw = draw;
            _planetRenderer = new(draw);
        }

        private WireframePlanet(WireframePlanet other)
        {
            _draw = other._draw;
            _planetRenderer = other._planetRenderer;
        }

        public Vector3 Location { get; set; } = new(0, 0, 123456);

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public ShipFlags Flags { get; set; }

        public ShipType Type { get; set; } = ShipType.Planet;

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public IObject Clone()
        {
            WireframePlanet planet = new(this);
            this.CopyTo(planet);
            return planet;
        }

        public void Draw()
        {
            (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
            if (v != null)
            {
                _draw.Graphics.DrawCircle(v.Value.Position, v.Value.Radius, EColors.White);
            }
        }
    }
}
