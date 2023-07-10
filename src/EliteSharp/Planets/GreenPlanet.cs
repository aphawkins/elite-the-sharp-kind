// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.Planets
{
    internal sealed class GreenPlanet : IPlanetRenderer
    {
        private readonly IDraw _draw;

        internal GreenPlanet(IDraw draw) => _draw = draw;

        public void Draw(Vector2 centre, float radius, Vector3[] vec) => _draw.Graphics.DrawCircleFilled(centre, radius, Colour.Green);
    }
}
