// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using EliteSharp.Audio;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace EliteSharp.SDL
{
    internal sealed class SDLSound : ISound
    {
        private readonly uint _deviceId;
        private readonly Dictionary<MusicType, nint> _music;
        private readonly Dictionary<SoundEffect, (nint SfxPtr, nint Data, uint Len)> _sfx;
        private bool _disposedValue;

        public SDLSound(SDLAssetLoader assetLoader)
        {
            Guard.ArgumentNull(assetLoader);

            if (SDL_Init(SDL_INIT_AUDIO) < 0)
            {
                SDLHelper.Throw(nameof(SDL_Init));
            }

            if (Mix_Init(MIX_InitFlags.MIX_INIT_OGG) < 0)
            {
                SDLHelper.Throw(nameof(Mix_Init));
            }

            SDL_AudioSpec audioSpecDesired = default;
            audioSpecDesired.channels = 1;
            audioSpecDesired.format = 32784;
            audioSpecDesired.freq = 44100;
            audioSpecDesired.samples = 4096;
            _deviceId = SDL_OpenAudioDevice(null, 0, ref audioSpecDesired, out SDL_AudioSpec audioSpecObtained, 0);
            Debug.Assert(AreEqual(audioSpecDesired, audioSpecObtained), "Failed to meet the desired audio spec.");

            if (Mix_OpenAudio(audioSpecDesired.freq, audioSpecDesired.format, audioSpecDesired.channels, audioSpecDesired.samples) < 0)
            {
                SDLHelper.Throw(nameof(Mix_OpenAudio));
            }

            if (Mix_AllocateChannels(2) < 0)
            {
                SDLHelper.Throw(nameof(Mix_AllocateChannels));
            }

            _music = assetLoader.LoadMusic();
            _sfx = assetLoader.LoadSfx();
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SDLSound()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Play(SoundEffect sfxType)
        {
            SDL_PauseAudioDevice(_deviceId, 1);

            if (SDL_QueueAudio(_deviceId, _sfx[sfxType].Data, _sfx[sfxType].Len) < 0)
            {
                SDLHelper.Throw(nameof(SDL_QueueAudio));
            }

            SDL_PauseAudioDevice(_deviceId, 0);
        }

        public void Play(MusicType musicType, bool repeat)
        {
            StopMusic();

            if (Mix_PlayMusic(_music[musicType], repeat ? -1 : 1) < 0)
            {
                SDLHelper.Throw(nameof(Mix_PlayMusic));
            }
        }

        public void StopMusic()
        {
            if (Mix_HaltMusic() < 0)
            {
                SDLHelper.Throw(nameof(Mix_HaltMusic));
            }
        }

        private static bool AreEqual(SDL_AudioSpec x, SDL_AudioSpec y) => x.channels == y.channels &&
            x.format == y.format &&
            x.freq == y.freq &&
            x.samples == y.samples;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                SDL_CloseAudioDevice(_deviceId);

                foreach (KeyValuePair<MusicType, nint> v in _music)
                {
                    SDL_FreeWAV(v.Value);
                }

                foreach (KeyValuePair<SoundEffect, (nint WavPtr, nint Data, uint Len)> v in _sfx)
                {
                    SDL_FreeWAV(v.Value.WavPtr);
                }

                if (Mix_HaltMusic() < 0)
                {
                    // Ignore
                }

                foreach (KeyValuePair<MusicType, nint> music in _music)
                {
                    Mix_FreeMusic(music.Value);
                }

                Mix_CloseAudio();
            }
        }
    }
}
