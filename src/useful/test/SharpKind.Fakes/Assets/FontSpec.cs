// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Fakes.Assets;

// One strike to put in a built .fon: the glyphs, as rows of '#' for ink and
// anything else for paper, one array per character from FirstChar upwards.
public sealed record FontSpec(
    ushort Version,
    int PixelHeight,
    bool IsProportional,
    char FirstChar,
    char LastChar,
    IReadOnlyList<string[]> Glyphs);
