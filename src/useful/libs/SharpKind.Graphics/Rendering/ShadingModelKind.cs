// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

// Which shading model a solid world uses - the config's name for the
// IShadingModel it wants built. An enum rather than a flag because the
// interesting axis is which model, not whether there is one.
public enum ShadingModelKind
{
    // The flat model colour, as both games looked before there was a light to
    // turn on and as the original hardware drew them.
    Unlit = 0,

    // One directional light against each face's own normal.
    Lambert = 1,

    // The same light against a normal per corner, blended across the face -
    // so a curve drawn as facets shades as a curve. The normals are derived
    // rather than authored (see VertexNormals), the assets carrying only
    // per-face ones.
    Gouraud = 2,
}
