// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using EliteSharp.Assets;
using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using static SDL2.SDL;
using static SDL2.SDL_mixer;
using static SDL2.SDL_ttf;

namespace EliteSharp.SDL
{
    public class SDLAssetLoader(IAssetLocator assets)
    {
        private readonly IAssetLocator _assets = assets;

        public Dictionary<ImageType, nint> LoadImages()
            => _assets.ImageAssets().ToDictionary(
                x => x.Key,
                x => SDLGuard.Execute(() => SDL_LoadBMP(x.Value)));

        public Dictionary<FontType, nint> LoadFonts()
        {
            IDictionary<FontType, string> fontPaths = _assets.FontAssets();

            nint fontLarge = SDLGuard.Execute(() => TTF_OpenFont(fontPaths[FontType.Large], 18));
            nint fontSmall = SDLGuard.Execute(() => TTF_OpenFont(fontPaths[FontType.Small], 12));

            return new()
            {
                { FontType.Small, fontSmall },
                { FontType.Large, fontLarge },
            };
        }

        public Dictionary<MusicType, nint> LoadMusic()
            => _assets.MusicAssets().ToDictionary(
                x => x.Key,
                x =>
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(x.Value), "Music is missing");
                    return SDLGuard.Execute(() => Mix_LoadMUS(x.Value));
                });

        public Dictionary<SoundEffect, nint> LoadSfx()
            => _assets.SfxAssets().ToDictionary(
                x => x.Key,
                x => SDLGuard.Execute(() => Mix_LoadWAV(x.Value)));
    }
}
