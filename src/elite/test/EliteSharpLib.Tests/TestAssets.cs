// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Renditions;
using SharpKind.Assets;

namespace EliteSharpLib.Tests;

// The game's assets and a rendition's, composed as the game composes them.
// Artwork, palettes, fonts and ship models live with the rendition that draws
// them now, so a test wanting any of those has to say which rendition it
// means - AssetLocator.Create() on its own finds only the music and the
// sound effects, which is all the game keeps for itself.
internal static class TestAssets
{
    private const string SixteenBit = "16-bit";

    internal static IAssetLocator Locator() => Locator(SixteenBit);

    // The rendition is named the way the config names it: the assembly it
    // lives in is EliteSharp.Renditions.SixteenBit for "16-bit", which is
    // also the folder the build drops it in, the same arrangement the app
    // ships.
    internal static IAssetLocator Locator(string rendition)
    {
        string assembly = rendition == SixteenBit
            ? "EliteSharp.Renditions.SixteenBit"
            : "EliteSharp.Renditions.EightBit";
        string folder = Path.Combine("Renditions", assembly);

        return new RenditionAssets(
            AssetLocator.CreateFrom(Path.Combine(AppContext.BaseDirectory, folder), rendition),
            AssetLocator.Create(rendition));
    }
}
