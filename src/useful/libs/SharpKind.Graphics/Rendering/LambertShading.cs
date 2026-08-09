// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// A single directional light dotted against a face's own normal, modulating
/// the face's colour. There is one light and it never moves relative to the
/// camera, so a face's shade depends only on which way it turns - which is
/// what a flat-shaded polygon renderer can afford and what the models can
/// express: they carry per-face normals and no per-vertex ones.
/// </summary>
/// <remarks>
/// Flat shading in the pipeline sense - one colour across the face. Gouraud
/// would be a second <see cref="IShadingModel"/> rather than a setting here,
/// and needs per-vertex normals the .obj assets do not carry.
/// </remarks>
public sealed class LambertShading(Vector3 lightDirection, float ambient) : IShadingModel
{
    // How lit a face turned fully away from the light still is. Not zero: an
    // unlit face of a ship in front of the player would otherwise be a black
    // silhouette against black space, and the ship would lose its shape
    // entirely rather than merely be dim.
    private readonly Vector3 _lightDirection = Vector3.Normalize(lightDirection);
    private readonly float _ambient = Math.Clamp(ambient, 0f, 1f);

    public LambertShading()
        : this(DefaultLightDirection, DefaultAmbient)
    {
    }

    public static float DefaultAmbient => 0.35f;

    // Over the camera's shoulder and slightly to the left, the convention
    // flight simulators settled on: it lights what the pilot is looking at
    // while still turning the near faces of a ship away from each other. The
    // normals it meets are camera-space, so this is camera-space too.
    public static Vector3 DefaultLightDirection { get; } = Vector3.Normalize(new(-0.4f, 0.5f, -1f));

    /// <summary>
    /// A face colour with the model's own baked-in shading taken back out:
    /// the same hue, at the brightness a fully-lit face of this model has.
    /// </summary>
    /// <remarks>
    /// The models pre-date lighting and fake it by hand, giving a face a
    /// darker shade of its material to suggest which way it turns - the
    /// Coriolis station is eight faces of one grey, four of a lighter one and
    /// two lighter again. Shading those directly lights an already-lit model
    /// twice, and the hand-picked shades then read as flicker rather than
    /// form. Rescaling each face to one brightness first discards the
    /// hand-painted guess and lets the light make the distinction instead, so
    /// a model's whole grey family becomes one grey and comes apart again by
    /// facing.
    /// <para>
    /// Black has no brightness to restore and no hue to restore it to, so it
    /// stays black - which is what the station's docking slot wants.
    /// </para>
    /// </remarks>
    public static FastColor UnlitBase(in FastColor colour, byte fullyLit)
    {
        int max = Math.Max(colour.R, Math.Max(colour.G, colour.B));

        return max == 0
            ? colour
            : new(
                colour.A,
                (byte)(colour.R * fullyLit / max),
                (byte)(colour.G * fullyLit / max),
                (byte)(colour.B * fullyLit / max));
    }

    /// <summary>
    /// The brightness a fully-lit face of a model painted in these colours
    /// takes: its brightest channel, plus a quarter again of headroom.
    /// </summary>
    /// <remarks>
    /// Capping at the model's own brightest keeps a lit model from turning
    /// whiter than its artist drew it - scaling to a flat 255 turned the
    /// station, painted in greys no lighter than 0x88, into a white one. The
    /// headroom on top buys shades: a rendition quantises what lighting
    /// computes, and the 16-bit tier's four-bit channels hold sixteen greys in
    /// all, of which stopping at 0x88 leaves six. Adjacent faces then round to
    /// the same grey about one time in six. Going a quarter beyond reaches
    /// 0xAA and eight shades, measurably fewer collisions, and still nothing
    /// like white. A model already painted at full brightness has no headroom
    /// to give and stays there.
    /// </remarks>
    public static byte FullyLit(IEnumerable<FastColor> faceColours)
    {
        ArgumentNullException.ThrowIfNull(faceColours);

        int brightest = 0;

        foreach (FastColor colour in faceColours)
        {
            brightest = Math.Max(brightest, Math.Max(colour.R, Math.Max(colour.G, colour.B)));
        }

        return (byte)Math.Min(255, brightest * 5 / 4);
    }

    /// <summary>
    /// The face colour as this light leaves it, unquantised - what a rendition
    /// can actually display is the rendition's business.
    /// </summary>
    public static FastColor Shade(
        in FastColor faceColour,
        Vector3 cameraNormal,
        Vector3 lightDirection,
        float ambient,
        byte fullyLit)
    {
        FastColor unlit = UnlitBase(faceColour, fullyLit);
        float intensity = Intensity(cameraNormal, lightDirection, ambient);

        return new(
            faceColour.A,
            Channel(unlit.R, intensity),
            Channel(unlit.G, intensity),
            Channel(unlit.B, intensity));
    }

    public FastColor Shade(in FastColor faceColour, Vector3 cameraNormal, byte fullyLit)
        => Shade(faceColour, cameraNormal, _lightDirection, _ambient, fullyLit);

    // The Lambert term, lifted onto the ambient floor so it spans
    // [ambient, 1] rather than [0, 1]. A degenerate normal - a face whose
    // points are collinear, which the models do contain - has no direction to
    // light, so it takes the full value and looks as it did before.
    internal static float Intensity(Vector3 cameraNormal, Vector3 lightDirection, float ambient)
    {
        if (cameraNormal == Vector3.Zero)
        {
            return 1f;
        }

        float lambert = Math.Clamp(Vector3.Dot(Vector3.Normalize(cameraNormal), lightDirection), 0f, 1f);

        return ambient + ((1f - ambient) * lambert);
    }

    // Away from zero, matching NearestLevel, so a half-lit channel does not
    // round one way here and the other way when a rendition quantises it.
    private static byte Channel(byte channel, float intensity)
        => (byte)MathF.Round(channel * intensity, MidpointRounding.AwayFromZero);
}
