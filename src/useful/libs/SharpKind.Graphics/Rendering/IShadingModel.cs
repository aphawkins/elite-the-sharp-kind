// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// What colour a face ends up, given which way it turns. The pipeline's
/// shading stage, and one of three independent choices about a solid world -
/// the others being how a primitive is filled (see
/// <see cref="FillMode"/>) and how the result is reduced to what the
/// display can show (see <see cref="IColourQuantiser"/>).
/// </summary>
/// <remarks>
/// A model here answers only for the colour, never for whether to compute one:
/// "no lighting" is <see cref="UnlitShading"/> rather than a null or a flag at
/// the call site, so the caller shades by asking and never by branching.
/// </remarks>
public interface IShadingModel
{
    /// <summary>
    /// The colour to fill one face with.
    /// </summary>
    /// <param name="faceColour">The model's own colour for the face.</param>
    /// <param name="cameraNormal">
    /// The face's normal, rotated into camera space. <see cref="Vector3.Zero"/>
    /// for a face with no normal of its own, which has no direction to light.
    /// </param>
    /// <param name="fullyLit">
    /// The brightness a fully-lit face of this model takes - see
    /// <see cref="LambertShading.FullyLit"/>.
    /// </param>
    /// <returns>The shaded colour, before any quantisation.</returns>
    public FastColor Shade(in FastColor faceColour, Vector3 cameraNormal, byte fullyLit);
}
