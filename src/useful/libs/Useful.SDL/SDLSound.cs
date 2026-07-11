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
    private Dictionary<string, nint> _music = [];
    private Dictionary<string, nint> _sfx = [];

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
        SDLGuard.Execute(() => Mix_AllocateChannels(16));
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

    // Mix_PlayChannel returns -1 both on a real error and when every channel
    // is busy; either way, dropping this one-shot effect beats crashing the game.
    public void Play(string sfxType) => Mix_PlayChannel(-1, _sfx[sfxType], 0);

    public void Play(string musicType, bool repeat)
    {
        StopMusic();

        SDLGuard.Execute(() => Mix_PlayMusic(_music[musicType], repeat ? -1 : 1));
    }

    public void StopMusic() => SDLGuard.Execute(Mix_HaltMusic);

    // Pitched loops are not supported by the SDL_mixer backend yet.
    public void PlayLoop(string sfxType, double pitch)
    {
    }

    public void StopLoop()
    {
    }

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

            foreach (KeyValuePair<string, nint> music in _music)
            {
                Mix_FreeMusic(music.Value);
            }

            foreach (KeyValuePair<string, nint> sfx in _sfx)
            {
                Mix_FreeChunk(sfx.Value);
            }

            Mix_CloseAudio();
        }
    }
}
