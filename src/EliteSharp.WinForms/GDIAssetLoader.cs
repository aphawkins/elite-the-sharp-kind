// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Drawing.Text;
using System.Media;
using EliteSharp.Assets;
using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using NAudio.Vorbis;
using NAudio.Wave;

namespace EliteSharp.WinForms
{
    public class GDIAssetLoader(IAssetLocator assets)
    {
        private readonly IAssetLocator _assets = assets;

        public Dictionary<ImageType, Bitmap> LoadImages()
            => _assets.ImageAssets().ToDictionary(x => x.Key, x => new Bitmap(x.Value));

        public Dictionary<FontType, Font> LoadFonts()
        {
            using PrivateFontCollection fonts = new();

            foreach (string fontPath in _assets.FontAssets())
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

        public Dictionary<MusicType, SoundPlayer> LoadMusic()
            => _assets.MusicAssets().ToDictionary(
                x => x.Key,
                x =>
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(x.Value), "Music is missing");
                    return SoundPlayerFromVorbis(x.Value);
                });

        public Dictionary<SoundEffect, SoundPlayer> LoadSfx()
            => _assets.SfxAssets().ToDictionary(
                x => x.Key,
                x =>
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(x.Value), "Sound effect is missing");
                    return SoundPlayerFromVorbis(x.Value);
                });

        private static SoundPlayer SoundPlayerFromVorbis(string fileName)
        {
            using MemoryStream memStream = new();
            using VorbisWaveReader vorbisStream = new(fileName);
            WaveFileWriter.WriteWavFileToStream(memStream, vorbisStream);
            memStream.Position = 0;
            SoundPlayer player = new(memStream);
            player.Load();
            Debug.Assert(player.IsLoadCompleted, "Sound failed to load");
            return player;
        }
    }
}
