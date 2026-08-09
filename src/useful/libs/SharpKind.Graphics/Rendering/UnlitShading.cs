// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// No lighting: a face is the flat colour the model painted it, which is how
/// both games looked before there was a light to turn on and what the original
/// hardware did.
/// </summary>
public sealed class UnlitShading : IShadingModel
{
    public FastColor Shade(in FastColor faceColour, Vector3 cameraNormal, byte fullyLit) => faceColour;
}
