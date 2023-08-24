// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Planets
{
    internal sealed class StripedPlanet : IObject
    {
        private readonly PlanetRenderer _planetRenderer;

        /// <summary>
        /// Colour map used to generate a striped style planet.
        /// </summary>
        private readonly EColor[] _stripeColours = new EColor[]
        {
            EColors.Purple,
            EColors.Purple,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.Blue,
            EColors.Blue,
            EColors.Blue,
            EColors.Blue,
            EColors.LightBlue,
            EColors.LightBlue,
            EColors.LighterGrey,
            EColors.Orange,
            EColors.Orange,
            EColors.Orange,
            EColors.Orange,
            EColors.LightOrange,
            EColors.Orange,
            EColors.Orange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.Orange,
            EColors.LightOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.DarkOrange,
            EColors.Orange,
            EColors.Orange,
            EColors.LightOrange,
            EColors.Orange,
            EColors.Orange,
            EColors.Orange,
            EColors.Orange,
            EColors.LighterGrey,
            EColors.LightBlue,
            EColors.LightBlue,
            EColors.Blue,
            EColors.Blue,
            EColors.Blue,
            EColors.Blue,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.DarkBlue,
            EColors.Purple,
            EColors.Purple,
        };

        internal StripedPlanet(IDraw draw)
        {
            _planetRenderer = new(draw);
            GenerateLandscape();
        }

        private StripedPlanet(StripedPlanet other) => _planetRenderer = other._planetRenderer;

        public ShipFlags Flags { get; set; }

        public Vector3 Location { get; set; } = new(0, 0, 123456);

        public Vector3[] Rotmat { get; set; } = new Vector3[3];

        public ShipType Type { get; set; } = ShipType.Planet;

        public float RotX { get; set; }

        public float RotZ { get; set; }

        public IObject Clone()
        {
            StripedPlanet planet = new(this);
            this.CopyTo(planet);
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
        /// Generate a landscape map.
        /// </summary>
        private void GenerateLandscape()
        {
            for (int y = 0; y <= PlanetRenderer.LandYMax; y++)
            {
                int colour = _stripeColours[y * (_stripeColours.Length - 1) / PlanetRenderer.LandYMax].ToArgb();
                for (int x = 0; x <= PlanetRenderer.LandXMax; x++)
                {
                    _planetRenderer._landscape[x, y] = colour;
                }
            }
        }
    }
}
