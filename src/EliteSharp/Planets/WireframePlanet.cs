﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Enums;

namespace EliteSharp.Planets
{
    internal sealed class WireframePlanet : PlanetRenderer
    {
        internal WireframePlanet(IGraphics graphics)
            : base(graphics)
        {
        }

        /// <summary>
        /// Draw a wireframe planet.
        /// </summary>
        public override void Draw(Vector2 centre, float radius, Vector3[] vec) =>
            _graphics.DrawCircle(centre, radius, Colour.White);

        public override void GenerateLandscape(int seed)
        {
        }
    }
}