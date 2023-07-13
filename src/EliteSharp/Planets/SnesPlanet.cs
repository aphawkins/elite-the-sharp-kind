// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets
{
    internal sealed class SnesPlanet : IShip
    {
        private readonly PlanetRenderer _planetRenderer;

        /// <summary>
        /// Colour map used to generate a SNES Elite style planet.
        /// </summary>
        private readonly Colour[] _snesPlanetColour = new Colour[]
        {
            Colour.Purple,
            Colour.Purple,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.LightBlue,
            Colour.LightBlue,
            Colour.LighterGrey,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.LighterGrey,
            Colour.LightBlue,
            Colour.LightBlue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.Purple,
            Colour.Purple,
        };

        internal SnesPlanet(IDraw draw)
        {
            _planetRenderer = new(draw);
            GenerateLandscape();
            Type = ShipType.Planet;
        }

        private SnesPlanet(SnesPlanet other) => _planetRenderer = other._planetRenderer;

        public ShipFlags Flags { get; set; }

        public Vector3 Location { get; set; }

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public ShipType Type { get; set; }

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public IShip Clone()
        {
            SnesPlanet planet = new(this);
            this.Copy(planet);
            return planet;
        }

        public void Draw()
        {
            (Vector2 Position, float Radius)? v = _planetRenderer.GetPlanetPosition(Location);
            if (v != null)
            {
                _planetRenderer.Draw(v.Value.Position, v.Value.Radius, Rotmat);
            }
        }

        /// <summary>
        /// Generate a landscape map for a SNES Elite style planet.
        /// </summary>
        private void GenerateLandscape()
        {
            for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
            {
                int colour = (int)_snesPlanetColour[y * (_snesPlanetColour.Length - 1) / PlanetRenderer.LandYMax];
                for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
                {
                    _planetRenderer._landscape[x, y] = colour;
                }
            }
        }
    }
}
