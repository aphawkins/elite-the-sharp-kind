// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

// Which shading model a solid world uses - the config's name for the
// IShadingModel it wants built. An enum rather than a flag because the
// interesting axis is which model, not whether there is one: Gouraud joins
// here when the assets can carry per-vertex normals.
public enum ShadingModelKind
{
    // The flat model colour, as both games looked before there was a light to
    // turn on and as the original hardware drew them.
    Unlit = 0,

    // One directional light against each face's own normal.
    Lambert = 1,
}
