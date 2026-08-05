// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Renditions;
using Useful.Assets;

namespace EliteSharpLib.Tests;

// The game's assets and a rendition's, composed as the game composes them.
// Artwork, palettes, fonts and ship models live with the rendition that draws
// them now, so a test wanting any of those has to say which rendition it
// means - AssetLocator.Create() on its own finds only the music and the
// sound effects, which is all the game keeps for itself.
internal static class TestAssets
{
    private const string Rendition = "16-bit";

    // Where the test project's build drops the renditions, which is the same
    // arrangement the app ships.
    private static readonly string s_renditionFolder = Path.Combine("Renditions", "EliteSharp.Renditions.SixteenBit");

    internal static IAssetLocator Locator() => new RenditionAssets(
        AssetLocator.CreateFrom(Path.Combine(AppContext.BaseDirectory, s_renditionFolder), Rendition),
        AssetLocator.Create(Rendition));
}
