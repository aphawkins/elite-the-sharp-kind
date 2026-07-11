// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Useful.Assets;
using Useful.Audio;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace Useful.SDL;

public sealed class SDLSound : ISound, IDisposable
{
    // Reserved so Mix_PlayChannel(-1, ...) (one-shot sfx) never picks this
    // channel; it is driven exclusively by the pitched loop below.
    private const int LoopChannel = 0;

    private readonly Dictionary<string, float[]> _loopSamples = [];
    private bool _disposedValue;
    private Mix_EffectFunc_t? _loopEffect;
    private float[] _loopSampleData = [];
    private float[] _loopScratch = [];
    private string? _loopName;
    private double _loopPitch = 1.0;
    private double _loopPosition;
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
        SDLGuard.Execute(() => Mix_ReserveChannels(1));

        // Mix_QuerySpec uses the inverted (0 = error) convention. The
        // compiler can't see that the lambda runs synchronously, so these
        // need an initial value to satisfy definite-assignment analysis.
        int frequency = 0;
        ushort format = 0;
        int channels = 0;
        SDLGuard.Execute(() => Mix_QuerySpec(out frequency, out format, out channels), zeroIndicatesError: true);
        Debug.Assert(frequency == 44100, "Loop pitch-shifting assumes 44100Hz chunk data.");
        Debug.Assert(format == AUDIO_F32SYS, "Loop pitch-shifting assumes float32 chunk data.");
        Debug.Assert(channels == 2, "Loop pitch-shifting assumes stereo chunk data.");
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

    // SDL_mixer has no built-in pitch control, so the loop is played on a
    // reserved channel whose output is entirely replaced, each callback, by
    // our own linearly-interpolated resample of the source chunk (mirroring
    // Useful.Audio.PitchedLoopSampleProvider). Mix_PlayChannel on that
    // channel is only there to keep the mixer invoking the effect callback;
    // its own (unmodified) output is never heard.
    public void PlayLoop(string sfxType, double pitch)
    {
        if (_loopName != sfxType)
        {
            StopLoop();

            _loopSampleData = GetLoopSamples(sfxType);
            _loopPosition = 0;
            _loopName = sfxType;
            _loopEffect = LoopEffect;

            // Channel 0 is reserved and used only here, so unlike the -1
            // (any free channel) case in Play(string), a failure here is a
            // real bug rather than expected contention — worth surfacing loudly.
            SDLGuard.Execute(() => Mix_PlayChannel(LoopChannel, _sfx[sfxType], -1));

            // Mix_RegisterEffect uses the inverted (0 = error) convention.
            SDLGuard.Execute(() => Mix_RegisterEffect(LoopChannel, _loopEffect, null, nint.Zero), zeroIndicatesError: true);
        }

        _loopPitch = pitch;
    }

    public void StopLoop()
    {
        if (_loopName is null)
        {
            return;
        }

        // Same inverted error convention as Mix_RegisterEffect above.
        SDLGuard.Execute(() => Mix_UnregisterEffect(LoopChannel, _loopEffect), zeroIndicatesError: true);

        // Mix_HaltChannel has no failure case at all (it always returns 0),
        // so wrapping it can never throw; kept for consistency with the rest
        // of this class routing every Mix_ call through SDLGuard.
        SDLGuard.Execute(() => Mix_HaltChannel(LoopChannel));
        _loopName = null;
        _loopEffect = null;
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

            StopLoop();

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

    private float[] GetLoopSamples(string sfxType)
    {
        if (!_loopSamples.TryGetValue(sfxType, out float[]? samples))
        {
            MIX_Chunk chunk = Marshal.PtrToStructure<MIX_Chunk>(_sfx[sfxType]);
            int sampleCount = (int)(chunk.alen / sizeof(float));
            samples = new float[sampleCount];
            Marshal.Copy(chunk.abuf, samples, 0, sampleCount);
            _loopSamples[sfxType] = samples;
        }

        return samples;
    }

    // Called by SDL_mixer once per audio buffer fill for the loop channel;
    // overwrites its entire output with a resample of _loopSampleData at
    // _loopPitch (1.0 = recorded rate), looping back to the start.
    private void LoopEffect(int channel, nint stream, int len, nint userData)
    {
        int floatCount = len / sizeof(float);
        if (_loopScratch.Length < floatCount)
        {
            _loopScratch = new float[floatCount];
        }

        int frames = _loopSampleData.Length / 2;
        if (frames < 2)
        {
            Array.Clear(_loopScratch, 0, floatCount);
            Marshal.Copy(_loopScratch, 0, stream, floatCount);
            return;
        }

        double pitch = _loopPitch;
        for (int i = 0; i < floatCount; i += 2)
        {
            int frame0 = (int)_loopPosition;
            int frame1 = frame0 + 1 >= frames ? 0 : frame0 + 1;
            float fraction = (float)(_loopPosition - frame0);

            _loopScratch[i] = _loopSampleData[frame0 * 2]
                + ((_loopSampleData[frame1 * 2] - _loopSampleData[frame0 * 2]) * fraction);
            _loopScratch[i + 1] = _loopSampleData[(frame0 * 2) + 1]
                + ((_loopSampleData[(frame1 * 2) + 1] - _loopSampleData[(frame0 * 2) + 1]) * fraction);

            _loopPosition += pitch;
            while (_loopPosition >= frames)
            {
                _loopPosition -= frames;
            }
        }

        Marshal.Copy(_loopScratch, 0, stream, floatCount);
    }
}
