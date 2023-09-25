// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Diagnostics;
using EliteSharp.Audio;
using static SDL2.SDL;

namespace EliteSharp.SDL
{
    internal sealed class SDLSound : ISound
    {
        private readonly uint _deviceId;
        private readonly ConcurrentDictionary<MusicType, nint> _musics = new();
        private readonly ConcurrentDictionary<SoundEffect, (nint SfxPtr, nint Data, uint Len)> _sfxs = new();
        private bool _disposedValue;

        public SDLSound()
        {
            SDL_AudioSpec audioSpecDesired = default;
            audioSpecDesired.channels = 1;
            audioSpecDesired.format = 32784;
            audioSpecDesired.freq = 44100;
            audioSpecDesired.samples = 4096;
            string device = SDL_GetAudioDeviceName(0, 0);
            _deviceId = SDL_OpenAudioDevice(device, 0, ref audioSpecDesired, out SDL_AudioSpec audioSpecObtained, 0);
            Debug.Assert(AreEqual(audioSpecDesired, audioSpecObtained), "Failed to meet the desired audio spec.");
        }

        ////public async Task LoadAsync(MusicType musicType, string filePath, CancellationToken token) =>
        ////    await Task.Run(() => _musics[musicType] = Marshal.PtrToStructure<SDL_AudioSpec>(SDL_LoadWAV(filePath, out _, out _, out _)), token).ConfigureAwait(false);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Load(MusicType musicType, string filePath)
        {
        }

        public void Load(SoundEffect sfxType, string filePath)
        {
            nint sfxPtr = SDL_LoadWAV(filePath, out SDL_AudioSpec audioSpec, out nint data, out uint len);
            if (sfxPtr == nint.Zero)
            {
                SDLHelper.Throw(nameof(SDL_LoadWAV));
            }

            _sfxs[sfxType] = (sfxPtr, data, len);
        }

        public void Play(SoundEffect sfxType)
        {
            SDL_PauseAudioDevice(_deviceId, 1);

            if (SDL_QueueAudio(_deviceId, _sfxs[sfxType].Data, _sfxs[sfxType].Len) < 0)
            {
                SDLHelper.Throw(nameof(SDL_QueueAudio));
            }

            SDL_PauseAudioDevice(_deviceId, 0);
        }

        public void Play(MusicType musicType, bool repeat) => StopMusic();

        public void StopMusic()
        {
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
                    SDL_CloseAudioDevice(_deviceId);

                    foreach (KeyValuePair<MusicType, nint> v in _musics)
                    {
                        SDL_FreeWAV(v.Value);
                    }

                    foreach (KeyValuePair<SoundEffect, (nint WavPtr, nint Data, uint Len)> v in _sfxs)
                    {
                        SDL_FreeWAV(v.Value.WavPtr);
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
            }
        }
    }
}
