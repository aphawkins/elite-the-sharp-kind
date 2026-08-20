// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Assets;
using SharpKind.Graphics;

namespace SharpKind.SDL;

/// <summary>
/// Builds the set of font kinds a rendition can be drawn with. It is composed
/// here because one of the three needs SDL, and both backends are built here -
/// so both get the same set, which is what makes them draw the same text.
/// </summary>
public static class FontRasterisers
{
    /// <summary>
    /// Every kind the rendition declared something for, with the wanted one
    /// selected. The rendition's own sheets are always present, and stand in
    /// for a kind it declared nothing for.
    /// </summary>
    /// <param name="assets">The rendition's loaded assets.</param>
    /// <param name="assetLocator">Where its TrueType faces are declared.</param>
    /// <param name="kind">The kind wanted.</param>
    /// <returns>The set, ready to draw with.</returns>
    public static FontRasteriserSet Load(AssetSet assets, IAssetLocator assetLocator, FontKind kind)
    {
        ArgumentNullException.ThrowIfNull(assets);
        ArgumentNullException.ThrowIfNull(assetLocator);

        Dictionary<FontKind, IFontRasteriser> rasterisers = new()
        {
            { FontKind.Bitmap, new BitmapFontRasteriser(assets.BitmapFonts) },
        };

        // A kind is offered only where the rendition declared a font for it.
        // Loading one that was never declared would mean inventing a size and
        // a face for it, and neither is the engine's to choose.
        if (assets.FonFonts.Count > 0)
        {
            RequireEveryFontType(assets.BitmapFonts.Keys, assets.FonFonts.Keys, FontKind.Fon, assetLocator.Rendition);
            rasterisers[FontKind.Fon] = new FonFontRasteriser(assets.FonFonts);
        }

        if (assetLocator.FontTrueTypes.Count > 0)
        {
            RequireEveryFontType(
                assets.BitmapFonts.Keys,
                assetLocator.FontTrueTypes.Keys,
                FontKind.TrueType,
                assetLocator.Rendition);

            rasterisers[FontKind.TrueType] = TrueTypeRasteriser.Load(assetLocator.FontTrueTypes);
        }

        return new(rasterisers, kind);
    }

    // Declaring nothing for a kind is a choice - the rendition's own sheets
    // stand in for it. Declaring some of it is a mistake, and one that would
    // otherwise wait until a screen drew text in the type that was left out,
    // since a kind is chosen as a whole and nothing checks a type until it is
    // asked for. The sheets say which types the game asks for, being the one
    // kind every rendition has.
    internal static void RequireEveryFontType(
        IEnumerable<string> sheets,
        IEnumerable<string> declared,
        FontKind kind,
        string rendition)
    {
        string[] missing = [.. sheets.Except(declared, StringComparer.Ordinal).Order(StringComparer.Ordinal)];

        if (missing.Length > 0)
        {
            throw new SharpKindException(
                $"The {rendition} asset set declares {kind} fonts but not for {string.Join(", ", missing)}.");
        }
    }
}
