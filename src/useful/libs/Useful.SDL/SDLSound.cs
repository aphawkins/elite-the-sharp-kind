// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using Useful.Assets;
using Useful.Audio;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace Useful.SDL;

public sealed class SDLSound : ISound, IDisposable
{
    private bool _disposedValue;
    private Dictionary<int, nint> _music = [];
    private Dictionary<int, nint> _sfx = [];

    public SDLSound()
    {
        SDLGuard.Execute(() => SDL_Init(SDL_INIT_AUDIO));
        SDLGuard.Execute(() => Mix_Init(MIX_InitFlags.MIX_INIT_OGG));

        SDL_AudioSpec audioSpecDesired = default;
        audioSpecDesired.channels = 2;
        audioSpecDesired.format = AUDIO_F32SYS;
        audioSpecDesired.freq = 44100;
        audioSpecDesired.samples = 1024;

        SDLGuard.Execute(
            () => Mix_OpenAudio(audioSpecDesired.freq, audioSpecDesired.format, audioSpecDesired.channels, audioSpecDesired.samples));
        SDLGuard.Execute(() => Mix_AllocateChannels(2));
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

    public void Initialize(IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        _music = assetLocator.MusicPaths.ToDictionary(
            x => x.Key,
            x =>
            {
                Debug.Assert(!string.IsNullOrWhiteSpace(x.Value), "Music is missing");
                return SDLGuard.Execute(() => Mix_LoadMUS(x.Value));
            });

        _sfx = assetLocator.SfxPaths.ToDictionary(
            x => x.Key,
            x => SDLGuard.Execute(() => Mix_LoadWAV(x.Value)));
    }

    public void Play(int sfxType) => SDLGuard.Execute(() => Mix_PlayChannel(-1, _sfx[sfxType], 0));

    public void Play(int musicType, bool repeat)
    {
        StopMusic();

        SDLGuard.Execute(() => Mix_PlayMusic(_music[musicType], repeat ? -1 : 1));
    }

    public void StopMusic() => SDLGuard.Execute(Mix_HaltMusic);

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
            if (Mix_HaltMusic() < 0)
            {
                // Ignore
            }

            foreach (KeyValuePair<int, nint> v in _music)
            {
                SDL_FreeWAV(v.Value);
            }

            foreach (KeyValuePair<int, nint> v in _sfx)
            {
                Mix_FreeMusic(v.Value);
            }

            foreach (KeyValuePair<int, nint> music in _music)
            {
                Mix_FreeMusic(music.Value);
            }

            Mix_CloseAudio();
        }
    }
}
