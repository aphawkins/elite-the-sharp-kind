// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.Planets
{
    internal sealed class WireframePlanet : IPlanetRenderer
    {
        private readonly IDraw _draw;

        internal WireframePlanet(IDraw draw) => _draw = draw;

        /// <summary>
        /// Draw a wireframe planet.
        /// </summary>
        public void Draw(Vector2 centre, float radius, Vector3[] vec) =>
            _draw.Graphics.DrawCircle(centre, radius, Colour.White);
    }
}
