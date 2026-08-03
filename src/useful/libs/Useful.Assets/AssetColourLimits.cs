// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

//// JSON serializable

// What a rendition says its own colours are limited to. The game cannot know
// this about a rendition it was never built against - a stranger's could be
// four colours or sixteen million - so the rendition declares it alongside the
// assets it constrains, and the load-time checks are run against what it
// declared.
//
// The defaults are "no constraint", which is the right answer for a rendition
// that says nothing: nothing it ships can then be rejected for a limit it
// never claimed to have.
public sealed class AssetColourLimits
{
    // The most distinct opaque colours the whole asset set may use between
    // it. The cap is on the union across the set, not per image, because a
    // rendition standing in for a machine stands in for one palette.
    public int MaxColours { get; set; } = int.MaxValue;

    // Whether the palette is the rendition's complete colour set, so a bitmap
    // may only use colours it names. True of indexed-colour hardware, where
    // every pixel was a palette entry; false of direct-colour hardware, where
    // the palette is only a set of names the geometry draws with.
    public bool PaletteNamesEveryColour { get; set; }

    // Bits per colour channel the rendition's hardware could actually drive.
    // Eight means no constraint. Four is a 12-bit DAC, so a channel may hold
    // only one of sixteen levels, and a colour between two of them is one the
    // machine could not have shown.
    public int ChannelBits { get; set; } = 8;
}
