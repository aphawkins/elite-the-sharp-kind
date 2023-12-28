// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets.Fonts;

namespace EliteSharp.Assets
{
    public class SoftwareAssetLocator : AssetLocator
    {
        protected override string GetName(FontType font) => font switch
        {
            FontType.Small => "font1.bmp",
            FontType.Large => "font2.bmp",
            _ => throw new EliteException(),
        };
    }
}
