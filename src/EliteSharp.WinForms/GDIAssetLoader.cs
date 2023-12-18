// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Drawing.Text;
using EliteSharp.Assets;
using EliteSharp.Assets.Fonts;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    public class GDIAssetLoader(IAssetLocator assets)
    {
        private readonly IAssetLocator _assets = assets;

        public Dictionary<ImageType, Bitmap> LoadImages()
            => _assets.ImageAssetPaths().ToDictionary(x => x.Key, x => new Bitmap(x.Value));

        public Dictionary<FontType, Font> LoadFonts()
        {
            using PrivateFontCollection fonts = new();

            foreach (string fontPath in _assets.FontAssetPaths())
            {
                fonts.AddFontFile(fontPath);
            }

            FontFamily[] fontFamilies = fonts.Families;
            string fontName = fontFamilies[0].Name;

            return new()
            {
                { FontType.Small, new(fontName, 12, FontStyle.Regular, GraphicsUnit.Pixel) },
                { FontType.Large, new(fontName, 18, FontStyle.Regular, GraphicsUnit.Pixel) },
            };
        }
    }
}
