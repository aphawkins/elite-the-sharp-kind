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
                x =>
                {
                    nint surface = SDL_LoadBMP(x.Value);
                    if (surface == nint.Zero)
                    {
                        SDLHelper.Throw(nameof(SDL_LoadBMP));
                    }

                    return surface;
                });

        public Dictionary<FontType, nint> LoadFonts()
        {
            string fontPath = _assets.FontAssets().ToList()[0];

            nint fontLarge = TTF_OpenFont(fontPath, 18);
            if (fontLarge == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_OpenFont));
            }

            nint fontSmall = TTF_OpenFont(fontPath, 12);
            if (fontSmall == nint.Zero)
            {
                SDLHelper.Throw(nameof(TTF_OpenFont));
            }

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
                    nint music = Mix_LoadMUS(x.Value);
                    if (music == nint.Zero)
                    {
                        SDLHelper.Throw(nameof(Mix_LoadMUS));
                    }

                    return music;
                });

        public Dictionary<SoundEffect, nint> LoadSfx()
            => _assets.SfxAssets().ToDictionary(
                x => x.Key,
                x =>
                {
                    nint sfx = Mix_LoadWAV(x.Value);
                    if (sfx == nint.Zero)
                    {
                        SDLHelper.Throw(nameof(Mix_LoadWAV));
                    }

                    return sfx;
                });
    }
}
